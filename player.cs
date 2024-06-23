using System;
using Godot;

public partial class player : CharacterBody3D
{
	
	float mouseSensitivity = 0.15f;
	float aimMouseSensitivity = 0.075f;

	float walkSpeed = 4;
	float runSpeed = 10;
	float JumpVelocity = 4.5f;
	float glideGravity = 3;

	float friction = 50.0f;
	float midAirFriction = 0.05f;
	float midAirControlledFriction = 10.0f;
	float airJumpFriction = 350.0f;



	private bool pause = false;
	public bool isRunning = false;
	public bool isWalking = true;
	public bool isCrouching = false;
	public bool isSliding = false;

	float zoomFOV = 80;
	float walkingFOV = 90;
	float sprintingFOV = 100;

	private Node3D head;
	private Camera3D camera;
	bool canJump;
	Vector3 movementDirection;

	float speedAtJump;
	float currentGravity;
	float currentFriction;
	float currentSpeed;
	float currentMouseSensitivity;




	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();



    public override void _Ready()
    {
        head = GetNode<Node3D>("head");
		camera = head.GetNode<Camera3D>("Camera3D");
		currentGravity = gravity;
		currentMouseSensitivity = mouseSensitivity;
    }

    public override void _PhysicsProcess(double delta)
	{

		GD.Print(Engine.GetFramesPerSecond());
		if (Godot.Input.IsActionJustPressed("ESC")) pause = !pause;
        Input.MouseMode = pause ? Godot.Input.MouseModeEnum.Captured : Godot.Input.MouseModeEnum.Visible;

		Vector3 velocity = Velocity;
		Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (IsOnFloor())
		{
			GD.Print("on floor");
			isWalking = true;
			canJump = true;
			speedAtJump = currentSpeed;
			camera.Fov = Mathf.Lerp(camera.Fov, 90, 0.2f);
			currentGravity = gravity;
			currentSpeed = walkSpeed;
			currentFriction = friction;
		}



		if (!IsOnFloor())
		{
			if (Input.IsActionPressed("aim"))
			{
				currentGravity = glideGravity;
			}
			else
			{
				currentGravity = gravity;
			}
			GD.Print("in air");
			currentSpeed = speedAtJump;
			velocity.Y -= currentGravity * (float)delta;
			currentFriction = (inputDir != Vector2.Zero) ? midAirControlledFriction : midAirFriction;
		}

		if (Input.IsActionPressed("aim"))
		{
			if (velocity.Y > 0)
			{
				velocity.Y = Mathf.MoveToward(velocity.Y, 0, 0.5f);
			}
			camera.Fov = Mathf.Lerp(camera.Fov, 80, 0.2f);
			
			currentMouseSensitivity = aimMouseSensitivity;
		} else
		{
			camera.Fov = Mathf.Lerp(camera.Fov, 90, 0.2f);
			currentMouseSensitivity = mouseSensitivity;
		}



		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && canJump)
		{
			if (!IsOnFloor())
			{
				currentFriction = airJumpFriction;
				canJump = false;
			}
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionPressed("sprint") && IsOnFloor())
		{
			isRunning = true;
			currentSpeed = runSpeed;
			//camera.Fov = Mathf.Lerp(camera.Fov, 100, 0.25f);
		}

		float turningRate = currentFriction / ( velocity.Length() + 2.5f);

		GD.Print(delta);

		movementDirection = movementDirection.Lerp(direction, (float)delta * turningRate);

		
		if (movementDirection != Vector3.Zero)
		{
			velocity.X = movementDirection.X * currentSpeed;
			velocity.Z = movementDirection.Z * currentSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, currentFriction);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, currentFriction);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventKey)
		{
			head.RotateX(-Mathf.DegToRad(eventKey.Relative.Y) * currentMouseSensitivity);
			RotateY(-Mathf.DegToRad(eventKey.Relative.X) * currentMouseSensitivity);
			head.RotationDegrees = new Vector3(Mathf.Clamp(head.RotationDegrees.X, -80, 80), head.RotationDegrees.Y, head.RotationDegrees.Z);
		}
	}



}
