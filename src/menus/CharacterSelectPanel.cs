public partial class CharacterSelectPanel : MarginContainer
{
  [Signal] public delegate void PlayerReadyEventHandler ();

  public bool HasPlayer => PlayerContainer.Visible;
  BNPlayer Player => HasPlayer ? MatchSettingsState.GetPlayer(PlayerNumber) : null;
  public int PlayerNumber { get; private set; }

  List<string> ProfileNames { get; set; }

  #region Child nodes
  VBoxContainer NoPlayerContainer, PlayerContainer;
  Panel Panel;

  Label InputLabel, PlayerNumberLabel, TeamLabel;
  OptionButton ProfileSelectButton;
  Button PrevCharacterButton, NextCharacterButton;
  public Button ReadyButton { get; private set; }
  #endregion

  public override void _Ready ()
  {
    NoPlayerContainer = GetNode("%NoPlayerControl") as VBoxContainer;
    NoPlayerContainer.Visible = true;
    PlayerContainer = GetNode("%PlayerControl") as VBoxContainer;
    PlayerContainer.Visible = false;

    Panel = GetNode("Panel") as Panel;

    InputLabel = GetNode("%InputLabel") as Label;
    PlayerNumberLabel = GetNode("%PlayerNumberLabel") as Label;
    TeamLabel = GetNode("%TeamLabel") as Label;

    ProfileSelectButton = GetNode("%ProfileSelect") as OptionButton;
    ProfileSelectButton.ItemSelected += SelectPlayerProfile;

    PrevCharacterButton = GetNode("%PrevCharacterButton") as Button;
    PrevCharacterButton.Pressed += () => { ChangeCharacter(false); };
    NextCharacterButton = GetNode("%NextCharacterButton") as Button;
    NextCharacterButton.Pressed += () => { ChangeCharacter(true); };
    ReadyButton = GetNode("%ReadyButton") as Button;
    ReadyButton.Pressed += () => { EmitSignal(SignalName.PlayerReady); };
  }

  public override void _UnhandledInput (InputEvent evt)
  {
    if (!HasPlayer || !Visible)
      return;

    if (evt is InputEventKey keyEvent && !Player.IsGamepad)
    {
      if (keyEvent.Pressed && (keyEvent.Keycode == Key.Q || keyEvent.Keycode == Key.E))
      {
        GetViewport().SetInputAsHandled();
        ChangeCharacter(keyEvent.Keycode == Key.E);
      }
    }

    if (evt is InputEventJoypadButton buttonEvent &&
      Player.IsGamepad && Player.DeviceIndex == (evt as InputEventJoypadButton).Device)
    {
      if (buttonEvent.Pressed &&
        (buttonEvent.ButtonIndex == JoyButton.LeftShoulder || buttonEvent.ButtonIndex == JoyButton.RightShoulder))
      {
        GetViewport().SetInputAsHandled();
        ChangeCharacter(buttonEvent.ButtonIndex == JoyButton.RightShoulder);
      }

      if (buttonEvent.Pressed && buttonEvent.ButtonIndex == JoyButton.X)
      {
        ReadyButton.ButtonPressed = !ReadyButton.ButtonPressed;
        EmitSignal(SignalName.PlayerReady);
      }
    }
  }

  public void SetPlayer (BNPlayer player, int number)
  {
    NoPlayerContainer.Visible = false;
    PlayerContainer.Visible = true;

    InputLabel.Text = player.IsGamepad ? $"Gamepad - idx {player.DeviceIndex}" : "Keyboard";

    SetPlayerNumber(number);
    SetColor(player.Color.ToColor());
  }

  void RefreshPlayer () => SetPlayer(MatchSettingsState.GetPlayer(PlayerNumber), PlayerNumber);

  public void Reset ()
  {
    NoPlayerContainer.Visible = true;
    PlayerContainer.Visible = false;

    InputLabel.Text = "";
    TeamLabel.Text = "Team: ";
    for (int i = ProfileSelectButton.ItemCount - 1; i >= 0; i--)
      ProfileSelectButton.RemoveItem(i);
  }

  public void SetPlayerNumber (int number)
  {
    PlayerNumber = number;
    PlayerNumberLabel.Text= $"PLAYER #{++number}";
    FillProfileSelectButton();
  }

  public void SetProfileNames (List<string> profileNames) => ProfileNames = profileNames;

  void FillProfileSelectButton ()
  {
    List<string> options = new(ProfileNames);
    int currentIndex = ProfileSelectButton.Selected;
    string currentOption = "";

    if (currentIndex > -1) 
      currentOption = ProfileSelectButton.GetItemText(currentIndex);

    for (int i = options.Count - 1; i >= 0; i--)
      for (int p = 1; p <= 8; p++)
        if (PlayerNumber + 1 != p && options[i] == $"P{p}")
        {
          options.RemoveAt(i);
          break;
        }

    if (currentIndex > -1)
    {
      if (!options.Contains(currentOption))
        currentOption = $"P{PlayerNumber}";
      currentIndex = options.FindIndex(opt => opt == currentOption);
    }

    for (int i = ProfileSelectButton.ItemCount - 1 ; i >= 0; i--)
      ProfileSelectButton.RemoveItem(i);
    for (int i = 0; i < options.Count; i++)
      ProfileSelectButton.AddItem(options[i], i);
    
    ProfileSelectButton.Selected = (currentIndex > -1) ? currentIndex : options.FindIndex(opt => opt == $"P{PlayerNumber + 1}");
  }

  void SelectPlayerProfile (long profileIndex)
  {
    string selected = ProfileSelectButton.GetItemText((int)profileIndex);
    MatchSettingsState.SetPlayerProfile(PlayerNumber, PlayerProfileManager.GetProfile(selected));
  }

  void ChangeCharacter (bool tickForward)
  {
    List<CharacterColor> colors = MatchSettingsState.GetUnusedCharacterColors(Player);

    int idx = colors.FindIndex(color => color == Player.Color) + (tickForward ? 1 : -1);
    if (idx < 0)  
      idx = colors.Count - 1;
    if (idx == colors.Count)
      idx = 0;
    
    Player.Color = colors[idx];
    SetColor(Player.Color.ToColor());
  }

  void SetColor (Color color)
  {
    var stylebox = Panel.GetThemeStylebox("panel") as StyleBoxFlat;
    stylebox.BgColor = color;

    Panel.AddThemeStyleboxOverride("player_color", stylebox);
  }
}