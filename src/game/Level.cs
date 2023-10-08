using System.Linq;

public partial class Level : Node3D
{
  [Export] LevelTheme Theme = null;

  public Camera3D Camera { get; private set; }
  public GridMap Map { get; private set; }

  [ExportCategory("Level Fill Parameters")]
  [Export(PropertyHint.Range, "0,1,0.01")] float DestructibleFillPercentage = 0.4f;
  [Export(PropertyHint.Range, "0,1,0.01")] float ObstacleFillPercentage = 0.2f;

  public override void _Ready ()
  {
    Map = GetNode("GridMap") as GridMap;
    Camera = GetNode("Camera") as Camera3D;

    MatchSettingsState.GeneratePlayerBindings();
    GenerateHazards();
    SetLevelTheme(Theme);
  }

  public void SetLevelTheme (LevelTheme theme)
  {
    if (theme == null)
      return;

    Theme = theme;
    SetMeshLibrary(theme);
    Camera.Environment = Theme.Environment;

    // assign material overrides to all child nodes in group Characters
  }

  void OverwriteGridMapCell (Vector3I cell, MeshLibrary meshlib, string cellType, int count)
  {
    int orientation = Map.GetCellItemOrientation(cell);
    if (orientation == -1)
    {
      Logger.Warning($"GridMap cell at { cell.X } , { cell.Y } , { cell.Z } is empty, unable to overwrite");
      return;
    }

    string name = $"{cellType}{(count > 1 ? new Random().Next(count) : 0)}";
    int item = meshlib.FindItemByName(name);
    if (item == -1)
    {
      Logger.Warning($"Mesh library {meshlib.ResourceName} does not contain item with name {name}, unable to overwrite");
      return;
    }

    Map.SetCellItem(cell, item, orientation);
  }

  void SetMeshLibrary (LevelTheme theme)
  {
    MeshLibrary meshlib = theme.MeshLibrary;
    int floorTileCount = theme.FloorTileCount;
    int wallTileCount = theme.WallTileCount;
    int pillarTileCount = theme.PillarTileCount;
    int rampTileCount = theme.RampTileCount;

    var allCells = new Godot.Collections.Array<Vector3I> [(int)MetaCell.CELL_COUNT];
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
        } break;

        case (int)MetaCell.WALL:
        {
          foreach (var cell in cells)
            OverwriteGridMapCell(cell, meshlib, "wall", wallTileCount);
        } break;

        case (int)MetaCell.PILLAR:
        {
          foreach (var cell in cells)
            OverwriteGridMapCell(cell, meshlib, "pillar", pillarTileCount);
        } break;

        case (int)MetaCell.RAMP:
        {
          foreach (var cell in cells)
            OverwriteGridMapCell(cell, meshlib, "ramp", rampTileCount);
        } break;

        default:
        {
          foreach (var cell in cells)
            Map.SetCellItem(cell, -1);
        } break;
      }
    }
  }

  private IEnumerable<T> GetPercentageOfList <[MustBeVariant] T> (Godot.Collections.Array<T> list, float percentage)
  {
    var copy = list.Duplicate();
    copy.Shuffle();

    return copy[..(int)(copy.Count * percentage)];
  }

  private void GenerateHazards ()
  {
    PackedScene destruScene = ResourceLoader.Load("res://scenes/hazards/Destructible.tscn") as PackedScene;
    PackedScene obstaScene = ResourceLoader.Load("res://scenes/hazards/Obstacle.tscn") as PackedScene;

    var destruPotents = Map.GetUsedCellsByItem((int)MetaCell.RANDOM_DESTRUCTIBLE);
    var destruPermas = Map.GetUsedCellsByItem((int)MetaCell.PERMA_DESTRUCTIBLE);
    var destrus = destruPermas.Concat(GetPercentageOfList(destruPotents, DestructibleFillPercentage));
    foreach (Vector3 loc in destrus)
    {
      var d = destruScene.Instantiate() as Node3D;
      d.AddToGroup("Destru");
      d.Position = loc + new Vector3(0.5f, 0f, 0.5f);
      AddChild(d);
    }

    var obstaPotents = Map.GetUsedCellsByItem((int)MetaCell.RANDOM_OBSTACLE);
    var obstaPermas = Map.GetUsedCellsByItem((int)MetaCell.PERMA_OBSTACLE);
    var obstas = obstaPermas.Concat(GetPercentageOfList(obstaPotents, ObstacleFillPercentage));
    foreach (Vector3 loc in obstas)
    {
      var o = obstaScene.Instantiate() as Node3D;
      o.AddToGroup("Obsta");
      o.Position = loc + new Vector3(0.5f, 0f, 0.5f);
      AddChild(o);
    }
  }
}
