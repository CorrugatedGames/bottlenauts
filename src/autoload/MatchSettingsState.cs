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

public enum GameModifier
{
  NoFriendlyFire,
  NoTimeLimit,
  AlwaysSuddenDeath,
  TeamRevive,
  NoItems,
  TreasureChests,
  SymmetricalLoot,
  MaximumLoot,
  ShowLootSpawns,
  ItemDraft,
  OneBombAtATime,
  LimitedBombs,
  ExplodingCorpses,
  SlipperyFloors,
  DarkArena,
  SlowArena,

  ForceSprint,
  ForceSlow,
  Diarrhea,
  Paralysis,
  MinimumPower,
  CantStopMoving,
  ReverseControls,
  ShortFuseBombs,
  LongFuseBombs,
}

public partial class MatchSettingsState : SingletonNode
{
  public GameMode GameMode { get; set; } = GameMode.LastManStanding;
  public GameRounds GameRounds { get; set; } = GameRounds.Normal;
  public Dictionary<GameModifier, bool> GameModifiers { get; } = new ();

  public static string LevelName { get; set; } = "";
  public static Level Level { get; private set; }
  public static string ThemeName { get; set; } = "";
  public static LevelTheme Theme { get; private set; }

  const int MAX_PLAYER_COUNT = 8, MIN_PLAYER_COUNT = 2;
  public static int PlayerCount { get; private set; }
  public BNPlayer [] Players { get; private set; }
  static readonly BNPlayer [] NullPlayers = { null };
  public static bool HasPlayer => Instance.Players.Except(NullPlayers).Any();
  public static bool HasKeyboardPlayer => HasPlayer && Instance.Players.Where(player => player != null && !player.IsGamepad).Any();
  public static bool HasGamepadPlayer (int deviceIndex) =>
    HasPlayer && Instance.Players.Where(player => player != null && player.IsGamepad && deviceIndex == player.DeviceIndex).Any();
  public static int CurrentPlayerCount => Mathf.Clamp(Instance.Players.Except(NullPlayers).Count(), 0, MAX_PLAYER_COUNT);

  public static MatchSettingsState Instance { get; set; }

  public static int AddPlayer (bool isGamepad, int deviceIndex)
  {
    string profileName = $"P{CurrentPlayerCount + 1}";
    return AddPlayer(isGamepad, deviceIndex, PlayerProfileManager.GetProfile(profileName));
  }

  public static int AddPlayer (bool isGamepad, int deviceIndex, PlayerProfile profile)
  {
    int playerIndex = CurrentPlayerCount;

    Instance.Players[playerIndex] = new BNPlayer {
      IsCPU = false,
      IsDead = false,
      IsGamepad = isGamepad,
      DeviceIndex = deviceIndex,
      PlayerIndex = playerIndex,

      Profile = profile,
      Color = GetUnusedCharacterColors()[0],
    };

    return playerIndex;
  }

  public static int GenerateCPUPlayer () => GenerateCPUPlayer(CurrentPlayerCount);
  public static int GenerateCPUPlayer (int playerIndex)
  {
    CharacterColor color = GetUnusedCharacterColors()[(int)Randi() % GetUnusedCharacterColors().Count];
    Instance.Players[playerIndex] = new BNPlayer {
      IsCPU = true,
      IsDead = false,
      IsGamepad = true,
      DeviceIndex = -2,
      PlayerIndex = playerIndex,

      Profile = PlayerProfileManager.NEW_PROFILE(),
      Color = color,
    };

    return playerIndex;
  }

  public static void RemovePlayer (int playerIndex) => Instance.Players[playerIndex] = null;
  public static void RemovePlayer (PlayerProfile profile) =>
    Instance.Players[Array.FindIndex(Instance.Players, player => player.Profile.Name == profile.Name)] = null;

  public static void RemovePlayers () => Instance.Players = new BNPlayer [PlayerCount];

  public static BNPlayer GetPlayer (int playerNumber) => Instance.Players[playerNumber];

  public static void UpdatePlayerCount (int newCount)
  {
    if (newCount > MAX_PLAYER_COUNT || newCount < MIN_PLAYER_COUNT || newCount == PlayerCount)
      return;
      
    PlayerCount = newCount;

    BNPlayer [] currentArray = Instance.Players, playerArray = new BNPlayer [PlayerCount];

    if (currentArray != null)
      for (int i = 0; i < currentArray.Length; i++)
      {
        if (currentArray[i] == null || i == playerArray.Length)
          break;

        playerArray[i] = currentArray[i];
      }

    Instance.Players = playerArray;
  }

  public static void GeneratePlayerBindings ()
  {
    foreach (BNPlayer player in Instance.Players)
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
        foreach (InputEvent binding in bindings)
        {
          if (binding is InputEventKey keyEvent == player.IsGamepad)
            continue;

          if (player.IsGamepad)
            binding.Device = player.DeviceIndex;

          MPInputMap.ActionAddEvent(playerIndex, action, binding);
        }
      }
    }
  }

  public static void GenerateLevel ()
  {
    PackedScene levelScene = ResourceLoader.Load(LevelName.AsLevelFilePath()) as PackedScene;
    Level = levelScene.Instantiate() as Level;
    Theme = ResourceLoader.Load(ThemeName.AsThemeFilePath()) as LevelTheme;
    Level.Theme = Theme;
  }

  public static void SetPlayerProfile (int playerIdx, PlayerProfile profile)
  {
    if (playerIdx >= CurrentPlayerCount || playerIdx < 0 || Instance.Players[playerIdx] == null)
      return;
    
    Instance.Players[playerIdx].Profile = profile;
  }

  public static void SetGameMode (GameMode mode) => Instance.GameMode = mode;

  public override void _EnterTree ()
  {
    base._EnterTree();

    foreach (GameModifier modifier in Enum.GetValues(typeof(GameModifier)))
      GameModifiers[modifier] = false;

    Instance = this;

    UpdatePlayerCount(4);
    Logger.Info("Players array allocated!");
    
    Logger.Info($"Match settings singleton initialized!");
  }

  public static List<CharacterColor> GetUnusedCharacterColors (BNPlayer queryPlayer = null)
  {
    string [] names = Enum.GetNames(typeof(CharacterColor));
    List<CharacterColor> colors = new (names.Length);
    for (int i = 0; i < names.Length; i++)
      colors.Add((CharacterColor)Enum.Parse(typeof(CharacterColor), names[i]));

    foreach (BNPlayer player in Instance.Players)
    {
      if (player == null)
        break;

      if (queryPlayer != null && queryPlayer.Color == player.Color)
        continue;

      int idx = colors.FindIndex(color => color == player.Color);
      if (idx > -1)
        colors.RemoveAt(idx);
    }

    return colors;
  }

  public static void ToggleGameModifier (GameModifier modifier) =>
    Instance.GameModifiers[modifier] = !Instance.GameModifiers[modifier];
  public static void ToggleGameModifier (string modifierName) =>
    ToggleGameModifier(Enum.Parse<GameModifier>(modifierName));

  public static Dictionary<GameModifier, bool> GetGameModifiers () => Instance.GameModifiers;
  public static List<GameModifier> GetActiveGameModifiers ()
  {
    List<GameModifier> modifiers = new ();
    foreach (KeyValuePair<GameModifier, bool> pair in Instance.GameModifiers)
      if (pair.Value)
        modifiers.Add(pair.Key);

    return modifiers;
  }

  public static void PrintActiveGameModifiers ()
  {
    string printout = "[ ";

    for (int i = 0; i < GetActiveGameModifiers().Count; i++)
    {
      printout += GetActiveGameModifiers()[i];
      if (i < GetActiveGameModifiers().Count - 1)
        printout += ", ";
    }

    printout += " ]";
    Logger.Debug(printout);
  }
}
