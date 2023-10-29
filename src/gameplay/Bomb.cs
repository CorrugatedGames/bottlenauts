public partial class Bomb : RigidBody3D
{
    #region Signals
    [Signal] public delegate void InstancedEventHandler (int teamNumber, Vector3I position);
    [Signal] public delegate void ExplodedEventHandler (int teamNumber, Vector3I position);
    #endregion

    public int Team { get; set; } = 1;

    private int Frame = 0;

    private Area3D area;

    private bool HasExploded = false;

    [Export] float BaseDetonationTime = 3f;
    float DetonationTime {
        get {
            float time = BaseDetonationTime;
            if (MatchSettingsState.CheckGameModifier(GameModifier.DelayedBombs))
                time *= 2f;
            if (MatchSettingsState.CheckGameModifier(GameModifier.FastBombs))
                time *= 0.5f;

            return time;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area = GetNode<Area3D>("Area3D");

        Freeze = true; // on bomb throw, set this to false so we can apply force to it

        EmitSignal(SignalName.Instanced, Team, GlobalPosition);
        PrepareBoom();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Frame++;
    }

    public override void _PhysicsProcess(double delta)
    {
        // there's no collision on the first frame, give a bit of grace before processing physics
        if (Frame < 4)
            return;

        // check how many things are colliding with the bomb
        // if none, set the physics bit
        if (area.GetOverlappingBodies().Count == 0)
        {
            SetCollisionLayerValue((int)GameLayer.Bomb, true);
        }
    }

    private async void PrepareBoom()
    {
        await ToSignal(GetTree().CreateTimer(DetonationTime), SceneTreeTimer.SignalName.Timeout);

        Boom();
    }

    public void Boom()
    {
        if (HasExploded)
            return;

        HasExploded = true;

        var ExplosionScene =
            ResourceLoader.Load("res://scenes/hazards/Explosion.tscn") as PackedScene;
        var explosion = ExplosionScene.Instantiate() as Explosion;
        explosion.Team = Team;

        explosion.AddToGroup("Explosion");
        GetParent().GetParent().GetNode("Explosions").AddChild(explosion);

        explosion.GlobalPosition = GlobalPosition;
        EmitSignal(SignalName.Exploded, Team, GlobalPosition);

        GetParent().RemoveChild(this);

        QueueFree();
    }
}
