using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PyroSentryAI.Converters
{
    public class AlarmToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string alarmText = value as string;
            string iconPath = "pack://application:,,,/PyroSentryAI;component/Assets/Images/question.png"; // Varsayılan ikon

            if (!string.IsNullOrEmpty(alarmText))
            {
                if (alarmText.ToLower().Contains("fire"))
                {
                    iconPath = "pack://application:,,,/PyroSentryAI;component/Assets/Images/fire.png";
                }
                if (alarmText.ToLower().Contains("smoke"))
                {
                    iconPath = "pack://application:,,,/PyroSentryAI;component/Assets/Images/gas-mask.png";
                }
            }
            return new BitmapImage(new Uri(iconPath));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
