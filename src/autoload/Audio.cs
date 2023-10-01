public partial class Audio : Node
{
  public override void _Ready()
  {
    Logger.Info("Audio signals initializing...");
    for (int i = 0; i < AudioServer.BusCount; i++)
    {
      string bus = AudioServer.GetBusName(i);
      int volume;

      switch (bus)
      {
        case "Master":
          {
            volume = Settings.Audio.VolumeMaster;
          }
          break;

        case "BGM":
          {
            volume = Settings.Audio.VolumeBGM;
          }
          break;

        case "SFX":
          {
            volume = Settings.Audio.VolumeSFX;
          }
          break;

        default:
          {
            volume = 0;
          }
          break;
      }

      OnVolumeChanged(i, volume);
    }

    Settings.Instance.VolumeChanged += (bus, volume) =>  OnVolumeChanged(bus, volume);

    Logger.Info("Audio signals initialized!");
  }

  void OnVolumeChanged(int bus, int volume)
  {
    Logger.Info($"Volume bus {bus} changing volume to {volume}...");
    AudioServer.SetBusVolumeDb(bus, Mathf.LinearToDb(volume / 100.0f));

    switch (AudioServer.GetBusName(bus))
    {
      case "Master":
        {
          Settings.Audio.SetVolumeMaster(volume);
        }
        break;

      case "BGM":
        {
          Settings.Audio.SetVolumeBGM(volume);
        }
        break;

      case "SFX":
        {
          Settings.Audio.SetVolumeSFX(volume);
        }
        break;

      default: break;
    }

    Settings.SaveData();
  }
}