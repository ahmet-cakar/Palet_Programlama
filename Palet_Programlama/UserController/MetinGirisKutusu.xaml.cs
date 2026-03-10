using System.Windows;
using System.Windows.Input;

namespace Palet_Programlama.UserController
{
    public partial class MetinGirisKutusu : Window
    {
        public string GirilenMetin { get; private set; } = string.Empty;

        public MetinGirisKutusu()
        {
            InitializeComponent();
        }

        public void Ayarla(
            string baslik,
            string mesaj,
            string varsayilanMetin = "",
            string tamamButonYazisi = "Tamam",
            string iptalButonYazisi = "İptal")
        {
            txtBaslik.Text = baslik;
            txtMesaj.Text = mesaj;
            txtGiris.Text = varsayilanMetin;
            btnTamam.Content = tamamButonYazisi;
            btnIptal.Content = iptalButonYazisi;

            txtGiris.Focus();
            txtGiris.SelectAll();
        }
        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void btnTamam_Click(object sender, RoutedEventArgs e)
        {
            GirilenMetin = txtGiris.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(GirilenMetin))
            {
                BildirimGoster("MesajKutusu.isimGiriniz");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}