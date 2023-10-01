public partial class MPInput : Node
{
  public override void _EnterTree ()
  {
    Logger.Info("MPInput initialized");
  }

  static StringName PLAYER_ACTION (int playerIndex, StringName action) => $"P{playerIndex}__{action}";
  
  public static void ActionPress (int playerIndex, StringName action, float strength = 1.0f) =>
    Input.ActionPress(PLAYER_ACTION(playerIndex, action), strength);

  public static void ActionRelease (int playerIndex, StringName action) =>
    Input.ActionRelease(PLAYER_ACTION(playerIndex, action));

  public static float GetActionRawStrength (int playerIndex, StringName action, bool exactMatch = false) =>
    Input.GetActionRawStrength(PLAYER_ACTION(playerIndex, action), exactMatch);

  public static float GetActionStrength (int playerIndex, StringName action, bool exactMatch = false) =>
    Input.GetActionStrength(PLAYER_ACTION(playerIndex, action), exactMatch);

  public static float GetAxis (int playerIndex, StringName negativeAction, StringName positiveAction) =>
    Input.GetAxis(PLAYER_ACTION(playerIndex, negativeAction), PLAYER_ACTION(playerIndex, positiveAction));

  public static Vector2 GetVector (int playerIndex, StringName negativeX, StringName positiveX,
                                 StringName negativeY, StringName positiveY, float deadzone = -1.0f) =>
    Input.GetVector(PLAYER_ACTION(playerIndex, negativeX), PLAYER_ACTION(playerIndex, positiveX),
                    PLAYER_ACTION(playerIndex, negativeY), PLAYER_ACTION(playerIndex, positiveY), deadzone);

  public static bool IsActionJustPressed (int playerIndex, StringName action, bool exactMatch = false) =>
    Input.IsActionJustPressed(PLAYER_ACTION(playerIndex, action), exactMatch);

  public static bool IsActionJustReleased (int playerIndex, StringName action, bool exactMatch = false) =>
    Input.IsActionJustReleased(PLAYER_ACTION(playerIndex, action), exactMatch);

  public static bool IsActionPressed (int playerIndex, StringName action, bool exactMatch = false) =>
    Input.IsActionPressed(PLAYER_ACTION(playerIndex, action), exactMatch);

  public static void StartJoyVibration (int deviceIndex, float weakMagnitude, float strongMagnitude, float duration = 0f) =>
    Input.StartJoyVibration(deviceIndex, weakMagnitude, strongMagnitude, duration);

  public static void StopJoyVibration (int deviceIndex) => Input.StopJoyVibration(deviceIndex);
}