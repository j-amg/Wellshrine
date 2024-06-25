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

	private Node3D head;
	private Camera3D camera;
	private Node3D hands;
	private AnimatedSprite3D leftHand;
	private AnimatedSprite3D rightHand;

	public float _sensitivity;
	public float _gravity;

	public Vector3 velocity;

	private PackedScene bullet;

	private CollisionShape3D standCollision;
	private CollisionShape3D crouchCollision;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		bullet = ResourceLoader.Load<PackedScene>("res://projectile.tscn");
        head = GetNode<Node3D>("head");
		camera = head.GetNode<Camera3D>("Camera3D");
		hands = GetNode<Node3D>("hands");
		rightHand = hands.GetNode<AnimatedSprite3D>("rightHand");
		leftHand = hands.GetNode<AnimatedSprite3D>("leftHand");
		standCollision = GetNode<CollisionShape3D>("standCollision");
		crouchCollision = GetNode<CollisionShape3D>("crouchCollision");

		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
    }

    public override void _PhysicsProcess(double delta)
	{

		//GD.Print(Engine.GetFramesPerSecond());
		//GD.Print(delta);
		if (Godot.Input.IsActionJustPressed("ESC")) pause = !pause;
        Input.MouseMode = pause ? Godot.Input.MouseModeEnum.Visible : Godot.Input.MouseModeEnum.Captured;
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), hands.Rotation.Y, hands.Rotation.Z);
		//hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, inputDir.X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);

		// if (Input.IsActionPressed("RightMouse"))
		// {
		// 	leftHand.Play("aim");
		// 	rightHand.Play("aim");
		// 	if (velocity.Y > 0)
		// 	{
		// 		velocity.Y = Mathf.MoveToward(velocity.Y, 0, 0.5f);
		// 	}
		// } 


		// if (Input.IsActionJustPressed("LeftMouse"))
		// {
		// 	Shoot();
		// }
	}

	public void UpdateGravity(double delta)
	{
		if(!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}
		else
		{
			velocity.Y = 0;
		}
		
	}

	public void UpdateInput(float speed, float acceleration, float deceleration)
	{
		Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, acceleration);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, deceleration);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, deceleration);
		}
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
		b.Transform = head.GlobalTransform;
		b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
	}
}