using System.Reflection;

public partial class Settings : SingletonNode
{
  const string SETTINGS_FILE = "user://settings.cfg";
  public bool Loaded { get; set; }

  #region Audio signals

  [Signal]
  public delegate void VolumeChangedEventHandler (string bus_name, int volume);

  #endregion

  public AudioSettings Audio { get; private set; }

  public override void _EnterTree()
  {
    base._EnterTree();

    Audio = new AudioSettings();

    SetDefaultSettings();
    LoadData();
  }

  void SetDefaultSettings ()
  {
    Audio.SetVolumeMaster(10);
    Audio.SetVolumeBGM(10);
    Audio.SetVolumeSFX(10);
  }

  void LoadData ()
  {
    ConfigFile cfg = new ConfigFile();
    if (cfg.Load(SETTINGS_FILE) != Error.Ok)
      return;

    foreach (string section in cfg.GetSections())
      foreach (string key in cfg.GetSectionKeys(section))
        GetType().GetProperty(key)?.SetValue(this, cfg.GetValue(section, key));
  }

  void SaveData ()
  {
    ConfigFile cfg = new ConfigFile();

    foreach (PropertyInfo prop in Audio.GetType().GetProperties())
      cfg.SetValue("Audio", prop.Name, (Variant)prop.GetValue(Audio));

      cfg.Save(SETTINGS_FILE);
  }
}

public struct AudioSettings
{
  public int VolumeMaster { get; private set; }
  public int VolumeBGM { get; private set; }
  public int VolumeSFX { get; private set; }

  internal void SetVolumeMaster (int volume) => VolumeMaster = volume;
  internal void SetVolumeBGM (int volume) => VolumeBGM = volume;
  internal void SetVolumeSFX (int volume) => VolumeSFX = volume;
}