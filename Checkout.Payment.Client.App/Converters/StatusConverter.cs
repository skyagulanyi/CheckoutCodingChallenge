using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Checkout.Payment.Client.App.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) 
            {
                if ((bool)value) 
                {
                    return "Processed";
                }
                return "Not Processed";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
