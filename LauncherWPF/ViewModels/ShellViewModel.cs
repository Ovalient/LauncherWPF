using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace LauncherWPF.ViewModels
{
    public class ShellViewModel : Conductor<Screen>
    {
        #region Members
        private readonly IWindowManager _windowManager = new WindowManager();
        public System.Windows.Forms.NotifyIcon notifyIcon;
        #endregion

        #region Methods
        public void ShellView_Loaded()
        {
            // 시스템 트레이 아이콘 추가
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = Properties.Resources.VCSTsIcon;
            notifyIcon.Text = "VCS-Ts";
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // 사용자 설정(Settings.settings)에 입력된 ListToggle 값에 따라 리스트, 아이콘 뷰 Active
            if (Properties.Settings.Default.ListToggle)
                ActivateItem(new AppListViewModel(new EventAggregator()));
            else
                ActivateItem(new AppIconViewModel(new EventAggregator()));
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            _windowManager.ShowWindow(new ShellViewModel(), null, null);
        }

        public void ShellView_Closing()
        {
            CloseButton();
        }

        /// <summary>
        /// 설정 버튼
        /// </summary>
        public void SettingButton()
        {
            var settingsViewModel = IoC.Get<SettingsViewModel>();
            var result = _windowManager.ShowDialog(settingsViewModel);
            if(settingsViewModel.IsSelected)
            {
                // SettingsViewModel을 닫으면, ShellView 재시작
                Task.Run(() => ShellView_Loaded());
            }
        }

        /// <summary>
        /// 종료 버튼
        /// </summary>
        public void CloseButton()
        {
            System.Environment.Exit(0);
        }
        #endregion
    }
}
