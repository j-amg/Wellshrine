using System;
using System.Linq;
using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler();
	[Export]
	public AudioStream damageTakenSound;
	[Export]
	public AudioStream damageDealtSound;
	[Export]
	public AudioStream critDealtSound;
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
	public Area3D wallDetection;
	[Export]
	private Area3D iceCollision;
	[Export]
	private Area3D shockCollision;
	[Export]
	private PackedScene fireball;
	[Export]
	private PackedScene iceray;
	[Export]
	private AudioStream castFire;
	[Export]
	private AudioStream castIce;
	[Export]
	private AudioStream castShock;

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
	public float gravity = 12;
	public float acceleration = .5f;
	public float deceleration = .25f;
    public float airAcceleration = .1f;
    public float airDeceleration = 0.005f;
	public float dashVelocity = 15f;
	public float wallJumpVelocity = 10f;
    public float fallSpeed = 2;
	public float jumpVelocity = 7.5f;
	public float maxJumpCount = 2;
	public float maxDashCount = 1;
	public float wallJumpCD = 0.5f;

	public Vector3 bodyCrouchPosition = new(0, -0.3f, 0);
    public Vector3 bodyStandPosition = new (0,0,0);


	public float hitDistance;
	public float currentSpeed;
	public float currentJump;
	public float currentDash;
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
	public bool nearWall = false;
	public bool canWallJump = true;
	float IDamageable.Health{ get; set;}

	public override void _Ready()
	{
		vp = GetViewport();
		win = GetWindow();

		wallDetection.BodyEntered += OnBodyEntered;
		wallDetection.BodyExited += OnBodyExited;

		handSprite.Play(Global.Singleton.currentIdle);
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		currentSpeed = walkSpeed;
		FloorMaxAngle = Mathf.DegToRad(65);
		FloorConstantSpeed = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		AddToGroup("player");
    }

    private void OnBodyExited(Node3D body) { if (body is GridMap) nearWall = false; }
    private void OnBodyEntered(Node3D body) { if (body is GridMap) nearWall = true; }

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

	public async void wallJumpTimer()
	{
		canWallJump = false;
		await ToSignal(GetTree().CreateTimer(wallJumpCD), "timeout");
		canWallJump = true;
	}

    public override void _PhysicsProcess(double delta)
	{
		last_physics_pos = Position;
		if (inputPaused) return;
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), 0, 0);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, velocity.Normalized().X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);

		if (Input.IsActionJustPressed("LeftMouse") && !recharging)
		{
			if (Global.Singleton.equippedWeapon == null) return;
			Attack();
			Recharge(Global.Singleton.equippedWeapon.recharge * Global.Singleton.GetPlayerModifier("recharge"));
		}

		if (Input.IsActionJustPressed("interact"))
		{
			if (existingHit is IInteractable interactable && hitDistance <= Global.Singleton.interactionRange && interactable.Active) interactable.Interact();
		}

		if (Input.IsActionPressed("RightMouse"))
		{
			Global.Singleton.SetAction("aim");
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
		// Weird smoothnes fix
		float fraction = (float)Engine.GetPhysicsInterpolationFraction();
		if (applyTransform) { GlobalTransform = new Transform3D(GlobalTransform.Basis, last_physics_pos.Lerp(GlobalTransform.Origin, fraction)); }
		else { last_physics_pos = Position; applyTransform = true; }

		GodotObject currentHit = lookRay.GetCollider();
		hitDistance = currentHit != null ? lookRay.GlobalTransform.Origin.DistanceTo(lookRay.GetCollisionPoint()) : 0;
		if (currentHit != existingHit)
		{
			if (existingHit is IHoverable hov) hov.EndHover();
			existingHit = currentHit;
		}

		hud.reticle.Modulate = currentHit is IHoverable hit && (currentHit as IHoverable).Active ? hit.ReticleModulate : new Color(1, 1, 1);
		hud.interactLabel.Visible = currentHit is IInteractable && (currentHit as IInteractable).Active && hitDistance <= Global.Singleton.interactionRange && !Global.Singleton.inDialogue;
		if (currentHit is IHoverable h)
		{
			if (hitDistance <= h.HoverRange && h.Active) h.StartHover(); else h.EndHover();
		}

		GD.Print(nearWall);
		GD.Print(wallDetection.GetOverlappingBodies());
    }

	public override void _Input(InputEvent @event)
	{
		float sensitivityScale = win.Size.X / vp.GetVisibleRect().Size.X;
		if (@event is InputEventMouseMotion eventKey)
		{
			Global.Singleton.SetAction("look");
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

	void IDamageable.Damage(Damage d)
	{
		Global.Singleton.IncrementPlayerHealth(-d.amount);
		Global.Singleton.PlaySound2D(damageTakenSound);
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
			Global.Singleton.SetAction("walk");
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
		Damage d = Global.Singleton.GetPlayerDamage();
		d.damageExecuted += OnDamageExecuted;
		Global.Singleton.SetAction("attack");
		AttackAnim();
		if (Global.Singleton.equippedWeapon.name == "fireball")
		{
			Global.Singleton.PlaySound2D(castFire);
			Projectile b = fireball.Instantiate() as Projectile;
			var main = GetTree().CurrentScene;
			main.CallDeferred("add_child", b);
			b.damage = d;
			b.Transform = head.GlobalTransform;
			b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
		}

		if (Global.Singleton.equippedWeapon.name == "icespike")
		{
			Global.Singleton.PlaySound2D(castIce);
			foreach (IDamageable enemy in iceCollision.GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(d);
			foreach (Area3D area in iceCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
			IceRay ray = iceray.Instantiate() as IceRay;
			var main = GetTree().CurrentScene;
			main.CallDeferred("add_child", ray);
			ray.Transform = head.GlobalTransform;
		}

		if (Global.Singleton.equippedWeapon.name == "shockburst")
		{
			Global.Singleton.PlaySound2D(castShock);
			foreach (IDamageable enemy in shockCollision.GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(d);
			foreach (Area3D area in shockCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
			camera.GetNode<AnimatedSprite3D>("shock").Play("cycle");
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera.GetNode<AnimatedSprite3D>("shock"), "modulate", new Color(0,0,0,0), .25).From(new Color(1,1,1,.75f));

		}
	}

    private void OnDamageExecuted(Damage d)
    {
		if (d.crit) Global.Singleton.PlaySound2D(critDealtSound);
		Global.Singleton.PlaySound2D(damageDealtSound);
		hud.FlashCrossHair();
    }

    private async void AttackAnim()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(Global.Singleton.equippedWeapon.recoil, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .05);
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(0, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .25);
		handSprite.Play("attack");
		await ToSignal(GetTree().CreateTimer(MathF.Min(0.4f, Global.Singleton.GetPlayerModifier("recharge") * Global.Singleton.equippedWeapon.recharge)), "timeout");
		handSprite.Play(Global.Singleton.currentIdle);
	}
}