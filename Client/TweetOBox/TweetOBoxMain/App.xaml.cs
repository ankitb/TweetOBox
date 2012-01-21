using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using TOB.BLL;
using TOB.TweetSharpWrap;
using System.ComponentModel;
using System.Windows.Threading;
using TweetOBoxMain.Utility;
using System.Threading;
using TOB.Utility;

namespace TweetOBoxMain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private DispatcherTimer _timer = null;
        public App()
        {
            try
            {
                CheckForDatabase.Instance.CheckDataContext();
                ClientKUI.Instance.RegisterTOB();
                ClientKUI.Instance.TOBStart();
                TOB.Utility.CleanUpObjectsInDB.Instance.CleanUpObjects();
                TOB.Utility.MessageNotifier.Instance.NotifyMessage = TweetOBoxMain.Utility.MessageNotifier.Instance.NotifyMessage;
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(15);
                _timer.Tick += new EventHandler(_timer_Tick);
                _timer.Start();

                if (AccountManager.Instance.GetCurrentAccounts().Count == 0)
                {
                    this.MainWindow = new TweetOBoxAccount();
                    this.MainWindow.Show();
                }
                else
                {
                    this.MainWindow = new TweetOBoxMain.TOBMain();
                    this.MainWindow.Show();
                }
            }          
            catch (Exception ex)
            {
                TOB.Logger.TOBLogger.WriteDebugInfo(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            TweetOBoxMain.Utility.MessageNotifier.Instance.NotifyMessage("Ready");
        }

        public string ShowStatus(string message)
        {
            TOBMain mainWindow = this.MainWindow as TOBMain;
            return message;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        { 
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // define application exception handler
            Application.Current.DispatcherUnhandledException += new
               DispatcherUnhandledExceptionEventHandler(
                  AppDispatcherUnhandledException);
            base.OnStartup(e);
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //do whatever you need to do with the exception
            string message = "Opps. An application level error occurred due to which TOB cannot run properly. Regret the inconvenience caused to you. The error detals is: " + e.Exception.Message;
            TOB.Logger.TOBLogger.WriteDebugInfo(e.Exception.Message);
            MessageBoxResult result = MessageBox.Show(message, "ERROR", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.MainWindow.Close();
            }
            e.Handled = true;
            //e.Exception
        }
              
    }
}
