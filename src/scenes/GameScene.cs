using System.Linq;

public partial class GameScene : Node
{
  #region Child nodes

  public Referee Referee { get; private set; }
  public SubViewport Viewport { get; private set; }
  AudioStreamPlayer PlayerBGM, PlayerSFX;

  #endregion

  public override void _Ready ()
  {
    Referee = GetNode("%Referee") as Referee;
    Viewport = GetNode("%Viewport") as SubViewport;

    PlayerBGM = Viewport.GetNode("Audio/BGM") as AudioStreamPlayer;
    PlayerSFX = Viewport.GetNode("Audio/SFX") as AudioStreamPlayer;

    string levelEnv = OS.GetEnvironment(ENVIRON_LEVELNAME);
    Level level = (ResourceLoader.Load((levelEnv != "" ? levelEnv : MatchSettingsState.LevelName).AsLevelFilePath()) as PackedScene).Instantiate() as Level;

    string themeEnv = OS.GetEnvironment(ENVIRON_THEMENAME);
    level.Theme = ResourceLoader.Load((themeEnv != "" ? themeEnv : MatchSettingsState.ThemeName).AsThemeFilePath()) as LevelTheme;

    foreach (CharacterHUD hud in GetNode("%HUDContainer").GetChildren().Cast<CharacterHUD>())
      level.SpawnsSet += hud.SetCharacterInfo;

    Viewport.AddChild(level);
    level.Camera.MakeCurrent();

    foreach (Character character in GetTree().GetNodesInGroup(GROUP_CHARACTERS).Cast<Character>())
      Referee.ConnectCharacterSignals(character);
  }
}
