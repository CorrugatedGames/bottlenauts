public partial class SettingsLayer : ActiveCanvasLayer
{
  [Export]
  NodePath _ParentLayer;
  ActiveCanvasLayer ParentLayer;

  #region Child nodes

  Button BackButton;

  #endregion

  public override void _Ready ()
  {
    ParentLayer = GetNode(_ParentLayer) as ActiveCanvasLayer;

    BackButton = GetNode("Panel/BackButton") as Button;

    BackButton.Pressed += OnBackButtonPressed;

    SetProcess(false);
    SetProcessUnhandledInput(false);
    
    if (ParentLayer != null)
      SetProcessInput(false);
  }

  public override void _Input (InputEvent evt)
  {
    if (evt.IsActionPressed("pause"))
      SetActive(!Visible, false);
  }

  public override void SetActive (bool active, bool allowCharacterProcess)
  {
    base.SetActive(active, allowCharacterProcess);

    if (!active)
    {
      ParentLayer?.SetProcessInput(true);
      ParentLayer?.SetActive(true, false);
    }

    SetProcessInput(active);
  }

  void OnBackButtonPressed () => SetActive(false, false);
}
