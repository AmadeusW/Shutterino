using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using static System.Diagnostics.Debugger;

namespace AmadeusW.Shutterino.App.Helpers
{
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //Break();
            return value;
        }
    }
}
