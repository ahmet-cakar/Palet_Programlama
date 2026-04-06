using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Palet_Programlama.Languages;

namespace Palet_Programlama.UserController
{
    public partial class MetinGirisKutusu : Window
    {
        public string GirilenMetin { get; private set; } = string.Empty;
        public bool SeperatorKullanilacak { get; private set; } = false;
        public List<int> SecilenSeperatorKatlari { get; private set; } = new();

        private int _toplamKat = 0;

        public double SeperatorKalinlik { get; private set; } = 0;
        public MetinGirisKutusu()
        {
            InitializeComponent();
        }

        public void Ayarla(
            string baslik,
            string mesaj,
            string varsayilanMetin = "",
            string tamamButonYazisi = null,
            string iptalButonYazisi = null)
        {
            tamamButonYazisi ??= LanguageConverter.GetString("ButtonKey.btntamam");
            iptalButonYazisi ??= LanguageConverter.GetString("ButtonKey.btnIptal");
            txtBaslik.Text = baslik;
            txtMesaj.Text = mesaj;
            txtGiris.Text = varsayilanMetin;
            btnTamam.Content = tamamButonYazisi;
            btnIptal.Content = iptalButonYazisi;
            txtSeperatorKalinlik.Text = "";
            panelSeperatorKalinlik.Visibility = Visibility.Collapsed;
            chkSeperatorKullanilacak.IsChecked = false;
            panelSeperatorListe.Children.Clear();

            txtGiris.Focus();
            txtGiris.SelectAll();
        }

        public void SeperatorSecimleriniHazirla(int toplamKat)
        {
            _toplamKat = toplamKat;
            panelSeperatorListe.Children.Clear();
        }

        private void chkSeperatorKullanilacak_Checked(object sender, RoutedEventArgs e)
        {
            panelSeperatorKalinlik.Visibility = Visibility.Visible;
            SeperatorListesiniDoldur();
        }

        private void chkSeperatorKullanilacak_Unchecked(object sender, RoutedEventArgs e)
        {
            panelSeperatorKalinlik.Visibility = Visibility.Collapsed;
            txtSeperatorKalinlik.Text = "";
            panelSeperatorListe.Children.Clear();
        }

        private void txtSeperatorKalinlik_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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

        private void SeperatorListesiniDoldur()
        {
            panelSeperatorListe.Children.Clear();

            var paletCheckBox = new CheckBox
            {
                Content = LanguageConverter.GetString("DizilimKaydetPopup.paletUzerine"),
                Tag = 0,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 18,
                Height = 34,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };

            panelSeperatorListe.Children.Add(paletCheckBox);

            for (int i = 1; i <= _toplamKat; i++)
            {
                var checkBox = new CheckBox
                {
                    Content = string.Format(
                    LanguageConverter.GetString("DizilimKaydetPopup.katUzerine"),
                    i),
                    Tag = i,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 18,
                    Height = 34,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 8)
                };

                panelSeperatorListe.Children.Add(checkBox);
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

            SeperatorKullanilacak = chkSeperatorKullanilacak.IsChecked == true;

            if (SeperatorKullanilacak)
            {
                string giris = (txtSeperatorKalinlik.Text ?? "").Trim().Replace(',', '.');

                if (!double.TryParse(
                        giris,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double kalinlik) || kalinlik <= 0)
                {
                    BildirimGoster("MesajKutusu.gecerliSeperatorKalinligiGir");
                    return;
                }

                SeperatorKalinlik = kalinlik;

                SecilenSeperatorKatlari = panelSeperatorListe.Children
                    .OfType<CheckBox>()
                    .Where(x => x.IsChecked == true)
                    .Select(x => (int)x.Tag)
                    .ToList();
            }
            else
            {
                SeperatorKalinlik = 0;
                SecilenSeperatorKatlari.Clear();
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