using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace SpinnerNav
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Title = App.GetCurrentNamespace();
            this.Closing += (s, e) => { MainWindow.GlobalEB.Publish("EB_Popup", "Closing log window"); };
            this.Loaded += (s, e) => 
            {
                btnToggle.Content = "\uf205"; // on
            };
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            //btn.FontFamily = new FontFamily(new Uri("pack://application;,,,/Fonts/#FontAwesome"), "FontAwesome");

            if (btn != null && btn.Content == "\uf204")
            {
                btnToggle.Content = "\uf205"; // on
                tbToggle.Text = "Option enabled";
            }
            else if (btn != null && btn.Content == "\uf205")
            {
                btnToggle.Content = "\uf204"; // off
                tbToggle.Text = "Option disabled";
            }

        }
    }
}
