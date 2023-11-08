using System.Linq;

public partial class ActiveCanvasLayer : CanvasLayer
{
  public virtual void SetActive (bool active, bool allowCharacterProcess)
  {
    Visible = active;
    SetProcess(active);
    SetProcessUnhandledInput(active);

    foreach (Character character in GetTree().GetNodesInGroup(GROUP_CHARACTERS).Cast<Character>())
    {
      character.SetProcess(allowCharacterProcess);
      character.SetProcessInput(allowCharacterProcess);
      character.SetPhysicsProcess(allowCharacterProcess);
    }
  }
}