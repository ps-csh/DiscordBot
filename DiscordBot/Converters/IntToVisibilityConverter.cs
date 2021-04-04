using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace DiscordBot.Converters
{
    class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                throw new Exception("Target type was not Visibility");
            }

            try
            {
                return ((int)value > 0) ? Visibility.Visible : Visibility.Collapsed; 
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                throw ex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
