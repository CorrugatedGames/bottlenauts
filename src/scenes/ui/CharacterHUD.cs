using Godot;
using System;

public partial class CharacterHUD : MarginContainer
{
    RichTextLabel PlayerNumberLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerNumberLabel = GetNode("%PlayerNumberLabel") as RichTextLabel;
        int playerNumber = (int)GetMeta("PlayerNumber");

        var player = MatchSettingsState.GetPlayer(playerNumber);

        Print(playerNumber);
        Print(player);

        if (player == null)
        {
            Visible = false;
            return;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
