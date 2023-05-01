public partial class ButtonChangeWins : Button
{

  private GameRounds[] gameModes = new GameRounds[] {
    GameRounds.One,
    GameRounds.Two,
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
        Text = "1 Point";
        break;
      case GameRounds.Two:
        Text = "2 Points";
        break;
      case GameRounds.Three:
        Text = "3 Points";
        break;
      case GameRounds.Five:
        Text = "5 Points";
        break;
      case GameRounds.Seven:
        Text = "7 Points";
        break;
      case GameRounds.Ten:
        Text = "10 Points";
        break;
    }
  }
}
