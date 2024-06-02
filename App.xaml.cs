using PythoPlus.PopScreens;

namespace PythoPlus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LoadAppSettings(); // сначала инициализация, потом всё шо угодно
            MainPage = new AppShell();
        }
        private void LoadAppSettings()
        {
            Application.Current.Resources["ThemeColor"] = AppSettings.ThemeColor;
            Application.Current.Resources["ButtonColor"] = AppSettings.ButtonColor;
            Application.Current.Resources["TextColor"] = AppSettings.TextColor;
            Application.Current.Resources["FontSize"] = AppSettings.FontSize;
            Application.Current.Resources["FontFamily"] = AppSettings.FontFamily;
            Application.Current.Resources["BackColor"] = AppSettings.BackColor;
            Application.Current.Resources["ButtonTextColor"] = AppSettings.ButtonTextColor;
            Application.Current.Resources["ButtonTextColorTitled"] = AppSettings.ButtonTextColorTitled;
            Application.Current.Resources["BackColorTitled"] = AppSettings.BackColorTitled;
            Application.Current.Resources["ThemeColorTitled"] = AppSettings.ThemeColorTitled;
            Application.Current.Resources["ButtonColorTitled"] = AppSettings.ButtonColorTitled;
            Application.Current.Resources["TextColorTitled"] = AppSettings.TextColorTitled;
            Application.Current.Resources["ThemeSupColor"] = AppSettings.ThemeSupColor;

        }

        //public static T GetService<T>() where T : class
        //{
        //    public static T GetService<T>() where T : class
        //    {
        //        return Current.Services.GetService(typeof(T)) as T;
        //    }
        //}

    }
}
