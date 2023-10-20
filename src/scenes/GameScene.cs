public partial class GameScene : Node
{
  #region Child nodes

  public SubViewport Viewport { get; private set; }
  AudioStreamPlayer PlayerBGM, PlayerSFX;

  #endregion

  public override void _Ready ()
  {
    Viewport = GetNode("%Viewport") as SubViewport;

    PlayerBGM = Viewport.GetNode("Audio/BGM") as AudioStreamPlayer;
    PlayerSFX = Viewport.GetNode("Audio/SFX") as AudioStreamPlayer;

    Level level = MatchSettingsState.Level;
    level.Theme = MatchSettingsState.Theme;

    Viewport.AddChild(level);
    level.Camera.MakeCurrent();
  }
}
