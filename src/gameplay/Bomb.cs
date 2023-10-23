using Godot;
using System;

public partial class Bomb : RigidBody3D
{
    public int Team { get; set; } = 1;

    private int Frame = 0;

    private Area3D area;

    private bool HasExploded = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area = GetNode<Area3D>("Area3D");

        Freeze = true; // on bomb throw, set this to false so we can apply force to it

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
        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

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

        GetParent().RemoveChild(this);

        QueueFree();
    }
}
