#if TOOLS

[Tool]
public partial class MPInputEntry : EditorPlugin
{
	public override void _EnterTree()
	{
		AddAutoloadSingleton("MPInputMap", "res://addons/mp-input/MPInputMap.cs");
		AddAutoloadSingleton("MPInput", "res://addons/mp-input/MPInput.cs");
	}

	public override void _ExitTree()
	{
		RemoveAutoloadSingleton("MPInput");
		RemoveAutoloadSingleton("MPInputMap");
	}
}
#endif
