global using Godot;
global using static Godot.GD;
global using static Filepaths;

global using System;
global using System.Collections.Generic;

public static class Filepaths
{
  public readonly static string PLAYER_PROFILE_DIRECTORY = "user://player_profiles/";
  public readonly static string PLAYER_PROFILE_EXTENSION = ".bpro";

  public readonly static string LEVEL_DIRECTORY = "res://assets/levels/";
  public readonly static string LEVEL_EXTENSION = ".level.tscn";

  public readonly static string THEME_DIRECTORY = "res://assets/level_themes/";
  public readonly static string THEME_EXTENSION = ".leveltheme.tres";
}