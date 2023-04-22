public partial class GameScene : Node
{
  #region Child nodes

  SubViewport Viewport;
  Camera3D GodCamera;
  AudioStreamPlayer PlayerBGM, PlayerSFX;

  #endregion

  public override void _Ready ()
  {
	Viewport = GetNode("ViewportContainer/Viewport") as SubViewport;

	GodCamera = Viewport.GetNode("GodCamera") as Camera3D;

	PlayerBGM = Viewport.GetNode("Audio/BGM") as AudioStreamPlayer;
	PlayerSFX = Viewport.GetNode("Audio/SFX") as AudioStreamPlayer;
  }
}
