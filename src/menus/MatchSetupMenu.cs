using System.Linq;

public partial class MatchSetupMenu : Control
{
    #region Child nodes
    VBoxContainer CharacterSetupContainer,
        GameSetupContainer;

    GridContainer PlayerGrid1,
        PlayerGrid2;

    Button MainMenuButton,
        ToGameSetupButton,
        ToCharacterSetupButton,
        StartGameButton;
    OptionButton GameModeSelectButton,
        LevelSelectButton,
        ThemeSelectButton;

    // todo(jam): replace GameModeSelectButton with carousel, Level/ThemeSelectButtons with a popup menu?
    VBoxContainer ModifierListContainer;

    SpinBox PlayerCountSpinner;
    #endregion

    #region Menu slide transition
    [Export]
    float TransitionLength = 0.3f;
    float TransitionAccumulator = 0f;
    float TransitionDirection;
    bool InTransition = false;
    #endregion

    #region Asset directory lists
    List<string> ProfileNames;
    List<string> LevelNames;
    List<string> LevelThemeNames;
    #endregion

    public override void _EnterTree()
    {
        GetListFromDir(PLAYER_PROFILE_DIRECTORY, ref ProfileNames);
        GetListFromDir(LEVEL_DIRECTORY, ref LevelNames, true);
        GetListFromDir(THEME_DIRECTORY, ref LevelThemeNames, true);
    }

    void GetListFromDir(string dirPath, ref List<string> list, bool allowRandom = false)
    {
        list = new();
        if (allowRandom)
            list.Add("Random");
        DirAccess dir = DirAccess.Open(dirPath);
        foreach (string path in dir.GetFiles())
            list.Add(path.Split(".")[0]);
    }

    public override void _Ready()
    {
        CharacterSetupContainer = GetNode("%CharacterSetup") as VBoxContainer;
        GameSetupContainer = GetNode("%GameSetup") as VBoxContainer;

        MainMenuButton = GetNode("%MainMenuButton") as Button;
        MainMenuButton.Pressed += () =>
            GetTree().ChangeSceneToFile("res://scenes/menus/TitleScreen.tscn");
        ToGameSetupButton = GetNode("%ToGameSetupButton") as Button;
        ToGameSetupButton.Pressed += () => SlideMenu(false);
        ToCharacterSetupButton = GetNode("%ToCharacterSetupButton") as Button;
        ToCharacterSetupButton.Pressed += () => SlideMenu(true);
        StartGameButton = GetNode("%StartGameButton") as Button;
        StartGameButton.Pressed += StartGame;

        PlayerCountSpinner = GetNode("%PlayerCountSpinner") as SpinBox;

        PlayerGrid1 = GetNode("%PlayerGridContainer1") as GridContainer;
        PlayerGrid2 = GetNode("%PlayerGridContainer2") as GridContainer;
        foreach (CharacterSelectPanel panel in PlayerGrid1.GetChildren())
        {
            panel.SetProfileNames(ProfileNames);
            panel.PlayerReady += UpdatePlayersReady;
        }
        foreach (CharacterSelectPanel panel in PlayerGrid2.GetChildren())
            panel.SetProfileNames(ProfileNames);
        SetPlayerGrids(PlayerCountSpinner.Value);
        PlayerCountSpinner.ValueChanged += (double value) =>
        {
            MatchSettingsState.UpdatePlayerCount(Mathf.FloorToInt(value));
        };
        PlayerCountSpinner.ValueChanged += SetPlayerGrids;

        GameModeSelectButton = GetNode("%GameModeSelectButton") as OptionButton;
        foreach (string gameModeName in Enum.GetNames(typeof(GameMode)))
            GameModeSelectButton.AddItem(gameModeName);
        GameModeSelectButton.Select(0);
        GameModeSelectButton.ItemSelected += SelectGameMode;

        LevelSelectButton = GetNode("%LevelSelectButton") as OptionButton;
        foreach (string levelName in LevelNames)
            LevelSelectButton.AddItem(levelName); // todo(jam): eventually  add this is AddIconItem, with thumbnails!
        LevelSelectButton.Select(0);
        LevelSelectButton.ItemSelected += SelectLevel;

        ThemeSelectButton = GetNode("%ThemeSelectButton") as OptionButton;
        foreach (string themeName in LevelThemeNames)
            ThemeSelectButton.AddItem(themeName); // todo(jam): eventually  add this is AddIconItem, with thumbnails!
        ThemeSelectButton.Select(0);
        ThemeSelectButton.ItemSelected += SelectTheme;

        ModifierListContainer = GetNode("%ModifierListContainer") as VBoxContainer;
        foreach (string modifierName in Enum.GetNames(typeof(GameModifier)))
        {
            CheckButton checkButton = new() { Text = modifierName, ButtonPressed = false };
            checkButton.Pressed += () =>
            {
                GameModifier value = Enum.Parse<GameModifier>(modifierName);
                MatchSettingsState.ToggleGameModifier(value);

                MatchSettingsState.PrintActiveGameModifiers();
            };

            ModifierListContainer.AddChild(checkButton);
        }
    }

    public override void _Process(double dt)
    {
        if (InTransition)
        {
            TransitionAccumulator += (float)dt;

            float offset = (float)dt / TransitionLength * TransitionDirection;

            CharacterSetupContainer.AnchorLeft += offset;
            CharacterSetupContainer.AnchorRight += offset;

            GameSetupContainer.AnchorLeft += offset;
            GameSetupContainer.AnchorRight += offset;

            if (TransitionAccumulator > TransitionLength)
            {
                CharacterSetupContainer.AnchorLeft = TransitionDirection == 1 ? 0f : -1f;
                CharacterSetupContainer.AnchorRight = TransitionDirection == 1 ? 1f : 0f;

                GameSetupContainer.AnchorLeft = TransitionDirection == 1 ? 1f : 0f;
                GameSetupContainer.AnchorRight = TransitionDirection == 1 ? 2f : 1f;

                TransitionAccumulator = TransitionDirection = 0f;
                InTransition = false;
            }
        }
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (evt is InputEventKey keyEvent)
        {
            if (
                keyEvent.Keycode == Key.Space
                && keyEvent.Pressed
                && !MatchSettingsState.HasKeyboardPlayer
            )
            {
                GetViewport().SetInputAsHandled();
                AddPlayer(false, -1);
            }
        }

        if (evt is InputEventJoypadButton buttonEvent)
        {
            if (
                buttonEvent.Pressed
                && buttonEvent.ButtonIndex == JoyButton.A
                && !MatchSettingsState.HasGamepadPlayer(buttonEvent.Device)
            )
            {
                GetViewport().SetInputAsHandled();
                AddPlayer(true, buttonEvent.Device);
            }
        }
    }

    void AddPlayer(bool isGamepad, int deviceIdx)
    {
        int playerIdx = MatchSettingsState.AddPlayer(isGamepad, deviceIdx);
        Logger.Trace(
            $"Adding {(isGamepad ? "gamepad" : "keyboard")} player {playerIdx}{(isGamepad ? $" at device {deviceIdx}" : "")}"
        );
        Node[] panels = PlayerGrid1.GetChildren().Concat(PlayerGrid2.GetChildren()).ToArray();
        foreach (CharacterSelectPanel panel in panels.Cast<CharacterSelectPanel>())
            if (panel.PlayerNumber == playerIdx)
                panel.SetPlayer(MatchSettingsState.Instance.Players[playerIdx], panel.PlayerNumber);
        UpdatePlayersReady();
    }

    void SlideMenu(bool toCharacterScreen)
    {
        GetViewport().GuiGetFocusOwner().ReleaseFocus();

        if (InTransition)
            return;

        InTransition = true;
        TransitionDirection = toCharacterScreen ? 1f : -1f;
        TransitionAccumulator = 0f;
    }

    void SetPlayerGrids(double count)
    {
        int playerCount = Mathf.RoundToInt(count);
        bool needTwoRows = playerCount > 4;

        PlayerGrid1.Columns = needTwoRows ? Mathf.CeilToInt(playerCount / 2.0) : playerCount;
        for (int i = 0; i < PlayerGrid1.GetChildCount(); i++)
        {
            CharacterSelectPanel panel = PlayerGrid1.GetChild(i) as CharacterSelectPanel;
            panel.Visible = i < PlayerGrid1.Columns;
            if (panel.Visible)
                panel.SetPlayerNumber(panel.Visible ? i : -1);
            else
                panel.Reset();
        }

        PlayerGrid2.Visible = needTwoRows;
        PlayerGrid2.Columns = Mathf.FloorToInt(playerCount / 2.0);
        for (int i = 0; i < PlayerGrid2.GetChildCount(); i++)
        {
            CharacterSelectPanel panel = PlayerGrid2.GetChild(i) as CharacterSelectPanel;
            panel.Visible = i < PlayerGrid2.Columns;
            if (panel.Visible)
                panel.SetPlayerNumber(panel.Visible ? i + PlayerGrid1.Columns : -1);
            else
                panel.Reset();
        }
    }

    List<CharacterSelectPanel> GetActivePanels()
    {
        List<CharacterSelectPanel> panels = new();

        foreach (CharacterSelectPanel panel in PlayerGrid1.GetChildren())
            if (panel.Visible)
                panels.Add(panel);

        if (PlayerGrid2.Visible)
            foreach (CharacterSelectPanel panel in PlayerGrid2.GetChildren())
                if (panel.Visible)
                    panels.Add(panel);

        return panels;
    }

    void UpdatePlayersReady()
    {
        GetViewport().GuiGetFocusOwner()?.ReleaseFocus();

        bool playersReady = true;

        foreach (CharacterSelectPanel panel in GetActivePanels())
            if (panel.HasPlayer && !panel.ReadyButton.ButtonPressed)
            {
                playersReady = false;
                break;
            }

        ToGameSetupButton.Disabled = !playersReady;
    }

    void SelectGameMode(long idx) => MatchSettingsState.SetGameMode((GameMode)idx);

    void SelectLevel(long idx) =>
        MatchSettingsState.LevelName = LevelSelectButton.GetItemText((int)idx);

    void SelectTheme(long idx) =>
        MatchSettingsState.ThemeName = ThemeSelectButton.GetItemText((int)idx);

    void StartGame()
    {
        if (LevelSelectButton.Selected == 0) // if random
            SelectLevel(new Random().NextInt64(1, LevelSelectButton.ItemCount));

        if (ThemeSelectButton.Selected == 0)
            SelectTheme(new Random().NextInt64(1, ThemeSelectButton.ItemCount));

        while (MatchSettingsState.PlayerCount > MatchSettingsState.CurrentPlayerCount)
            MatchSettingsState.GenerateCPUPlayer();

        MatchSettingsState.Generate();

        GetTree().ChangeSceneToFile("res://scenes/GameScene.tscn");
    }
}
