using System;
using System.Linq;
using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler();
	[Export]
	public AudioStream hit;
	float mouseSensitivity = 0.1f;
	float aimMouseSensitivity = 0.05f;
	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;
	public float zoomFOV = 60;
	public float walkingFOV = 80;
	public Node3D body;
	public Node3D head;
	public Camera3D camera;
	public Hud hud;
	private Node3D hands;
	public AnimatedSprite3D handSprite;
	public TextureRect reticle;
	public float _sensitivity;
	public float _gravity;
	public Vector3 velocity;
	public Vector2 hvel;
	public Vector2 inputDir;
	public Vector3 direction;
	private PackedScene fireball;
	private PackedScene iceray;
	public CollisionShape3D standCollision;
	public CollisionShape3D crouchCollision;
	private Area3D iceCollision;
	private Area3D shockCollision;
	private Vector3 last_physics_pos;
	private StateMachine stateMachine;
	private RayCast3D lookRay;
	private GodotObject existingHit;
	public TextureRect hitFlash;
	private bool recharging = false;
	private ProgressBar rechargeBar;
	private Label interactLabel;
	public float bodyCrouchHeight = -0.3f;
    public float bodyStandHeight = 0;
	public float crouchSpeed = 0.2f;
	public float hitDistance = 0;
	private Viewport vp;
	private Window win;
	

	private bool applyTransform = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	//public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float gravity = 12;

    public override void _Ready()
    {
		fireball = ResourceLoader.Load<PackedScene>("res://playerProjectile.tscn");
		iceray = ResourceLoader.Load<PackedScene>("res://iceRay.tscn");
		body = GetNode<Node3D>("body");
        head = body.GetNode<Node3D>("head");
		camera = head.GetNode<Camera3D>("Camera3D");
		hands = body.GetNode<Node3D>("hands");
		handSprite = hands.GetNode<AnimatedSprite3D>("handSprite");
		lookRay = camera.GetNode<RayCast3D>("lookRay");
		standCollision = GetNode<CollisionShape3D>("standCollision");
		crouchCollision = GetNode<CollisionShape3D>("crouchCollision");
		stateMachine = GetNode<StateMachine>("playerStateMachine");
		reticle = camera.GetNode<TextureRect>("CanvasLayer/reticle");
		rechargeBar = camera.GetNode<ProgressBar>("CanvasLayer/recharge");
		hitFlash = camera.GetNode<TextureRect>("CanvasLayer/hit");
		interactLabel = camera.GetNode<Label>("CanvasLayer/interactLabel");
		iceCollision = camera.GetNode<Area3D>("iceSpikeCollision");
		shockCollision = camera.GetNode<Area3D>("shockBladeCollision");
		handSprite.Play(Global.Singleton.currentIdle);
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		FloorMaxAngle = Mathf.DegToRad(65);
		FloorConstantSpeed = true;
		vp = GetViewport();
		win = GetWindow();
		
		AddToGroup("player");
    }


    public override void _PhysicsProcess(double delta)
	{
		GD.Print(GetWindow().Size);
		if (Global.Singleton.paused) return;
        Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
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
			if (existingHit is IInteractable interactable && hitDistance <= Global.Singleton.interactionRange && interactable.Active) interactable.Interact();
		}

		if (Input.IsActionPressed("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", zoomFOV, .05);
		}

		if (Input.IsActionJustReleased("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", walkingFOV, .1);
		}
		last_physics_pos = Position;
	}

	private async void Recharge(float duration)
	{
		recharging = true;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(rechargeBar, "value", 0, duration).From(100);
		await ToSignal(GetTree().CreateTimer(duration), "timeout");
		recharging = false;
	}

	float IDamageable.Health{ get{ return Global.Singleton.currentPlayerHealth; } set{}}

	void IDamageable.Damage(float amount)
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(hitFlash, "modulate", new Color(0,0,0,0), .25).From(new Color(1,0,0,1));
		Global.Singleton.IncrementHealth(-amount);
		Global.Singleton.PlaySound2D(hit);
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken() {return;}

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
		//GD.Print(currentHit?.GetClass().ToString());
		hitDistance = currentHit != null ? lookRay.GlobalTransform.Origin.DistanceTo(lookRay.GetCollisionPoint()) : 0;
		if (currentHit != existingHit)
		{
			if (existingHit != null)
			{
				if (existingHit is Enemy enemy) enemy.highlighted = false;
				if (existingHit is IInteractable interactable)
				{
					interactable.Highlighted = false;
					interactLabel.Visible = false;
				} 
			} 
			existingHit = currentHit;
			
			if (currentHit is Enemy || currentHit is IInteractable)
			{
				if (currentHit is Enemy enemy)
				{
					enemy.highlighted = true;
					reticle.Modulate = new Color(1,0,0);
				} 
				if (currentHit is IInteractable interactable && interactable.Active)
				{
					interactable.Highlighted = true;
					reticle.Modulate = new Color(0,0,1);
				}
			}
			else reticle.Modulate = new Color(1, 1, 1);
		}

		if (currentHit is IInteractable interactable1)
		{
			if (hitDistance <= Global.Singleton.interactionRange && interactable1.Active)
			{
				interactLabel.Visible = true;
			} else interactLabel.Visible = false;

			if (!interactable1.Active) reticle.Modulate = new Color(1, 1, 1);
		}
    }


    public void UpdateInput(float speed, float acceleration, float deceleration)
	{
		velocity = Velocity;
		hvel = new Vector2(velocity.X, velocity.Z);
		inputDir = Input.GetVector("A", "D", "W", "S");
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		acceleration /= (hvel.Length()* 2) + 1;

		if (direction != Vector3.Zero)
		{
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


	public override void _Input(InputEvent @event)
	{
		float sensitivityScale = win.Size.X / vp.GetVisibleRect().Size.X;
		if (@event is InputEventMouseMotion eventKey && !Global.Singleton.paused && !Global.Singleton.dead)
		{
			float xRot = -Mathf.DegToRad(eventKey.Relative.Y) * _sensitivity * sensitivityScale;
			head.RotateX(xRot);
			RotateY(-Mathf.DegToRad(eventKey.Relative.X) * _sensitivity * sensitivityScale);
			head.RotationDegrees = new Vector3(Mathf.Clamp(head.RotationDegrees.X, -80, 80), head.RotationDegrees.Y, head.RotationDegrees.Z);
			//hands.Rotation = new Vector3(hands.Rotation.X, Mathf.LerpAngle(Mathf.DegToRad(eventKey.Relative.X) * _sensitivity*10, 0, 0.25f), hands.Rotation.Z);
		}
	}

	private void Attack()
	{
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
		//rightHand.Position = handCastPosition;
		handSprite.Play("attack");
		await ToSignal(GetTree().CreateTimer(MathF.Min(0.4f, Global.basePlayerRechargeBuff * Global.Singleton.equippedWeapon.recharge)), "timeout");
		handSprite.Play(Global.Singleton.currentIdle);
		//rightHand.Position = handDefaultPosition;
		//if (stateMachine.current_state.Name == "glide") rightHand.Play("aim"); else rightHand.Play("idle");
	}
}