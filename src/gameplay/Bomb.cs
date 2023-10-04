using Godot;
using System;

public partial class Bomb : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        prepareBoom();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

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
