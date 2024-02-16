using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SpinnerNav
{
    /// <summary>
    /// https://icon-sets.iconify.design/oi/
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            base.OnStartup(e);
        }

        /// <summary>
        /// Handle application object exceptions. (main UI thread only)
        /// </summary>
        void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Unhandled exception thrown from Dispatcher {e.Dispatcher.Thread.Name}: {e.Exception}");
            e.Handled = true;
        }

        /// <summary>
        /// Handle exceptions thrown from custom threads.
        /// </summary>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"[ERROR] Thread exception: {ex?.Message}");
        }

        /// <summary>
        /// Returns the declaring type's namespace.
        /// </summary>
        public static string GetCurrentNamespace() => System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace ?? "App";

        /// <summary>
        /// Returns the declaring type's assembly name.
        /// </summary>
        public static string GetCurrentAssemblyName() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "App"; //System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Assembly.FullName;

        /// <summary>
        /// Returns the AssemblyVersion, not the FileVersion.
        /// </summary>
        public static Version GetCurrentAssemblyVersion() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

    }
}
