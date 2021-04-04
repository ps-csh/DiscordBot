using DiscordBot.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DiscordBot.Converters
{
    class IntToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
            {
                //throw new Exception("Target type was not Color");
            }

            try
            {
                
                int colorInt = (int)value;
                //R = pv & 255; G = (pv >> 8) & 255; B = (pv >> 16) & 255; A = (pv >> 24) & 255;
                byte r = (byte)(colorInt >> 16 & 255);
                byte g = (byte)((colorInt >> 8) & 255);
                byte b = (byte)((colorInt) & 255);
                Color color = Color.FromRgb(r, g, b);
                Brush brush = new SolidColorBrush(color);
                return brush;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return Brushes.Black;
            }   
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
