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
            Routing.RegisterRoute(nameof(PopScreens.MatCatalog), typeof(PopScreens.MatCatalog));
            Routing.RegisterRoute(nameof(PopScreens.MatView), typeof(PopScreens.MatView));
            Routing.RegisterRoute(nameof(PopScreens.Achievments), typeof(PopScreens.Achievments));
            Routing.RegisterRoute(nameof(PopScreens.StatisticsUsr), typeof(PopScreens.StatisticsUsr));
            Routing.RegisterRoute(nameof(PopScreens.PassRestore), typeof(PopScreens.PassRestore));
        }
    }
}
