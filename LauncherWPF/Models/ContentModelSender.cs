using System.Windows.Media.Imaging;

namespace LauncherWPF.Models
{
    public class ContentModelSender
    {
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public string InstallPath { get; set; }
        public bool IsInstalled { get; set; }
    }
}
