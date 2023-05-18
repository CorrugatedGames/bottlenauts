public partial class AudioSettingsContainer : ScrollContainer
{
  #region Child nodes

  SliderContainer MasterVolume, MusicVolume, SFXVolume;

  #endregion

  public override void _Ready ()
  {
    MasterVolume = GetNode("%MasterVolumeContainer") as SliderContainer;
    MusicVolume = GetNode("%MusicVolumeContainer") as SliderContainer;
    SFXVolume = GetNode("%SFXVolumeContainer") as SliderContainer;

    MasterVolume.Value = Settings.Audio.VolumeMaster;
    MusicVolume.Value = Settings.Audio.VolumeBGM;
    SFXVolume.Value = Settings.Audio.VolumeSFX;

    MasterVolume.Slider.ValueChanged += OnMasterVolumeChanged;
    MusicVolume.Slider.ValueChanged += OnMusicVolumeChanged;
    SFXVolume.Slider.ValueChanged += OnSFXVolumeChanged;
  }

  void ChangeVolume (int volume, string bus) => Settings.EmitSignal(Settings.SignalName.VolumeChanged, AudioServer.GetBusIndex(bus), volume);

  void OnMasterVolumeChanged (double volume) => ChangeVolume((int)volume, "Master");
  void OnMusicVolumeChanged (double volume) => ChangeVolume((int)volume, "BGM");
  void OnSFXVolumeChanged (double volume) => ChangeVolume((int)volume, "SFX");
}
