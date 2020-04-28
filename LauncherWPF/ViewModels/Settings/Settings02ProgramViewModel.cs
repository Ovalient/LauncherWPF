using Caliburn.Micro;

namespace LauncherWPF.ViewModels.Settings
{
    public class Settings02ProgramViewModel : Screen
    {
        #region Properties
        public string _installPath = Properties.Settings.Default.DirectoryPath;
        public string InstallPath
        {
            get { return _installPath; }
            set
            {
                _installPath = value;
                NotifyOfPropertyChange(() => InstallPath);
            }
        }
        #endregion
    }
}
