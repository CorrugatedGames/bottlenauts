using static Godot.DisplayServer;

public partial class Graphics : Node
{
  public override void _Ready()
  {
    Logger.Info("Graphics signals initializing...");

    Settings.Instance.ResolutionChanged += (width, height) => OnResolutionChanged(width, height);
    Settings.Instance.DisplayChanged += (display) => OnDisplayChanged(display);

    OnResolutionChanged(Settings.Graphics.Resolution.X, Settings.Graphics.Resolution.Y);
    OnDisplayChanged((int)Settings.Graphics.Display);
    Logger.Info("Graphics signals initialized!");
  }

  internal void CenterWindow ()
  {
    Rect2I screenRect = ScreenGetUsableRect(WindowGetCurrentScreen());
    Vector2I windowSize = new Vector2I(Settings.Graphics.Resolution.X, Settings.Graphics.Resolution.Y);
    
    int x = screenRect.Position.X + ((screenRect.Size.X - windowSize.X) >> 1);
    int y = screenRect.Position.Y + ((screenRect.Size.Y - windowSize.Y) >> 1);
    WindowSetPosition(new Vector2I(x, y));
  }

  void OnResolutionChanged(int width, int height)
  {

    Logger.Info($"Resolution changing to {width}x{height}...");
    Settings.Graphics.SetResolution(new Vector2I(width, height));

    DisplayServer.WindowSetSize(new Vector2I(width, height));

    if (Settings.Graphics.Display == GraphicsDisplay.Windowed)
      CenterWindow();

    Settings.SaveData();
  }

  void OnDisplayChanged(int displayIndex)
  {
    Logger.Info($"Display type changing to {displayIndex}...");

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

        OnResolutionChanged(Settings.Graphics.Resolution.X, Settings.Graphics.Resolution.Y);
        break;

      case GraphicsDisplay.Fullscreen:
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        break;
    }

    Settings.SaveData();
  }
}