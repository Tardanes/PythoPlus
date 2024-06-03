using PythoPlus.PopScreens;

namespace PythoPlus
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Navigation.PushModalAsync(new Login());

        }

        public void OnUserLoggedIn()
        {
            // Логика, которая должна выполняться после успешного входа пользователя
            DisplayAlert("Welcome", "You have successfully logged in!", "OK");
        }
    }

}
