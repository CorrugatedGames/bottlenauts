public partial class Character : CharacterBody3D
{
  [Export] float MaxHorizontalVelocity = 2.5f;
  [Export] float MaxVerticalVelocity = 2.8f;

  [Export] float Gravity = 9.8f;

  Vector3 deltaVelocity = Vector3.Zero;

  public override void _Process (double dt)
  {
    deltaVelocity.X = deltaVelocity.Z = 0;

    if (Input.IsActionPressed("move_up"))
      deltaVelocity.X -= Input.GetActionStrength("move_up");

    if (Input.IsActionPressed("move_down"))
      deltaVelocity.X += Input.GetActionStrength("move_down");

    if (Input.IsActionPressed("move_right"))
      deltaVelocity.Z -= Input.GetActionStrength("move_right");

    if (Input.IsActionPressed("move_left"))
      deltaVelocity.Z += Input.GetActionStrength("move_left");
  }

  public override void _PhysicsProcess (double dt)
  {
    Vector3 updatedVelocity = Velocity;

    if (!IsOnFloor())
      deltaVelocity.Y -= Gravity * (float)dt;
    else
      deltaVelocity.Y = 0;

    Vector2 deltaH = new Vector2(deltaVelocity.X, deltaVelocity.Z).Normalized() * MaxHorizontalVelocity;
    float deltaY = Mathf.Clamp(Velocity.Y + deltaVelocity.Y, -MaxVerticalVelocity, 0);

    // Print(deltaH);

    Velocity = new Vector3(deltaH.X, deltaY, deltaH.Y);

    MoveAndSlide();
  }
}
