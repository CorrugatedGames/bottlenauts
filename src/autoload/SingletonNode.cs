public partial class SingletonNode : Node
{
  protected static Node SelfNode;

  public static void Connect (string signalName, Action handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T> (string signalName, Action<T> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T1, T2> (string signalName, Action<T1, T2> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T1, T2, T3> (string signalName, Action<T1, T2, T3> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  
  public override void _EnterTree ()
  {
    base._EnterTree();
    SelfNode = this;
  }
}