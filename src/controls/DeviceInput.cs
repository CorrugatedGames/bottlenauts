public partial class DeviceInput : RefCounted
{
  public bool IsDeviceConnected { get; private set; } = true;

  public int Device { get; set; } = -1;
  public bool IsKeyboard { get => Device < 0; }
  public bool IsGamepad { get => Device >= 0; }

  public string Guid { get => IsKeyboard ? "Keyboard" : Input.GetJoyGuid(Device); }
  public string Name { get => IsKeyboard ? "Keyboard" : Input.GetJoyName(Device); }

  [Signal]
  public delegate void ConnectionChangedEventHandler (bool connected);

  public DeviceInput (int device)
  {
    Device = device;
    Input.JoyConnectionChanged += (long device, bool connected) => {};
  }

  internal void OnGamepadConnectionChanged (long device, bool connected)
  {
    if (Device == device)
    {
      IsDeviceConnected = connected;
      EmitSignal(SignalName.ConnectionChanged, connected);
    }
  }

  public bool IsKnown () => IsKeyboard ? true : Input.IsJoyKnown(Device);
  
  public float GetVibrationDuration () => IsKeyboard ? 0f : Input.GetJoyVibrationDuration(Device);
  public Vector2 GetVibrationStrength () => IsKeyboard ? Vector2.Zero : Input.GetJoyVibrationStrength(Device);

  public void StartVibration (float weakMagnitude, float strongMagnitude, float duration = 0f)
  {
    if (IsKeyboard)
      return;

    Input.StartJoyVibration(Device, weakMagnitude, strongMagnitude, duration);
  }

  public void StopVibration ()
  {
    if (IsKeyboard)
      return;

    Input.StopJoyVibration(Device);
  }

  public float GetActionRawStrength (string action, bool exactMatch = false) =>
    IsDeviceConnected ? MPInput.GetActionRawStrength(Device, action, exactMatch) : 0f;

  public float GetActionStrength (string action, bool exactMatch = false) =>
    IsDeviceConnected ? MPInput.GetActionStrength(Device, action, exactMatch) : 0f;

  public float GetAxis (string negativeAction, string positiveAction) =>
    IsDeviceConnected ? MPInput.GetAxis(Device, negativeAction, positiveAction) : 0f;

  public Vector2 GetVector (string negativeX,
                            string positiveX,
                            string negativeY,
                            string positiveY,
                            float deadzone = -1.0f) =>
    IsDeviceConnected ? MPInput.GetVector(Device, negativeX, positiveX, negativeY, positiveY, deadzone) : Vector2.Zero;

  public bool IsActionPressed (string action, bool exactMatch = false) =>
    IsDeviceConnected ? MPInput.IsActionPressed(Device, action, exactMatch) : false;

  public bool IsActionJustPressed (string action, bool exactMatch = false) =>
    IsDeviceConnected ? MPInput.IsActionJustPressed(Device, action, exactMatch) : false;

  public bool IsActionJustReleased (string action, bool exactMatch = false) =>
    IsDeviceConnected ? MPInput.IsActionJustReleased(Device, action, exactMatch) : false;
}