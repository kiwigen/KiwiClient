using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KiwiClient.Converter
{
    /// <summary>
    /// Kovertiert Bool Value in Visibility um. True =  visible, false = Hidden
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }
}
