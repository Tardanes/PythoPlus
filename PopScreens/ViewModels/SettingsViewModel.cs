using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using Microsoft.UI.Xaml.Media;

namespace PythoPlus.PopScreens
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ------------ FIELDS ------------
        private Color themeColor;
        private Color buttonColor;
        private Color textColor;
        private double fontSize;
        private string fontFamily;
        private Color backColor;
        private Color buttonTextColor;
        private Color themeSupColor;
        // ------------ TITLES ------------
        public string themeColorTitled = AppSettings.ThemeColorTitled;
        public string buttonColorTitled = AppSettings.ButtonColorTitled;
        public string textColorTitled = AppSettings.TextColorTitled;
        public string backColorTitled = AppSettings.BackColorTitled;
        public string buttonTextColorTitled = AppSettings.ButtonTextColorTitled;
        // ------------ MASSIVES -----------
        public ObservableCollection<Color> AvailableColors { get; }
        public ObservableCollection<String> TitleColors { get; }
        Dictionary<string, Color> colorTitleMap;
        // ------------ COMMANDS ------------
        public ICommand SaveCommand { get; }
        public ICommand RestoreCommand { get; }

        public SettingsViewModel()
        {
            themeColor = AppSettings.ThemeColor;
            buttonColor = AppSettings.ButtonColor;
            textColor = AppSettings.TextColor;
            fontSize = AppSettings.FontSize;
            fontFamily = AppSettings.FontFamily;
            themeColorTitled = AppSettings.ThemeColorTitled;
            buttonColorTitled = AppSettings.ButtonColorTitled;
            textColorTitled = AppSettings.TextColorTitled;
            backColor = AppSettings.BackColor;
            buttonTextColor = AppSettings.ButtonTextColor;
            backColorTitled = AppSettings.BackColorTitled;
            buttonTextColorTitled = AppSettings.ButtonTextColorTitled;
            themeSupColor = AppSettings.ThemeSupColor;

            SaveCommand = new Command(SaveSettings);
            RestoreCommand = new Command(ResetSettings);


        }
        private void SaveSettings()
        {

            //AppSettings.TextColor = TextColor;
            //AppSettings.ThemeColor = ThemeColor;
            //AppSettings.ButtonColor = ButtonColor;
            //AppSettings.FontSize = FontSize;
            //AppSettings.FontFamily = FontFamily;
            //AppSettings.ThemeColorTitled = ThemeColorTitled;
            //AppSettings.ButtonColorTitled = ButtonColorTitled;
            //AppSettings.TextColorTitled = TextColorTitled;
            //AppSettings.ButtonTextColorTitled = ButtonTextColorTitled;
            //AppSettings.BackColorTitled = BackColorTitled;
            //AppSettings.ButtonTextColor = ButtonTextColor;
            //AppSettings.BackColor = BackColor;
            //AppSettings.ThemeSupColor = ThemeSupColor;
        }

        private void ResetSettings()
        {
            Preferences.Default.Clear();
        }
        private void UpdateApplicationResource(string key, object value)
        {
            if (Application.Current.Resources.ContainsKey(key))
            {
                Application.Current.Resources[key] = value;
            }
            else
            {
                Application.Current.Resources.Add(key, value);
            }
            SaveSettings();
        }


    }
}
