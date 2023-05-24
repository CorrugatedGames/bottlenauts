public partial class LeaveButton : Button
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

    settings.ChangePlayerColor(position, AlchemistColor.Empty);

    GetNode("%Player" + position).GetNode<CanvasItem>("ChoiceVisible").Hide();
    GetNode("%Player" + position).GetNode<CanvasItem>("ChoiceJoin").Show();
  }
}
