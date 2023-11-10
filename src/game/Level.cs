using System.Linq;

public partial class Level : Node3D
{
    [Export]
    public LevelTheme Theme { get; set; } = null;

    public LevelCamera Camera { get; private set; }
    public GridMap Map { get; private set; }

    [ExportCategory("Level Fill Parameters")]
    [Export(PropertyHint.Range, "0,1,0.01")]
    float DestructibleFillPercentage = 0.4f;

    [Export(PropertyHint.Range, "0,1,0.01")]
    float ObstacleFillPercentage = 0.2f;

    private bool HasEndSequenceStarted = false;
    private List<BNPlayer> Players = new List<BNPlayer>();

    public override void _Ready()
    {
        Map = GetNode("GridMap") as GridMap;
        Camera = GetNode("LevelCamera") as LevelCamera;

        MatchSettingsState.GeneratePlayerBindings();
        GenerateHazards();
        SetSpawns();
        SetLevelTheme(Theme);
    }

    public override void _Input(InputEvent evt)
    {
        if (evt is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed && keyEvent.Keycode == Key.F6)
                DEVStealPlayerOne(-1);

            if (keyEvent.Pressed && keyEvent.Keycode == Key.F9)
                DEVEndSequence();
        }

        if (evt is InputEventJoypadButton buttonEvent)
            if (buttonEvent.Pressed && buttonEvent.ButtonIndex == JoyButton.LeftStick)
                DEVStealPlayerOne(buttonEvent.Device);
    }

    public override void _Process(double delta)
    {
        if (ShouldEndGame())
        {
            BeginEndSequence();
        }
    }

    void DEVStealPlayerOne(int deviceIdx)
    {
#if DEBUG
        BNPlayer player = MatchSettingsState.GetPlayer(0);
        player.IsCPU = false;
        player.IsGamepad = deviceIdx > -1;
        player.DeviceIndex = deviceIdx;

        MatchSettingsState.Instance.Players[0] = player;
        MatchSettingsState.GeneratePlayerBindings();
#endif
    }

    void DEVEndSequence()
    {
#if DEBUG
        EndSequence();
#endif
    }

    public void SetLevelTheme(LevelTheme theme)
    {
        if (theme == null)
            return;

        Theme = theme;
        SetMeshLibrary(theme);
        Camera.Environment = Theme.Environment;

        // assign material overrides to all child nodes in group Characters
    }

    void OverwriteGridMapCell(Vector3I cell, MeshLibrary meshlib, string cellType, int count)
    {
        int orientation = Map.GetCellItemOrientation(cell);
        if (orientation == -1)
        {
            Logger.Warning(
                $"GridMap cell at {cell.X} , {cell.Y} , {cell.Z} is empty, unable to overwrite"
            );
            return;
        }

        string name = $"{cellType}{(count > 1 ? (Randi() % count) : 0)}";
        int item = meshlib.FindItemByName(name);
        if (item == -1)
        {
            Logger.Warning(
                $"Mesh library {meshlib.ResourceName} does not contain item with name {name}, unable to overwrite"
            );
            return;
        }

        Map.SetCellItem(cell, item, orientation);
    }

    void SetMeshLibrary(LevelTheme theme)
    {
        MeshLibrary meshlib = theme.MeshLibrary;
        int floorTileCount = theme.FloorTileCount;
        int wallTileCount = theme.WallTileCount;
        int pillarTileCount = theme.PillarTileCount;
        int rampTileCount = theme.RampTileCount;

        var allCells = new Godot.Collections.Array<Vector3I>[(int)MetaCell.CELL_COUNT];
        for (int cellIdx = 0; cellIdx < (int)MetaCell.CELL_COUNT; cellIdx++)
            allCells[cellIdx] = Map.GetUsedCellsByItem(cellIdx);

        Map.MeshLibrary = meshlib;

        for (int cellIdx = 0; cellIdx < (int)MetaCell.CELL_COUNT; cellIdx++)
        {
            var cells = allCells[cellIdx];

            switch (cellIdx)
            {
                case (int)MetaCell.FLOOR:

                    {
                        foreach (var cell in cells)
                            OverwriteGridMapCell(cell, meshlib, "floor", floorTileCount);
                    }
                    break;

                case (int)MetaCell.WALL:

                    {
                        foreach (var cell in cells)
                            OverwriteGridMapCell(cell, meshlib, "wall", wallTileCount);
                    }
                    break;

                case (int)MetaCell.PILLAR:

                    {
                        foreach (var cell in cells)
                            OverwriteGridMapCell(cell, meshlib, "pillar", pillarTileCount);
                    }
                    break;

                case (int)MetaCell.RAMP:

                    {
                        foreach (var cell in cells)
                            OverwriteGridMapCell(cell, meshlib, "ramp", rampTileCount);
                    }
                    break;

                default:

                    {
                        foreach (var cell in cells)
                            Map.SetCellItem(cell, -1);
                    }
                    break;
            }
        }
    }

    private IEnumerable<T> GetPercentageOfList<[MustBeVariant] T>(
        Godot.Collections.Array<T> list,
        float percentage
    )
    {
        var copy = list.Duplicate();
        copy.Shuffle();

        return copy[..(int)(copy.Count * percentage)];
    }

    private void GenerateHazards()
    {
        PackedScene destruScene =
            ResourceLoader.Load("res://scenes/objects/hazards/Destructible.obj.tscn")
            as PackedScene;
        PackedScene obstaScene =
            ResourceLoader.Load("res://scenes/objects/hazards/Obstacle.obj.tscn") as PackedScene;

        var destruPotents = Map.GetUsedCellsByItem((int)MetaCell.RANDOM_DESTRUCTIBLE);
        var destruPermas = Map.GetUsedCellsByItem((int)MetaCell.PERMA_DESTRUCTIBLE);
        var destrus = destruPermas.Concat(
            GetPercentageOfList(destruPotents, DestructibleFillPercentage)
        );
        foreach (Vector3 loc in destrus)
        {
            var d = destruScene.Instantiate() as Node3D;
            d.AddToGroup("Destru");
            d.Position = loc.SnappedToGrid();
            AddChild(d);
        }

        var obstaPotents = Map.GetUsedCellsByItem((int)MetaCell.RANDOM_OBSTACLE);
        var obstas = GetPercentageOfList(obstaPotents, ObstacleFillPercentage);
        foreach (Vector3 loc in obstas)
        {
            var o = obstaScene.Instantiate() as Node3D;
            o.AddToGroup("Obsta");
            o.Position = loc.SnappedToGrid();
            AddChild(o);
        }
    }

    void SetSpawns()
    {
        var spawns = Map.GetUsedCellsByItem((int)MetaCell.PLAYER_SPAWN);
        var randomSpawns = GetPercentageOfList(
            spawns,
            (MatchSettingsState.PlayerCount + 0.5f) / spawns.Count
        );
        for (int i = 0; i < MatchSettingsState.PlayerCount; i++)
        {
            BNPlayer player = MatchSettingsState.GetPlayer(i);
            Vector3 spawn = randomSpawns.ElementAt(i).SnappedToGrid();

            // todo(jam): set visuals here based on player.Color
            PackedScene characterScene =
                ResourceLoader.Load("res://scenes/objects/characters/TestCharacter.obj.tscn")
                as PackedScene;
            Character character = characterScene.Instantiate() as Character;
            character.AddToGroup(GROUP_CHARACTERS);
            character.AddToGroup(GROUP_GAMEPLAY);
            character.SetPlayerNumber(i);
            AddChild(character);
            character.GlobalPosition = spawn;

            Players.Add(player);
        }
    }

    BNPlayer PlayerAlive()
    {
        return MatchSettingsState.GetPlayer(
            Range(MatchSettingsState.PlayerCount)
                .First(i => !MatchSettingsState.GetPlayer(i).IsDead)
        );
    }

    int NumPlayersAlive()
    {
        return MatchSettingsState.PlayerCount
            - Range(MatchSettingsState.PlayerCount)
                .Count(i => MatchSettingsState.GetPlayer(i).IsDead);
    }

    bool ShouldEndGame()
    {
        return NumPlayersAlive() <= 1;
    }

    async void BeginEndSequence()
    {
        if (HasEndSequenceStarted)
            return;

        Print("Starting game over sequence.");
        HasEndSequenceStarted = true;

        await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

        EndSequence();
    }

    void EndSequence()
    {
        MatchSettingsState.LockInput = true;
        string winnerString = "Tie!";

        if (NumPlayersAlive() == 1)
        {
            winnerString = PlayerAlive().Color.ToColorString() + " Alchemist Wins!";
        }

        Print(winnerString);
    }
}
