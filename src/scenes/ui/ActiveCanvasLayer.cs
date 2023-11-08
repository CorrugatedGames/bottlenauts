using System.Linq;

public partial class ActiveCanvasLayer : CanvasLayer
{
  public virtual void SetActive (bool active, bool allowGameplayProcess)
  {
    Visible = active;
    SetProcess(active);
    SetProcessUnhandledInput(active);

    foreach (Node node in GetTree().GetNodesInGroup(GROUP_GAMEPLAY))
    {
      node.SetProcess(allowGameplayProcess);
      node.SetPhysicsProcess(allowGameplayProcess);
      node.SetProcessInput(allowGameplayProcess);
      node.SetProcessUnhandledInput(allowGameplayProcess);

      if (node is Timer timer)
        timer.Paused = !allowGameplayProcess;
    }
  }
}