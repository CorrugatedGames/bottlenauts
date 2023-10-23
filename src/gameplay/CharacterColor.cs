public enum CharacterColor
{
  RED,
  BLUE,
  GREEN,
  YELLOW,
  PURPLE,
  PINK,
  ORANGE,
  LIGHTBLUE,
}

public static class CharacterColorExt
{
  public static Color ToColor (this CharacterColor c)
  {
    switch (c)
    {
      case CharacterColor.RED: return new Color("#B22222");
      case CharacterColor.BLUE: return new Color("#0000CD");
      case CharacterColor.GREEN: return new Color("#008000");
      case CharacterColor.YELLOW: return new Color("#FFFF00");
      case CharacterColor.PURPLE: return new Color("#800080");
      case CharacterColor.PINK: return new Color("#FF69B4");
      case CharacterColor.ORANGE: return new Color ("#FF8C00");
      case CharacterColor.LIGHTBLUE: return new Color("#87CEEB");

      default: return new Color("#000000");
    }
  }
}