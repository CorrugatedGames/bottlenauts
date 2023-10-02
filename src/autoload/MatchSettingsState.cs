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
  VeryShort = 1,
  Short = 3,
  Normal = 5,
  Long = 7,
  VeryLong = 10
}

// public enum AlchemistColor
// {
//   Empty,
//   Red,
//   Blue,
//   Green,
//   Yellow,
//   Purple,
//   White,
//   Black,
//   Pink
// }

public partial class MatchSettingsState : SingletonNode
{
  public GameMode GameMode { get; set; } = GameMode.LastManStanding;
  public GameRounds GameRounds { get; set; } = GameRounds.Five;

  const int MAX_PLAYER_COUNT = 8;
  public BNPlayer [] Players { get; private set; }
  public bool HasPlayer => Players.Except(null).Count() > 0;
  public int PlayerCount => Mathf.Clamp(Players.Except(null).Count(), 1, MAX_PLAYER_COUNT);

  static MatchSettingsState Instance;

  public int AddPlayer (PlayerProfile profile, bool isGamepad, int deviceIndex)
  {
    int playerIndex = PlayerCount;

    Players[playerIndex] = new BNPlayer {
      IsGamepad = isGamepad,
      DeviceIndex = deviceIndex,
      PlayerIndex = playerIndex,

      Profile = profile
    };

    return playerIndex;
  }

  public void RemovePlayer (int playerIndex) => Players[playerIndex] = null;
  public void RemovePlayer (PlayerProfile profile) =>
    Players[Array.FindIndex(Players, player => player.Profile.Name == profile.Name)] = null;

  public void RemovePlayers () => Players = new BNPlayer [MAX_PLAYER_COUNT];

  public static void GeneratePlayerBindings ()
  {
    foreach (var player in Instance.Players)
    {
      if (player == null)
        continue;

      int playerIndex = player.PlayerIndex;
      float deadzone = player.Profile.Deadzone;
      Dictionary<string, List<InputEvent>> events = player.Profile.Bindings.AsInputEvents();

      foreach (var evt in events)
      {
        string action = evt.Key;
        List<InputEvent> bindings = evt.Value;
        
        if (MPInputMap.HasAction(playerIndex, action))
          MPInputMap.EraseActionDeep(playerIndex, action);

        MPInputMap.AddAction(playerIndex, action, deadzone);
        foreach (var binding in bindings)
          MPInputMap.ActionAddEvent(playerIndex, action, binding);
      }
    }
  }

  // public List<AlchemistColor> AlchemistColors { get; } = new List<AlchemistColor> {
  //   AlchemistColor.Empty,
  //   AlchemistColor.Empty,
  //   AlchemistColor.Empty,
  //   AlchemistColor.Empty
  // };

  // public List<AlchemistColor> AlchemistColorsNotEmpty
  // {
  //   get => AlchemistColors.Where(color => color != AlchemistColor.Empty).ToList();
  // }

  // public readonly int MaxPlayers = 4;

  // public AlchemistColor[] AllColors { get; } = new AlchemistColor[] {
  //   AlchemistColor.Blue,
  //   AlchemistColor.Red,
  //   AlchemistColor.Green,
  //   AlchemistColor.Yellow,
  //   AlchemistColor.Purple,
  //   AlchemistColor.White,
  //   AlchemistColor.Black,
  //   AlchemistColor.Pink
  // };

  // public void ChangePlayerColor(int position, AlchemistColor newColor)
  // {
  //   AlchemistColors[position] = newColor;
  // }

  public override void _EnterTree ()
  {
    base._EnterTree();

    Instance = this;

    Players = new BNPlayer [MAX_PLAYER_COUNT];
    Logger.Info("Players array allocated!");
    
    Logger.Info($"Match settings singleton initialized!");
  }
}
