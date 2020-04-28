using System.Windows.Media.Imaging;

namespace LauncherWPF.Models
{
    public class AppModel
    {
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
