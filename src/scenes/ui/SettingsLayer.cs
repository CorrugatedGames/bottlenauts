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
      SetActive(!Visible);
  }

  public override void SetActive (bool active)
  {
    base.SetActive(active);

    if (!active)
    {
      ParentLayer?.SetProcessInput(true);
      ParentLayer?.SetActive(true);
    }

    SetProcessInput(active);
  }

  void OnBackButtonPressed () => SetActive(false);
}
