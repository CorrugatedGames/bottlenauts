public struct PlayerProfile
{
  public string Name, Path;

  public float Deadzone;
  public Dictionary<string, List<ControlBinding>> Bindings;
}