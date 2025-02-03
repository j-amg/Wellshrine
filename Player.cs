using System;
using Godot;

public partial class Player : CharacterBody3D
{
	
	float mouseSensitivity = 0.15f;
	float aimMouseSensitivity = 0.075f;
	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;
	private bool pause = false;
	float zoomFOV = 80;
	float walkingFOV = 90;
	float sprintingFOV = 100;
	public Node3D body;
	public Node3D head;
	private Camera3D camera;
	private Node3D hands;
	public AnimatedSprite3D leftHand;
	public AnimatedSprite3D rightHand;
	private TextureRect reticle;
	public float _sensitivity;
	public float _gravity;
	public Vector3 velocity;
	public Vector2 hvel;
	public Vector2 inputDir;
	public Vector3 direction;
	private PackedScene bullet;
	public CollisionShape3D standCollision;
	public CollisionShape3D crouchCollision;
	Vector3 last_physics_pos;
	private StateMachine stateMachine;
	private RayCast3D lookRay;
	private Enemy existingHit;
	private bool toggled = false;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

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
		reticle = camera.GetNode<TextureRect>("reticle");
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		AddToGroup("player");
    }

    public override void _PhysicsProcess(double delta)
	{
		if (Godot.Input.IsActionJustPressed("ESC")) pause = !pause;
        Input.MouseMode = pause ? Godot.Input.MouseModeEnum.Visible : Godot.Input.MouseModeEnum.Captured;
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), hands.Rotation.Y, hands.Rotation.Z);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, velocity.Normalized().X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);
		if (Input.IsActionJustPressed("LeftMouse")) Shoot();
		if (Input.IsActionJustPressed("toggle")) Toggle();
		last_physics_pos = Position;
	}

	private void BuffStat(String stat, float amount)
	{

	}
    private void Toggle()
    {
		reticle.Visible = Global.Singleton.toggled;
		hands.Visible = Global.Singleton.toggled;
		Global.Singleton.toggled = !Global.Singleton.toggled;
       
    }

    public override void _Process(double delta)
    {
        float fraction = (float)Engine.GetPhysicsInterpolationFraction();
		GlobalTransform = new Transform3D(GlobalTransform.Basis, last_physics_pos.Lerp(GlobalTransform.Origin, fraction));
		Enemy currentHit = (Enemy)lookRay.GetCollider();
		if (currentHit != existingHit)
		{
			if (existingHit != null) existingHit.highlighted = false;
			existingHit = currentHit;
			if (currentHit != null)
			{
				reticle.Modulate = new Color(1,0,0);
				currentHit.highlighted = true;
			}
			else reticle.Modulate = new Color(1, 1, 1);
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
		if (@event is InputEventMouseMotion eventKey)
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
		Projectile b = bullet.Instantiate() as Projectile;
		var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", b);
		b.damage = 5;
		b.Transform = head.GlobalTransform;
		b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
	}
}