using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TreeListView.Extensions
{
    internal static class WpfExtensions
    {
        internal static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            var returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual || returnVal is Visual3D)
                {
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                }
                if (tempReturnVal == null)
                {
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                }
                else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }
    }
}