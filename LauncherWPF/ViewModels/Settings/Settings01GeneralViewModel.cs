using Caliburn.Micro;

namespace LauncherWPF.ViewModels.Settings
{
    public class Settings01GeneralViewModel : Screen
    {
        #region Properties
        private bool _listChecked;
        public bool ListChecked
        {
            get { return _listChecked; }
            set
            {
                if (value.Equals(_listChecked)) return;
                _listChecked = value;
                Properties.Settings.Default.ListToggle = value;
                NotifyOfPropertyChange(() => ListChecked);
            }
        }
        #endregion

        #region Members
        public void LoadSettings01()
        {
            Properties.Settings.Default.Reload();
            ListChecked = Properties.Settings.Default.ListToggle;
        }
        #endregion
    }
}
