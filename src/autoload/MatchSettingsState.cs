
using System.Collections.Generic;
using System.Linq;

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
  Empty,

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

  public List<AlchemistColor> AlchemistColors { get; } = new List<AlchemistColor> {
    AlchemistColor.Empty,
    AlchemistColor.Empty,
    AlchemistColor.Empty,
    AlchemistColor.Empty
  };

  public List<AlchemistColor> AlchemistColorsNotEmpty
  {
    get
    {
      return AlchemistColors.Where(color => color != AlchemistColor.Empty).ToList();
    }
  }

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

  public void ChangePlayerColor(int position, AlchemistColor newColor)
  {
    AlchemistColors[position] = newColor;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }
}
