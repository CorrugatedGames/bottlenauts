public partial class Character : CharacterBody3D
{
    [Export]
    float MaxHorizontalVelocity = 2.5f;

    [Export]
    float MaxVerticalVelocity = 2.8f;

    [Export]
    float Gravity = 9.8f;

    [Export(PropertyHint.Range, "1,8,1")]
    int Team = 1;

    Vector3 DeltaVelocity = Vector3.Zero;

    PackedScene BombScene;

    private DateTime PlaceBombCooldown = DateTime.Now;

    private Vector3I CurrentPosition
    {
        get
        {
            return new Vector3I(
                (int)Math.Floor(GlobalPosition.X),
                (int)Math.Floor(GlobalPosition.Y),
                (int)Math.Floor(GlobalPosition.Z)
            );
        }
    }

    public override void _Ready()
    {
        BombScene = ResourceLoader.Load("res://scenes/hazards/Bomb.tscn") as PackedScene;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("place_bomb") && canPlaceBomb())
        {
            var d = BombScene.Instantiate() as Bomb;
            d.Team = Team;

            d.GlobalPosition = CurrentPosition + new Vector3(0.5f, 0f, 0.5f);
            d.AddToGroup("Bomb");
            GetParent().GetNode("Bombs").AddChild(d);

            setBombCooldown();
        }
    }

    public override void _Process(double dt)
    {
        DeltaVelocity.X = DeltaVelocity.Z = 0;

        if (Input.IsActionPressed("move_up"))
            DeltaVelocity.X -= Input.GetActionStrength("move_up");

        if (Input.IsActionPressed("move_down"))
            DeltaVelocity.X += Input.GetActionStrength("move_down");

        if (Input.IsActionPressed("move_right"))
            DeltaVelocity.Z -= Input.GetActionStrength("move_right");

        if (Input.IsActionPressed("move_left"))
            DeltaVelocity.Z += Input.GetActionStrength("move_left");
    }

    public override void _PhysicsProcess(double dt)
    {
        if (!IsOnFloor())
            DeltaVelocity.Y -= Gravity * (float)dt;
        else
            DeltaVelocity.Y = 0;

        Vector2 deltaH =
            new Vector2(DeltaVelocity.X, DeltaVelocity.Z).Normalized() * MaxHorizontalVelocity;
        float deltaY = Mathf.Clamp(Velocity.Y + DeltaVelocity.Y, -MaxVerticalVelocity, 0);

        Velocity = new Vector3(deltaH.X, deltaY, deltaH.Y);

        MoveAndSlide();
    }

    private bool canPlaceBomb()
    {
        // TODO: Check if there is a bomb already placed at the current position, if so, bail

        return DateTime.Now >= PlaceBombCooldown;
    }

    private void setBombCooldown()
    {
        PlaceBombCooldown = DateTime.Now.AddMilliseconds(500);
    }
}
