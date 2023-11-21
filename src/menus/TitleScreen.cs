public partial class TitleScreen : Control
{
    #region Child nodes
    Button LocalMultiplayerButton,
        OptionsButton,
        QuitButton;
    ActiveCanvasLayer MenuLayer;
    SettingsLayer SettingsLayer;
    #endregion

    public override void _Ready()
    {
        MenuLayer = GetNode("%CanvasLayer") as ActiveCanvasLayer;
        SettingsLayer = GetNode("%SettingsLayer") as SettingsLayer;

        LocalMultiplayerButton = GetNode("%LocalMultiplayer") as Button;
        LocalMultiplayerButton.Pressed += () =>
            GetTree().ChangeSceneToFile("res://scenes/ui/matchsetup/MatchSetup.menu.tscn");

        OptionsButton = GetNode("%Options") as Button;
        OptionsButton.Pressed += OnOptionsButtonPressed;

        QuitButton = GetNode("%Quit") as Button;
        QuitButton.Pressed += () => GetTree().Quit();

        var skipToTestLevel = OS.GetEnvironment("LOAD_TEST_LEVEL");
        if (skipToTestLevel != null)
        {
            MatchSettingsState.ThemeName = "foo";
            MatchSettingsState.LevelName = "Test";
            while (MatchSettingsState.PlayerCount > MatchSettingsState.CurrentPlayerCount)
                MatchSettingsState.GenerateCPUPlayer();

            MatchSettingsState.Generate();

            BNPlayer player = MatchSettingsState.GetPlayer(0);
            player.IsCPU = false;
            player.IsGamepad = false;
            player.DeviceIndex = -1;

            MatchSettingsState.Instance.Players[0] = player;
            MatchSettingsState.GeneratePlayerBindings();

            GetTree().ChangeSceneToFile("res://scenes/ui/ingame/GameScene.tscn");
        }
    }

    void OnOptionsButtonPressed()
    {
        MenuLayer.SetActive(false, true);
        MenuLayer.SetProcessInput(false);
        SettingsLayer.SetActive(true, true);
    }
}
