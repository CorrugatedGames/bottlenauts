public partial class ActiveCanvasLayer : CanvasLayer
{
  public virtual void SetActive (bool active)
  {
    Visible = active;
    SetProcess(active);
    SetProcessUnhandledInput(active);
  }
}