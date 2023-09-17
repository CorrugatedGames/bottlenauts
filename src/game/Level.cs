public partial class Level : Node
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

    GenerateHazards();
    SetLevelTheme(Theme);
    HideHelperCells();
  }

  public void SetLevelTheme (LevelTheme theme)
  {
    if (theme == null)
      return;

    Theme = theme;
    Map.MeshLibrary = Theme.MeshLibrary;
    Camera.Environment = Theme.Environment;

    // assign material overrides to all child nodes in group Characters
  }

  private void HideHelperCells ()
  {
    Material mat = Load("res://assets/textures/transparent.mat.tres") as Material;
    
    Mesh destru = Map.MeshLibrary.GetItemMesh((int)MetaCell.DESTRUCTIBLE);
    for (int i = 0; i < destru.GetSurfaceCount(); i++)
      destru.SurfaceSetMaterial(i, mat);
    Map.MeshLibrary.SetItemMesh((int)MetaCell.DESTRUCTIBLE, destru);

    Mesh obsta = Map.MeshLibrary.GetItemMesh((int)MetaCell.OBSTACLE);
    for (int i = 0; i < obsta.GetSurfaceCount(); i++)
      obsta.SurfaceSetMaterial(i, mat);
    Map.MeshLibrary.SetItemMesh((int)MetaCell.OBSTACLE, destru);
  }

  private IEnumerable<T> GetPercentageOfList <[MustBeVariant] T> (Godot.Collections.Array<T> list, float percentage)
  {
    var copy = list.Duplicate();
    copy.Shuffle();

    return copy[..(int)(copy.Count * percentage)];
  }

  private void GenerateHazards ()
  {
    var destruPotents = Map.GetUsedCellsByItem((int)MetaCell.DESTRUCTIBLE);
    var destrus = GetPercentageOfList(destruPotents, DestructibleFillPercentage);
    PackedScene destruScene = ResourceLoader.Load("res://scenes/hazards/Destructible.tscn") as PackedScene;
    foreach (Vector3 loc in destrus)
    {
      var d = destruScene.Instantiate() as Node3D;
      d.Position = loc + new Vector3(0.5f, 0f, 0.5f);
      AddChild(d);
    }

    var obstaPotents = Map.GetUsedCellsByItem((int)MetaCell.OBSTACLE);
    var obstas = GetPercentageOfList(obstaPotents, ObstacleFillPercentage);
    PackedScene obstaScene = ResourceLoader.Load("res://scenes/hazards/Obstacle.tscn") as PackedScene;
    foreach (Vector3 loc in obstas)
    {
      var d = obstaScene.Instantiate() as Node3D;
      d.Position = loc + new Vector3(0.5f, 0f, 0.5f);
      AddChild(d);
    }

  }
}
