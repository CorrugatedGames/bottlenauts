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
    public static Color ToColor(this CharacterColor c)
    {
        switch (c)
        {
            case CharacterColor.RED:
                return new Color("#B22222");
            case CharacterColor.BLUE:
                return new Color("#0000CD");
            case CharacterColor.GREEN:
                return new Color("#008000");
            case CharacterColor.YELLOW:
                return new Color("#FFD700");
            case CharacterColor.PURPLE:
                return new Color("#800080");
            case CharacterColor.PINK:
                return new Color("#FF69B4");
            case CharacterColor.ORANGE:
                return new Color("#FF8C00");
            case CharacterColor.LIGHTBLUE:
                return new Color("#87CEEB");

            default:
                return new Color("#000000");
        }
    }

    public static string ToColorString(this CharacterColor c)
    {
        switch (c)
        {
            case CharacterColor.RED:
                return "Red";
            case CharacterColor.BLUE:
                return "Blue";
            case CharacterColor.GREEN:
                return "Green";
            case CharacterColor.YELLOW:
                return "Yellow";
            case CharacterColor.PURPLE:
                return "Purple";
            case CharacterColor.PINK:
                return "Pink";
            case CharacterColor.ORANGE:
                return "Orange";
            case CharacterColor.LIGHTBLUE:
                return "Sky";

            default:
                return "Unknown";
        }
    }
}
