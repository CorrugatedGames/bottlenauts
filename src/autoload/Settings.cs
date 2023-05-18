using System.Reflection;

public partial class Settings : SingletonNode
{
  const string SETTINGS_FILE = "user://settings.cfg";
  public bool Loaded { get; set; }

  #region Audio signals

  [Signal]
  public delegate void VolumeChangedEventHandler (int bus, int volume);

  #endregion

  public static AudioSettings Audio { get; private set; }

  public override void _EnterTree()
  {
    base._EnterTree();

    Audio = new AudioSettings();

    LoadData();
  }

  void LoadData ()
  {
    ConfigFile cfg = new ConfigFile();
    if (cfg.Load(SETTINGS_FILE) != Error.Ok)
    {
      SaveData();
      return;
    }

    foreach (PropertyInfo prop in Audio.GetType().GetProperties())
    {
      int volume = (int)cfg.GetValue("Audio", prop.Name);
      int bus = AudioServer.GetBusIndex(prop.Name.Replace("Volume", ""));

      EmitSignal(SignalName.VolumeChanged, bus, volume);

      switch (prop.Name)
      {
        case "VolumeMaster":
        {
          Audio.SetVolumeMaster((int)cfg.GetValue("Audio", prop.Name));
        } break;
        
        case "VolumeBGM":
        {
          Audio.SetVolumeBGM((int)cfg.GetValue("Audio", prop.Name));
        } break;
        
        case "VolumeSFX":
        {
          Audio.SetVolumeSFX((int)cfg.GetValue("Audio", prop.Name));
        } break;

        default: break;
      }
    }
  }

  public static void SaveData ()
  {
    ConfigFile cfg = new ConfigFile();

    foreach (PropertyInfo prop in Audio.GetType().GetProperties())
      cfg.SetValue("Audio", prop.Name, Variant.From<int>((int)prop.GetValue(Audio)));

    cfg.Save(SETTINGS_FILE);
  }
}

public class AudioSettings
{
  public int VolumeMaster { get; set; }
  public int VolumeBGM { get; set; }
  public int VolumeSFX { get; set; }

  public void SetVolumeMaster (int volume) => VolumeMaster = volume;
  public void SetVolumeBGM (int volume) => VolumeBGM = volume;
  public void SetVolumeSFX (int volume) => VolumeSFX = volume;

  public AudioSettings ()
  {
    VolumeMaster = 100;
    VolumeBGM = 100;
    VolumeSFX = 100;
  }
}