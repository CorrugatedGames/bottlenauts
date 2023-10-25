public partial class Character : CharacterBody3D
{
    #region Child nodes
    MeshInstance3D Mesh;
    #endregion

    [Export]
    float MaxHorizontalVelocity = 2.5f;

    [Export]
    float MaxVerticalVelocity = 2.8f;

    [Export]
    float Gravity = 9.8f;

    [Export(PropertyHint.Range, "1,8,1")]
    int Team = 1;
    public int PlayerNumber { get; private set; }

    private BNPlayer PlayerRef { get; set; }

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
        Mesh = GetNode("Mesh") as MeshInstance3D;

        // until we move away from TestCharacter
        PlayerRef =
            MatchSettingsState.GetPlayer(PlayerNumber)
            ?? MatchSettingsState.GetPlayer(MatchSettingsState.GenerateCPUPlayer());
        Mesh.SetSurfaceOverrideMaterial(
            0,
            new StandardMaterial3D() { AlbedoColor = PlayerRef.Color.ToColor() }
        );
    }

    public override void _Input(InputEvent @event) { }

    public override void _Process(double dt)
    {
        if (PlayerRef.IsDead)
            return;

        DeltaVelocity.X = DeltaVelocity.Z = 0;

        if (MPInput.IsActionPressed(PlayerNumber, "move_up"))
            DeltaVelocity.Z -= MPInput.GetActionStrength(PlayerNumber, "move_up");

        if (MPInput.IsActionPressed(PlayerNumber, "move_down"))
            DeltaVelocity.Z += MPInput.GetActionStrength(PlayerNumber, "move_down");

        if (MPInput.IsActionPressed(PlayerNumber, "move_right"))
            DeltaVelocity.X += MPInput.GetActionStrength(PlayerNumber, "move_right");

        if (MPInput.IsActionPressed(PlayerNumber, "move_left"))
            DeltaVelocity.X -= MPInput.GetActionStrength(PlayerNumber, "move_left");

        if (MPInput.IsActionPressed(PlayerNumber, "place_bomb"))
        {
            CreateBomb();
        }
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

    private bool CanPlaceBomb()
    {
        // TODO: Check if there is a bomb already placed at the current position, if so, bail

        return DateTime.Now >= PlaceBombCooldown;
    }

    private void CreateBomb()
    {
        if (!CanPlaceBomb())
            return;

        var d = BombScene.Instantiate() as Bomb;
        d.Team = Team;

        d.AddToGroup("Bomb");
        GetParent().GetNode("Bombs").AddChild(d);

        d.GlobalPosition = CurrentPosition + new Vector3(0.5f, 0f, 0.5f);

        SetBombCooldown();
    }

    private void SetBombCooldown()
    {
        PlaceBombCooldown = DateTime.Now.AddMilliseconds(500);
    }

    public void Die()
    {
        if (PlayerRef.IsDead)
            return;

        PlayerRef.IsDead = true;
        Print("Player has died.");
    }

    public void SetPlayerNumber(int playerNumber) =>
        SetPlayerNumber(playerNumber, playerNumber + 1);

    public void SetPlayerNumber(int playerNumber, int teamNumber)
    {
        PlayerNumber = playerNumber;
        Team = teamNumber;
    }
}
