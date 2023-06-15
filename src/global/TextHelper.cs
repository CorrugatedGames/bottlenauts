public static class TextHelper
{
  public static string AsAlchemist (this AlchemistColor color) => $"${color.ToString()} Alchemist";
  public static string Centered (this string text) => $"[center]{text}[/center]";
}