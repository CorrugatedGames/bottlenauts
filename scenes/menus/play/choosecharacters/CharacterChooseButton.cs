public partial class CharacterChooseButton : Button
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void _on_pressed()
  {
    MatchSettingsState settings = GetNode<MatchSettingsState>("/root/MatchSettingsState");

    int position = (int)GetMeta("Position");
    int direction = (int)GetMeta("Direction");

    AlchemistColor[] allColors = settings.AllColors;
    List<AlchemistColor> validColors = new List<AlchemistColor>(allColors);
    List<AlchemistColor> mutableValidColors = new List<AlchemistColor>(allColors);

    AlchemistColor currentColor = settings.AlchemistColors[position];
    int currentIndex = Array.IndexOf(allColors, currentColor);
    mutableValidColors.Remove(currentColor);

    foreach (AlchemistColor color in settings.AlchemistColors)
    {
      if (color == currentColor) continue;

      validColors.Remove(color);
      mutableValidColors.Remove(color);
    }

    List<AlchemistColor> colorsInOrder = new List<AlchemistColor>();
    colorsInOrder.AddRange(mutableValidColors);
    colorsInOrder.AddRange(validColors);
    colorsInOrder.AddRange(mutableValidColors);

    int currentBigIndex = Array.IndexOf(colorsInOrder.ToArray(), currentColor);
    AlchemistColor nextColor = colorsInOrder[currentBigIndex + direction];

    settings.ChangePlayerColor(position, nextColor);

    GetNode<RichTextLabel>("%CharName" + position).Text = nextColor.AsAlchemist().Centered();
  }
}
