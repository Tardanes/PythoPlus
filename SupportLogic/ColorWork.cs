using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class ColorWork
    {
        public ColorWork()
        {
        }
        public Color AdjustSaturation(string hexColor, float factor)
        {
            var color = Color.FromArgb(hexColor);
            var hsl = RgbToHsl(color);
            hsl.S = hsl.S * factor;
            var adjustedColor = HslToRgb(hsl);
            string result = adjustedColor.ToArgbHex();
            return Color.FromArgb(result);
        }

        public (double H, double S, double L) RgbToHsl(Color color)
        {
            double r = color.Red, g = color.Green, b = color.Blue;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double h, s, l = (max + min) / 2.0;

            if (max == min)
            {
                h = s = 0; // achromatic
            }
            else
            {
                double d = max - min;
                s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);

                if (max == r)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else
                {
                    h = (r - g) / d + 4;
                }
                h /= 6;
            }

            return (h, s, l);
        }

        public Color HslToRgb((double H, double S, double L) hsl)
        {
            double r, g, b;
            double h = hsl.H, s = hsl.S, l = hsl.L;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                Func<double, double, double, double> hue2rgb = (p, q, t) =>
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1 / 6.0) return p + (q - p) * 6 * t;
                    if (t < 1 / 3.0) return q;
                    if (t < 1 / 2.0) return p + (q - p) * (2 / 3.0 - t) * 6;
                    return p;
                };

                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;
                r = hue2rgb(p, q, h + 1 / 3.0);
                g = hue2rgb(p, q, h);
                b = hue2rgb(p, q, h - 1 / 3.0);
            }

            return Color.FromRgba((float)r, (float)g, (float)b, 1.0f);
        }
    }
}
