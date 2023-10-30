public partial class TitleScreen : Control
{
  #region Child nodes
  Button LocalMultiplayerButton, OptionsButton, QuitButton;
  ActiveCanvasLayer MenuLayer;
  SettingsLayer SettingsLayer;
  #endregion

  public override void _Ready ()
  {
    MenuLayer = GetNode("%CanvasLayer") as ActiveCanvasLayer;
    SettingsLayer = GetNode("%SettingsLayer") as SettingsLayer;

    LocalMultiplayerButton = GetNode("%LocalMultiplayer") as Button;
    LocalMultiplayerButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/menus/MatchSetupMenu.tscn");

    OptionsButton = GetNode("%Options") as Button;
    OptionsButton.Pressed += OnOptionsButtonPressed;

    QuitButton = GetNode("%Quit") as Button;
    QuitButton.Pressed += () => GetTree().Quit();
  }

  void OnOptionsButtonPressed ()
  {
    MenuLayer.SetActive(false);
    MenuLayer.SetProcessInput(false);
    SettingsLayer.SetActive(true);
  }
}
