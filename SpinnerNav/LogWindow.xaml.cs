using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SpinnerNav.Support;

namespace SpinnerNav
{
    /// <summary>
    /// A sample of code-behind for FontAwesome changing.
    /// This could also be done with a converter binding.
    /// </summary>
    public partial class LogWindow : Window
    {
        /*
            Checkbox solid       = "\uf0c8"
            Checkbox empty       = "\uf096"
            Checkbox checked     = "\uf046"
            Checkbox minus       = "\uf147"
            Checkbox solid minus = "\uf146"
            Circle unselected    = "\uf1db" or "\uf10c"
            Circle selected      = "\uf192"
            Circle play          = "\uf01d"
            Circle stop          = "\uf28e"
            Circle pause         = "\uf28c"
        */
        public LogWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Title = App.GetCurrentNamespace();
            this.Closing += (s, e) => { MainWindow.GlobalEB.Publish("EB_Popup", "Closing log window"); };
            this.Loaded += (s, e) => 
            {
                btnToggle1.Content = "\uf205";   // on
                btnToggle2.Content = "\uf204";   // off
                btnCheckbox1.Content = "\uf096"; // off
                btnCheckbox2.Content = "\uf046"; // on
                btnCircle1.Content = "\uf28c";   // pause

                if (e.Source is Window wnd)
                    wnd.WindowStyle = WindowStyle.None;
            };
        }

        public LogWindow(MainWindow window) : this()
        {
            this.DataContext = window.DataContext;
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            //btn.FontFamily = new FontFamily(new Uri("pack://application;,,,/Fonts/#FontAwesome"), "FontAwesome");

            if (btn != null && btn.Name.Contains("Toggle") && btn.Content == "\uf204")
                btn.Content = "\uf205"; // on
            else if (btn != null && btn.Name.Contains("Toggle") && btn.Content == "\uf205")
                btn.Content = "\uf204"; // off
            else if (btn != null && btn.Name.Contains("Check") && btn.Content == "\uf096")
                btn.Content = "\uf046"; // on
            else if (btn != null && btn.Name.Contains("Check") && btn.Content == "\uf046")
                btn.Content = "\uf096"; // off
            else if (btn != null && btn.Name.Contains("Circle") && btn.Content == "\uf1db")
                btn.Content = "\uf192"; // on
            else if (btn != null && btn.Name.Contains("Circle") && btn.Content == "\uf192")
                btn.Content = "\uf1db"; // off
            else if (btn != null && btn.Name.Contains("Circle") && btn.Content == "\uf01d")
                btn.Content = "\uf28c"; // pause
            else if (btn != null && btn.Name.Contains("Circle") && btn.Content == "\uf28c")
                btn.Content = "\uf01d"; // play
            else if (btn != null && btn.Name.Contains("Close"))
                this.Close();
        }

        void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
