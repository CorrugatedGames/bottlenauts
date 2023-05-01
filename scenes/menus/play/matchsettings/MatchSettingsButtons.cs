
public partial class MatchSettingsButtons : Button
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
		GetTree().ChangeSceneToFile("res://scenes/menus/play/choosecharacters/ChooseCharacters.tscn");
	}

	public void _on_pressed_goforward() {
		GetTree().ChangeSceneToFile("res://scenes/menus/play/playgame/PlayGame.tscn");
	}

	public void _on_pressed_choosevariants() {
		GetTree().ChangeSceneToFile("res://scenes/menus/play/choosevariants/ChooseVariants.tscn");
	}

	public void _on_pressed_choosemap() {
		GetTree().ChangeSceneToFile("res://scenes/menus/play/choosemap/ChooseMap.tscn");
	}
}
