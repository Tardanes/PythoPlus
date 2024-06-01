namespace PythoPlus
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();
            RegisterRoutes();
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(PopScreens.Login), typeof(PopScreens.Login));
            Routing.RegisterRoute(nameof(PopScreens.Registration), typeof(PopScreens.Registration));
            Routing.RegisterRoute(nameof(PopScreens.SettingsUsr), typeof(PopScreens.SettingsUsr));
        }
    }
}
