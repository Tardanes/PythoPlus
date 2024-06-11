namespace PythoPlus.PopScreens;

public partial class SettingsUsr : ContentPage
{
	public SettingsUsr()
	{
		InitializeComponent();
	}
    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        var newStep = Math.Round(e.NewValue);
        ((Slider)sender).Value = newStep;
    }
}