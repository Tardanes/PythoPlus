using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

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

            AvailableColors = new ObservableCollection<Color>
            {
                Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Black, Colors.White, Colors.Gray
            };
            TitleColors = new ObservableCollection<string>
            {
                "Червоний",
                "Зелений",
                "Блакитний",
                "Темно-зелений",
                "Чорний",
                "Білий",
                "Сірий"
            };
            colorTitleMap = new Dictionary<string, Color>();

            for (int i = 0; i < AvailableColors.Count; i++)
            {
                colorTitleMap.Add(TitleColors[i], AvailableColors[i]);
            }
        }
        private void SaveSettings()
        {

            AppSettings.TextColor = TextColor;
            AppSettings.ThemeColor = ThemeColor;
            AppSettings.ButtonColor = ButtonColor;
            AppSettings.FontSize = FontSize;
            AppSettings.FontFamily = FontFamily;
            AppSettings.ThemeColorTitled = ThemeColorTitled;
            AppSettings.ButtonColorTitled = ButtonColorTitled;
            AppSettings.TextColorTitled = TextColorTitled;
            AppSettings.ButtonTextColorTitled = ButtonTextColorTitled;
            AppSettings.BackColorTitled = BackColorTitled;
            AppSettings.ButtonTextColor = ButtonTextColor;
            AppSettings.BackColor = BackColor;
            AppSettings.ThemeSupColor = ThemeSupColor;
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

        public string ThemeColorTitled
        {
            get => themeColorTitled;
            set
            {
                themeColorTitled = value;
                ThemeColor = colorTitleMap[value];
                OnPropertyChanged();
                UpdateApplicationResource("ThemeColorTitled", value);
            }
        }

        public string ButtonColorTitled
        {
            get => buttonColorTitled;
            set
            {
                buttonColorTitled = value;
                ButtonColor = colorTitleMap[value];
                OnPropertyChanged();
                UpdateApplicationResource("ButtonColorTitled", value);
            }
        }

        public string TextColorTitled
        {
            get => textColorTitled;
            set
            {
                textColorTitled = value;
                TextColor = colorTitleMap[value];
                OnPropertyChanged();
                UpdateApplicationResource("TextColorTitled", value);
            }
        }

        public string BackColorTitled
        {
            get => backColorTitled;
            set
            {
                backColorTitled = value;
                BackColor = colorTitleMap[value];
                OnPropertyChanged();
                UpdateApplicationResource("BackColorTitled", value);
            }
        }

        public string ButtonTextColorTitled
        {
            get => buttonTextColorTitled;
            set
            {
                buttonTextColorTitled = value;
                ButtonTextColor = colorTitleMap[value];
                OnPropertyChanged();
                UpdateApplicationResource("ButtonTextColorTitled", value);
            }
        }

        public Color ThemeColor
        {
            get => themeColor;
            set
            {
                themeColor = value;
                ThemeSupColor = value;
                OnPropertyChanged();
                UpdateApplicationResource("ThemeColor", value);
            }
        }

        public Color ThemeSupColor
        {
            get => themeSupColor;
            set
            {
                themeSupColor = new Color(
                    (float)ClampEx.Clamp(value.Red * 1.4),
                    (float)ClampEx.Clamp(value.Green * 1.4),
                    (float)ClampEx.Clamp(value.Blue * 1.4),
                    value.Alpha);
                OnPropertyChanged();
                UpdateApplicationResource("ThemeSupColor", new Color(
                    (float)ClampEx.Clamp(value.Red * 1.4),
                    (float)ClampEx.Clamp(value.Green * 1.4),
                    (float)ClampEx.Clamp(value.Blue * 1.4),
                    value.Alpha));
            }
        }
        public Color ButtonColor
        {
            get => buttonColor;
            set
            {
                buttonColor = value;
                OnPropertyChanged();
                UpdateApplicationResource("ButtonColor", value);
            }
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                OnPropertyChanged();
                UpdateApplicationResource("TextColor", value);
            }
        }

        public double FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                OnPropertyChanged();
                UpdateApplicationResource("FontSize", value);
            }
        }

        public string FontFamily
        {
            get => fontFamily;
            set
            {
                fontFamily = value;
                OnPropertyChanged();
                UpdateApplicationResource("FontFamily", value);
            }
        }

        public Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                OnPropertyChanged();
                UpdateApplicationResource("BackColor", value);
            }
        }

        public Color ButtonTextColor
        {
            get => buttonTextColor;
            set
            {
                buttonTextColor = value;
                OnPropertyChanged();
                UpdateApplicationResource("ButtonTextColor", value);
            }
        }
    }
}
