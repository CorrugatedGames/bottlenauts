public partial class Level : Node
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    GenerateLevel();
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  private void GenerateLevel()
  {
    HideSpawns();
    GenerateObstacles();
    GenerateDestructibles();
  }

  private int ValidPercentage(int percentage)
  {
    return Math.Clamp(percentage, 0, 100);
  }

  private IEnumerable<T> PercentageOfList<[MustBeVariant] T>(Godot.Collections.Array<T> list, int percentage)
  {
    var listCopy = list.Duplicate();
    listCopy.Shuffle();

    int takeItems = percentage * listCopy.Count / 100;
    return listCopy.Slice(0, takeItems);
  }

  private void HideSpawns()
  {
    var spawns = GetNode<Node3D>("%Spawn");
    var allChildren = spawns.GetChildren();

    foreach (var item in allChildren)
    {
      item.QueueFree();
    }
  }

  private void GenerateObstacles()
  {
    int obstacleFillPercent = ValidPercentage((int)GetMeta("ObstacleFillPercent"));

    var obstacles = GetNode<Node3D>("%RandomObstacle");
    var allChildren = obstacles.GetChildren();

    var hideChildren = PercentageOfList(allChildren, obstacleFillPercent);

    foreach (var item in hideChildren)
    {
      item.QueueFree();
    }
  }

  private void GenerateDestructibles()
  {
    int destructibleFillPercent = (int)GetMeta("DestructibleFillPercent");

    var destructibles = GetNode<Node3D>("%RandomDestructible");
    var allChildren = destructibles.GetChildren();

    var hideChildren = PercentageOfList(allChildren, destructibleFillPercent);

    foreach (var item in hideChildren)
    {
      item.QueueFree();
    }
  }
}
