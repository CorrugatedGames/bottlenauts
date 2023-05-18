public partial class SingletonNode : Node
{
  static Node SelfNode;

  public static void Connect (string signalName, Action handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T> (string signalName, Action<T> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T1, T2> (string signalName, Action<T1, T2> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  public static void Connect <T1, T2, T3> (string signalName, Action<T1, T2, T3> handler) => SelfNode.Connect(signalName, Callable.From(handler));
  
  new public static void EmitSignal (StringName signal_name, params Variant [] args) => SelfNode.EmitSignal(signal_name, args);

  public override void _EnterTree ()
  {
    base._EnterTree();
    SelfNode = this;
  }
}