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

    Logger.Info("Settings load");

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

    Logger.Info("Loading audio config...");
    Audio.LoadFromConfigFile(cfg, "Audio");

    Logger.Info("Loading graphics config...");
    Graphics.LoadFromConfigFile(cfg, "Graphics");
  }

  public static void SaveData()
  {
    Logger.Info("Starting config file save...");
    ConfigFile cfg = new ConfigFile();

    Audio.SaveToConfigFile(cfg, "Audio");
    Graphics.SaveToConfigFile(cfg, "Graphics");

    cfg.Save(SETTINGS_FILE);
    Logger.Info("Saved config file!");
  }
}