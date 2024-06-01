namespace PythoPlus.PopScreens;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
        if (BindingContext is LoginViewModel viewModel)
        {
            viewModel.LoginSuccessful += OnLoginSuccessful;
        }
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
        Shell.SetNavBarIsVisible(Shell.Current.CurrentItem, false);
    }
    private async void OnLoginSuccessful(object sender, EventArgs e)
    {
        // ������� �� MainPage ����� ��������� �����
        await Shell.Current.GoToAsync("//MainPage");

        // ����� ����� ������� ����� � MainPage
        if (Application.Current.MainPage is Shell shell)
        {
            if (shell.CurrentPage is MainPage mainPage)
            {
                mainPage.OnUserLoggedIn();
            }
        }
    }

    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        // ����� �������� �������������� ������ �������� email
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        // ����� �������� �������������� ������ �������� ������
    }
}