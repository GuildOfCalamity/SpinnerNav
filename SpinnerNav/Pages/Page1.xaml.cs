using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SpinnerNav.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool spin1Visible = true;
        public bool Spin1Visible
        {
            get => spin1Visible;
            set
            {
                spin1Visible = value;
                OnPropertyChanged();
            }
        }

        private bool spin2Visible = true;
        public bool Spin2Visible
        {
            get => spin2Visible;
            set
            {
                spin2Visible = value;
                OnPropertyChanged();
            }
        }

        private bool spin3Visible = true;
        public bool Spin3Visible
        {
            get => spin3Visible;
            set
            {
                spin3Visible = value;
                OnPropertyChanged();
            }
        }

        public Page1()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //IsSpinVisible ^= true;
        }

        public void Spin1_Click(object sender, RoutedEventArgs e)
        {
            Spin1Visible ^= true;
            Spin2Visible = true;
            Spin3Visible = true;
        }
        public void Spin2_Click(object sender, RoutedEventArgs e)
        {
            Spin2Visible ^= true;
            Spin1Visible = Spin3Visible = true;
        }
        public void Spin3_Click(object sender, RoutedEventArgs e)
        {
            Spin3Visible ^= true;
            Spin1Visible = Spin2Visible = true;
        }
    }
}
