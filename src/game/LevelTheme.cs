[GlobalClass]
public partial class LevelTheme : Resource
{
  [Export] public string Name { get; private set; }
  [Export] public MeshLibrary MeshLibrary { get; private set; }
  [Export] public Godot.Environment Environment { get; private set; }
  [Export] public ShaderMaterial [] CharacterMaterialOverrides { get; private set; }
}