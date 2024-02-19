using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace SpinnerNav
{
    /*
        <ScrollViewer x:Name="scroll" HorizontalAlignment="Stretch"
            local:AutoScrollToBottomProperty.Value="True">
            <CONTENT_HERE>
        </ScrollViewer>
    */

    /// <summary>
    /// Scroll an items control to the bottom when the data context changes.
    /// </summary>
    public class ScrollToBottomOnLoadProperty : BaseAttachedProperty<ScrollToBottomOnLoadProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Don't hook and fire events if designer is active
            if (DesignerProperties.GetIsInDesignMode(sender))
                return;

            // We only want ScrollViewers
            if (!(sender is ScrollViewer control))
                return;

            // Un-hook any previous DataContext change event
            control.DataContextChanged -= Control_DataContextChanged;
            // Hook DataContext change event
            control.DataContextChanged += Control_DataContextChanged;
        }

        private void Control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Force a scroll to bottom on the control
            (sender as ScrollViewer)?.ScrollToBottom();
        }
    }

    /// <summary>
    /// Automatically keep keep the scroll at the bottom of the screen.
    /// </summary>
    public class AutoScrollToBottomProperty : BaseAttachedProperty<AutoScrollToBottomProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Don't hook and fire events if designer is active
            if (DesignerProperties.GetIsInDesignMode(sender))
                return;

            // We only want ScrollViewers
            if (!(sender is ScrollViewer control))
                return;

            // Un-hook any previous DataContext change event
            control.ScrollChanged -= Control_ScrollChanged;
            // Hook DataContext change event
            control.ScrollChanged += Control_ScrollChanged;
        }

        void Control_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scroll = sender as ScrollViewer;
            if (scroll != null)
            {
                // If the difference between where we are and the very bottom is less than 20
                if ((scroll.ScrollableHeight - scroll.VerticalOffset) < 20)
                {
                    scroll.ScrollToEnd();
                }
            }
        }
    }
}
