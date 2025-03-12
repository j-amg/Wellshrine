using System;
using System.Linq;
using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler();
	[Export]
	public AudioStream hit;
	[Export]
	public StateMachine stateMachine;
	[Export]
	private RayCast3D lookRay;
	[Export]
	public Node3D body;
	[Export]
	public Node3D head;
	[Export]
	public Camera3D camera;
	[Export]
	public Hud hud;
	[Export]
	private Node3D hands;
	[Export]
	public AnimatedSprite3D handSprite;
	[Export]
	public CollisionShape3D standCollision;
	[Export]
	public CollisionShape3D crouchCollision;
	[Export]
	private Area3D iceCollision;
	[Export]
	private Area3D shockCollision;
	[Export]
	private PackedScene fireball;
	[Export]
	private PackedScene iceray;

	private float mouseSensitivity = 0.1f;
	private float aimMouseSensitivity = 0.075f;
	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;
	public float zoomFOV = 60;
	public float walkingFOV = 80;
	public float walkSpeed = 5;
	public  float aimSpeed = 3;
	public float slideSpeed = 25;
	public float crouchSpeed = 3;
	public float crouchAnimSpeed = .2f;
	public float interactionRange = 3;
	public float gravity = 12;
	public float acceleration = .5f;
	public float deceleration = .25f;
    public float airAcceleration = .1f;
    public float airDeceleration = 0.005f;
	public float dashVelocity = 15f;
    public float fallSpeed = 2;
	public float jumpVelocity = 5;
	public Vector3 bodyCrouchPosition = new (0,-0.3f,0);
    public Vector3 bodyStandPosition = new (0,0,0);


	public float hitDistance;
	public float currentSpeed;
	public float _sensitivity;
	public Vector3 velocity;
	public Vector2 hvel;
	public Vector2 inputDir;
	public Vector3 direction;
	private Vector3 last_physics_pos;
	private GodotObject existingHit;
	


	private Viewport vp;
	private Window win;
	private bool applyTransform = false;
	public bool inputPaused = false;
	private bool recharging = false;

	

    public override void _Ready()
    {
		vp = GetViewport();
		win = GetWindow();

		handSprite.Play(Global.Singleton.currentIdle);
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		currentSpeed = walkSpeed;
		FloorMaxAngle = Mathf.DegToRad(65);
		FloorConstantSpeed = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		AddToGroup("player");
    }

	public void PauseInput()
	{
		SetProcessInput(false);
		inputPaused = true;

	}

	public void ResumeInput()
	{
		SetProcessInput(true);
		inputPaused = false;
	}

    public override void _PhysicsProcess(double delta)
	{
		last_physics_pos = Position;
		if (inputPaused) return;
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing),0, 0);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, velocity.Normalized().X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);

		if (Input.IsActionJustPressed("LeftMouse") && !recharging)
		{
			if (Global.Singleton.equippedWeapon == null) return;
			Attack();
			Recharge(Global.Singleton.equippedWeapon.recharge * Global.Singleton.playerRechargeBuff);
		}

		if (Input.IsActionJustPressed("interact"))
		{
			if (existingHit is IInteractable interactable && hitDistance <= interactionRange && interactable.Active) interactable.Interact();
		}

		if (Input.IsActionPressed("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", zoomFOV, .05);
			_sensitivity = aimMouseSensitivity;
			currentSpeed = aimSpeed;
		}

		if (Input.IsActionJustReleased("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", walkingFOV, .1);
			_sensitivity = mouseSensitivity;
			currentSpeed = walkSpeed;
		}
		
	}

	public override void _Process(double delta)
    {
        float fraction = (float)Engine.GetPhysicsInterpolationFraction();
		if (applyTransform) { GlobalTransform = new Transform3D(GlobalTransform.Basis, last_physics_pos.Lerp(GlobalTransform.Origin, fraction));
		} 
		else {
			last_physics_pos = Position;
			applyTransform = true;
		} 
		GodotObject currentHit = lookRay.GetCollider();
		hitDistance = currentHit != null ? lookRay.GlobalTransform.Origin.DistanceTo(lookRay.GetCollisionPoint()) : 0;
		if (currentHit != existingHit)
		{
			if (existingHit != null)
			{
				if (existingHit is Enemy enemy) enemy.highlighted = false;
				if (existingHit is IInteractable interactable)
				{
					interactable.Highlighted = false;
					hud.interactLabel.Visible = false;
				} 
			} 
			existingHit = currentHit;
			
			if (currentHit is Enemy || currentHit is IInteractable)
			{
				if (currentHit is Enemy enemy)
				{
					enemy.highlighted = true;
					hud.reticle.Modulate = new Color(1,0,0);
				} 
				if (currentHit is IInteractable interactable && interactable.Active)
				{
					interactable.Highlighted = true;
					hud.reticle.Modulate = new Color(0,0,1);
				}
			}
			else hud.reticle.Modulate = new Color(1, 1, 1);
		}

		if (currentHit is IInteractable interactable1)
		{
			if (hitDistance <= interactionRange && interactable1.Active && !inputPaused)
			{
				hud.interactLabel.Visible = true;
			} else hud.interactLabel.Visible = false;

			if (!interactable1.Active) hud.reticle.Modulate = new Color(1, 1, 1);
		}
    }

	public override void _Input(InputEvent @event)
	{
		if (Global.Singleton.awaitedAction == "look") Global.Singleton.ClosePopUp();
		float sensitivityScale = win.Size.X / vp.GetVisibleRect().Size.X;
		if (@event is InputEventMouseMotion eventKey)
		{
			float xRot = -Mathf.DegToRad(eventKey.Relative.Y) * _sensitivity * sensitivityScale;
			head.RotateX(xRot);
			RotateY(-Mathf.DegToRad(eventKey.Relative.X) * _sensitivity * sensitivityScale);
			head.RotationDegrees = new Vector3(Mathf.Clamp(head.RotationDegrees.X, -80, 80), head.RotationDegrees.Y, head.RotationDegrees.Z);
		}
	}

	private async void Recharge(float duration)
	{
		recharging = true;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(hud.rechargeBar, "value", 0, duration).From(100);
		await ToSignal(GetTree().CreateTimer(duration), "timeout");
		recharging = false;
	}

	public void SetCrouch(bool val)
	{
		crouchCollision.Disabled = !val;
		standCollision.Disabled = val;
		Vector3 pos = val ? bodyCrouchPosition : bodyStandPosition;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(body, "position", pos, crouchAnimSpeed).SetTrans(Tween.TransitionType.Sine);
	}

	float IDamageable.Health{ get{ return Global.Singleton.currentPlayerHealth; } set{}}

	void IDamageable.Damage(float amount)
	{
		hud.Flash(new Color(1,0,0,0));
		Global.Singleton.IncrementHealth(-amount);
		Global.Singleton.PlaySound2D(hit);
		EmitSignal(SignalName.damageTaken);
	}

    public void UpdateInput(float speed, float acceleration, float deceleration)
	{
		
		velocity = Velocity;
		hvel = new Vector2(velocity.X, velocity.Z);
		inputDir = Input.GetVector("A", "D", "W", "S");
		if (inputPaused) inputDir = new Vector2(0,0);
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		acceleration /= (hvel.Length()* 2) + 1;

		if (direction != Vector3.Zero)
		{
			if (Global.Singleton.awaitedAction == "walk") Global.Singleton.ClosePopUp();
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, acceleration);
		}
		else velocity = velocity.MoveToward(new Vector3(0,velocity.Y, 0), deceleration);
	}

	public void UpdateVelocity()
	{
		Velocity = velocity;
		MoveAndSlide();
	}

	private void Attack()
	{
		if (Global.Singleton.awaitedAction == "attack") Global.Singleton.ClosePopUp();
		AttackAnim();
		if (Global.Singleton.equippedWeapon.name == "fireball")
		{
			Projectile b = fireball.Instantiate() as Projectile;
			var main = GetTree().CurrentScene;
			main.CallDeferred("add_child", b);
			b.damage = Global.Singleton.GetDamage();
			b.Transform = head.GlobalTransform;
			b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
		}

		if (Global.Singleton.equippedWeapon.name == "icespike")
		{
			foreach (IDamageable enemy in iceCollision.GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(Global.Singleton.GetDamage());
			foreach (Area3D area in iceCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
			IceRay ray = iceray.Instantiate() as IceRay;
			var main = GetTree().CurrentScene;
			main.CallDeferred("add_child", ray);
			ray.Transform = head.GlobalTransform;
		}

		if (Global.Singleton.equippedWeapon.name == "shockburst")
		{
			foreach (IDamageable enemy in shockCollision.GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(Global.Singleton.GetDamage());
			foreach (Area3D area in shockCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
			camera.GetNode<AnimatedSprite3D>("shock").Play("cycle");
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera.GetNode<AnimatedSprite3D>("shock"), "modulate", new Color(0,0,0,0), .25).From(new Color(1,1,1,.75f));

		}
	}

	private async void AttackAnim()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(Global.Singleton.equippedWeapon.recoil, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .1);
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(0, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .25);
		handSprite.Play("attack");
		await ToSignal(GetTree().CreateTimer(MathF.Min(0.4f, Global.basePlayerRechargeBuff * Global.Singleton.equippedWeapon.recharge)), "timeout");
		handSprite.Play(Global.Singleton.currentIdle);
	}
}