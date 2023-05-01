public enum GameMode
{
  LastManStanding,
  Deathmatch,

  // TeamDeathmatch
  // CoinCollect
  // CaptureTheFlag?
}

public enum GameRounds
{
  One = 1,
  Three = 3,
  Five = 5,
  Seven = 7,
  Ten = 10
}

public partial class MatchSettingsState : Node
{

  public GameMode GameMode { get; set; }
  public GameRounds GameRounds { get; set; }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    GameMode = GameMode.LastManStanding;
    GameRounds = GameRounds.Five;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }
}
