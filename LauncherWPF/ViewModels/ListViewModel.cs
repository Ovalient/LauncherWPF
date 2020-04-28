using Caliburn.Micro;
using LauncherWPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LauncherWPF.ViewModels
{
    public class ListViewModel : Conductor<Screen>, IHandle<AppModelSender>
    {
        public ListViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }

        public void Handle(AppModelSender _sender)
        {
            if(_sender.TargetView == "ListView")
            {
                Icon = _sender.Apps.Icon;
                Name = _sender.Apps.Name;
            }
        }

        #region Members
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager = new WindowManager();
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

        public string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string _button;
        public string Button
        {
            get { return _button; }
            set
            {
                _button = value;
                NotifyOfPropertyChange(() => Button);
            }
        }

        public string _installPath;
        public string InstallPath
        {
            get { return _installPath; }
            set
            {
                _installPath = value;
                NotifyOfPropertyChange(() => InstallPath);
            }
        }

        public bool _isClickable = true;
        public bool IsClickable
        {
            get { return _isClickable; }
            set
            {
                _isClickable = value;
                NotifyOfPropertyChange(() => IsClickable);
            }
        }

        public bool _isInstalled;
        public bool IsInstalled
        {
            get { return _isInstalled; }
            set
            {
                _isInstalled = value;
                NotifyOfPropertyChange(() => IsInstalled);
            }
        }

        public string _infoText;
        public string InfoText
        {
            get { return _infoText; }
            set
            {
                _infoText = value;
                NotifyOfPropertyChange(() => InfoText);
            }
        }

        public int _currentProgress = 0;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
            }
        }

        public int _maximumProgress;
        public int MaximumProgress
        {
            get { return _maximumProgress; }
            set
            {
                _maximumProgress = value;
                NotifyOfPropertyChange(() => MaximumProgress);
            }
        }
        #endregion

        #region Methods
        public void ListViewModel_Loaded()
        {
            string fileName = Name + ".exe";

            if (GetFullPath(fileName) != null)
            {
                Button = "실행";
                IsInstalled = false;
            }
            else
            {
                Button = "설치";
            }
        }

        /// <summary>
        /// 경로 검색
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Properties.Settings.Default.DirectoryPath + Name;
            InstallPath = values.ToString();
            foreach(var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        /// <summary>
        /// 프로그램 설치가 된 상태이면 프로세스 실행
        /// 설치가 되지 않으면 ShowDialog(DownloadViewModel)
        /// </summary>
        public void ButtonClick()
        {
            string fileName = Name + ".exe";

            if (Button == "실행")
            {
                // GetFullPath에서 리턴 받은 경로의 fileName 프로세스 실행
                Process.Start(GetFullPath(fileName));
            }
            else
            {
                // 버튼 비활성화
                IsClickable = false;

                var downloadViewModel = IoC.Get<DownloadViewModel>();
                downloadViewModel.Icon = Icon;
                downloadViewModel.Name = Name;
                downloadViewModel.InstallPath = InstallPath;

                var result = _windowManager.ShowDialog(downloadViewModel);
                if (downloadViewModel.IsSelected)
                {
                    // '설치' 버튼을 누르면 CheckFtpPath 메소드 실행
                    IsInstalled = true;
                    Task.Run(() => CheckFtpPath(fileName));
                }
                else
                {
                    // 버튼 활성화
                    IsClickable = true;
                }
            }
        }

        /// <summary>
        /// 경로 확인 후 다운로드 실행
        /// </summary>
        /// <param name="fileName"></param>
        private void CheckFtpPath(string fileName)
        {
            // FTP 서버 경로
            // 로컬 다운로드 경로
            string serverPath = Path.Combine("ftp://192.168.150.117/", Name);
            string downloadPath = Properties.Settings.Default.DirectoryPath + "\\" + Name;
            DirectoryInfo dir = new DirectoryInfo(downloadPath);

            if(dir.Exists)
            {
                // 경로가 존재하면
                // 바로 DownloadFileFtp 메소드 실행
                DownloadFileFtp(serverPath, downloadPath);
            }
            else
            {
                // 경로가 없으면
                // 경로 생성 후,
                // DownloadFileFtp 메소드 실행
                dir.Create();
                DownloadFileFtp(serverPath, downloadPath);
            }
        }

        /// <summary>
        /// FTP 서버로 부터 다운로드 시작
        /// </summary>
        /// <param name="serverPath"></param>
        /// <param name="downloadPath"></param>
        private void DownloadFileFtp(string serverPath, string downloadPath)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(serverPath);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = new NetworkCredential("dantech", "dantech");

            List<string> lines = new List<string>();

            using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    // 폴더 내 파일 목록 전부 List에 추가
                    lines.Add(listReader.ReadLine());
                }
            }

            // 폴더 내 파일 모두 다운로드 시작
            foreach (string line in lines)
            {
                string[] tokens = line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[8];
                string permissions = tokens[0];

                string localFilePath = Path.Combine(downloadPath, name);
                string fileUrl = Path.Combine(serverPath, name);

                if (permissions[0] == 'd')
                {
                    if (!Directory.Exists(localFilePath))
                    {
                        Directory.CreateDirectory(localFilePath);
                    }

                    DownloadFileFtp(fileUrl, localFilePath);
                }
                else
                {
                    FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = new NetworkCredential("dantech", "dantech");

                    using (FtpWebResponse downloadResponse = (FtpWebResponse)downloadRequest.GetResponse())
                    using (Stream sourceStream = downloadResponse.GetResponseStream())
                    using (Stream targetStream = File.Create(localFilePath))
                    {
                        byte[] buffer = new byte[10240];
                        int read;
                        string fullSize = GetFullSize(fileUrl).ToString();
                        while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            targetStream.Write(buffer, 0, read);
                            int position = (int)targetStream.Position;

                            // ProgressBar Value 설정
                            // TextBlock 설정
                            CurrentProgress = position;
                            InfoText = "다운로드 중 [" + targetStream.Position + "/" + fullSize + "]";
                        }
                        InfoText = "다운로드 완료";
                        IsInstalled = true;
                    }
                }
            }
            IsClickable = true;
            ListViewModel_Loaded();
        }

        /// <summary>
        /// FTP 서버 파일의 전체 크기 가져오기
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        private long GetFullSize(string fileUrl)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileUrl);
            request.Proxy = null;
            request.Credentials = new NetworkCredential("dantech", "dantech");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                long size = (long)response.ContentLength;
                MaximumProgress = (int)size;
                return size;
            }
        }
        #endregion
    }
}
