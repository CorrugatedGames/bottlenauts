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

public enum AlchemistColor
{
  Red,
  Blue,
  Green,
  Yellow,
  Purple,
  White,
  Black,
  Pink
}

public partial class MatchSettingsState : Node
{

  public GameMode GameMode { get; set; } = GameMode.LastManStanding;
  public GameRounds GameRounds { get; set; } = GameRounds.Five;

  public AlchemistColor[] AlchemistColors { get; set; } = new AlchemistColor[] {
    AlchemistColor.Blue,
    AlchemistColor.Red,
    AlchemistColor.Green,
    AlchemistColor.Yellow
  };

  public readonly int MaxPlayers = 4;

  public AlchemistColor[] AllColors { get; } = new AlchemistColor[] {
    AlchemistColor.Blue,
    AlchemistColor.Red,
    AlchemistColor.Green,
    AlchemistColor.Yellow,
    AlchemistColor.Purple,
    AlchemistColor.White,
    AlchemistColor.Black,
    AlchemistColor.Pink
  };

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }
}
