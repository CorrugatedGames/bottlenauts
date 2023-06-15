using System.Linq;

public enum GraphicsDisplay
{
  Borderless = 0,
  Windowed = 1,
  Fullscreen = 2,
}

public sealed class GraphicsSettings : ISettingsCategory
{
  public bool LoadedConfig { get; set; }

  public Vector2I Resolution { get; private set; }
  public Vector2I [] Resolutions { get; } = {
    new Vector2I { X = 1366, Y = 768 },
    new Vector2I { X = 1440, Y = 900 },
    new Vector2I { X = 1600, Y = 900 },
    new Vector2I { X = 1920, Y = 1080 },
    new Vector2I { X = 2560, Y = 1440 },
    new Vector2I { X = 2560, Y = 1600 },
    new Vector2I { X = 3840, Y = 2160 },
  };

  public GraphicsDisplay Display { get; private set; }
  public GraphicsDisplay [] Displays { get; } = {
    GraphicsDisplay.Borderless,
    GraphicsDisplay.Windowed,
    GraphicsDisplay.Fullscreen,
  };

  public void LoadFromConfigFile (ConfigFile cfg, string category)
  {
    SetResolution(new Vector2I ( (int)cfg.GetValue(category, "ResolutionWidth"), (int)cfg.GetValue(category, "ResolutionHeight") ));
    SetDisplay((GraphicsDisplay)(int)cfg.GetValue(category, "DisplayMode"));
  }

  public void SaveToConfigFile (ConfigFile cfg, string category)
  {
    cfg.SetValue(category, "ResolutionWidth", Variant.From<int>(Resolution.X));
    cfg.SetValue(category, "ResolutionHeight", Variant.From<int>(Resolution.Y));
    cfg.SetValue(category, "DisplayMode", Variant.From<int>((int)Display));
  }

  public void SetDefaultSettings ()
  {
    if (LoadedConfig)
      return;
  
    Logger.Info("Loading default graphics config...");

    Vector2I screenSize = DisplayServer.ScreenGetSize();

    Resolution = FindResolution(screenSize.X, screenSize.Y);
    Display = (Resolution.X == screenSize.X && Resolution.Y == Resolution.Y) ? GraphicsDisplay.Borderless : GraphicsDisplay.Windowed;

    Logger.Info($"Default resolution: {Resolution.X}x{Resolution.Y}");
  }

  public void SetResolution (Vector2I resolution) => Resolution = resolution;
  public void SetDisplay (GraphicsDisplay display) => Display = display;

  public Vector2I FindResolution (int width, int height) =>
    Resolutions.FirstOrDefault(
      r => r.X == width && r.Y == height,
      new Vector2I { X = 1920, Y = 1080 }
    );

  public int FindResolutionIndex (int width, int height) =>
    Resolutions
      .Select((res, index) => (res, index))
      .First(d => d.res.X == width && d.res.Y == height).index;

  public int FindResolutionIndex (Vector2I resolution) =>
    Resolutions
      .Select((res, index) => (res, index))
      .First(d => d.res.X == resolution.X && d.res.Y == resolution.Y).index;
}