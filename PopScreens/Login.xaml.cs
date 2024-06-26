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

        // ���������� ����������
        if (BindingContext is LoginViewModel viewModel)
        {
            await viewModel.UpdateUserStats();
        }
    }

    private async void OnRegLabelTapped(object sender, EventArgs e)
    {
        //await Shell.Current.Navigation.PopAsync();
        await Shell.Current.Navigation.PushAsync(new Registration());
    }

    private void OnForLabelTapped(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new PassRestore());
    }
}
