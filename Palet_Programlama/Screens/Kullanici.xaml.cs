using Palet_Programlama.Languages;
using Palet_Programlama.Services;
using Palet_Programlama.UserController;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Palet_Programlama.Screens
{
    /// <summary>
    /// Interaction logic for Kullanici.xaml
    /// </summary>
    public partial class Kullanici : Page
    {
        private readonly Frame _mainFrame;
        private readonly KullanicilarServisi _kullanicilarServisi;
        private bool _sayfaHazirMi = false;
        private readonly AyarlarServisi _ayarlarServisi;
        private readonly GirisAyarlariServisi _girisAyarlariServisi;

        public Kullanici(Frame Main)
        {
            InitializeComponent();
            _mainFrame = Main;
            _kullanicilarServisi = new KullanicilarServisi();
            _ayarlarServisi = new AyarlarServisi();
            _girisAyarlariServisi = new GirisAyarlariServisi();
            HatirlananGirisiYukle();
            string seciliDil = _ayarlarServisi.SeciliDiliGetir();

            if (seciliDil == "eng")
            {
                cmbDil.SelectedIndex = (int)LanguagesEnum.eng;
            }
            else
            {
                cmbDil.SelectedIndex = (int)LanguagesEnum.tr;
            }

            DiliUygula();
            MetinleriGuncelle();

            _sayfaHazirMi = true;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateKullaniciPlaceholder();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateKullaniciPlaceholder();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void HatirlananGirisiYukle()
        {
            var ayarlar = _girisAyarlariServisi.AyarlariYukle();

            if (ayarlar == null || !ayarlar.BeniHatirla)
                return;

            kullanici_textbox.Text = ayarlar.SonGirenKullaniciAdi ?? "";
            passwordBox.Password = ayarlar.SonGirenSifre ?? "";
            chkBeniHatirla.IsChecked = true;
        }
        private void UpdatePasswordPlaceholder()
        {
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                passwordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateKullaniciPlaceholder();
        }

        private void UpdateKullaniciPlaceholder()
        {
            if (string.IsNullOrEmpty(kullanici_textbox.Text))
            {
                KullaniciPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                KullaniciPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void BildirimGoster(string mesajKey, string butonKey = "ButtonKey.btntamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void btnGiris_Click(object sender, RoutedEventArgs e)
        {
            string kullaniciAdi = kullanici_textbox.Text.Trim();
            string sifre = passwordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                BildirimGoster("Ayarlar.kullanciSifreZorunlu");
                return;
            }

            var kullanicilar = _kullanicilarServisi.TumKullanicilariGetir();

            var kullanici = kullanicilar.FirstOrDefault(x =>
                x.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase) &&
                x.Sifre == sifre &&
                x.AktifMi);

            if (kullanici == null)
            {
                BildirimGoster("Kullanici.kullaniciAdiSifreHatali");
                return;
            }

            bool beniHatirla = chkBeniHatirla.IsChecked == true;

            _girisAyarlariServisi.BasariliGirisiKaydet(kullaniciAdi, sifre, beniHatirla);

            _mainFrame.Navigate(new Anasayfa(_mainFrame));
        }

        private void DiliUygula()
        {
            int dilIndex = cmbDil.SelectedIndex;

            if (dilIndex == (int)LanguagesEnum.tr)
            {
                LanguageConverter.DilYukle("tr");
            }
            else if (dilIndex == (int)LanguagesEnum.eng)
            {
                LanguageConverter.DilYukle("eng");
            }
            else
            {
                LanguageConverter.DilYukle("tr");
            }
        }

        private void MetinleriGuncelle()
        {
            KullaniciPlaceholder.Text = LanguageConverter.GetString("Kullanici.kullaniciAdi");
            passwordPlaceholder.Text = LanguageConverter.GetString("Kullanici.kullaniciSifre");
            chkBeniHatirla.Content = LanguageConverter.GetString("Kullanici.beniHatirla");
            btnGiris.Content = LanguageConverter.GetString("ButtonKey.btnGiris");
        }

        private void cmbDil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_sayfaHazirMi || cmbDil == null)
                return;

            DiliUygula();
            MetinleriGuncelle();

            if (cmbDil.SelectedIndex == (int)LanguagesEnum.tr)
            {
                _ayarlarServisi.SeciliDiliKaydet("tr");
            }
            else if (cmbDil.SelectedIndex == (int)LanguagesEnum.eng)
            {
                _ayarlarServisi.SeciliDiliKaydet("eng");
            }
        }
    }
}