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

    Level level = (ResourceLoader.Load(OS.GetEnvironment(ENVIRON_LEVELNAME).AsLevelFilePath()) as PackedScene).Instantiate() as Level;
    level.Theme = ResourceLoader.Load(OS.GetEnvironment(ENVIRON_THEMENAME).AsThemeFilePath()) as LevelTheme;

    foreach (CharacterHUD hud in GetNode("%HUDContainer").GetChildren().Cast<CharacterHUD>())
      level.SpawnsSet += hud.SetCharacterInfo;

    Viewport.AddChild(level);
    level.Camera.MakeCurrent();

    foreach (Character character in GetTree().GetNodesInGroup(GROUP_CHARACTERS).Cast<Character>())
      Referee.ConnectCharacterSignals(character);
  }
}
