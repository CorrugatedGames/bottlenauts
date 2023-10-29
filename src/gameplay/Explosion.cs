public partial class Explosion : Node3D
{
    #region Signals
    [Signal] public delegate void CharacterHitEventHandler (Character character);
    [Signal] public delegate void DestructibleHitEventHandler (Destructible destru);
    [Signal] public delegate void BombHitEventHandler (Bomb bomb);
    [Signal] public delegate void ExpansionFinishedEventHandler ();
    #endregion

    public int Team { get; set; } = 1;
    private int Frame = 0;
    private int MaxFrames = 60;

    private Area3D area;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area = GetNode<Area3D>("MeshInstance3D/Area3D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Frame++;
        Expandify();
    }

    public void _on_area_3d_body_entered(Node3D node)
    {
        if (node is Character character)
        {
            EmitSignal(SignalName.CharacterHit, character);
            character.Die();
        }
        else if (node is Destructible destructible)
        {
            EmitSignal(SignalName.DestructibleHit, destructible);
            destructible.GetBlownUp();
        }
        else if (node is Bomb bomb)
        {
            EmitSignal(SignalName.BombHit, bomb);
            bomb.Boom();
        }
    }

    private void Expandify()
    {
        Scale += new Vector3(0.05f, 0.05f, 0.05f);

        if (Frame > MaxFrames)
        {
            FinishExpanding();
        }
    }

    private void FinishExpanding()
    {
        EmitSignal(SignalName.ExpansionFinished);
        GetParent().RemoveChild(this);
        QueueFree();
    }
}
