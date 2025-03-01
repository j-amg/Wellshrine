using System;
using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler();
	float mouseSensitivity = 0.15f;
	float aimMouseSensitivity = 0.075f;
	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;
	float zoomFOV = 70;
	float walkingFOV = 80;
	float sprintingFOV = 100;
	public Node3D body;
	public Node3D head;
	private Camera3D camera;
	public Hud hud;
	private Node3D hands;
	public AnimatedSprite3D leftHand;
	public AnimatedSprite3D rightHand;
	public TextureRect reticle;
	public float _sensitivity;
	public float _gravity;
	public Vector3 velocity;
	public Vector2 hvel;
	public Vector2 inputDir;
	public Vector3 direction;
	private PackedScene bullet;
	public CollisionShape3D standCollision;
	public CollisionShape3D crouchCollision;
	private Vector3 last_physics_pos;
	private StateMachine stateMachine;
	private RayCast3D lookRay;
	private GodotObject existingHit;
	private TextureRect hitFlash;
	private string equippedWeapon;
	private float attackRecharge = 1f;
	private bool recharging = false;
	private ProgressBar rechargeBar;
	private Label interactLabel;
	public float bodyCrouchHeight = -0.3f;
    public float bodyStandHeight = 0;
	public float crouchSpeed = 0.2f;
	public float hitDistance = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	//public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float gravity = 12;

    public override void _Ready()
    {
		bullet = ResourceLoader.Load<PackedScene>("res://playerProjectile.tscn");
		body = GetNode<Node3D>("body");
        head = body.GetNode<Node3D>("head");
		camera = head.GetNode<Camera3D>("Camera3D");
		hands = body.GetNode<Node3D>("hands");
		rightHand = hands.GetNode<AnimatedSprite3D>("rightHand");
		leftHand = hands.GetNode<AnimatedSprite3D>("leftHand");
		lookRay = camera.GetNode<RayCast3D>("lookRay");
		standCollision = GetNode<CollisionShape3D>("standCollision");
		crouchCollision = GetNode<CollisionShape3D>("crouchCollision");
		stateMachine = GetNode<StateMachine>("playerStateMachine");
		reticle = camera.GetNode<TextureRect>("CanvasLayer/reticle");
		rechargeBar = camera.GetNode<ProgressBar>("CanvasLayer/recharge");
		hitFlash = camera.GetNode<TextureRect>("CanvasLayer/hit");
		interactLabel = camera.GetNode<Label>("CanvasLayer/interactLabel");
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		AddToGroup("player");
    }

    public override void _PhysicsProcess(double delta)
	{
		if (Global.Singleton.paused) return;
		//if (Godot.Input.IsActionJustPressed("ESC")) pause = !pause;
        Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), hands.Rotation.Y, hands.Rotation.Z);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, velocity.Normalized().X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);

		//GD.Print(head.GlobalBasis.Z);

		if (Input.IsActionJustPressed("LeftMouse") && !recharging)
		{
			Shoot();
			Recharge(attackRecharge);
		}

		if (Input.IsActionJustPressed("interact"))
		{
			if (existingHit is IInteractable interactable && hitDistance <= Global.Singleton.interactionRange && interactable.Active) interactable.Interact();
		}

		if (Input.IsActionPressed("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", zoomFOV, .1);
		}

		if (Input.IsActionJustReleased("RightMouse"))
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(camera, "fov", walkingFOV, .25);
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
		GD.Print("player takes damage");
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(hitFlash, "modulate", new Color(0,0,0,0), .25).From(new Color(1,1,1,1));
		Global.Singleton.IncrementHealth(-amount);
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken() {return;}

    public override void _Process(double delta)
    {
        float fraction = (float)Engine.GetPhysicsInterpolationFraction();
		GlobalTransform = new Transform3D(GlobalTransform.Basis, last_physics_pos.Lerp(GlobalTransform.Origin, fraction));
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
		//GD.Print(hvel);
		//GD.Print(speed);
	}

	public void UpdateVelocity()
	{
		Velocity = velocity;
		MoveAndSlide();
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventKey && !Global.Singleton.paused && !Global.Singleton.dead)
		{
			float xRot = -Mathf.DegToRad(eventKey.Relative.Y) * _sensitivity;
			head.RotateX(xRot);
			RotateY(-Mathf.DegToRad(eventKey.Relative.X) * _sensitivity);
			head.RotationDegrees = new Vector3(Mathf.Clamp(head.RotationDegrees.X, -80, 80), head.RotationDegrees.Y, head.RotationDegrees.Z);
		}
	}

	private void Shoot()
	{
		//GD.Print("shoot");
		ShootAnim();
		//leftHand.Play("shoot");
		Projectile b = bullet.Instantiate() as Projectile;
		var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", b);
		b.damage = GD.Randf() <= .25f ? 5 * Global.Singleton.playerDamageBuff * Global.Singleton.playerCritDamageBuff : 5 * Global.Singleton.playerDamageBuff;
		b.Transform = head.GlobalTransform;
		b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
	}

	private async void ShootAnim()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(5, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .1);
		tween.TweenProperty(camera, "rotation_degrees", new Vector3(0, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .5);
		rightHand.Play("shoot");
		await ToSignal(GetTree().CreateTimer(.25), "timeout");
		if (stateMachine.current_state.Name == "glide")
		{
			rightHand.Play("aim");
		} else {
		rightHand.Play("idle");
		}
	}
}