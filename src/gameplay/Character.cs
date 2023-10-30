using System.Linq;

public partial class Character : CharacterBody3D
{
    #region Signals
    [Signal]
    public delegate void InstancedEventHandler(int playerNumber, Vector3 position);

    [Signal]
    public delegate void BombPlacedEventHandler(int playerNumber, Vector3 bombPosition);

    [Signal]
    public delegate void DiedEventHandler(int playerNumber);
    #endregion

    #region Child nodes
    MeshInstance3D Mesh;
    Timer BombCooldownTimer;
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

    // private DateTime PlaceBombCooldown = DateTime.Now;
    [Export]
    int BombCooldownMs { get; set; } = 500;
    bool IsBombCooldown = false;

    private Vector3I CurrentPosition
    {
        get =>
            new(
                Mathf.FloorToInt(GlobalPosition.X),
                Mathf.FloorToInt(GlobalPosition.Y),
                Mathf.FloorToInt(GlobalPosition.Z)
            );
    }

    public override void _Ready()
    {
        BombScene = ResourceLoader.Load("res://scenes/hazards/Bomb.tscn") as PackedScene;
        Mesh = GetNode("Mesh") as MeshInstance3D;
        BombCooldownTimer = GetNode("BombCooldownTimer") as Timer;
        BombCooldownTimer.Timeout += OnBombCooldownTimerTimeout;

        // until we move away from TestCharacter
        PlayerRef =
            MatchSettingsState.GetPlayer(PlayerNumber)
            ?? MatchSettingsState.GetPlayer(MatchSettingsState.GenerateCPUPlayer());
        Mesh.SetSurfaceOverrideMaterial(
            0,
            new StandardMaterial3D() { AlbedoColor = PlayerRef.Color.ToColor() }
        );

        EmitSignal(SignalName.Instanced, PlayerNumber, GlobalPosition);
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

    private void OnBombCooldownTimerTimeout() => IsBombCooldown = false;

    private bool CanPlaceBomb()
    {
        bool alreadyBombAtLocation = GetParent()
            .GetNode("Bombs")
            .GetChildren()
            .Any(
                b =>
                    b is Bomb
                    && (b as Bomb).GlobalPosition.IsEqualApprox(CurrentPosition.SnapToGrid())
            );

        return !IsBombCooldown && !alreadyBombAtLocation;
    }

    private void CreateBomb()
    {
        if (!CanPlaceBomb())
            return;

        var d = BombScene.Instantiate() as Bomb;
        d.Team = Team;

        d.AddToGroup("Bomb");
        GetParent().GetNode("Bombs").AddChild(d);

        d.GlobalPosition = CurrentPosition.SnapToGrid();

        EmitSignal(SignalName.BombPlaced, PlayerNumber, d.GlobalPosition);
        SetBombCooldown();
    }

    private void SetBombCooldown()
    {
        // PlaceBombCooldown = DateTime.Now.AddMilliseconds(500);
        BombCooldownTimer.Start(BombCooldownMs / 1000f);
        IsBombCooldown = true;
    }

    public void Die()
    {
        if (PlayerRef.IsDead)
            return;

        PlayerRef.IsDead = true;
        DeltaVelocity = Vector3.Zero;
        EmitSignal(SignalName.Died, PlayerNumber);
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
