using System.Linq;

[GlobalClass]
public partial class LevelTheme : Resource
{
  [Export] public string Name { get; private set; }

  [ExportGroup("Mesh Data")]
  [Export] public MeshLibrary MeshLibrary { get; private set; }

  [ExportGroup("Environment Data")]
  [Export] public Godot.Environment Environment { get; private set; }

  [ExportGroup("Shader Data")]
  [Export] public ShaderMaterial [] CharacterMaterialOverrides { get; private set; }

  #region MeshLibrary cell counts

  private string [] CellNames = null;
  int GetCellCount (string prefix)
  {
    if (CellNames == null)
    {
      int [] cellIndices = MeshLibrary.GetItemList();
      CellNames = new string [cellIndices.Length];
      for (int idx = 0; idx < cellIndices.Length; idx++)
        CellNames[idx] = MeshLibrary.GetItemName(cellIndices[idx]);
    }

    return CellNames.Where(name => name.StartsWith(prefix)).Count();
  }

  public int FloorTileCount { get => GetCellCount("floor"); }
  public int WallTileCount { get => GetCellCount("wall"); }
  public int PillarTileCount { get => GetCellCount("pillar"); }
  public int RampTileCount { get => GetCellCount("ramp"); }
  
  #endregion
}