public struct PlayerProfile
{
  public string Name, Path;

  public float LeftStickDeadzone, RightStickDeadzone;
  public Dictionary<string, List<ControlBinding>> Bindings;
}