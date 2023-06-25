public partial class ProfilesSettingsContainer : ScrollContainer
{
  VBoxContainer ProfileListContainer, ProfileEditContainer;
  Button NewProfileButton;
  ButtonGroup ProfileButtonGroup = new ButtonGroup();

  LineEdit NameField;
  HSlider LeftStickDeadzone;
  Label LeftStickDeadzoneValue;
  HBoxContainer MoveUp, MoveDown, MoveLeft, MoveRight;

  PlayerProfile ProfileActive = new PlayerProfile {};

  Dictionary<string, Texture2D> Icons = new Dictionary<string, Texture2D>();

  bool ReadingInputs = false, ReadingGamepad = false;
  string EditingInput = "", PreviousBinding = "";
  Button EditingButton = null;

  public override void _EnterTree ()
  {
    foreach (string file in DirAccess.GetFilesAt("res://assets/icons/"))
    {
      if (file.Contains(".import"))
        continue;

      Icons.Add(file.Replace(".png", ""), Load($"res://assets/icons/{file}") as Texture2D);
    }
  }

  public override void _Ready ()
  {
    NewProfileButton = GetNode("%NewProfileButton") as Button;
    ProfileListContainer = GetNode("%ProfileList") as VBoxContainer;
    ProfileEditContainer = GetNode("%ProfileEdit") as VBoxContainer;
    ProfileButtonGroup = new ButtonGroup();

    PopulateProfileButtons();

    NewProfileButton.Pressed += () => {
      if (!ReadingInputs)
        InitLoadProfile(PlayerProfileManager.NEW_PROFILE());
    };

    ProfileEditContainer.Visible = false;

    NameField = GetNode("%NameField") as LineEdit;
    LeftStickDeadzone = GetNode("%LSDeadzoneSlider") as HSlider;
    LeftStickDeadzoneValue = GetNode("%LSDeadzoneValue") as Label;
    LeftStickDeadzoneValue.Text = $"{Math.Round(LeftStickDeadzone.Value, 2)}";

    MoveUp = GetNode("%MoveUp") as HBoxContainer;
    MoveDown = GetNode("%MoveDown") as HBoxContainer;
    MoveLeft = GetNode("%MoveLeft") as HBoxContainer;
    MoveRight = GetNode("%MoveRight") as HBoxContainer;

    HookUpInputEvents();
    HookUpSaveEvents();
  }

  void HookUpInputEvents ()
  {
    LeftStickDeadzone.ValueChanged += (double value) => LeftStickDeadzoneValue.Text = $"{Math.Round(value, 2)}";

    ProfileEditContainer.VisibilityChanged += () => {
      if (ProfileEditContainer.Visible)
        NameField.Text = ProfileActive.Name;
    };

    foreach (Node child in MoveUp.GetChildren())
      if (child is Button)
        (child as Button).Pressed += () => OnInputButtonPressed(child as Button);

    foreach (Node child in MoveDown.GetChildren())
      if (child is Button)
        (child as Button).Pressed += () => OnInputButtonPressed(child as Button);

    foreach (Node child in MoveLeft.GetChildren())
      if (child is Button)
        (child as Button).Pressed += () => OnInputButtonPressed(child as Button);

    foreach (Node child in MoveRight.GetChildren())
      if (child is Button)
        (child as Button).Pressed += () => OnInputButtonPressed(child as Button);
  }

  void HookUpSaveEvents ()
  {
    NameField.TextSubmitted += (string s) => Save();
    LeftStickDeadzone.DragEnded += (bool changed) => { if (changed) Save(); };
  }

  public override void _Process (double dt)
  {
  }

  void InitLoadProfile (PlayerProfile profile)
  {
    ProfileActive = profile;
    ProfileEditContainer.Visible = false;

    LeftStickDeadzone.Value = profile.Deadzone;
    PopulateInputButtons();

    ProfileEditContainer.Visible = true;
  }
  
  void Save ()
  {
    string oldName = ProfileActive.Name;

    ProfileActive.Name = NameField.Text;
    ProfileActive.Deadzone = (float)LeftStickDeadzone.Value;

    string testPath = $"{PlayerProfileManager.DIR_PATH}{ProfileActive.Name}.{PlayerProfileManager.FILE_EXTENSION}";
    if (ProfileActive.Path != $"{PlayerProfileManager.DIR_PATH}{ProfileActive.Name}.{PlayerProfileManager.FILE_EXTENSION}")
    {
      PlayerProfileManager.GetProfiles().Remove(oldName);
      DirAccess.RemoveAbsolute(ProfileActive.Path);
      ProfileActive.Path = testPath;
    }

    PlayerProfileManager.SaveProfile(ProfileActive);

    foreach (Node child in ProfileListContainer.GetChildren())
      if (child.Name != NewProfileButton.Name)
      {
        ProfileListContainer.RemoveChild(child);
        child.Free();
      }

    PopulateProfileButtons();
  }

  void PopulateProfileButtons ()
  {
    foreach (string profileName in PlayerProfileManager.GetProfiles().Keys)
    {
      Button profileButton = new Button();
      profileButton.Text = PlayerProfileManager.GetProfiles()[profileName].Name;
      profileButton.ToggleMode = true;
      profileButton.ButtonGroup = ProfileButtonGroup;
      profileButton.Toggled += (bool pressed) => {
        if (pressed && !ReadingInputs)
          InitLoadProfile(PlayerProfileManager.GetProfiles()[profileName]);
      };
      ProfileListContainer.AddChild(profileButton);
    }
    ProfileListContainer.MoveChild(NewProfileButton, -1);
  }

  void PopulateInputButtons ()
  {
    PopulateInputButtonRow(MoveUp, "move_up");
    PopulateInputButtonRow(MoveDown, "move_down");
    PopulateInputButtonRow(MoveLeft, "move_left");
    PopulateInputButtonRow(MoveRight, "move_right");
  }

  internal void PopulateInputButtonRow (HBoxContainer row, string name)
  {
    foreach (Node child in row.GetChildren())
      if (child is Button)
      {
        (child as Button).Text = "";
        (child as Button).Icon = null;
      }

    List<ControlBinding> bindings = ProfileActive.Bindings[name];
    int kbAssigned = 0, gpAssigned = 0;
    foreach (ControlBinding binding in bindings)
    {
      if (kbAssigned <= 2 && binding.ControlType == typeof(InputEventKey))
      {
        (row.GetNode($"KB{++kbAssigned}") as Button).Text = ((Key)binding.ControlIndex).ToString();
        (row.GetNode($"KB{kbAssigned}") as Button).SetMeta("Binding", binding.AsText());
      }
      
      if (gpAssigned <= 2 && binding.ControlType != typeof(InputEventKey))
      {
        bool isAxis = binding.ControlType == typeof(InputEventJoypadMotion);
        string iconPath = $"{(isAxis ? "gpa" : "gpb")}{binding.ControlIndex}{(isAxis && binding.ControlIndex < 4 ? (binding.ControlDirection > 0 ? "+" : "-") : "")}";

        (row.GetNode($"GP{++gpAssigned}") as Button).Icon = Icons[iconPath];
        (row.GetNode($"GP{gpAssigned}") as Button).SetMeta("Binding", binding.AsText());
      }
    }
  }

  void OnInputButtonPressed (Button button)
  {
    if (ReadingInputs)
      return;

    ReadingInputs = true;
    EditingButton = button;
    ReadingGamepad = (bool)button.GetMeta("IsGamepad");
    EditingInput = (string)button.GetMeta("Input");
    PreviousBinding = (string)button.GetMeta("Binding");
    
    button.Icon = null;
    button.Text = $"Awaiting {(ReadingGamepad ? "gamepad" : "keyboard")} input";
  }

  public override void _Input (InputEvent evt)
  {
    if (!ReadingInputs)
    {
      base._Input(evt);
      return;
    }

    if (ReadingGamepad == evt is InputEventKey)
      return;
      
    if (ReadingGamepad && evt is InputEventMouseMotion)
      return;

    if ((ReadingGamepad && evt is InputEventJoypadButton && (evt as InputEventJoypadButton).ButtonIndex == JoyButton.Start) ||
        (!ReadingGamepad && (evt as InputEventKey).PhysicalKeycode == Key.Escape))
    {
      ReadingInputs = false;
      EditingButton = null;
      PopulateInputButtons();
      return;
    }

    AcceptEvent();

    ControlBinding newBind = ControlBinding.FromInputEvent(evt);
    if (PreviousBinding.Length == 0)
    {
      ProfileActive.Bindings[EditingInput].Add(newBind);
      EditingButton.SetMeta("binding", newBind.AsText());
    }
    else
      try
      {
        foreach (ControlBinding bind in ProfileActive.Bindings[EditingInput])
          if (PreviousBinding == bind.AsText())
          {
            int idx = ProfileActive.Bindings[EditingInput].FindIndex(b => b.AsText() == bind.AsText());
            ProfileActive.Bindings[EditingInput][idx] = newBind;

            EditingButton.SetMeta("binding", newBind.AsText());
          }
      }
      catch (Exception)
      { }

    ReadingInputs = false;
    EditingButton = null;
    PlayerProfileManager.SaveProfile(ProfileActive);
    PopulateInputButtons();
  }

  public override void _GuiInput (InputEvent evt)
  {
    if (ReadingInputs)
      return;

    base._GuiInput(evt);
  }
}
