public partial class PositionHelper : SingletonNode
{
    public static Vector3 SnapToGrid(Vector3 currentPosition)
    {
        return currentPosition + new Vector3(0.5f, 0f, 0.5f);
    }
}
