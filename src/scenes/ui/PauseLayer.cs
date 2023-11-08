public partial class PauseLayer : ActiveCanvasLayer
{
  #region Child nodes

  SettingsLayer Settings;
  Button BackButton, SettingsButton, MainMenuButton, ExitButton;

  #endregion

  public override void _Ready ()
  {
    Settings = GetNode("SettingsLayer") as SettingsLayer;

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

  public override void _Input (InputEvent evt)
  {
    if (evt.IsActionPressed("pause"))
      SetActive(!Visible, Visible);
  }

  public override void SetActive (bool active, bool allowCharacterProcess)
  {
    base.SetActive(active, allowCharacterProcess);

    if (active)
      BackButton.GrabFocus();
  }

  void OnBackButtonPressed () => SetActive(false, true);

  void OnSettingsButtonPressed ()
  {
    SetActive(false, false);
    SetProcessInput(false);
    Settings.SetActive(true, false);
  }

  void OnMainMenuButtonPressed ()
  {}

  void OnExitButtonPressed ()
  {
	  SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameExit);
  }
}
