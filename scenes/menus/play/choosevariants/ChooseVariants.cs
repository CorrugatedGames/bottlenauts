public partial class ChooseVariants : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_pressed_goback() {
		GetTree().ChangeSceneToFile("res://scenes/menus/play/matchsettings/MatchSettings.tscn");
	}
}
