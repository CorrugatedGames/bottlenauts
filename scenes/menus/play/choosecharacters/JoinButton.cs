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
    int position = (int)GetMeta("Position");

    // var availableColors = settings.AllColors.Except(settings.AlchemistColorsNotEmpty).ToArray();
    // var newColor = availableColors.First();

    // settings.ChangePlayerColor(position, newColor);

    GetNode($"%Player{position}").GetNode<CanvasItem>("ChoiceJoin").Hide();
    GetNode($"%Player{position}").GetNode<CanvasItem>("ChoiceVisible").Show();
    // GetNode<RichTextLabel>($"%CharName{position}").Text = newColor.AsAlchemist().Centered();
  }
}
