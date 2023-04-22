public partial class PauseLayer : CanvasLayer
{
  #region Child nodes

  Button BackButton, SettingsButton, MainMenuButton, ExitButton;

  #endregion

  public override void _Ready ()
  {
    BackButton = GetNode("OptionsContainer/BackButton") as Button;
    SettingsButton = GetNode("OptionsContainer/SettingsButton") as Button;
    MainMenuButton = GetNode("OptionsContainer/MainMenuButton") as Button;
    ExitButton = GetNode("OptionsContainer/ExitButton") as Button;

    BackButton.Pressed += OnBackButtonPressed;
    SettingsButton.Pressed += OnSettingsButtonPressed;
    MainMenuButton.Pressed += OnMainMenuButtonPressed;
    ExitButton.Pressed += OnExitButtonPressed;

    BackButton.GrabFocus();
  }

  void OnBackButtonPressed ()
  {}

  void OnSettingsButtonPressed ()
  {}

  void OnMainMenuButtonPressed ()
  {}

  void OnExitButtonPressed ()
  {
    SignalBus.EmitSignal(SignalBus.SignalName.GameExit);
  }
}
