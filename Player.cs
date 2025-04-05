using Godot;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	private Node3D _neck;
	private Camera3D _camera;


	public override void _Ready()
	{
		_neck = GetNode<Node3D>("Neck");
		_camera = GetNode<Camera3D>("Neck/Camera3D");
	}

	public override void _UnhandledInput(InputEvent input)
	{
		SetInputMode(input);

		if (Input.MouseMode == Input.MouseModeEnum.Captured && input is InputEventMouseMotion motionInput)
		{
			MoveCamera(motionInput);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (_neck.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void MoveCamera(InputEventMouseMotion input)
	{
		// Rotate the neck and the camera.
		_neck.RotateY(-input.Relative.X * 0.01f);
		_camera.RotateX(-input.Relative.Y * 0.01f);

		// Restrict the rotation so we can't move the camera into extremes.
		Vector3 cameraRotation = _camera.Rotation;
		cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-30), Mathf.DegToRad(60));
		_camera.Rotation = cameraRotation;
	}

	private static void SetInputMode(InputEvent input)
	{
		if (input is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (input.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
