using Caliburn.Micro;
using LauncherWPF.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

namespace LauncherWPF.ViewModels
{
    [Export(typeof(DownloadViewModel))]
    public class DownloadViewModel : Screen, IHandle<ContentModelSender>
    {
        #region Construction
        public DownloadViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }

        public void Handle(ContentModelSender _sender)
        {
            if (_sender.IsInstalled)
            {
                Icon = _sender.Icon;
                Name = _sender.Name;
                InstallPath = _sender.InstallPath;
            }
        }
        #endregion

        #region Members
        private readonly IEventAggregator _events;
        #endregion

        #region Properties
        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private string _installPath;
        public string InstallPath
        {
            get { return _installPath; }
            set
            {
                _installPath = value;
                NotifyOfPropertyChange(() => InstallPath);
            }
        }

        public bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
        #endregion

        #region Methods
        public void Ok()
        {
            this.IsSelected = true;
            TryClose();
        }

        public void Cancel()
        {
            this.IsSelected = false;
            TryClose();
        }

        public void ChangeButton()
        {
            CommonOpenFileDialog commonDialog = new CommonOpenFileDialog();
            commonDialog.IsFolderPicker = true;
            if(commonDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Properties.Settings.Default.DirectoryPath = commonDialog.FileName + "\\";
                Properties.Settings.Default.Save();
                InstallPath = commonDialog.FileName + "\\" + Name;
            }
        }
        #endregion
    }
}
