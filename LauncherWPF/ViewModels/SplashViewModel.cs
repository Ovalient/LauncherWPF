using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace LauncherWPF.ViewModels
{
    public class SplashViewModel : Screen
    {
        private IEventAggregator _events;

        private string _splashMessage;
        public string SplashMessage
        {
            get { return _splashMessage; }
            set
            {
                _splashMessage = value;
                NotifyOfPropertyChange(() => SplashMessage);
            }
        }

        public SplashViewModel(IEventAggregator events)
        {
            _events = events;

            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TryClose();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(3000);
        }
    }
}
