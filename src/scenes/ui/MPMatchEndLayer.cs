public partial class MPMatchEndLayer : ActiveCanvasLayer
{
    #region Child nodes

    Button PlayAgainButton;
    Button MatchSettingsButton;
    Button MainMenuButton;

    Label GameEndString;

    #endregion

    public override void _Ready()
    {
        GameEndString = GetNode("GameEndString") as Label;

        PlayAgainButton = GetNode("ActionsContainer/ReplayButton") as Button;
        MatchSettingsButton = GetNode("ActionsContainer/MatchSettingsButton") as Button;
        MainMenuButton = GetNode("ActionsContainer/MainMenuButton") as Button;

        PlayAgainButton.Pressed += PlayAgainButtonPressed;
        MatchSettingsButton.Pressed += OnMatchSettingsButtonPressed;
        MainMenuButton.Pressed += OnMainMenuButtonPressed;

        PlayAgainButton.GrabFocus();
    }

    public override void _Process(double delta)
    {
        GameEndString.Text = MatchSettingsState.WinnerString;
    }

    void PlayAgainButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/ingame/GameScene.tscn");
    }

    void OnMatchSettingsButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/matchsetup/MatchSetup.menu.tscn");
    }

    void OnMainMenuButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/mainmenu/Main.menu.tscn");
    }
}
