



public partial class GraphicsSettingsContainer : ScrollContainer
{
  #region Child nodes

  OptionButton Display, Resolution;

  #endregion

  public override void _Ready()
  {
    Display = GetNode("%DisplayType") as OptionButton;
    Resolution = GetNode("%DisplaySize") as OptionButton;

    int resolutionIndex = Settings.Graphics.FindResolutionIndex(
      Settings.Graphics.ResolutionWidth,
      Settings.Graphics.ResolutionHeight
    );

    Resolution.Select(resolutionIndex);
    Display.Select((int)Settings.Graphics.Display);
  }

  public void _on_display_size_item_selected(int index)
  {
    Settings.EmitSignal(
      Settings.SignalName.ResolutionChanged,
      Settings.Graphics.Resolutions[index].Width,
      Settings.Graphics.Resolutions[index].Height
    );
  }

  public void _on_display_type_item_selected(int index)
  {
    Settings.EmitSignal(
      Settings.SignalName.DisplayChanged,
      index
    );
  }
}
