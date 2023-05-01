public partial class ButtonChangeWins : Button
{

  private GameRounds[] gameModes = new GameRounds[] {
    GameRounds.One,
    GameRounds.Three,
    GameRounds.Five,
    GameRounds.Seven,
    GameRounds.Ten
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
    GameRounds currentRounds = settings.GameRounds;
    int currentGameModeIndex = Array.IndexOf(gameModes, currentRounds);

    if (Input.IsActionJustPressed("ui_left"))
    {
      if (currentGameModeIndex == 0)
      {
        settings.GameRounds = gameModes[gameModes.Length - 1];
      }
      else
      {
        settings.GameRounds = gameModes[currentGameModeIndex - 1];
      }

      ChangeName();
    }

    if (Input.IsActionJustPressed("ui_right"))
    {
      if (currentGameModeIndex == gameModes.Length - 1)
      {
        settings.GameRounds = gameModes[0];
      }
      else
      {
        settings.GameRounds = gameModes[currentGameModeIndex + 1];
      }

      ChangeName();
    }
  }

  public void ChangeName()
  {
    Print("Change Name");
    MatchSettingsState settings = GetNode<MatchSettingsState>("/root/MatchSettingsState");

    switch (settings.GameRounds)
    {
      case GameRounds.One:
        Text = "Instant Match\n1 Point";
        break;
      case GameRounds.Three:
        Text = "Quick Match\n3 Points";
        break;
      case GameRounds.Five:
        Text = "Normal Match\n5 Points";
        break;
      case GameRounds.Seven:
        Text = "Long Match\n7 Points";
        break;
      case GameRounds.Ten:
        Text = "Epic Match\n10 Points";
        break;
    }
  }
}
