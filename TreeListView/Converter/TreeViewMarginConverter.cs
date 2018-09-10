using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TreeListView.Extensions;

namespace TreeListView.Converter
{
    //Taken from https://github.com/MahApps/MahApps.Metro/blob/2fed9c9d91695903d0548fc4dffb7f1af3a1a917/src/MahApps.Metro/MahApps.Metro.Shared/Converters/TreeViewMarginConverter.cs
    internal class TreeViewMarginConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TreeViewItem item))
                return new Thickness(0);

            return new Thickness(Length * item.GetDepth(), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}