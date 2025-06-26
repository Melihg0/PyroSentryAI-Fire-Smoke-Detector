using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PyroSentryAI.Converters
{
    //IValueConverter interface kuralları asagıdakı 2 metodu implement etmemizi zorunlu kılar.
    public class BooleanToVisibilityConverter : IValueConverter
    {

        // Bu metot, ViewModel'den View'e (arayüze) giderken çalışır.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Gelen değer önce boolmu sonra true'mu diye kontrol edilir.
            if (value is bool && (bool)value)
            {
                // Değer 'true' ise, 'Görünür' olarak çevir.
                return Visibility.Visible;
            }
            // Değer 'false' ise veya bool değilse, 'Gizli' olarak çevir.
            return Visibility.Collapsed;
        }


        // Bu metot, View'den ViewModel'e giderken çalışır (TwoWay binding için).
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
