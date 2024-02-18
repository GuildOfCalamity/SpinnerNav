using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SpinnerNav.Support;

namespace SpinnerNav.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page, INotifyPropertyChanged
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

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

        private bool spin4Visible = false;
        public bool Spin4Visible
        {
            get => spin4Visible;
            set
            {
                spin4Visible = value;
                OnPropertyChanged();
            }
        }

        public Page1()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        async void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Spin4Visible)
                {
                    // User wants to cancel the work.
                    _cts.Cancel();
                    return;
                }

                tbDescription.Text = "Working...";
                Spin4Visible = true;

                // Create new CTS in the event that user has canceled previous one.
                _cts = new CancellationTokenSource();

                // Call work method and wait.
                _ = await Task.Run(() => PerformSomeWork(2000, _cts.Token));
                tbDescription.Text = "Almost done...";

                // Or, return a value directly to the control.
                tbDescription.Text = await Task.Run(() => PerformSomeWork(2000, _cts.Token));

                // If not using INotify then we could call our home-brew UI refresh. (not recommended)
                //Extensions.DoEvents(true);
            }
            catch (Exception) { }
            finally
            {
                Spin4Visible = false;
            }
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

        /// <summary>
        /// Place-holder method for testing.
        /// </summary>
        /// <param name="msTimeout">time to wait (in milliseconds)</param>
        string PerformSomeWork(int pause = 2000, CancellationToken token = default)
        {
            int count = 0;
            while (++count < (pause / 10))
            {
                if (token.IsCancellationRequested)
                    break;

                //new System.Threading.ManualResetEvent(false).WaitOne(10); // An over-engineered Thread.Sleep()
                Thread.Sleep(10);
            }

            if (token.IsCancellationRequested)
                return $"Canceled work at {DateTime.Now.ToLongTimeString()}";
            else
                return $"Finished work at {DateTime.Now.ToLongTimeString()}";
        }
    }
}
