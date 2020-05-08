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

        private bool _priorityChecked;
        public bool PriorityChecked
        {
            get { return _priorityChecked; }
            set
            {
                if (value.Equals(_priorityChecked)) return;
                _priorityChecked = value;
                Properties.Settings.Default.PriorityToggle = value;
                NotifyOfPropertyChange(() => PriorityChecked);
            }
        }
        #endregion

        #region Members
        public void LoadSettings01()
        {
            Properties.Settings.Default.Reload();
            ListChecked = Properties.Settings.Default.ListToggle;
            PriorityChecked = Properties.Settings.Default.PriorityToggle;
        }
        #endregion
    }
}
