#if TOOLS

[Tool]
public partial class MapEditorEntry : EditorPlugin
{
	PackedScene MapEditorScene = ResourceLoader.Load<PackedScene>("res://addons/bottlenauts-map-editor/MapEditorPanel.tscn");
	MapEditor MapEditorPanel;

	public override void _EnterTree ()
	{
		MapEditorPanel = MapEditorScene.Instantiate() as MapEditor;
		MapEditorPanel.EditorInterface = GetEditorInterface();
		GetEditorInterface().GetEditorMainScreen().AddChild(MapEditorPanel);

		SetVisibility(GetEditorInterface().GetEditedSceneRoot());
		SceneChanged += (Node sceneRoot) => SetVisibility(sceneRoot);
	}

	public override void _ExitTree ()
	{
		if (MapEditorPanel != null)
		{
			MapEditorPanel.ClearViewport();
			MapEditorPanel.QueueFree();
		}
	}

	public override bool _HasMainScreen () => true;

	public override void _MakeVisible (bool visible)
	{
		if (MapEditorPanel != null)
			MapEditorPanel.Visible = visible;
	}

	void SetVisibility (Node editingScene)  {
		bool visible = editingScene.IsInGroup("Level");

		if (visible)
			MapEditorPanel.SetViewport(editingScene);

		_MakeVisible(visible);
	}

	public override string _GetPluginName () => "Map Editor";

	public override Texture2D _GetPluginIcon () =>
		GetEditorInterface().GetBaseControl().GetThemeIcon("CanvasLayer", "EditorIcons");
}

#endif
