extends CharacterBody3D


const SPEED = 5.0
const JUMP_VELOCITY = 4.5
var pause = false
var camera_sensitivity = 100

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")


func _physics_process(delta):
	
	if Input.is_action_just_pressed("ui_accept"):
		pause = false if pause else true
			
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE if pause else Input.MOUSE_MODE_CAPTURED

	
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle jump.
	#if Input.is_action_just_pressed("ui_accept") and is_on_floor():
	#	velocity.y = JUMP_VELOCITY
	

		
	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	#var input_dir = Input.get_vector("A", "D", "W", "S")
	var input_dir = Input.get_vector("A", "D", "W", "S")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	
	#var direction = rotation.rotate($SpringArm3D.rotation.y)
	
	if Input.is_action_pressed("W"):
		rotation.y = $SpringArm3D.rotation.y
		$SpringArm3D.rotation.y = rotation.y
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()
	
func _input(event):
	if event is InputEventMouseMotion and pause == false:
		$SpringArm3D.rotation.y -= event.relative.x / camera_sensitivity
		$SpringArm3D.rotation.x -= event.relative.y / camera_sensitivity
		$SpringArm3D.rotation.x = clamp($SpringArm3D.rotation.x, deg_to_rad(-90), deg_to_rad(90))
