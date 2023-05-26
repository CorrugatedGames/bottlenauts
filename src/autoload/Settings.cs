using System.Reflection;
using System.Linq;

public partial class Settings : SingletonNode
{
  const string SETTINGS_FILE = "user://settings.cfg";
  public bool Loaded { get; set; }

  #region Audio signals

  [Signal]
  public delegate void VolumeChangedEventHandler(int bus, int volume);

  #endregion

  #region Graphics signals

  [Signal]
  public delegate void ResolutionChangedEventHandler(int width, int height);

  [Signal]
  public delegate void DisplayChangedEventHandler(int displayIndex);

  #endregion

  public static AudioSettings Audio { get; private set; }
  public static GraphicsSettings Graphics { get; private set; }

  public override void _EnterTree()
  {
    base._EnterTree();

    Audio = new AudioSettings();
    Graphics = new GraphicsSettings();

    LoadData();
  }

  void LoadData()
  {
    ConfigFile cfg = new ConfigFile();
    if (cfg.Load(SETTINGS_FILE) != Error.Ok)
    {
      SaveData();
      return;
    }

    if (cfg.HasSection("Audio"))
      Audio.LoadedConfig = true;

    if (cfg.HasSection("Graphics"))
      Graphics.LoadedConfig = true;

    foreach (PropertyInfo prop in Audio.GetType().GetProperties())
    {
      if (prop.Name == "LoadedConfig") continue;

      int volume = (int)cfg.GetValue("Audio", prop.Name);
      int bus = AudioServer.GetBusIndex(prop.Name.Replace("Volume", ""));

      EmitSignal(SignalName.VolumeChanged, bus, volume);

      switch (prop.Name)
      {
        case "VolumeMaster":
          {
            Audio.SetVolumeMaster((int)cfg.GetValue("Audio", prop.Name));
          }
          break;

        case "VolumeBGM":
          {
            Audio.SetVolumeBGM((int)cfg.GetValue("Audio", prop.Name));
          }
          break;

        case "VolumeSFX":
          {
            Audio.SetVolumeSFX((int)cfg.GetValue("Audio", prop.Name));
          }
          break;

        default: break;
      }
    }

    Graphics.SetResolutionWidth((int)cfg.GetValue("Graphics", "ResolutionWidth"));
    Graphics.SetResolutionHeight((int)cfg.GetValue("Graphics", "ResolutionHeight"));
    Graphics.SetDisplay((GraphicsDisplay)(int)cfg.GetValue("Graphics", "DisplayMode"));
  }

  public static void SaveData()
  {
    ConfigFile cfg = new ConfigFile();

    foreach (PropertyInfo prop in Audio.GetType().GetProperties())
    {
      if (prop.Name == "LoadedConfig") continue;
      cfg.SetValue("Audio", prop.Name, Variant.From<int>((int)prop.GetValue(Audio)));
    }

    cfg.SetValue("Graphics", "ResolutionWidth", Variant.From<int>(Graphics.ResolutionWidth));
    cfg.SetValue("Graphics", "ResolutionHeight", Variant.From<int>(Graphics.ResolutionHeight));
    cfg.SetValue("Graphics", "DisplayMode", Variant.From<int>((int)Graphics.Display));

    cfg.Save(SETTINGS_FILE);
  }
}

public class AudioSettings
{
  public bool LoadedConfig { get; set; }
  public int VolumeMaster { get; set; }
  public int VolumeBGM { get; set; }
  public int VolumeSFX { get; set; }

  public void SetVolumeMaster(int volume) => VolumeMaster = volume;
  public void SetVolumeBGM(int volume) => VolumeBGM = volume;
  public void SetVolumeSFX(int volume) => VolumeSFX = volume;

  public AudioSettings()
  {
    VolumeMaster = 100;
    VolumeBGM = 100;
    VolumeSFX = 100;
  }
}

public class Resolution
{
  public int Width { get; set; }
  public int Height { get; set; }
}

public enum GraphicsDisplay
{
  Borderless = 0,
  Windowed = 1,
  Fullscreen = 2
}

public class GraphicsSettings
{
  public bool LoadedConfig { get; set; }

  public int ResolutionWidth { get; set; }
  public int ResolutionHeight { get; set; }

  public GraphicsDisplay Display { get; set; }

  public GraphicsDisplay[] Displays { get; } = {
    GraphicsDisplay.Borderless,
    GraphicsDisplay.Windowed,
    GraphicsDisplay.Fullscreen
  };

  public Resolution[] Resolutions { get; } = {
    new Resolution { Width = 1366, Height = 768 },
    new Resolution { Width = 1440, Height = 900 },
    new Resolution { Width = 1600, Height = 900 },
    new Resolution { Width = 1920, Height = 1080 },
    new Resolution { Width = 2560, Height = 1440 },
    new Resolution { Width = 2560, Height = 1600 },
    new Resolution { Width = 3840, Height = 2160 }
  };

  public void SetResolutionWidth(int width) => ResolutionWidth = width;
  public void SetResolutionHeight(int height) => ResolutionHeight = height;
  public void SetDisplay(GraphicsDisplay display) => Display = display;

  public GraphicsSettings()
  {
    SetDefaultSettings();
  }

  private void SetDefaultSettings()
  {
    if (LoadedConfig) return;

    Vector2I resolution = DisplayServer.ScreenGetSize();
    Resolution resolutionFound = FindResolution(resolution.X, resolution.Y);

    SetResolutionWidth(resolutionFound.Width);
    SetResolutionHeight(resolutionFound.Height);

    if (ResolutionWidth == resolution.X && ResolutionHeight == resolution.Y)
      Display = GraphicsDisplay.Borderless;
    else
      Display = GraphicsDisplay.Windowed;
  }

  public Resolution FindResolution(int width, int height)
  {
    return Resolutions.FirstOrDefault(
      res => res.Width == width && res.Height == height,
      new Resolution { Width = 1920, Height = 1080 }
    );
  }

  public int FindResolutionIndex(int width, int height)
  {
    return Resolutions
    .Select((res, index) => (res, index))
    .First(d => d.res.Width == width && d.res.Height == height).index;
  }
}