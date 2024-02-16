using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpinnerNav.Controls
{
    public class NavigationItem : ListBoxItem
    {
        static NavigationItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationItem), new FrameworkPropertyMetadata(typeof(NavigationItem)));
        }

        public Uri NavLink
        {
            get { return (Uri)GetValue(NavLinkProperty); }
            set { SetValue(NavLinkProperty, value); }
        }
        public static readonly DependencyProperty NavLinkProperty =  DependencyProperty.Register(
            nameof(NavLink), 
            typeof(Uri), 
            typeof(NavigationItem), 
            new PropertyMetadata(null));

        /// <summary>
        /// This will be path data.
        /// </summary>
        public Geometry NavIcon
        {
            get { return (Geometry)GetValue(NavIconProperty); }
            set { SetValue(NavIconProperty, value); }
        }
        public static readonly DependencyProperty NavIconProperty = DependencyProperty.Register(
            nameof(NavIcon), 
            typeof(Geometry), 
            typeof(NavigationItem), 
            new PropertyMetadata(null));
    }
}
