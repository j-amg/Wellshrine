using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;

public partial class Player : Entity
{
	// [Export]
	// public AudioStream damageTakenSound;
	// [Export]
	// public AudioStream damageDealtSound;
	// [Export]
	// public AudioStream critDealtSound;

	[Export]
	public AttackStateMachine attackStateMachine;
	[Export]
	public Node3D body;
	[Export]
	public Node3D head;
	[Export]
	public Camera3D camera;
	[Export]
	private Node3D hands;
	[Export]
	public AnimatedSprite3D handSprite;
	[Export]
	public Node3D hands3D;
	[Export]
	public CollisionShape3D standCollision;
	[Export]
	public CollisionShape3D crouchCollision;
	[Export]
	public Area3D wallDetection;

	[Export] public InventoryData[] inventoryData;
	[Export] public SpellData spellData;

	private float mouseSensitivity = 0.0025f;
	private float aimMouseSensitivity = 0.00125f;
	private float handsMaxXRot = 30f;
	private float handsMinXRot = -70f;
	private float handsMaxXPos = 0.05f;
	private float handsMovementSmoothing = 10;
	public float zoomFOV = 60;
	public float walkingFOV = 80;
	public float walkSpeed = 5;
	public float aimSpeed = 3;
	public float slideSpeed = 8; // Additive with current speed
	public float slideDelta = 6; // how fast the player changes direction during slide
	public float crouchSpeed = 3;
	public float crouchAnimSpeed = .2f;
	public float gravity = 12;
	public float acceleration = .8f;
	public float deceleration = .5f;
	public float airAcceleration = .1f;
	public float airDeceleration = 0.0f;
	public float dashVelocity = 10f;
	public float wallJumpVelocity = 2f;
	public float fallSpeed = 2;
	public float jumpVelocity = 7.5f;
	public float maxJumpCount = 2;
	public float maxDashCount = 1;
	public float wallJumpCD = 0.5f;
	public Vector3 bodyCrouchPosition = new(0, -0.3f, 0);
	public Vector3 bodyStandPosition = new(0, 0, 0);


	public float hitDistance;
	public float currentSpeed;
	public float currentJump;
	public float currentDash;
	public float _sensitivity;
	public Vector3 velocity;
	public Vector2 hvel;
	public Vector2 inputDir;
	public Vector3 direction;
	private GodotObject existingHit;
	public bool nearWall = false;
	public bool canWallJump = true;


	public Vector3 target_rotation;
	public Vector3 smooth_rotation = new();


	public override void _Ready()
	{
		base._Ready();
		wallDetection.BodyEntered += OnBodyEntered;
		wallDetection.BodyExited += OnBodyExited;

		// hands3D.GetNode<AnimationPlayer>("AnimationPlayer").Play("idle");
		velocity = Vector3.Zero;
		_sensitivity = mouseSensitivity;
		currentSpeed = walkSpeed;
		FloorMaxAngle = Mathf.DegToRad(65);
		FloorConstantSpeed = true;
		AddToGroup("player");
		target_rotation = Rotation;
	}

	public override void _PhysicsProcess(double delta)
	{

		smooth_rotation = smooth_rotation.Lerp(target_rotation, (float)delta * 20);
		Rotation = new Vector3(0, target_rotation.Y, 0);
		head.Rotation = new Vector3(target_rotation.X, 0, 0);
		hands.Rotation = new Vector3(Mathf.LerpAngle(hands.Rotation.X, Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(handsMinXRot), Mathf.DegToRad(handsMaxXRot)), (float)delta * handsMovementSmoothing), 0, 0);
		hands.Position = new Vector3(Mathf.Lerp(hands.Position.X, velocity.Normalized().X * handsMaxXPos, (float)delta * handsMovementSmoothing), Mathf.Lerp(hands.Position.Y, 1f, (float)delta * 5f), hands.Position.Z);

		GodotObject currentHit = lookRay.GetCollider();
		if (currentHit == null) return;
		hitDistance = lookRay.GlobalTransform.Origin.DistanceTo(lookRay.GetCollisionPoint());
		if (currentHit != existingHit)
		{
			if (existingHit is IHoverable hov) hov.EndHover();
			existingHit = currentHit;
		}

		Global.Singleton.hud.reticle.Modulate = currentHit is IHoverable hit && (currentHit as IHoverable).Active ? hit.ReticleModulate : new Color(1, 1, 1);
		Global.Singleton.hud.interactLabel.Visible = currentHit is IInteractable && (currentHit as IInteractable).Active && hitDistance <= Global.Singleton.interactionRange && !Global.Singleton.inDialogue;
		if (currentHit is IHoverable h) { if (hitDistance <= Global.Singleton.interactionRange && h.Active) h.StartHover(); else h.EndHover(); }

	}

	public override void _UnhandledInput(InputEvent @event)
	{
		float sensitivityScale = GetWindow().Size.X / GetViewport().GetVisibleRect().Size.X;
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion eventKey)
			{
				target_rotation.X -= eventKey.Relative.Y * mouseSensitivity * sensitivityScale;
				target_rotation.X = Mathf.Clamp(target_rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
				target_rotation.Y -= eventKey.Relative.X * mouseSensitivity * sensitivityScale;
			}

			if (Input.MouseMode == Input.MouseModeEnum.Visible)
			{
				GetViewport().SetInputAsHandled();
			}
			if (Input.IsActionJustPressed("interact"))
			{
				if (existingHit is IInteractable interactable && hitDistance <= Global.Singleton.interactionRange && interactable.Active) interactable.Interact();
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
	}

	public void UpdateInput(float speed, float acceleration, float deceleration)
	{

		velocity = Velocity;
		hvel = new Vector2(velocity.X, velocity.Z);
		inputDir = Input.GetVector("A", "D", "W", "S");
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		acceleration /= (hvel.Length() * 2) + 1;
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, acceleration);
		}
		else velocity = velocity.MoveToward(new Vector3(0, velocity.Y, 0), deceleration);
	}

	public void UpdateVelocity()
	{
		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnBodyExited(Node3D body) { if (body is GridMap) nearWall = false; }
	private void OnBodyEntered(Node3D body) { if (body is GridMap) nearWall = true; }

	public Vector3 GetDropPosition()
	{
		Vector3 dir = -camera.GlobalTransform.Basis.Z;
		return camera.GlobalPosition + dir;
	}

	public void PauseInput()
	{
		SetProcessInput(false);
	}

	public void ResumeInput()
	{
		SetProcessInput(true);
	}

	public async void WallJumpTimer()
	{
		canWallJump = false;
		await ToSignal(GetTree().CreateTimer(wallJumpCD), "timeout");
		canWallJump = true;
	}

	public void SetCrouch(bool val)
	{
		crouchCollision.Disabled = !val;
		standCollision.Disabled = val;
		Vector3 pos = val ? bodyCrouchPosition : bodyStandPosition;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(body, "position", pos, crouchAnimSpeed).SetTrans(Tween.TransitionType.Sine);
	}

	public void JumpAnim()
	{
		hands.Position = new Vector3(hands.Position.X, Mathf.Lerp(hands.Position.Y, hands.Position.Y - .1f, 5f), hands.Position.Z);
	}

	// public async void AttackAnim()
	// {
	// 	//Tween tween = GetTree().CreateTween();
	// 	//tween.TweenProperty(camera, "rotation_degrees", new Vector3(Global.Singleton.equippedWeapon.recoil, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .05);
	// 	//.TweenProperty(camera, "rotation_degrees", new Vector3(0, camera.RotationDegrees.Y, camera.RotationDegrees.Z), .25);
	// 	// handSprite.Play("attack");
	// 	await ToSignal(GetTree().CreateTimer(MathF.Min(0.4f, .2f)), "timeout");
	// }

	// private async void OpenPortal()
	// {

	// }
}
