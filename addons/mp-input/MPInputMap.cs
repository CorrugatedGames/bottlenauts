public partial class MPInputMap : Node
{
  public override void _EnterTree ()
  {
    Logger.Info("MPInputMap initialized");
  }

  static StringName PLAYER_ACTION (int playerIndex, StringName action) => $"P{playerIndex}__{action}";

  public static void ActionAddEvent (int playerIndex, StringName action, InputEvent evt) =>
    InputMap.ActionAddEvent(PLAYER_ACTION(playerIndex, action), evt);

  public static void ActionEraseEvent (int playerIndex, StringName action, InputEvent evt) =>
    InputMap.ActionEraseEvent(PLAYER_ACTION(playerIndex, action), evt);

  public static void ActionEraseEvents (int playerIndex, StringName action) =>
    InputMap.ActionEraseEvents(PLAYER_ACTION(playerIndex, action));

  public static float ActionGetDeadzone (int playerIndex, StringName action) =>
    InputMap.ActionGetDeadzone(PLAYER_ACTION(playerIndex, action));

  public static void ActionSetDeadzone (int playerIndex, StringName action, float deadzone) =>
    InputMap.ActionSetDeadzone(PLAYER_ACTION(playerIndex, action), deadzone);

  public static void AddAction (int playerIndex, StringName action, float deadzone) =>
    InputMap.AddAction(PLAYER_ACTION(playerIndex, action), deadzone);

  public static void EraseAction (int playerIndex, StringName action) =>
    InputMap.EraseAction(PLAYER_ACTION(playerIndex, action));

  public static bool HasAction (int playerIndex, StringName action) => InputMap.HasAction(PLAYER_ACTION(playerIndex, action));

  public static void AddActionBindings (int playerIndex, StringName action, float deadzone, ref InputEvent [] events)
  {
    AddAction(playerIndex, action, deadzone);
    foreach (var evt in events)
      ActionAddEvent(playerIndex, action, evt);
  }

  public static void EraseActionDeep (int playerIndex, StringName action)
  {
    if (!HasAction(playerIndex, action))
      return;
      
    ActionEraseEvents(playerIndex, action);
    EraseAction(playerIndex, action);
  }
}