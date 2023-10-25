using Godot;
using System;

public partial class Explosion : Node3D
{
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
            character.Die();
        }
        else if (node is Destructible destructible)
        {
            destructible.GetBlownUp();
        }
        else if (node is Bomb bomb)
        {
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
        GetParent().RemoveChild(this);
        QueueFree();
    }
}
