using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using SpinnerNav.Support;

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

        #region [Props]
        CancellationTokenSource _cts = new CancellationTokenSource();

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

        #region [Animation Properties]
        private bool _dimmableOverlayVisible = false;
        public bool DimmableOverlayVisible
        {
            get => _dimmableOverlayVisible;
            set
            {
                _dimmableOverlayVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _performSlide = true;
        public bool PerformSlide
        {
            get => _performSlide;
            set
            {
                _performSlide = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        public Page1()
        {
            InitializeComponent();
            this.KeepAlive = true; //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = this;
        }

        /// <summary>
        /// Test for page caching.
        /// </summary>
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    if (e.NavigationMode == NavigationMode.Back)
        //    {
        //        // Load previous state.
        //    }
        //}

        async void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Spin4Visible)
                {
                    DimmableOverlayVisible = false;
                    // User wants to cancel the work.
                    _cts.Cancel();
                    return;
                }

                PerformSlide = false;
                MainWindow.GlobalEB.Publish("EB_Popup", "Working...");
                Spin4Visible = DimmableOverlayVisible = true;

                // Create new CTS in the event that user has canceled previous one.
                _cts = new CancellationTokenSource();

                // Call work method and wait.
                _ = await Task.Run(() => PerformSomeWork(2000, _cts.Token));
                MainWindow.GlobalEB.Publish("EB_Popup", "Almost done...");

                var done = await Task.Run(() => PerformSomeWork(2000, _cts.Token));
                MainWindow.GlobalEB.Publish("EB_Popup", done);

                // If not using INotify then we could call our home-brew UI refresh. (not recommended)
                //Extensions.DoEvents(true);
            }
            catch (Exception ex) 
            {
                MainWindow.GlobalEB.Publish("EB_Popup", ex);
            }
            finally
            {
                Spin4Visible = DimmableOverlayVisible = false;
                PerformSlide = true;
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
            try
            {
                int count = 0;
                while (++count < (pause / 10))
                {
                    if (token.IsCancellationRequested)
                        break;

                    //new System.Threading.ManualResetEvent(false).WaitOne(10); // An over-engineered Thread.Sleep()
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                MainWindow.GlobalEB.Publish("EB_Popup", ex);
            }

            if (token.IsCancellationRequested)
                return $"Canceled work at {DateTime.Now.ToLongTimeString()}";
            else
                return $"Finished work at {DateTime.Now.ToLongTimeString()}";
        }
    }
}
