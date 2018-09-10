using System.Windows;

namespace TreeListView.Extensions
{
    internal static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty SuppressBringIntoViewProperty = DependencyProperty.RegisterAttached(
            "SuppressBringIntoView", typeof (bool), typeof (FrameworkElementExtensions),
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

        public static void SetSuppressBringIntoView(DependencyObject element, bool value)
        {
            element.SetValue(SuppressBringIntoViewProperty, value);
        }

        public static bool GetSuppressBringIntoView(DependencyObject element)
        {
            return (bool) element.GetValue(SuppressBringIntoViewProperty);
        }
    }
}