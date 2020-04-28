using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.Windows.Navigation;

namespace LauncherWPF.ViewModels.Settings
{
    public class Settings03InfoViewModel : Screen
    {
        #region Properties
        public string _programVersion = Properties.Settings.Default.ProgramVersion;
        public string ProgramVersion
        {
            get { return _programVersion; }
            set
            {
                _programVersion = value;
                NotifyOfPropertyChange(() => ProgramVersion);
            }
        }

        public string _license01 = Properties.Settings.Default.CaliburnMicroLicense;
        public string License01
        {
            get { return _license01; }
            set
            {
                _license01 = value;
                NotifyOfPropertyChange(() => License01);
            }
        }

        public string _license02 = Properties.Settings.Default.MaterialDesignLicense;
        public string License02
        {
            get { return _license02; }
            set
            {
                _license02 = value;
                NotifyOfPropertyChange(() => License02);
            }
        }
        #endregion

        #region Methods
        public void LinkClick(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }
        #endregion
    }
}
