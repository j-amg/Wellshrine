using System;
using Godot;

public partial class Player : CharacterBody3D
{
	
	float mouseSensitivity = 0.15f;
	float aimMouseSensitivity = 0.075f;

	float crouchSpeed = 2;
	float walkSpeed = 4;
	float runSpeed = 7;
	float slideSpeed = 5;
	float jumpVelocity = 4.5f;
	float dashVelocity = 10f;
	float glideGravity = 1.5f;
	float dashDuration = .5f;

	float friction = 50.0f;
	float midAirFriction = 0.05f;
	float midAirControlledFriction = 10.0f;
	float airJumpFriction = 350.0f;
	float slideFriction = .5f;
	float dashFriction = 0;


	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;

	private bool pause = false;
	public bool isRunning = false;
	public bool isWalking = true;
	public bool isCrouching = false;
	public bool isSliding = false;
	public bool isDashing = false;

	float zoomFOV = 80;
	float walkingFOV = 90;
	float sprintingFOV = 100;

	private Node3D head;
	private Camera3D camera;
	private Node3D hands;
	private AnimatedSprite3D leftHand;
	private AnimatedSprite3D rightHand;
	bool canJump;
	Vector3 movementDirection;

	float speedAtJump;
	float currentGravity;
	float currentFriction;
	float currentSpeed;
	float currentMouseSensitivity;

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


		currentGravity = gravity;
		currentMouseSensitivity = mouseSensitivity;
    }

    public override void _PhysicsProcess(double delta)
	{

		//GD.Print(Engine.GetFramesPerSecond());
		if (Godot.Input.IsActionJustPressed("ESC")) pause = !pause;
        Input.MouseMode = pause ? Godot.Input.MouseModeEnum.Visible : Godot.Input.MouseModeEnum.Captured;

		Vector3 velocity = Velocity;
		Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), hands.Rotation.Y, hands.Rotation.Z);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, inputDir.X * handsMaxXPos, (float)delta * handsMovementSmoothing), hands.Position.Y, hands.Position.Z);

		if (IsOnFloor() && !isCrouching && !isDashing)
		{
			//GD.Print("on floor");
			isWalking = true;
			canJump = true;
			speedAtJump = velocity.Length();
			camera.Fov = Mathf.Lerp(camera.Fov, 90, 0.2f);
			currentGravity = gravity;
			currentSpeed = walkSpeed;
			currentFriction = Mathf.Lerp(currentFriction, friction, .025f);
		}

		if (!IsOnFloor())
		{
			if (Input.IsActionPressed("RightMouse"))
			{
				currentGravity = glideGravity;
			}
			else
			{
				currentGravity = gravity;
			}
			
			if (!isDashing)
			{
				currentSpeed = speedAtJump;
				currentFriction = (inputDir != Vector2.Zero) ? midAirControlledFriction : midAirFriction;
			}

			velocity.Y -= currentGravity * (float)delta;
		}

		if (Input.IsActionPressed("RightMouse"))
		{
			leftHand.Play("aim");
			rightHand.Play("aim");
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
			leftHand.Play("idle");
			rightHand.Play("idle");
		}


		if (Input.IsActionJustPressed("LeftMouse"))
		{
			Shoot();
		}




		if (Input.IsActionPressed("Alt") && IsOnFloor() && !isCrouching && !isDashing)
		{
			isRunning = true;
			currentSpeed = runSpeed;
			//camera.Fov = Mathf.Lerp(camera.Fov, 100, 0.25f);
		}

		if (Input.IsActionJustPressed("Shift") && IsOnFloor() && inputDir != Vector2.Zero)
		{
			GD.Print("should be moving");
			//currentSpeed = 15;
			currentSpeed = Mathf.Lerp(currentSpeed, currentSpeed + slideSpeed, .75f);
			isSliding = true;
		}

		if (Input.IsActionPressed("Shift"))
		{
			isCrouching = true;
			if (IsOnFloor())
			{
				currentFriction = slideFriction;
				currentSpeed = Mathf.MoveToward(currentSpeed, crouchSpeed, (float)delta * 10f);

				if (currentSpeed <= crouchSpeed)
				{
					isSliding = false;
					currentFriction = friction;
				}

			}
			standCollision.CallDeferred("set_disabled", true);
			crouchCollision.CallDeferred("set_disabled", false);
		}
		else
		{
			isCrouching = false;
			standCollision.CallDeferred("set_disabled", false);
			crouchCollision.CallDeferred("set_disabled", true);
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && canJump && !isCrouching)
		{
			if (!IsOnFloor())
			{
				currentFriction = airJumpFriction;
				canJump = false;
			}
			velocity.Y = jumpVelocity;
		}

		if (Input.IsActionJustPressed("ui_accept") && canJump && isCrouching)
		{
			Dash();
			currentSpeed = 20;
			velocity = -camera.GlobalBasis.Z.Normalized() * 10;
		}

		float turningRate = currentFriction / ( velocity.Length() + 2.5f);

		if (!isDashing)
		{
			movementDirection = movementDirection.Lerp(direction, (float)delta * turningRate);
		}
		

		
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

		GD.Print(currentFriction);

		Velocity = velocity;
		MoveAndSlide();
	}

	private async void Dash()
	{
		isDashing = true;
		currentFriction = dashFriction;
		await ToSignal(GetTree().CreateTimer(dashDuration), "timeout");
		isDashing = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventKey)
		{
			float xRot = -Mathf.DegToRad(eventKey.Relative.Y) * currentMouseSensitivity;
			head.RotateX(xRot);
			RotateY(-Mathf.DegToRad(eventKey.Relative.X) * currentMouseSensitivity);
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