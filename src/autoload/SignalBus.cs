public partial class SignalBus : Node
{
  #region Game client signals

  [Signal]
  public delegate void GameExitEventHandler ();

  #endregion

  public override void _EnterTree()
  {
    base._EnterTree();

    GameExit += () => GetTree().Quit();
  }
}