using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using SpinnerNav.Controls;
using SpinnerNav.Support;

namespace SpinnerNav
{
    /// <summary>
    /// Driver for the spinner control demo.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        IntPtr _winHandle = IntPtr.Zero;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private Geometry svgData = Geometry.Parse("M3 12V6.75l6-1.32v6.48zm17-9v8.75l-10 .15V5.21zM3 13l6 .09v6.81l-6-1.15zm17 .25V22l-10-1.91V13.1z");
        public Geometry SvgData
        {
            get => svgData;
            set
            {
                if (svgData != value)
                {
                    svgData = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Title = App.GetCurrentNamespace();
        }

        /// <summary>
        /// <see cref="ListBox"/> event.
        /// </summary>
        void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavigationItem;
            if (selected != null)
            {
                frmNav.Navigate(selected.NavLink);
                #region [Superfluous]
                switch (sidebar.SelectedIndex)
                {
                    case 0:
                        SvgData = Geometry.Parse("M3 12V6.75l6-1.32v6.48zm17-9v8.75l-10 .15V5.21zM3 13l6 .09v6.81l-6-1.15zm17 .25V22l-10-1.91V13.1z");
                        break;
                    case 1:
                        SvgData = Geometry.Parse("M12.5 12.5H20V20h-7.5zm0-1V4H20v7.5zm-1 0H4V4h7.5zm0 1V20H4v-7.5z");
                        break;
                    case 2:
                        SvgData = Geometry.Parse("M11 11V3H5c-1.1 0-2 .9-2 2v6zm2 0h8V5c0-1.1-.9-2-2-2h-6zm-2 2H3v6c0 1.1.9 2 2 2h6zm2 0v8h6c1.1 0 2-.9 2-2v-6z");
                        break;
                    case 3:
                        SvgData = Geometry.Parse("M13 13h8v8h-8zm0-2V3h8v8zm-2 0H3V3h8zm0 2v8H3v-8z");
                        break;
                    case 4:
                        SvgData = Geometry.Parse("M216 136h-80a8 8 0 0 0-8 8v57.45a8 8 0 0 0 6.57 7.88l80 14.54a7.61 7.61 0 0 0 1.43.13a8 8 0 0 0 8-8v-72a8 8 0 0 0-8-8m-8 70.41l-64-11.63V152h64ZM104 136H40a8 8 0 0 0-8 8v40a8 8 0 0 0 6.57 7.87l64 11.64a8.54 8.54 0 0 0 1.43.13a8 8 0 0 0 8-8V144a8 8 0 0 0-8-8m-8 50.05l-48-8.73V152h48ZM221.13 33.86a8 8 0 0 0-6.56-1.73l-80 14.55a8 8 0 0 0-6.57 7.87V112a8 8 0 0 0 8 8h80a8 8 0 0 0 8-8V40a8 8 0 0 0-2.87-6.14M208 104h-64V61.22l64-11.63Zm-98.87-49.78a8 8 0 0 0-6.56-1.73l-64 11.64A8 8 0 0 0 32 72v40a8 8 0 0 0 8 8h64a8 8 0 0 0 8-8V60.36a8 8 0 0 0-2.87-6.14M96 104H48V78.68L96 70Z");
                        break;
                    case 5:
                        SvgData = Geometry.Parse("M136 144h80v72l-80-14.55Zm-96 40l64 11.64V144H40Zm96-129.45V112h80V40ZM40 112h64V60.36L40 72Z M216 136h-80a8 8 0 0 0-8 8v57.45a8 8 0 0 0 6.57 7.88l80 14.54a7.61 7.61 0 0 0 1.43.13a8 8 0 0 0 8-8v-72a8 8 0 0 0-8-8m-8 70.41l-64-11.63V152h64ZM104 136H40a8 8 0 0 0-8 8v40a8 8 0 0 0 6.57 7.87l64 11.64a8.54 8.54 0 0 0 1.43.13a8 8 0 0 0 8-8V144a8 8 0 0 0-8-8m-8 50.05l-48-8.73V152h48ZM221.13 33.86a8 8 0 0 0-6.56-1.73l-80 14.55a8 8 0 0 0-6.57 7.87V112a8 8 0 0 0 8 8h80a8 8 0 0 0 8-8V40a8 8 0 0 0-2.87-6.14M208 104h-64V61.22l64-11.63Zm-98.87-49.78a8 8 0 0 0-6.56-1.73l-64 11.64A8 8 0 0 0 32 72v40a8 8 0 0 0 8 8h64a8 8 0 0 0 8-8V60.36a8 8 0 0 0-2.87-6.14M96 104H48V78.68L96 70Z");
                        break;
                    case 6:
                        SvgData = Geometry.Parse("M21 13v7.434a1.5 1.5 0 0 1-1.553 1.499l-.133-.011L12 21.008V13zm-11 0v7.758l-5.248-.656A2 2 0 0 1 3 18.117V13zm9.314-10.922a1.5 1.5 0 0 1 1.68 1.355l.006.133V11h-9V2.992zM10 3.242V11H3V5.883a2 2 0 0 1 1.752-1.985z");
                        break;
                    case 7:
                        SvgData = Geometry.Parse("m 0,0 -9.885,-1.456 0,-7.155 L 0,-8.533 0,0 z m -17.998,-2.548 0.007,-6.117 7.188,0.054 0,7.03 -7.195,-0.967 z m 0.005,-6.843 10e-4,-6.12 7.189,-0.985 0,7.105 -7.19,0 z m 8.108,-0.114 0,-7.141 L 0,-18 l 0.002,8.495 -9.887,0 z");
                        break;
                    case 8:
                        SvgData = Geometry.Parse("m11.788 2.974l-3.038.434V7.25h4.75V4.46a1.5 1.5 0 0 0-1.712-1.486M13.5 8.75H8.75v3.842l3.038.434A1.5 1.5 0 0 0 13.5 11.54zm-6.25-1.5V3.622l-3.462.495A1.5 1.5 0 0 0 2.5 5.602V7.25zM2.5 8.75h4.75v3.628l-3.462-.495A1.5 1.5 0 0 1 2.5 10.398zm1.076-6.118A3 3 0 0 0 1 5.602v4.796a3 3 0 0 0 2.576 2.97l8 1.143A3 3 0 0 0 15 11.54V4.459a3 3 0 0 0-3.424-2.97z");
                        break;
                    case 9:
                        SvgData = Geometry.Parse("M2.67 5.3v.61l-.71.3V5.6zm0 5.94v.62l-.71.29v-.59zm0 6.03v.62l-.71.29v-.59zM2.6 7.29v.55l-.57.26v-.54zm0 1.99v.54l-.57.26v-.54zm0 4.03v.53l-.57.26v-.54zm0 2.02v.54l-.57.26v-.53zm1.9-9.69v.72l-1 .37V6zm0 6.02v.71l-1 .37v-.71zm0 6.04v.71l-1 .4v-.71zm-.07-9.98v.66l-.79.3V8zm0 2.01v.64l-.78.3V10zm0 4.01v.64l-.78.31v-.66zm0 2.02v.65l-.78.31v-.66zm1.88-9.67v.85l-1.26.49v-.84zm0 6.02v.85l-1.26.49v-.84zm0 5.97v.85l-1.26.5v-.85zm-.06-9.9v.76l-1.06.4v-.73zm0 2v.75l-1.06.42v-.75zm0 4.01v.75l-1.06.43v-.75zm0 1.95v.76l-1.06.42v-.75zm2.04-10.1v1.12l-1.57.62V6.67zm0 6.02v1.13l-1.57.61v-1.12zm0 5.97v1.12l-1.57.62v-1.11zm-.09-9.9v1.03l-1.31.53V8.66zm0 2v1.02l-1.31.53v-1.03zm0 4.02v1.03l-1.31.52v-1.03zm0 1.95v1.02l-1.31.52v-1.01zm2.14-10.25v1.47L8.61 8V6.56zm0 6.02v1.46l-1.73.7v-1.47zm0 5.97v1.46l-1.73.7v-1.46zm-.08-9.79v1.23l-1.48.59V8.64zm0 1.92v1.23l-1.48.58v-1.22zm0 4.09v1.25l-1.48.57v-1.23zm0 1.94v1.25l-1.48.59V16.6zm2.19-10.88v2l-1.86.77V6c.64-.35 1.26-.65 1.86-.88m0 2.21v1.73l-1.86.78V8.1zm0 1.95v1.76l-1.86.78v-1.76zm0 1.97V13l-1.86.77V12zm0 1.96V15l-1.86.78V14zm0 2v1.75l-1.86.8V16zm0 1.96v1.87c-.73.28-1.35.55-1.86.8v-1.88zm9.59-11.99v14.05c-1.19-.79-2.67-1.18-4.45-1.18c-1.47 0-3.12.3-4.94.91v-1.9c.97-.37 2.03-.64 3.19-.8v-4.57c-.98.12-2.04.46-3.19 1.02V11.4c.99-.46 2.06-.77 3.19-.94V6c-1.02.18-2.08.53-3.19 1V5.03C14.27 4.34 15.86 4 17.41 4c1.68 0 3.22.39 4.63 1.18m-1.89 1.23c-.76-.41-1.65-.59-2.73-.59c-.13 0-.25.01-.37.02v4.54l.41-.01c.91 0 1.81.13 2.69.43zm0 5.69c-.81-.36-1.72-.54-2.71-.54c-.13 0-.26.01-.39.02v4.58h.41c.99 0 1.89.12 2.69.37z");
                        break;
                    default:
                        SvgData = Geometry.Parse("M14.814.111A.5.5 0 0 1 15 .5V7H7V1.596L14.395.01a.5.5 0 0 1 .42.1M6 1.81L.395 3.011A.5.5 0 0 0 0 3.5V7h6zM0 8v4.5a.5.5 0 0 0 .43.495l5.57.796V8zm7 5.934l7.43 1.061A.5.5 0 0 0 15 14.5V8H7z");
                        break;
                }
                #endregion
            }
        }

        #region [Dark Title Bar]
        void Window_Initialized(object sender, EventArgs e)
        {
            // Required for the dark title bar.
            if (sender is Window wnd)
                wnd.WindowState = WindowState.Minimized;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // You can use the text version of the path data via a binding, but this can only be done once. (unless you add logic to clear the binding)
            //var bind = new Binding
            //{
            //    Source = SvgData // "M3 12V6.75l6-1.32v6.48zm17-9v8.75l-10 .15V5.21zM3 13l6 .09v6.81l-6-1.15zm17 .25V22l-10-1.91V13.1z"
            //};
            //BindingOperations.SetBinding(logo, System.Windows.Shapes.Path.DataProperty, bind);

            try
            {
                // Get some dark title bar.
                if (e.Source is Window wnd)
                {
                    _winHandle = new WindowInteropHelper(wnd).Handle;
                    GlassHelper.UseDarkTitleBar(_winHandle);

                    // Optional: center screen
                    wnd.Top = (SystemParameters.WorkArea.Bottom - wnd.Height) / 2;
                    wnd.Left = (SystemParameters.WorkArea.Right - wnd.Width) / 2;

                    wnd.WindowState = WindowState.Normal;
                }

                // Auto-navigate to page #1.
                Task.Run(async () =>
                {
                    await Task.Delay(600);
                    Extensions.RunOnUIThread(() =>
                    {
                        sidebar.SelectedIndex = 0;
                    });
                });
            }
            catch (Exception ex)
            {
                Extensions.ShowDialogThreadSafe($"Loaded: {ex.Message}", nonModal: false, imagePath: "AppLogo.png");
            }        
        }
        #endregion

        public void NavigateTo(int page = 1)
        {
            try
            {
                frmNav.Navigate(new Uri($"/Pages/Page{page}.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Extensions.ShowDialogThreadSafe($"NavigateTo: {ex.Message}", isWarning: true, nonModal: false, imagePath: "AppLogo.png");
            }
        }
    }
}
  