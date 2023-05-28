public partial class TextHelper : Node
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    Logger.Info("TextHelper initialized!");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public string AsAlchemist(AlchemistColor color)
  {
    return color.ToString() + " Alchemist";
  }

  public string CenterText(string text)
  {
    return "[center]" + text + "[/center]";
  }
}
