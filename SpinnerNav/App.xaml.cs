using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
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
            WriteToLog($"UnhandledException => {e.Exception}");
        }

        /// <summary>
        /// Handle exceptions thrown from custom threads.
        /// </summary>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"[ERROR] Thread exception: {ex?.Message}");
            WriteToLog($"ThreadException => {ex?.Message}");
        }

        /// <summary>
        /// Returns the declaring type's namespace.
        /// </summary>
        public static string GetCurrentNamespace() => System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace ?? "WpfApp";

        /// <summary>
        /// Returns the declaring type's assembly name.
        /// </summary>
        public static string GetCurrentAssemblyName() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "WpfApp"; //System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Assembly.FullName;

        /// <summary>
        /// Returns the AssemblyVersion, not the FileVersion.
        /// </summary>
        public static Version GetCurrentAssemblyVersion() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

        #region [Simple Logging]
        public static bool WriteToLog(string message)
        {
            try
            {
                var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "WpfApp";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name}.log");
                using (var fileStream = new StreamWriter(File.OpenWrite(path)))
                {
                    fileStream.BaseStream.Seek(0, SeekOrigin.End);
                    fileStream.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")}] {message}");
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "Log"}]: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> WriteToLogAsync(string message, CancellationToken token = default)
        {
            try
            {
                string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "WpfApp";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name}.log");
                await File.AppendAllTextAsync(path, $"[{DateTime.Now.ToString("hh:mm:ss.fff tt")}] {message}{Environment.NewLine}", token);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[{System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "Log"}]: {ex.Message}");
                return await Task.FromResult(false);
            }
        }
        #endregion
    }
}
