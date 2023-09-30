public struct PlayerProfile
{
  public string Name, Path;

  public float Deadzone;
  public Dictionary<string, List<ControlBinding>> Bindings;
}

public static class PlayerProfileExt
{
  static ControlBinding DEFAULT_BINDING = (ControlBinding)Activator.CreateInstance(typeof(ControlBinding));

  public static bool ContainsBinding (this Dictionary<string, List<ControlBinding>> bindings, ControlBinding binding)
  {
    foreach (string name in bindings.Keys)
      if (bindings[name].Find(b => b.AsText() == binding.AsText()).AsText() != DEFAULT_BINDING.AsText())
        return true;

    return false;
  }

  public static string GetActionWithBinding (this Dictionary<string, List<ControlBinding>> bindings, ControlBinding binding)
  {
    foreach (string name in bindings.Keys)
      if (bindings[name].Find(b => b.AsText() == binding.AsText()).AsText() != DEFAULT_BINDING.AsText())
        return name;

    return "";
  }

  public static Dictionary<string, List<InputEvent>> AsInputEvents (this Dictionary<string, List<ControlBinding>> bindings)
  {
    Dictionary<string, List<InputEvent>> events = new ();

    foreach (string name in bindings.Keys)
    {
      List<ControlBinding> bindingsList = bindings[name];
      List<InputEvent> eventsList = new ();

      foreach (ControlBinding bind in bindingsList)
        eventsList.Add(bind.AsInputEvent());
        
      events.Add(name, eventsList);
    }

    return events;
  }
}