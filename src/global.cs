global using Godot;
global using static Godot.GD;

global using System;
global using System.Collections.Generic;

global using static Filepaths;
global using static GroupNames;
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

public static class GroupNames
{
  public readonly static string GROUP_CHARACTERS = "Character";
  public readonly static string GROUP_BOMBS = "Bomb";
}

public static class MathHelpers
{
  readonly static Vector3 GRID_OFFSET = new (0.5f, 0, 0.5f);
  public static void SnapToGrid (ref this Vector3 vec) => vec += GRID_OFFSET;
  /// <summary>Does NOT mutate the Vector3 it's called on!</summary>
  public static Vector3 SnappedToGrid (this Vector3 vec) => vec + GRID_OFFSET;
  /// <summary>Does NOT mutate the Vector3I it's called on!</summary>
  public static Vector3 SnappedToGrid (this Vector3I vec) => new Vector3(vec.X, vec.Y, vec.Z) + GRID_OFFSET;
}