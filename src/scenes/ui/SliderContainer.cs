public partial class SliderContainer : HBoxContainer
{
  #region Child nodes

  Label Title, Count;
  public HSlider Slider { get; private set; }

  #endregion

  int LastValue;
  public int Value;

  public override void _EnterTree ()
  {
    Title = GetNode("Title") as Label;
    Count = GetNode("Count") as Label;
    Slider = GetNode("Slider") as HSlider;

    Slider.ValueChanged += OnValueChanged;

    LastValue = Value = (int)Slider.Value;
  }

  public override void _Process (double dt)
  {
    if (LastValue != Value)
    {
      Slider.Value = Value;
      Count.Text = Value.ToString();
      LastValue = Value;
    }
  }

  void OnValueChanged (double newValue)
  {
    LastValue = Value = (int)newValue;
    Count.Text = Value.ToString();
  }
}
