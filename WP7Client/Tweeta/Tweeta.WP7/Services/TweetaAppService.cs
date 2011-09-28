using System.Net;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common;
using Tweeta.Common.Logging;
using WP7HelperFX;

namespace Tweeta.Services
{
    public class TweetaAppService : IAppService
    {
        public static long MAIN_THREAD_ID = 0;

        public void Init()
        {
            Thread.CurrentThread.Name = "MAIN THREAD";
            MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;
            Log.Info("Main Thread ID " + Thread.CurrentThread.ManagedThreadId);

            Application.Current.UnhandledException += Application_UnhandledException;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Application.Current.Host.Settings.EnableFrameRateCounter = true;
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
                //Application.Current.Host.Settings.EnableRedrawRegions = true;
            }

            Log.Initialize();
            MemoryProfiler.Initialize();

            DispatcherHelper.Initialize();

            App.AppSettings.Load();

            App.AppSettings.Save();
        }
        
        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(delegate
            {
                MessageBox.Show(e.ExceptionObject.ToString());
            });
            if (e.ExceptionObject is WebException)
            {
                e.Handled = true;
                
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }
}
