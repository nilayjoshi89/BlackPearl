using System;
using System.Windows;
using System.Windows.Controls;

namespace BlackPearl.Controls.Library.Behavior
{
    //References - http://stackoverflow.com/questions/1114092/listbox-scrollbar-doesnt-follow-selected-item-with-icollectionview
    //           - https://www.codeproject.com/Articles/28959/Introduction-to-Attached-Behaviors-in-WPF
    public static class ListBoxItemBehavior
    {
        #region IsBroughtIntoViewWhenSelected

        public static bool GetIsBroughtIntoViewWhenSelected(ListBoxItem listBoxItem) => (bool)listBoxItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);

        public static void SetIsBroughtIntoViewWhenSelected(
          ListBoxItem listBoxItem, bool value)
        {
            listBoxItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "IsBroughtIntoViewWhenSelected",
            typeof(bool),
            typeof(ListBoxItemBehavior),
            new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        static void OnIsBroughtIntoViewWhenSelectedChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ListBoxItem item = depObj as ListBoxItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.Selected += OnListBoxItemSelected;
            else
                item.Selected -= OnListBoxItemSelected;
        }

        static void OnListBoxItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the ListBoxItem
            // whose IsSelected property was modified.  Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            if (e.OriginalSource is ListBoxItem item)
                item.BringIntoView();
        }

        #endregion // IsBroughtIntoViewWhenSelected
    }
}
