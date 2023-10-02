public sealed class BNPlayer : IPlayer<PlayerProfile>
{
  public bool IsGamepad { get; set; }
  public int DeviceIndex { get; set; }
  public int PlayerIndex { get; set; }

  public PlayerProfile Profile { get; set; }
  public CharacterColor Color { get; set; }
}