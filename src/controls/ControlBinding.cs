internal static class ControlType
{
  public static string Prefix (this Type type)
  {
    switch (true)
    {
      case true when type == typeof(InputEventKey): return "kb";
      case true when type == typeof(InputEventJoypadMotion): return "gpa";
      case true when type == typeof(InputEventJoypadButton): return "gpb";

      default: return "";
    }
  }
}

public struct ControlBinding
{
  public Type ControlType;
  public int ControlIndex;
  public int ControlDirection;

  public string AsText () => $"{ControlType.Prefix()}:{ControlIndex}{(ControlDirection == 0 ? "" : ControlDirection > 0 ? "+" : "-")}";

  public static ControlBinding FromText (string code)
  {
    string [] split = code.Split(":");

    string prefix = split[0];
    int dir = split[1].Contains("+") ? 1 : split[1].Contains("-") ? -1 : 0;
    int idx = int.Parse(split[1].Replace("+", "").Replace("-", ""));

    ControlBinding binding = new ControlBinding { ControlIndex = idx, ControlDirection = dir };

    binding.ControlType =
      prefix == "kb" ? typeof(InputEventKey) :
      prefix == "gpa" ? typeof(InputEventJoypadMotion) :
      prefix == "gpb" ? typeof(InputEventJoypadButton) : null;

    return binding;
  }

  #nullable enable
  public InputEvent? AsInputEvent ()
  {
    if (ControlType == null)
      return null;

    switch (true)
    {
      case true when ControlType == typeof(InputEventKey):
        return new InputEventKey { PhysicalKeycode = (Key)ControlIndex };
      case true when ControlType == typeof(InputEventJoypadMotion):
        return new InputEventJoypadMotion { Axis = (JoyAxis)ControlIndex, AxisValue = (float)ControlDirection };
      case true when ControlType == typeof(InputEventJoypadButton):
        return new InputEventJoypadButton { ButtonIndex = (JoyButton)ControlIndex };

      default: return null;
    }
  }

  public static ControlBinding FromInputEvent (InputEvent e)
  {
    ControlBinding binding = new ControlBinding { ControlType = e.GetType() };

    switch (true)
    {
      case true when e.GetType() == typeof(InputEventKey):
      {
        binding.ControlIndex = (int)((int?)(e as InputEventKey)?.PhysicalKeycode ?? -1);
      } break;

      case true when e.GetType() == typeof(InputEventJoypadMotion):
      {
        binding.ControlIndex = (int)((int?)(e as InputEventJoypadMotion)?.Axis ?? -1);
        binding.ControlDirection = (int)((int?)(e as InputEventJoypadMotion)?.AxisValue ?? 0);
      } break;

      case true when e.GetType() == typeof(InputEventJoypadButton):
      {
        binding.ControlIndex = (int)((int?)(e as InputEventJoypadButton)?.ButtonIndex ?? -1);
      } break;
    }

    return binding;
  }
}