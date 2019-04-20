using System;
using System.Threading;
using System.Windows.Forms;
using GrepLib;

namespace SimpleGrep
{
    static class Program
    {
        private const bool FOR_DEBUG_LOG = false;

        [STAThread]
        static void Main()
        {
            Logger.IsEnable = FOR_DEBUG_LOG;

            try
            {
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Logger.Open();
                Logger.ClearCache();
                Logger.Write("■SimpleGrep START-------------------------------------------");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            finally
            {
                Logger.Write("■SimpleGrep END---------------------------------------------");
                Logger.WriteCache(Utils.GetUniqueFileNmae("SimpleGrep.log"));
                Logger.Close();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                Utils.ShowMessageBoxAndWriteLogForException(e.Exception);
            }
            finally
            {
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Utils.ShowMessageBoxAndWriteLogForException((Exception)e.ExceptionObject);
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
