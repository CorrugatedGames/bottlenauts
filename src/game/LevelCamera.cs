using System.ComponentModel.DataAnnotations;
using System.Linq;

[GlobalClass]
public partial class LevelCamera : Camera3D
{
  const float MIN_XROTATION = 65f, MAX_XROTATION = 88f, MAX_MAPHEIGHT = 6f;

  float XRotation = 70f;
  [Export] float Margin = 0.4f;
  [Export] NodePath MapPath;
  GridMap Map;

  public override void _Ready ()
  {
    Fov = 90f;
    Near = 0.05f;
    Far = 100f;

    Map = (MapPath != null ? GetNode(MapPath) : GetParent().GetNode("GridMap")) as GridMap;
    CalculateRotation();
    CalculatePosition();
  }

  (Vector3I, Vector3I) GetMapCorners ()
  {
    var cells = Map.GetUsedCells();

    var orderedX = cells.OrderBy(cell => cell.X);
    var orderedY = cells.OrderBy(cell => cell.Y);
    var orderedZ = cells.OrderBy(cell => cell.Z);

    Vector3I cornerA = new ()
    {
      X = orderedX.First().X,
      Y = orderedY.First().Y,
      Z = orderedZ.First().Z,
    };
    Vector3I cornerB = new ()
    {
      X = orderedX.Last().X,
      Y = orderedY.Last().Y,
      Z = orderedZ.Last().Z,
    };

    return (cornerA, cornerB);
  }

  static float CenterOfCoords (float a, float b) => (a + b + 1) * 0.5f;
  static float LevelMagnitude (Vector3I a, Vector3I b)
  {
    float X = Mathf.Abs(a.X) + Mathf.Abs(b.X);
    float Y = Mathf.Abs(a.Y) + Mathf.Abs(b.Y);
    float Z = Mathf.Abs(a.Z) + Mathf.Abs(b.Z);

    return Mathf.Max(Mathf.Max(X, Y), Z);
  }

  void CalculatePosition ()
  {
    var (cornerA, cornerB) = GetMapCorners();

    float magnitude = LevelMagnitude(cornerA, cornerB) * 0.5f;
    float hFov = GetCameraProjection().GetFov(); // godot handles camera fov oddly to allow for multiple aspect ratios

    float cameraDistance = magnitude * (0.8f + Margin) / Mathf.Sin(Mathf.DegToRad(hFov * 0.5f));

    float x = CenterOfCoords(cornerA.X, cornerB.X);
    float y = cameraDistance * Mathf.Sin(Mathf.DegToRad(XRotation));
    float z = cameraDistance * Mathf.Cos(Mathf.DegToRad(XRotation)) + CenterOfCoords(cornerA.Z, cornerB.Z) + 1;
    
    GlobalPosition = new Vector3(x, y, z);
  }

  void CalculateRotation () 
  {
    XRotation = Mathf.Min(MAX_XROTATION, MIN_XROTATION + (GetMapCorners().Item2.Y * (MAX_XROTATION - MIN_XROTATION) / MAX_MAPHEIGHT));
    GlobalRotationDegrees = Vector3.Left * XRotation;
  }
}