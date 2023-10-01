[GlobalClass]
public partial class LevelTheme : Resource
{
  [Export] public string Name { get; private set; }

  [ExportGroup("Mesh Data")]
  [Export] public MeshLibrary MeshLibrary { get; private set; }
  [ExportSubgroup("Mesh Library Tile Counts")]
  [Export] public int FloorTileCount { get; private set; }
  [Export] public int WallTileCount { get; private set; }
  [Export] public int PillarTileCount { get; private set; }
  [Export] public int RampTileCount { get; private set; }

  [ExportGroup("Environment Data")]
  [Export] public Godot.Environment Environment { get; private set; }

  [ExportGroup("Shader Data")]
  [Export] public ShaderMaterial [] CharacterMaterialOverrides { get; private set; }
}