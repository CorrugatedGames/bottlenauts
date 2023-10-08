#if TOOLS

public partial class MapEditorViewport : SubViewport
{
  bool Dragging = false;

  public override void _EnterTree()
  {
    HandleInputLocally = true;
  }
}

#endif