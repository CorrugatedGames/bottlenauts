public partial class PlayerProfileManager : SingletonNode
{
  static string DIR_PATH = "user://player_profiles/";
  static string FILE_EXTENSION = "bpro";

  static PlayerProfileManager Instance = new PlayerProfileManager();

  public Dictionary<string, PlayerProfile> Profiles { get; } = new Dictionary<string, PlayerProfile>();

  public override void _EnterTree ()
  {
    if (DirAccess.DirExistsAbsolute(DIR_PATH))
    {
      string [] files = DirAccess.GetFilesAt(DIR_PATH);
      foreach (string file in files)
        LoadProfile($"{DIR_PATH}{file}");

      if (!HasDefaultProfiles())
        CreateDefaultProfiles();
    }
    else
    {
      Logger.Info("No player_profiles/ directory found, creating one...");
      DirAccess.MakeDirRecursiveAbsolute(DIR_PATH);

      Logger.Info("Creating default player profile files...");
      CreateDefaultProfiles();
    }
  }

  internal bool HasDefaultProfiles ()
  {
    for (int id = 1; id <= 4; id++)
      if (!FileAccess.FileExists($"{DIR_PATH}P{id}.{FILE_EXTENSION}"))
        return false;

    return true;
  }

  public static void CreateDefaultProfiles ()
  {
    Dictionary<string, List<ControlBinding>> defaultBindings = new Dictionary<string, List<ControlBinding>> ();
    defaultBindings.Add("move_up", new List<ControlBinding> {
      new ControlBinding { ControlType = typeof(InputEventKey), ControlIndex = (int)Key.W },
      new ControlBinding { ControlType = typeof(InputEventJoypadMotion), ControlIndex = (int)JoyAxis.LeftY, ControlDirection = -1 },
      new ControlBinding { ControlType = typeof(InputEventJoypadButton), ControlIndex = (int)JoyButton.DpadUp },
    });
    defaultBindings.Add("move_down", new List<ControlBinding> {
      new ControlBinding { ControlType = typeof(InputEventKey), ControlIndex = (int)Key.S },
      new ControlBinding { ControlType = typeof(InputEventJoypadMotion), ControlIndex = (int)JoyAxis.LeftY, ControlDirection = 1 },
      new ControlBinding { ControlType = typeof(InputEventJoypadButton), ControlIndex = (int)JoyButton.DpadDown },
    });
    defaultBindings.Add("move_left", new List<ControlBinding> {
      new ControlBinding { ControlType = typeof(InputEventKey), ControlIndex = (int)Key.A },
      new ControlBinding { ControlType = typeof(InputEventJoypadMotion), ControlIndex = (int)JoyAxis.LeftX, ControlDirection = -1 },
      new ControlBinding { ControlType = typeof(InputEventJoypadButton), ControlIndex = (int)JoyButton.DpadLeft },
    });
    defaultBindings.Add("move_right", new List<ControlBinding> {
      new ControlBinding { ControlType = typeof(InputEventKey), ControlIndex = (int)Key.D },
      new ControlBinding { ControlType = typeof(InputEventJoypadMotion), ControlIndex = (int)JoyAxis.LeftX, ControlDirection = 1 },
      new ControlBinding { ControlType = typeof(InputEventJoypadButton), ControlIndex = (int)JoyButton.DpadRight },
    });
    
    for (int id = 1; id <= 4; id++)
    {
      if (FileAccess.FileExists($"{DIR_PATH}P{id}.{FILE_EXTENSION}"))
        continue;

      SaveProfile(new PlayerProfile {
        Name = $"P{id}",

        LeftStickDeadzone = 0.3f,
        RightStickDeadzone = 0.3f,
        Bindings = defaultBindings,
      });
    }
  }

  public static void SaveProfile (PlayerProfile profile)
  {
    ConfigFile file = new ConfigFile();

    file.SetValue("General", "Name", profile.Name);

    file.SetValue("Controls", "LeftStickDeadzone", profile.LeftStickDeadzone);
    file.SetValue("Controls", "RightStickDeadzone", profile.RightStickDeadzone);
    foreach (string action in profile.Bindings.Keys)
    {
      List<string> codes = new List<string>();
      foreach (ControlBinding binding in profile.Bindings[action])
        codes.Add(binding.AsText());

      file.SetValue("Controls", action, codes.ToArray().Join(";"));
    }

    Logger.Info($"Saved profile {profile.Name} to {DIR_PATH}{profile.Name}.{FILE_EXTENSION}");
    file.Save($"{DIR_PATH}{profile.Name}.{FILE_EXTENSION}");

    Instance.Profiles.Add(profile.Name, profile);
  }

  public static void LoadProfile (string absolutePath)
  {
    ConfigFile file = new ConfigFile();
    if (file.Load(absolutePath) != Error.Ok)
      return;

    PlayerProfile profile = new PlayerProfile();

    profile.Name = (string)file.GetValue("General", "Name");

    profile.Bindings = new Dictionary<string, List<ControlBinding>>();
    foreach (string controlsKey in file.GetSectionKeys("Controls"))
      switch (controlsKey)
      {
        case "LeftStickDeadzone":
          profile.LeftStickDeadzone = (float)file.GetValue("Controls", controlsKey);
          break;

        case "RightStickDeadzone":
          profile.LeftStickDeadzone = (float)file.GetValue("Controls", controlsKey);
          break;

        default:
          string [] codes = ((string)file.GetValue("Controls", controlsKey)).Split(";");
          List<ControlBinding> bindings = new List<ControlBinding>();

          foreach (string code in codes)
            bindings.Add(ControlBinding.FromText(code));

          profile.Bindings.Add(controlsKey, bindings);
          break;
      }

    Instance.Profiles.Add(profile.Name, profile);
  }

  public static Dictionary<string, PlayerProfile> GetProfiles () => Instance.Profiles;
}