public partial class SingletonNode : Node
{
  static Node SelfNode;

  new public static void EmitSignal (StringName signal_name, params Variant [] args) => SelfNode.EmitSignal(signal_name, args);

  public override void _EnterTree ()
  {
    base._EnterTree();
    SelfNode = this;
  }
}