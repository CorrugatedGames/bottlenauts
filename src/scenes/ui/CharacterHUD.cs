public partial class CharacterHUD : MarginContainer
{
    Label PlayerNumberLabel;

    public override void _Ready()
    {
        PlayerNumberLabel = GetNode("%PlayerNumberLabel") as Label;
    }

    public void SetCharacterInfo ()
    {
        int playerNumber = (int)GetMeta("PlayerNumber");

        if (playerNumber >= MatchSettingsState.PlayerCount)
        {
            Visible = false;
            PlayerNumberLabel.Visible = false;
            PlayerNumberLabel.Text = "";
            return;
        }

        var player = MatchSettingsState.GetPlayer(playerNumber);
        PlayerNumberLabel.Text = player.Color.ToColorString() + " Alchemist";
    }
}
