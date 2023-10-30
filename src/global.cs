global using Godot;
global using static Godot.GD;

global using System;
global using System.Collections.Generic;

global using static Filepaths;
global using static MathHelpers;

public static class Filepaths
{
  public readonly static string PLAYER_PROFILE_DIRECTORY = "user://player_profiles/";
  public readonly static string PLAYER_PROFILE_EXTENSION = ".bpro";
  public static string AsProfileFilePath (this string name) =>
    $"{PLAYER_PROFILE_DIRECTORY}{name}{PLAYER_PROFILE_EXTENSION}";

  public readonly static string LEVEL_DIRECTORY = "res://assets/levels/";
  public readonly static string LEVEL_EXTENSION = ".level.tscn";
  public static string AsLevelFilePath (this string name) => $"{LEVEL_DIRECTORY}{name}{LEVEL_EXTENSION}";

  public readonly static string THEME_DIRECTORY = "res://assets/level_themes/";
  public readonly static string THEME_EXTENSION = ".leveltheme.tres";
  public static string AsThemeFilePath (this string name) => $"{THEME_DIRECTORY}{name}{THEME_EXTENSION}";
}

public static class MathHelpers
{
  public static Vector3 SnapToGrid (Vector3 currentPosition) =>
    currentPosition + new Vector3(0.5f, 0f, 0.5f);
}