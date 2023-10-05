using Godot;
using System;

public partial class Bomb : Node3D
{
    public int Team { get; set; } = 1;

    private int Frame = 0;

    private Area3D area;
    private RigidBody3D physicsBody;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area = GetNode<Node3D>("Model").GetNode<Area3D>("Area3D");
        physicsBody = GetNode<Node3D>("Model").GetNode<RigidBody3D>("RigidBody3D");
        prepareBoom();
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
            physicsBody.SetCollisionLayerValue((int)GameLayer.Bomb, true);
        }
    }

    private async void prepareBoom()
    {
        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        boom();
    }

    private void boom()
    {
        Print("Boom!");
        GetParent().RemoveChild(this);
    }
}
