namespace PythoPlus.PopScreens;

public partial class Registration : ContentPage
{
	public Registration()
	{
		InitializeComponent();		
	}

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Навигация на предыдущую страницу
        await Navigation.PopAsync();
    }
}