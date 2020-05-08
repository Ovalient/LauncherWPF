using Caliburn.Micro;
using LauncherWPF.Extensions;
using LauncherWPF.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LauncherWPF.ViewModels
{
    public class AppListViewModel : Conductor<Screen>
    {
        #region Construction
        public AppListViewModel(IEventAggregator events)
        {
            _events = events;
        }
        #endregion

        #region Members
        private readonly IEventAggregator _events;
        #endregion

        #region Properties
        public ListViewModel ContentViewModel { get; set; }
        public BindableCollection<AppModel> AppModels { get; set; } = new BindableCollection<AppModel>();

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

        private bool _isLoaded = true;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = value;
                NotifyOfPropertyChange(() => IsLoaded);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// product.db의 데이터를 불러와 ListBox에 나열
        /// </summary>
        public void AppListViewModel_Loaded()
        {
            string path = string.Format(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\product.db");
            string strConn = string.Format("Data Source={0}", path);
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                if(Properties.Settings.Default.PriorityToggle)
                {
                    cmd.CommandText = "SELECT * FROM product ORDER BY sort DESC";
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM product";
                }                

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

        public void ListBox_SelectionChanged()
        {
            IsLoaded = false;
            ActivateItem(new ListViewModel(_events));
            _events.PublishOnUIThread(new AppModelSender() { Apps = SelectedApp, TargetView = "ListView" });
        }

        public void LinkClick(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }
        #endregion
    }
}
