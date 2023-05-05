using System.Linq;

public partial class JoinButton : Button
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
    int position = (int)GetParent().GetParent().GetMeta("Position");

    var availableColors = settings.AllColors.Except(settings.AlchemistColorsNotEmpty).ToArray();
    var newColor = availableColors.First();

    settings.ChangePlayerColor(position, newColor);

    GetParent<CanvasItem>().Hide();
    GetParent().GetParent().GetNode<CanvasItem>("ChoiceVisible").Show();
    GetParent().GetParent().GetNode<CanvasItem>("ChoiceVisible").GetNode<RichTextLabel>("CharName").Text =
      "[center]" + newColor.ToString() + " Alchemist" + "[/center]";
  }
}
