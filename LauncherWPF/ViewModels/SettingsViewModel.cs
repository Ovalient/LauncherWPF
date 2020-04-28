using Caliburn.Micro;
using LauncherWPF.Models;
using LauncherWPF.ViewModels.Settings;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace LauncherWPF.ViewModels
{
    public class SettingsViewModel : Conductor<Screen>, IScreen
    {
        #region Construction
        public SettingsViewModel()
        {
            // 기본 세팅 화면(Settings01GeneralViewModel)
            ActivateItem(new Settings01GeneralViewModel());

            // Navigation Bar 리스트
            SettingsModels.Add(new SettingsModel { Icon = PackIconKind.FileCog, Menu = "일반" });
            SettingsModels.Add(new SettingsModel { Icon = PackIconKind.Download, Menu = "프로그램 설치/업데이트" });
            SettingsModels.Add(new SettingsModel { Icon = PackIconKind.InformationCircleOutline, Menu = "정보" });
        }
        #endregion

        #region Members
        Settings01GeneralViewModel settings01 = new Settings01GeneralViewModel();
        private BindableCollection<SettingsModel> SettingsModels { get; set; } = new BindableCollection<SettingsModel>();
        #endregion

        #region Properties
        private SettingsModel _selectedSetting;
        public SettingsModel SelectedSetting
        {
            get { return _selectedSetting; }
            set
            {
                _selectedSetting = value;
                NotifyOfPropertyChange(() => SelectedSetting);
            }
        }

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
        /// <summary>
        /// Navigation Bar 목록 선택 시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            // 인덱스에 따라 다른 뷰 Active
            switch (listBox.SelectedIndex)
            {
                case 0:
                    ActivateItem(new Settings01GeneralViewModel());
                    break;
                case 1:
                    ActivateItem(new Settings02ProgramViewModel());
                    break;
                case 2:
                    ActivateItem(new Settings03InfoViewModel());
                    break;
                default:
                    break;
            }
        }

        public void OkButton()
        {
            this.IsSelected = true;
            // 사용자 설정(Settings.settings) 저장
            Properties.Settings.Default.Save();
            TryClose();
        }

        public void CloseButton()
        {
            this.IsSelected = false;
            TryClose();
        }
        #endregion
    }
}
