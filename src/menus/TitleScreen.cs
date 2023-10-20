public partial class TitleScreen : Control
{
  #region Child nodes
  Button LocalMultiplayerButton, OptionsButton, QuitButton;
  #endregion

  public override void _Ready ()
  {
    LocalMultiplayerButton = GetNode("%LocalMultiplayer") as Button;
    LocalMultiplayerButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/menus/MatchSetupMenu.tscn");

    QuitButton = GetNode("%Quit") as Button;
    QuitButton.Pressed += () => GetTree().Quit();
  }
}
