public interface ISettingsCategory
{
  public bool LoadedConfig { get; set; }

  public void LoadFromConfigFile (ConfigFile cfg, string category);
  public void SaveToConfigFile (ConfigFile cfg, string category);
  internal void SetDefaultSettings ();

}