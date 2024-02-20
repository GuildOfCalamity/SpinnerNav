#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Path = System.IO.Path;
using Con = System.Diagnostics.Debug;
using System.Windows.Threading;
using System.Threading;
using System.IO;

namespace SpinnerNav.Support
{
    public enum MessageLevel
    {
        Info = 1,
        Attention = 2,
        Success = 3,
        Warning = 4,
        Error = 5,
    }

    public static class Extensions
    {
        private static readonly WeakReference s_random = new WeakReference(null);
        /// <summary>
        /// A garbage-friendly globally-accessible random.
        /// </summary>
        public static Random Rnd
        {
            get
            {
                Random? r = s_random.Target as Random;
                if (r == null) { s_random.Target = r = new Random(); }
                return r;
            }
        }

        /// <summary>
        /// An updated string truncation helper.
        /// </summary>
        public static string Truncate(this string text, int maxLength, string mesial = "…")
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (maxLength > 0 && text.Length > maxLength)
            {
                var limit = maxLength / 2;
                if (limit > 1)
                {
                    return String.Format("{0}{1}{2}", text.Substring(0, limit).Trim(), mesial, text.Substring(text.Length - limit).Trim());
                }
                else
                {
                    var tmp = text.Length <= maxLength ? text : text.Substring(0, maxLength).Trim();
                    return String.Format("{0}{1}", tmp, mesial);
                }
            }
            return text;
        }

        public static T? ParseEnum<T>(this string value)
        {
            try { return (T)Enum.Parse(typeof(T), value, true); }
            catch (Exception) { return default(T); }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            return val.CompareTo(min) < 0 ? min : (val.CompareTo(max) > 0 ? max : val);
        }

        public static TimeSpan Multiply(this TimeSpan timeSpan, double scalar) => new TimeSpan((long)(timeSpan.Ticks * scalar));

        /// <summary>
        /// LINQ extension.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
                action(i);
        }

        /// <summary>
        /// Simplified try/catch block.
        /// </summary>
        public static void TryThis(this Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                string[] lines = {
                    $"A {ex.GetType()} was thrown!",
                    ex.Message,
                    ex.InnerException?.Message ?? "",
                };
                Con.WriteLine(string.Join(Environment.NewLine, lines));
            }
        }

        public static string LocalApplicationDataFolder(string moduleName = "Settings")
        {
            var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}\\{moduleName}");
            return result;
        }

        /// <summary>
        /// Generic method for using a control's built-in dispatcher.
        /// </summary>
        /// <param name="ctrl">common <see cref="System.Windows.Controls.Control"/></param>
        /// <param name="message">the text for the control's content</param>
        public static void UpdateControlContent(this System.Windows.Controls.Control ctrl, string message)
        {
            if (ctrl.Dispatcher == null)
                return;

            ctrl.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                // This could be changed to a switch/case.
                if (ctrl is ListView lv)
                    lv.Items.Add(message);
                else if (ctrl is ListBox lb)
                    lb.Items.Add(message);
                else if (ctrl is Label lbl)
                    lbl.Content = message;
                else if (ctrl is TextBox tb)
                    tb.Text = message;
                else if (ctrl is Button btn)
                    btn.Content = message;
                else if (ctrl is GroupBox grb)
                    grb.Content = message;
                else if (ctrl is CheckBox ckb)
                    ckb.Content = message;
                else if (ctrl is RadioButton rdb)
                    rdb.Content = message;
                else if (ctrl is ComboBox cmb)
                    cmb.Text = message;
                else if (ctrl is ProgressBar pb)
                    pb.Value = Double.Parse(message);
                else
                    Con.WriteLine($"Undefined control type: {ctrl.GetType()}");
            }));
        }

        /// <summary>
        /// Uses <see cref="Dispatcher.Invoke(Action, DispatcherPriority)"/> for thread safety.
        /// </summary>
        public static void RunOnUIThread(Action action)
        {
            // The Application.Current may be null when closing the
            // window while a background thread is still running.
            if (action == null || System.Windows.Application.Current == null)
                return;

            Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(DispatcherPriority.Normal, (Delegate)(action));
        }

        /// <summary>
        /// Uses <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])"/> for thread safety.
        /// </summary>
        public static void RunOnUIThreadAsync(Action action)
        {
            // The Application.Current may be null when closing the
            // window while a background thread is still running.
            if (action == null || System.Windows.Application.Current == null)
                return;

            Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate)(action));
        }

        /// <summary>
        /// A throw-back to the good ol' WinForm days.
        /// </summary>
        /// <param name="useNestedFrame">if true, employ <see cref="Dispatcher.PushFrame"/></param>
        public static void DoEvents(bool useNestedFrame = false)
        {
            if (!useNestedFrame)
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new System.Threading.ThreadStart(() => System.Threading.Thread.Sleep(0)));
            else
            {
                // Create new nested message pump.
                DispatcherFrame nested = new DispatcherFrame(true);

                // Dispatch a callback to the current message queue, when getting called,
                // this callback will end the nested message loop. The priority of this
                // callback should always be lower than that of the UI event messages.
                #pragma warning disable CS8622
                var exitFrameOp = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate (object arg)
                {
                    DispatcherFrame? f = arg as DispatcherFrame;
                    if (f != null) { f.Continue = false; }
                }, nested);
                #pragma warning restore CS8622

                // Pump the nested message loop, the nested message loop will
                // immediately process the messages left inside the message queue.
                Dispatcher.PushFrame(nested);

                // If the exit frame callback doesn't get completed, abort it.
                if (exitFrameOp.Status != DispatcherOperationStatus.Completed)
                    exitFrameOp.Abort();
            }
        }

#if SYSTEM_DRAWING
        public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Icon icon)
        {
            System.Windows.Media.ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
#endif
        public static void RotateImage(this Image imgControl, double angle = 90.0)
        {
            if (imgControl.Source == null)
                return;

            var img = (BitmapSource)(imgControl.Source);
            var cache = new CachedBitmap(img, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            imgControl.Source = new TransformedBitmap(cache, new RotateTransform(angle));
        }

        public static void TurnBlackAndWhite(this Image imgControl, double alphaThresh = 1.0)
        {
            if (imgControl.Source == null)
                return;

            var img = (BitmapSource)(imgControl.Source);
            imgControl.Source = new FormatConvertedBitmap(img, PixelFormats.Gray8, BitmapPalettes.Gray256, alphaThresh);
        }

        /// <summary>
        /// Image helper method
        /// </summary>
        /// <param name="UriPath"></param>
        /// <returns><see cref="BitmapFrame"/></returns>
        public static BitmapFrame? GetBitmapFrame(string UriPath)
        {
            try
            {
                IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri(UriPath,
                    UriKind.RelativeOrAbsolute),
                    BitmapCreateOptions.None,
                    BitmapCacheOption.Default);
                return ibd.Frames[0];
            }
            catch (FileNotFoundException ex)
            {
                Con.WriteLine($"GetBitmapFrame(NotFoundException): {ex.Message}");
                return null;
            }
            catch (FileFormatException ex)
            {
                Con.WriteLine($"GetBitmapFrame(FormatException): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Simple dialog that may be called from any thread.
        /// </summary>
        /// <remarks>
        /// This dialog uses a dark theme style.
        /// </remarks>
        public static void ShowDialogThreadSafe(string msg, string caption = "Notice", bool nonModal = false, bool isWarning = false, bool shadows = true, string imagePath = "", Action? onClose = null)
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    // Upon entry set the synchronization context to the current dispatcher.
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Threading.Dispatcher.CurrentDispatcher));

                    #region [Common colors]
                    Brush tbForeground = new SolidColorBrush(Color.FromRgb(220, 220, 220));
                    Brush btnForeground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
                    var clrShadow = new Color { A = 255, R = 1, G = 1, B = 5 };
                    var btnEffect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = clrShadow,
                        Direction = 310,
                        ShadowDepth = 2.5,
                        Opacity = 0.9,
                        BlurRadius = 2
                    };
                    #endregion

                    // NOTE: Borders can only have one child element. If you need to place borders around
                    //       multiple elements, you must place a border around each element individually.
                    Border border = new Border();
                    border.Width = 600;
                    border.Height = 200;
                    if (isWarning)
                        border.Background = ChangeBackgroundColor(Color.FromRgb(160, 30, 10), Color.FromRgb(90, 20, 5), Color.FromRgb(50, 10, 2));
                    else
                        border.Background = ChangeBackgroundColor(Color.FromRgb(39, 39, 46), Color.FromRgb(22, 22, 27), Color.FromRgb(12, 12, 17));
                    border.BorderThickness = new Thickness(1.5);
                    border.BorderBrush = new SolidColorBrush(Colors.Gray);
                    border.CornerRadius = new CornerRadius(5);
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.VerticalAlignment = VerticalAlignment.Stretch;

                    // The canvas will hold the controls and the border will hold the canvas.
                    Canvas cnvs = new Canvas();
                    cnvs.VerticalAlignment = VerticalAlignment.Stretch;
                    cnvs.HorizontalAlignment = HorizontalAlignment.Stretch;

                    #region [Configure FrameworkElements]
                    // StackPanel setup
                    var sp = new StackPanel
                    {
                        Background = Brushes.Transparent,
                        Orientation = Orientation.Vertical,
                        Height = border.Height,
                        Width = border.Width
                    };

                    Image? img = null;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        if (!imagePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                            imagePath = $"/{imagePath}";

                        // "pack://application:,,,/Assets/MB_Info.png".ReturnImageSource();
                        var imgSource = $"pack://application:,,,{imagePath}".ReturnImageSource();
                        if (imgSource != null)
                        {
                            var ib = new System.Windows.Media.ImageBrush(new System.Windows.Media.Imaging.BitmapImage(new Uri(@$"pack://application:,,,{imagePath}")));
                            img = new Image()
                            {
                                Width = 42,
                                Opacity = 0.9,
                                Margin = new Thickness(6),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Source = imgSource,
                            };
                        }
                        else
                        {
                            Con.WriteLine($"[WARNING] Image was not found for the dialog.");
                        }
                    }

                    // Determine if additional margin is needed for the image.
                    Thickness tbPad = new Thickness(0, 0, 0, 0);
                    if (img != null)
                    {
                        tbPad = new Thickness(20, 0, 0, 0);
                        RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.Fant);
                    }

                    // TextBox setup
                    var tbx = new TextBox()
                    {
                        Background = sp.Background,
                        FontSize = 18,
                        AcceptsReturn = true,
                        BorderThickness = new Thickness(0),
                        MaxHeight = border.Height / 2,
                        MinHeight = border.Height / 2,
                        MaxWidth = border.Width / 1.111,
                        MinWidth = border.Width / 1.111,
                        Padding = tbPad,
                        Margin = new Thickness(10, 25, 10, 15),
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = tbForeground,
                        FontWeight = FontWeights.Regular,
                        Text = msg
                    };

                    // Button setup
                    var btn = new Button()
                    {
                        Width = border.Width / 6,
                        Height = 34,
                        Content = "OK",
                        FontSize = 20,
                        FontWeight = FontWeights.Regular,
                        Foreground = btnForeground,
                        Margin = new Thickness(10, 10, 10, 10),
                        VerticalContentAlignment = VerticalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = sp.Background
                    };
                    // Add shadow effect to button.
                    if (shadows)
                        btn.Effect = btnEffect;
                    #endregion

                    // Add textbox to StackPanel.
                    sp.Children.Add(tbx);

                    // Add button to StackPanel.
                    sp.Children.Add(btn);

                    // Add image to Canvas, not StackPanel.
                    if (img != null)
                        cnvs.Children.Add(img);

                    // Add StackPanel to Canvas.
                    cnvs.Children.Add(sp);

                    // Borders can only have one child element.
                    border.Child = cnvs;

                    // Add shadow effect to dialog border.
                    if (shadows)
                    {
                        border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                        {
                            Color = clrShadow,
                            Direction = 310,
                            ShadowDepth = 6,
                            Opacity = 0.5,
                            BlurRadius = 8
                        };
                    }

                    #region [Create the window and show]
                    // The window will hold our control content.
                    var w = new Window();
                    w.WindowStyle = WindowStyle.None;
                    w.AllowsTransparency = true;
                    w.Background = Brushes.Transparent;
                    w.VerticalAlignment = VerticalAlignment.Center;
                    w.HorizontalAlignment = HorizontalAlignment.Center;
                    // Add padding for shadow effect.
                    w.Height = border.Height + 20;
                    w.Width = border.Width + 20;

                    // Apply content to new window.
                    w.Content = border;

                    if (string.IsNullOrEmpty(caption))
                        caption = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

                    // Set the window title.
                    w.Title = caption;

                    // Set the window start position.
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    #region [Events]
                    // Setup a delegate for the window loaded event.
                    w.Loaded += (s, e) =>
                    {
                        // We could add a timer here to self-close.
                        Con.WriteLine($"[INFO] Thread-safe dialog loaded.");
                    };

                    // Setup a delegate for the window closed event.
                    w.Closed += (s, e) =>
                    {
                        onClose?.Invoke();
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
                    };

                    // Setup a delegate to support escape keypress close.
                    w.PreviewKeyUp += (s, e) =>
                    {
                        if (e.Key == System.Windows.Input.Key.Escape)
                            w.Close();
                    };

                    // Setup a delegate for the close button click event.
                    btn.Click += (s, e) =>
                    {
                        w.Close();
                    };

                    // Setup a delegate for the mouse enter event.
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                        if (shadows)
                        {
                            btn.Effect = new System.Windows.Media.Effects.DropShadowEffect
                            {
                                Color = new Color { A = 255, R = 190, G = 230, B = 253 },
                                Direction = 90,
                                ShadowDepth = 0.1,
                                Opacity = 0.9,
                                BlurRadius = 24
                            };
                        }
                    };

                    // Setup a delegate for the mouse leave event.
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.Foreground = btnForeground;
                        if (shadows)
                            btn.Effect = btnEffect;
                    };

                    btn.PreviewMouseDown += (s, e) =>
                    {
                        if (shadows)
                            btn.Effect = btnEffect;
                    };

                    // Setup a delegate for the window mouse down event.
                    w.MouseDown += (s, e) =>
                    {
                        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                        {
                            w.DragMove();
                        }
                        else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                        {
                            // There could be a formatting sitch where the close button
                            // is pushed off the window, so provide a backup close method.
                            w.Close();
                        }
                    };
                    #endregion

                    // We'll ensure topmost since the dialog could become burried.
                    if (nonModal)
                        w.Topmost = true;

                    // Show our constructed window. We might not bo on the
                    // main UI thread, so we shouldn't use "w.ShowDialog()"
                    w.Show();
                    #endregion

                    // Make sure the essential WPF duties are taken care of.
                    System.Windows.Threading.Dispatcher.Run();
                });

                // You can only show a dialog in a STA thread.
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
                // If modal then block until the dialog is closed.
                if (!nonModal)
                    thread.Join();
            }
            catch (Exception ex) { Con.WriteLine($"[WARNING] Could not show dialog: {ex.Message}"); }
        }

        /// <summary>
        /// <see cref="System.Windows.Media.Imaging.BitmapImage"/> helper method.
        /// </summary>
        /// <param name="uriPath">the pack uri path to the image</param>
        /// <returns><see cref="System.Windows.Media.Imaging.BitmapImage"/></returns>
        /// <remarks>
        /// URI Packing can assume the following formats:
        /// 1) Content File...
        ///    "pack://application:,,,/Assets/logo.png"
        ///    https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8#content-file-pack-uris
        /// 2) Referenced Assembly Resource...
        ///    "pack://application:,,,/AssemblyNameHere;component/Resources/logo.png"
        ///    https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8#referenced-assembly-resource-file
        /// 3) Site Of Origin...
        ///    "pack://siteoforigin:,,,/Assets/SiteOfOriginFile.xaml"
        ///    https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8#site-of-origin-pack-uris
        /// </remarks>
        public static System.Windows.Media.Imaging.BitmapImage? ReturnImageSource(this string uriPath)
        {
            try
            {
                System.Windows.Media.Imaging.BitmapImage holder = new System.Windows.Media.Imaging.BitmapImage();
                holder.BeginInit();
                holder.UriSource = new Uri(uriPath); //new Uri("pack://application:,,,/AssemblyName;component/Resources/logo.png");
                holder.EndInit();
                return holder;
            }
            catch (Exception ex)
            {
                Con.WriteLine($"[ERROR] ReturnImageSource: {ex.Message}");
                return null;
            }
        }

        public static LinearGradientBrush ChangeBackgroundColor(Color c1, Color c2, Color c3)
        {
            var gs1 = new GradientStop(c1, 0);
            var gs2 = new GradientStop(c2, 0.5);
            var gs3 = new GradientStop(c3, 1);
            var gsc = new GradientStopCollection { gs1, gs2, gs3 };
            var lgb = new LinearGradientBrush
            {
                StartPoint = new Point(1, 0),
                EndPoint = new Point(1, 1),
                GradientStops = gsc
            };

            return lgb;
        }

        public static RadialGradientBrush ChangeBackgroundColor(Color c1, Color c2)
        {
            var gs1 = new GradientStop(c1, 0);
            var gs2 = new GradientStop(c2, 1);
            var gsc = new GradientStopCollection { gs1, gs2 };
            var lgb = new RadialGradientBrush
            {
                GradientStops = gsc
            };

            return lgb;
        }

        /// <summary>
        /// Gets a contrasting color based on the current color.
        /// var blue = System.Drawing.Color.Orange.GetContrastingColor();
        /// </summary>
        /// <param name="value"><see cref="System.Windows.Media.Color"/></param>
        /// <returns><see cref="System.Windows.Media.Color"/></returns>
        public static System.Windows.Media.Color GetContrastingColor(this System.Windows.Media.Color value)
        {
            var d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * value.R + 0.587 * value.G + 0.114 * value.B) / 255;

            if (a < 0.5)
                d = 0;   // bright colors - black font
            else
                d = 255; // dark colors - white font

            return System.Windows.Media.Color.FromRgb((byte)d, (byte)d, (byte)d);
        }

        /// <summary>
        /// Find & return a WPF control based on its resource key name.
        /// </summary>
        public static T FindControl<T>(this System.Windows.FrameworkElement control, string resourceKey) where T : System.Windows.FrameworkElement
        {
            return (T)control.FindResource(resourceKey);
        }

        /// <summary>
        /// Finds all controls on a Window object by their type.
        /// NOTE: If you're trying to get this to work and finding that your 
        /// Window (for instance) has 0 visual children, try running this method 
        /// in the Loaded event handler. If you run it in the constructor 
        /// (even after InitializeComponent()), the visual children aren't 
        /// loaded yet, and it won't work. 
        /// </summary>
        /// <typeparam name="T">type of control to find</typeparam>
        /// <param name="depObj">the <see cref="DependencyObject"/> to search</param>
        /// <returns><see cref="IEnumerable{T}"/> of controls matching the type</returns>
        public static IEnumerable<T> FindVisualChilds<T>(this DependencyObject depObj) where T : DependencyObject
        {
            /* EXAMPLE USE...
            foreach (TextBlock tb in FindVisualChildren<TextBlock>(window))
            {
                // do something with tb here
            }
            */
            if (depObj == null)
                yield return (T)Enumerable.Empty<T>();

            // NOTE: Switching VisualTreeHelper to LogicalTreeHelpers will cause invisible elements to be included too.
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null)
                    continue;
                if (ithChild is T t)
                    yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild))
                    yield return childOfChild;
            }
        }

        public static void HideAllVisualChilds<T>(this UIElementCollection coll) where T : UIElementCollection
        {
            // Casting the UIElementCollection into List
            List<FrameworkElement> lstElement = coll.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            // Hide all Controls
            foreach (Control control in lstControl)
            {
                if (control == null)
                    continue;

                control.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public static IEnumerable<System.Windows.Controls.Control> GetAllControls<T>(this UIElementCollection coll) where T : UIElementCollection
        {
            // Casting the UIElementCollection into List
            List<FrameworkElement> lstElement = coll.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            // Iterate control objects
            foreach (Control control in lstControl)
            {
                if (control == null)
                    continue;

                yield return control;
            }
        }

        /// <summary>
        /// EXAMPLE: IEnumerable<DependencyObject> cntrls = this.FindUIElements();
        /// NOTE: If you're trying to get this to work and finding that your 
        /// Window (for instance) has 0 visual children, try running this method 
        /// in the Loaded event handler. If you run it in the constructor 
        /// (even after InitializeComponent()), the visual children aren't 
        /// loaded yet, and it won't work. 
        /// </summary>
        /// <param name="parent">some parent control like <see cref="System.Windows.Window"/></param>
        /// <returns>list of <see cref="IEnumerable{DependencyObject}"/></returns>
        public static IEnumerable<DependencyObject> FindUIElements(this DependencyObject parent)
        {
            if (parent == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject o = VisualTreeHelper.GetChild(parent, i);

                foreach (DependencyObject obj in FindUIElements(o))
                {
                    if (obj == null)
                        continue;

                    if (obj is UIElement ret)
                        yield return ret;
                }
            }

            yield return parent;
        }

        /// <summary>
        /// btnTest.RemoveClickEvent();
        /// </summary>
        /// <param name="btn"><see cref="Button"/></param>
        public static void RemoveClickEvent(this Button btn)
        {
            FieldInfo? f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            if (f1 != null)
            {
                object? obj = f1.GetValue(btn);
                if (obj != null)
                {
                    PropertyInfo? pi = btn?.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (pi != null)
                    {
                        System.ComponentModel.EventHandlerList? list = (System.ComponentModel.EventHandlerList)pi.GetValue(btn, null);
                        list?.RemoveHandler(obj, list[obj]);
                    }
                }
            }
        }

        #region [Task Helpers]
        /// <summary>
        /// Chainable task helper.
        /// var result = await SomeLongAsyncFunction().WithTimeout(TimeSpan.FromSeconds(2));
        /// </summary>
        /// <typeparam name="TResult">the type of task result</typeparam>
        /// <returns><see cref="Task"/>TResult</returns>
        public async static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            Task winner = await (Task.WhenAny(task, Task.Delay(timeout)));

            if (winner != task)
                throw new TimeoutException();

            return await task;   // Unwrap result/re-throw
        }

        /// <summary>
        /// Task extension to add a timeout.
        /// </summary>
        /// <returns>The task with timeout.</returns>
        /// <param name="task">Task.</param>
        /// <param name="timeoutInMilliseconds">Timeout duration in Milliseconds.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async static Task<T> WithTimeout<T>(this Task<T> task, int timeoutInMilliseconds)
        {
            var retTask = await Task.WhenAny(task, Task.Delay(timeoutInMilliseconds))
                .ConfigureAwait(false);

            #pragma warning disable CS8603 // Possible null reference return.
            return retTask is Task<T> ? task.Result : default;
            #pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Chainable task helper.
        /// </summary>
        /// <example>
        /// var result = await SomeLongTaskFunction().WithCancellation(cts.Token);
        /// </example>
        /// <typeparam name="TResult">the type of task result</typeparam>
        /// <returns><see cref="Task"/>TResult</returns>
        public static Task<TResult> WithCancellation<TResult>(this Task<TResult> task, CancellationToken cancelToken)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            CancellationTokenRegistration reg = cancelToken.Register(() => tcs.TrySetCanceled());
            task.ContinueWith(ant =>
            {
                reg.Dispose();
                if (ant.IsCanceled)
                    tcs.TrySetCanceled();
                else if (ant.IsFaulted)
                    tcs.TrySetException(ant.Exception?.InnerException ?? new Exception("empty inner exception"));
                else
                    tcs.TrySetResult(ant.Result);
            });
            return tcs.Task;  // Return the TaskCompletionSource result
        }

        public static Task<T> WithAllExceptions<T>(this Task<T> task)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            task.ContinueWith(ignored =>
            {
                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                        Con.WriteLine($"[TaskStatus.Canceled]");
                        tcs.SetCanceled();
                        break;
                    case TaskStatus.RanToCompletion:
                        tcs.SetResult(task.Result);
                        Con.WriteLine($"[TaskStatus.RanToCompletion({task.Result})]");
                        break;
                    case TaskStatus.Faulted:
                        // SetException will automatically wrap the original AggregateException
                        // in another one. The new wrapper will be removed in TaskAwaiter, leaving
                        // the original intact.
                        Con.WriteLine($"[TaskStatus.Faulted: {task.Exception?.Message}]");
                        tcs.SetException(task.Exception ?? new Exception("empty exception"));
                        break;
                    default:
                        Con.WriteLine($"[TaskStatus: Continuation called illegally.]");
                        tcs.SetException(new InvalidOperationException("Continuation called illegally."));
                        break;
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Task.Factory.StartNew (() => { throw null; }).IgnoreExceptions();
        /// </summary>
        public static void IgnoreExceptions(this Task task, bool logEx = false)
        {
            task.ContinueWith(t =>
            {
                AggregateException ignore = t.Exception ?? new AggregateException("empty aggregate exception");

                ignore?.Flatten().Handle(ex =>
                {
                    if (logEx)
                        App.WriteToLog($"Type: {ex.GetType()}, Message: {ex.Message}");
                    return true; // don't re-throw
                });

            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        #pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        /// <summary>
        /// Attempts to await on the task and catches exception
        /// </summary>
        /// <param name="task">Task to execute</param>
        /// <param name="onException">What to do when method has an exception</param>
        /// <param name="continueOnCapturedContext">If the context should be captured.</param>
        public static async void SafeFireAndForget(this Task task, Action<Exception>? onException = null, bool continueOnCapturedContext = false)
        #pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null)
            {
                onException.Invoke(ex);
            }
            catch (Exception ex) when (onException == null)
            {
                App.WriteToLog($"SafeFireAndForget: {ex.Message}");
            }
        }
        #endregion
    }
}
