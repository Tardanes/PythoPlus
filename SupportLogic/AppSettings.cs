using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public static class AppSettings
    {
        private const string BackColorKey = "back_color";
        private const string BackColorTitledKey = "back_color_titled";
        private const string ThemeColorKey = "theme_color";
        private const string ButtonColorKey = "button_color";
        private const string ButtonTextColorTitledKey = "button_text_color_titled";
        private const string ButtonTextColorKey = "button_text_color";
        private const string TextColorKey = "text_color";
        private const string FontSizeKey = "font_size";
        private const string FontFamilyKey = "font_family";
        private const string ThemeColorTitledKey = "theme_color_titled";
        private const string ButtonColorTitledKey = "button_color_titled";
        private const string TextColorTitledKey = "text_color_titled";
        private const string ThemeSupColorKey = "theme_sup_color";

        public static Color ThemeColor
        {
            get => Color.FromArgb(Preferences.Get(ThemeColorKey, Colors.Yellow.ToArgbHex()));
            set => Preferences.Set(ThemeColorKey, value.ToArgbHex());
        }

        public static Color ThemeSupColor
        {
            get => Color.FromArgb(Preferences.Get(ThemeSupColorKey, new Color(
                    (float)ClampEx.Clamp(Colors.Yellow.Red * 1.4),
                    (float)ClampEx.Clamp(Colors.Yellow.Green * 1.4),
                    (float)ClampEx.Clamp(Colors.Yellow.Blue * 1.4),
                    Colors.Yellow.Alpha).ToArgbHex()));
            set => Preferences.Set(ThemeSupColorKey, value.ToArgbHex());
        }

        public static Color ButtonColor
        {
            get => Color.FromArgb(Preferences.Get(ButtonColorKey, Colors.Blue.ToArgbHex()));
            set => Preferences.Set(ButtonColorKey, value.ToArgbHex());
        }

        public static Color TextColor
        {
            get => Color.FromArgb(Preferences.Get(TextColorKey, Colors.Black.ToArgbHex()));
            set => Preferences.Set(TextColorKey, value.ToArgbHex());
        }

        public static double FontSize
        {
            get => Preferences.Get(FontSizeKey, 16.0);
            set => Preferences.Set(FontSizeKey, value);
        }

        public static string FontFamily
        {
            get => Preferences.Get(FontFamilyKey, "Arial");
            set => Preferences.Set(FontFamilyKey, value);
        }

        public static string ThemeColorTitled
        {
            get => Preferences.Get(ThemeColorTitledKey, "Сірий");
            set => Preferences.Set(ThemeColorTitledKey, value);
        }

        public static string ButtonColorTitled
        {
            get => Preferences.Get(ButtonColorTitledKey, "Блакитний");
            set => Preferences.Set(ButtonColorTitledKey, value);
        }

        public static string TextColorTitled
        {
            get => Preferences.Get(TextColorTitledKey, "Чорний");
            set => Preferences.Set(TextColorTitledKey, value);
        }

        public static Color BackColor
        {
            get => Color.FromArgb(Preferences.Get(BackColorKey, Colors.White.ToArgbHex()));
            set => Preferences.Set(BackColorKey, value.ToArgbHex());
        }

        public static Color ButtonTextColor
        {
            get => Color.FromArgb(Preferences.Get(ButtonTextColorKey, Colors.White.ToArgbHex()));
            set => Preferences.Set(ButtonTextColorKey, value.ToArgbHex());
        }

        public static string BackColorTitled
        {
            get => Preferences.Get(BackColorTitledKey, "Білий");
            set => Preferences.Set(BackColorTitledKey, value);
        }

        public static string ButtonTextColorTitled
        {
            get => Preferences.Get(ButtonTextColorTitledKey, "Білий");
            set => Preferences.Set(ButtonTextColorTitledKey, value);
        }
    }

    public static class ClampEx
    {
        public static double Clamp(double value)
        {
            return Math.Min(1.0, Math.Max(0, value));
        }
    }
}
