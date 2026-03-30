using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Palet_Programlama.UserController
{
    public partial class MetinGirisKutusu : Window
    {
        public string GirilenMetin { get; private set; } = string.Empty;
        public bool SeparatorKullanilacak { get; private set; } = false;
        public List<int> SecilenSeparatorKatlari { get; private set; } = new();

        private int _toplamKat = 0;

        public double SeparatorKalinlik { get; private set; } = 0;
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
            txtSeparatorKalinlik.Text = "";
            panelSeparatorKalinlik.Visibility = Visibility.Collapsed;
            chkSeparatorKullanilacak.IsChecked = false;
            panelSeparatorListe.Children.Clear();

            txtGiris.Focus();
            txtGiris.SelectAll();
        }

        public void SeparatorSecimleriniHazirla(int toplamKat)
        {
            _toplamKat = toplamKat;
            panelSeparatorListe.Children.Clear();
        }

        private void chkSeparatorKullanilacak_Checked(object sender, RoutedEventArgs e)
        {
            panelSeparatorKalinlik.Visibility = Visibility.Visible;
            SeparatorListesiniDoldur();
        }

        private void chkSeparatorKullanilacak_Unchecked(object sender, RoutedEventArgs e)
        {
            panelSeparatorKalinlik.Visibility = Visibility.Collapsed;
            txtSeparatorKalinlik.Text = "";
            panelSeparatorListe.Children.Clear();
        }

        private void txtSeparatorKalinlik_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            string yeniMetin = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            int noktaVirgulSayisi = yeniMetin.Count(c => c == '.' || c == ',');

            bool gecersizKarakterVar = e.Text.Any(c => !char.IsDigit(c) && c != '.' && c != ',');

            if (gecersizKarakterVar || noktaVirgulSayisi > 1)
            {
                e.Handled = true;
            }
        }

        private void SeparatorListesiniDoldur()
        {
            panelSeparatorListe.Children.Clear();

            var paletCheckBox = new CheckBox
            {
                Content = "Palet üzerine",
                Tag = 0,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 18,
                Height = 34,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };

            panelSeparatorListe.Children.Add(paletCheckBox);

            for (int i = 1; i <= _toplamKat; i++)
            {
                var checkBox = new CheckBox
                {
                    Content = $"{i}. kat üzerine",
                    Tag = i,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 18,
                    Height = 34,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 8)
                };

                panelSeparatorListe.Children.Add(checkBox);
            }
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

            SeparatorKullanilacak = chkSeparatorKullanilacak.IsChecked == true;

            if (SeparatorKullanilacak)
            {
                string giris = (txtSeparatorKalinlik.Text ?? "").Trim().Replace(',', '.');

                if (!double.TryParse(
                        giris,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double kalinlik) || kalinlik <= 0)
                {
                    BildirimGoster("MesajKutusu.gecerliSeparatorKalinligiGir");
                    return;
                }

                SeparatorKalinlik = kalinlik;

                SecilenSeparatorKatlari = panelSeparatorListe.Children
                    .OfType<CheckBox>()
                    .Where(x => x.IsChecked == true)
                    .Select(x => (int)x.Tag)
                    .ToList();
            }
            else
            {
                SeparatorKalinlik = 0;
                SecilenSeparatorKatlari.Clear();
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