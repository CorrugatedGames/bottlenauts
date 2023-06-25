public partial class MPInput : Node
{
  string [] Actions;
  Dictionary <string, string> [] DeviceActions;

  static MPInput _Self;

  public override void _EnterTree ()
  {
    _Self = this;
    base._EnterTree();
    Actions = new string [] {};
  }

  public static void ResetActions ()
  {
    InputMap.LoadFromProjectSettings();
    int idx = 0;
    foreach (string action in InputMap.GetActions())
      _Self.Actions.SetValue(action, idx++);

    foreach (string action in _Self.Actions)
      foreach (InputEvent evt in InputMap.ActionGetEvents(action))
        if (evt is InputEventJoypadButton || evt is InputEventJoypadMotion)
          evt.Device = 9;
  }

  public static void AssignProfileToDevice (long device, PlayerProfile profile)
  {
    _Self.DeviceActions[device] = new Dictionary <string, string> {};

    float deadzone = profile.Deadzone;
    foreach (string action in _Self.Actions)
    {
      string deviceAction = $"device{device}__{action}";
      
      List <ControlBinding> bindings = profile.Bindings[action];
      if (bindings.Count > 0)
      {
        InputMap.AddAction(deviceAction, deadzone);
        _Self.DeviceActions[device][action] = deviceAction;

        foreach (ControlBinding binding in bindings)
        {
          InputEvent evt = binding.AsInputEvent();
          evt.Device = (int)device;

          InputMap.ActionAddEvent(deviceAction, evt);
        }
      }
    }
  }

  public static void DeleteActionsForDevice (int device)
  {
    _Self.DeviceActions[device] = null;
    
    foreach (string action in InputMap.GetActions())
      if (action.Contains($"device{device}"))
        InputMap.EraseAction(action);
  }

  internal static string GetActionName (int device, string action)
  {
    if (device >= 0)
      return _Self.DeviceActions[device][action] ?? "N/A";

    return action;
  }

  public static float GetActionRawStrength (int device, string action, bool exactMatch = false)
  {
    if (device >= 0)
      action = GetActionName(device, action);

    return Input.GetActionRawStrength(action, exactMatch);
  }

  public static float GetActionStrength (int device, string action, bool exactMatch = false)
  {
    if (device >= 0)
      action = GetActionName(device, action);

    return Input.GetActionStrength(action, exactMatch);
  }

  public static float GetAxis (int device, string negativeAction, string positiveAction)
  {
    if (device >= 0)
    {
      negativeAction = GetActionName(device, negativeAction);
      positiveAction = GetActionName(device, positiveAction);
    }

    return Input.GetAxis(negativeAction, positiveAction);
  }

  public static Vector2 GetVector (int device,
                                   string negativeX,
                                   string positiveX,
                                   string negativeY,
                                   string positiveY,
                                   float deadzone = -1.0f)
  {
    if (device >= 0)
    {
      negativeX = GetActionName(device, negativeX);
      positiveX = GetActionName(device, positiveX);
      negativeY = GetActionName(device, negativeY);
      positiveY = GetActionName(device, positiveY);
    }

    return Input.GetVector(negativeX, positiveX, negativeY, positiveY, deadzone);
  }

  public static bool IsActionPressed (int device, string action, bool exactMatch)
  {
    if (device >= 0)
      action = GetActionName(device, action);

    return Input.IsActionPressed(action, exactMatch);
  }

  public static bool IsActionJustPressed (int device, string action, bool exactMatch)
  {
    if (device >= 0)
      action = GetActionName(device, action);

    return Input.IsActionJustPressed(action, exactMatch);
  }

  public static bool IsActionJustReleased (int device, string action, bool exactMatch)
  {
    if (device >= 0)
      action = GetActionName(device, action);

    return Input.IsActionJustReleased(action, exactMatch);
  }
}