public interface IPlayer <T>
{
  bool IsGamepad { get; set; }
  int DeviceIndex { get; set; }
  int PlayerIndex { get; set; }

  T Profile { get; set; }
}