using Godot;
using System;

public partial class CharacterHUD : MarginContainer
{
    Label PlayerNumberLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerNumberLabel = GetNode("%PlayerNumberLabel") as Label;
        int playerNumber = (int)GetMeta("PlayerNumber");

        var player = MatchSettingsState.GetPlayer(playerNumber);

        if (player == null)
        {
            Visible = false;
            return;
        }

        PlayerNumberLabel.Text =
            MatchSettingsState.GetPlayer(playerNumber).Color.ToColorString() + " Alchemist";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
