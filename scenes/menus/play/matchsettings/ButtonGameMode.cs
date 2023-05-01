public partial class ButtonGameMode : Button
{

  private GameMode[] gameModes = new GameMode[] {
    GameMode.LastManStanding,
    GameMode.Deathmatch
  };

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void _on_gui_input(InputEvent evt)
  {
    if (!HasFocus()) return;

    MatchSettingsState settings = GetNode<MatchSettingsState>("/root/MatchSettingsState");
    GameMode currentGameMode = settings.GameMode;
    int currentGameModeIndex = Array.IndexOf(gameModes, currentGameMode);

    if (Input.IsActionJustPressed("ui_left"))
    {
      if (currentGameModeIndex == 0)
      {
        settings.GameMode = gameModes[gameModes.Length - 1];
      }
      else
      {
        settings.GameMode = gameModes[currentGameModeIndex - 1];
      }

      ChangeName();
    }

    if (Input.IsActionJustPressed("ui_right"))
    {
      if (currentGameModeIndex == gameModes.Length - 1)
      {
        settings.GameMode = gameModes[0];
      }
      else
      {
        settings.GameMode = gameModes[currentGameModeIndex + 1];
      }

      ChangeName();
    }
  }

  public void ChangeName()
  {
    Print("Change Name");
    MatchSettingsState settings = GetNode<MatchSettingsState>("/root/MatchSettingsState");
    switch (settings.GameMode)
    {
      case GameMode.LastManStanding:
        Text = "Last Man Standing";
        break;

      case GameMode.Deathmatch:
        Text = "Deathmatch";
        break;

      default:
        Text = "Unknown";
        break;
    }
  }
}
