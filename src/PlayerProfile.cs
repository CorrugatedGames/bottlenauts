public struct PlayerProfile
{
  public string Name;

  public float LeftStickDeadzone;
  public float RightStickDeadzone;
  public Dictionary<string, List<ControlBinding>> Bindings;
}