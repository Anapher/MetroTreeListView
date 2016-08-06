using System.Windows;

namespace TreeListViewProject.Extensions
{
    internal static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty SupressBringIntoViewProperty = DependencyProperty.RegisterAttached(
            "SupressBringIntoView", typeof (bool), typeof (FrameworkElementExtensions),
            new PropertyMetadata(default(bool), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = (FrameworkElement) dependencyObject;
            frameworkElement.RequestBringIntoView -= FrameworkElementOnRequestBringIntoView; //prevent memory leak

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
                frameworkElement.RequestBringIntoView += FrameworkElementOnRequestBringIntoView;
        }

        private static void FrameworkElementOnRequestBringIntoView(object sender,
            RequestBringIntoViewEventArgs requestBringIntoViewEventArgs)
        {
            requestBringIntoViewEventArgs.Handled = true;
        }

        public static void SetSupressBringIntoView(DependencyObject element, bool value)
        {
            element.SetValue(SupressBringIntoViewProperty, value);
        }

        public static bool GetSupressBringIntoView(DependencyObject element)
        {
            return (bool) element.GetValue(SupressBringIntoViewProperty);
        }
    }
}