﻿using System.Globalization;

namespace MyToDo.Common.Converters
{
    public class IntToVisibilityConverter //: IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value == null && int.TryParse(value.ToString(), out int result))
        //    {
        //        if (result == 0)
        //            result Visibility;
        //    }
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
