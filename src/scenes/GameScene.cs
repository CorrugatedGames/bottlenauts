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

    Level level = MatchSettingsState.Level;
    level.Theme = MatchSettingsState.Theme;

    Viewport.AddChild(level);
    level.Camera.MakeCurrent();

    foreach (Character character in GetTree().GetNodesInGroup("Character"))
      Referee.ConnectCharacterSignals(character);
  }
}
