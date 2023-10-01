using System.Reflection;

public sealed class AudioSettings : ISettingsCategory
{
  public bool LoadedConfig { get; set; }

  public int VolumeMaster { get; private set; }
  public int VolumeBGM { get; private set; }
  public int VolumeSFX { get; private set; }

  public void LoadFromConfigFile (ConfigFile cfg, string category)
  {
    foreach (PropertyInfo prop in GetType().GetProperties())
    {
      if (prop.Name == "LoadedConfig") continue;

      int volume = (int)cfg.GetValue(category, prop.Name);
      int bus = AudioServer.GetBusIndex(prop.Name.Replace("Volume", ""));
      
      Settings.Instance.EmitSignal(Settings.SignalName.VolumeChanged, bus, volume);

      switch (prop.Name)
      {
        case "VolumeMaster":
          {
            SetVolumeMaster((int)cfg.GetValue(category, prop.Name));
          }
          break;

        case "VolumeBGM":
          {
            SetVolumeBGM((int)cfg.GetValue(category, prop.Name));
          }
          break;

        case "VolumeSFX":
          {
            SetVolumeSFX((int)cfg.GetValue(category, prop.Name));
          }
          break;

        default: break;
      }
    }
  }

  public void SaveToConfigFile (ConfigFile cfg, string category)
  {
    foreach (PropertyInfo prop in GetType().GetProperties())
    {
      if (prop.Name == "LoadedConfig") continue;
      cfg.SetValue(category, prop.Name, Variant.From<int>((int)prop.GetValue(this)));
    }
  }

  public void SetDefaultSettings () {
    VolumeMaster = 100;
    VolumeBGM = 100;
    VolumeSFX = 100;
  }

  public void SetVolumeMaster (int volume) => VolumeMaster = volume;
  public void SetVolumeBGM (int volume) => VolumeBGM = volume;
  public void SetVolumeSFX (int volume) => VolumeSFX = volume;
}