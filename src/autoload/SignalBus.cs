public partial class SignalBus : SingletonNode
{
  #region Game client signals

  [Signal]
  public delegate void GameExitEventHandler ();

  #endregion

  public static SignalBus Instance => SelfNode.GetNode("/root/SignalBus") as SignalBus;

  public override void _EnterTree()
  {
    base._EnterTree();

    GameExit += () => GetTree().Quit();
  }
}