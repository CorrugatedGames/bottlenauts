#if TOOLS

[Tool]
public partial class MapEditor : Control
{
  public EditorInterface EditorInterface;

  SubViewport Viewport;
  Node3D ViewportCameraTarget;
  Camera3D ViewportCamera;

  ItemList Palette;

  SubViewport PreviousViewport;

  const float MOUSE_ROTATION_SPEED = 0.05f;
  const float MOUSE_STRAFE_SPEED = 0.13f;

  public override void _Ready ()
  {
    Viewport = GetNode("%Viewport") as SubViewport;
    ViewportCameraTarget = GetNode("%ViewportCameraTarget") as Node3D;
    ViewportCamera = GetNode("%ViewportCamera") as Camera3D;

    Palette = GetNode("%Palette") as ItemList;

    LoadPalette();
  }

  void LoadPalette ()
  {
    Palette.Clear();

    MeshLibrary devMeshLib = ResourceLoader.Load("res://assets/dev.meshlib.tres") as MeshLibrary;
    for (int idx = 0; idx < devMeshLib.GetItemList().Length; idx++)
      Palette.AddItem(devMeshLib.GetItemName(idx), devMeshLib.GetItemPreview(idx), true);

    Palette.QueueRedraw();
  }

  public void SetViewport (Node editingScene)
  {
    PreviousViewport = editingScene.GetParent() as SubViewport;
    editingScene.Reparent(Viewport);
  }

  public void ClearViewport ()
  {
    Viewport.GetChild(1).Reparent(PreviousViewport);
    PreviousViewport = null;
  }

  public override void _Process (double dt)
  {
    if (EditorInterface == null)
      return;
  }

  void RotateCameraFromMouseMotion (Vector2 mouseDelta)
  {
    mouseDelta *= MOUSE_ROTATION_SPEED;

    ViewportCameraTarget.Basis = ViewportCameraTarget.Basis.Rotated(Vector3.Left, mouseDelta.Y / 1.5f);
    ViewportCameraTarget.Basis = ViewportCameraTarget.Basis.Rotated(Vector3.Down, mouseDelta.X);

    Vector3 targetRot = ViewportCameraTarget.Rotation;

    ViewportCameraTarget.Rotation = new () {
      X = Mathf.Clamp(targetRot.X, -Mathf.Pi / 2, Mathf.Pi / 2),
      Y = targetRot.Y,
      Z = Mathf.Clamp(targetRot.Z, -Mathf.Pi / 2, Mathf.Pi / 2)
    };

    ViewportCamera.LookAtFromPosition(ViewportCamera.GlobalPosition, ViewportCameraTarget.GlobalPosition);
  }

  void StrafeCameraFromMouseMotion (Vector2 mouseDelta)
  {
    mouseDelta *= MOUSE_STRAFE_SPEED;

    Vector3 offset = new Vector3(-mouseDelta.X, mouseDelta.Y, 0);
    ViewportCameraTarget.Translate(offset);

    ViewportCamera.LookAtFromPosition(ViewportCamera.GlobalPosition, ViewportCameraTarget.GlobalPosition);
  }

  void ZoomCamera (bool zoomIn)
  {
    Vector3 camPosition = ViewportCamera.Position;
    ViewportCamera.Position = new Vector3 {
      X = camPosition.X,
      Y = camPosition.Y,
      Z = zoomIn ? Mathf.Max(Mathf.Abs(camPosition.Z - 0.5f), 0) : camPosition.Z + 0.5f
    };
  }

  bool HandleCameraInput (InputEvent evt)
  {
    if (evt is InputEventMouseMotion mouseEvent && mouseEvent.ButtonMask == MouseButtonMask.Middle)
    {
      Vector2 mouseDelta = mouseEvent.Relative.Normalized();

      if (Input.IsKeyPressed(Key.Shift))
        StrafeCameraFromMouseMotion(mouseDelta);
      else
        RotateCameraFromMouseMotion(mouseDelta);

      return true;
    }
    else
    if (evt is InputEventMouseButton buttonEvent)
    {
      MouseButton buttonIndex = buttonEvent.ButtonIndex;

      if (buttonIndex == MouseButton.WheelUp || buttonIndex == MouseButton.WheelDown)
        ZoomCamera(buttonIndex == MouseButton.WheelUp);

      return true;
    }

    return false;
  }

  public override void _GuiInput (InputEvent evt)
  {
    if (HandleCameraInput(evt))
      GetViewport().SetInputAsHandled();
  }
}

#endif