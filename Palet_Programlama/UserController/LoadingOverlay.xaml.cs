using System.Windows;
using System.Windows.Controls;

namespace Palet_Programlama.UserController
{
    public partial class LoadingOverlay : UserControl
    {
        public LoadingOverlay()
        {
            InitializeComponent();
        }

        public void Goster(string mesaj = "Yükleniyor...")
        {
            TxtMesaj.Text = string.IsNullOrWhiteSpace(mesaj) ? "Yükleniyor..." : mesaj;
            Visibility = Visibility.Visible;
        }

        public void Gizle()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}