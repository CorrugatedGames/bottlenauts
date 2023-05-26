public partial class Graphics : Node
{
  public override void _Ready()
  {
    Settings.Connect<int, int>(Settings.SignalName.ResolutionChanged, (width, height) => OnResolutionChanged(width, height));

    Settings.Connect<int>(Settings.SignalName.DisplayChanged, (display) => OnDisplayChanged(display));

    OnResolutionChanged(Settings.Graphics.ResolutionWidth, Settings.Graphics.ResolutionHeight);
    OnDisplayChanged((int)Settings.Graphics.Display);
  }

  void OnResolutionChanged(int width, int height)
  {
    Settings.Graphics.SetResolutionWidth(width);
    Settings.Graphics.SetResolutionHeight(height);

    DisplayServer.WindowSetSize(new Vector2I(width, height));

    Settings.SaveData();
  }

  void OnDisplayChanged(int displayIndex)
  {
    GraphicsDisplay display = Settings.Graphics.Displays[displayIndex];
    Settings.Graphics.SetDisplay(display);

    switch (display)
    {
      case GraphicsDisplay.Borderless:
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        DisplayServer.WindowSetPosition(Vector2I.Zero);

        var resolution = DisplayServer.ScreenGetSize();
        OnResolutionChanged(resolution.X, resolution.Y);
        break;

      case GraphicsDisplay.Windowed:
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

        OnResolutionChanged(Settings.Graphics.ResolutionWidth, Settings.Graphics.ResolutionHeight);
        break;

      case GraphicsDisplay.Fullscreen:
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        break;
    }

    Settings.SaveData();
  }
}