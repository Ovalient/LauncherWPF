using Caliburn.Micro;
using LauncherWPF.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LauncherWPF.ViewModels
{
    public class AppIconViewModel : Conductor<Screen>
    {
        #region Construction
        public AppIconViewModel(IEventAggregator events)
        {
            _events = events;
        }
        #endregion

        #region Members
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager = new WindowManager();
        public ListViewModel ContentViewModel { get; set; }
        public BindableCollection<AppModel> AppModels { get; set; } = new BindableCollection<AppModel>();
        #endregion

        #region Properties
        private AppModel _selectedApp;
        public AppModel SelectedApp
        {
            get { return _selectedApp; }
            set
            {
                _selectedApp = value;
                NotifyOfPropertyChange(() => SelectedApp);
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
        /// <summary>
        /// product.db의 데이터를 불러와 ListBox에 나열
        /// </summary>
        public void AppIconViewModel_Loaded()
        {
            string path = string.Format(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\product.db");
            string strConn = string.Format("Data Source={0}", path);
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM product ORDER BY sort DESC";

                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    BitmapImage icon;
                    string appName;
                    int sort;

                    Byte[] data = (Byte[])rdr["icon"];
                    using (var ms = new MemoryStream(data))
                    {
                        icon = new BitmapImage();
                        icon.BeginInit();
                        icon.CacheOption = BitmapCacheOption.OnLoad;
                        icon.StreamSource = ms;
                        icon.EndInit();
                    }
                    appName = rdr["name"] as string;
                    if (rdr["sort"] == DBNull.Value)
                    {
                        sort = 0;
                    }
                    else
                    {
                        sort = Convert.ToInt32(rdr["sort"]);
                    }

                    AppModels.Add(new AppModel { Icon = icon, Name = appName, Sort = sort });
                }
                rdr.Close();
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

            var values = Properties.Settings.Default.DirectoryPath + SelectedApp.Name;
            InstallPath = values.ToString();
            foreach (var path in values.Split(Path.PathSeparator))
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
        /// FTP 서버 경로, 다운로드 경로 설정
        /// 다운로드 경로가 없으면 디렉토리 생성
        /// </summary>
        /// <param name="fileName"></param>
        private void CheckFtpPath(string fileName)
        {
            string serverPath = Path.Combine("ftp://192.168.150.117/", SelectedApp.Name);
            string downloadPath = Properties.Settings.Default.DirectoryPath + "\\" + SelectedApp.Name;
            DirectoryInfo dir = new DirectoryInfo(downloadPath);

            if (dir.Exists)
            {
                DownloadFileFtp(serverPath, downloadPath);
            }
            else
            {
                dir.Create();

                DownloadFileFtp(serverPath, downloadPath);
            }
        }

        /// <summary>
        /// 프로그램 설치가 된 상태이면 프로세스 실행
        /// 설치가 되지 않으면 ShowDialog(DownloadViewModel)
        /// </summary>
        public void AppListBox_MouseLeftButtonUp()
        {
            string fileName = SelectedApp.Name + ".exe";

            if (GetFullPath(fileName) != null)
            {
                Process.Start(GetFullPath(fileName));
                IsInstalled = false;
            }
            else
            {
                var downloadViewModel = IoC.Get<DownloadViewModel>();
                downloadViewModel.Icon = SelectedApp.Icon;
                downloadViewModel.Name = SelectedApp.Name;
                downloadViewModel.InstallPath = InstallPath;

                var result = _windowManager.ShowDialog(downloadViewModel);
                if (downloadViewModel.IsSelected)
                {
                    IsInstalled = true;
                    Task.Run(() => CheckFtpPath(fileName));
                }
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
                    lines.Add(listReader.ReadLine());
                }
            }

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
                            CurrentProgress = position;
                            InfoText = SelectedApp.Name + " | 다운로드 중 [" + targetStream.Position + "/" + fullSize + "]";
                        }
                        InfoText = "다운로드 완료";
                        IsInstalled = true;
                    }
                }
            }
            InfoText = string.Empty;
            IsInstalled = false;
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
