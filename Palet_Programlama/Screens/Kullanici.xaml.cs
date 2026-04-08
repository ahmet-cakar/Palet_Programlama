using Palet_Programlama.Languages;
using Palet_Programlama.Services;
using Palet_Programlama.UserController;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public Kullanici(Frame Main)
        {
            InitializeComponent();
            this._mainFrame = Main;
            _kullanicilarServisi = new KullanicilarServisi();
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

        private void UpdatePasswordPlaceholder()
        {
            // Eğer şifre boşsa, placeholder'ı göster
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                passwordPlaceholder.Visibility = Visibility.Visible; // Placeholder görünür
            }
            else
            {
                passwordPlaceholder.Visibility = Visibility.Collapsed; // Placeholder gizlenir
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
                KullaniciPlaceholder.Visibility = Visibility.Visible; // Placeholder görünür
            }
            else
            {
                KullaniciPlaceholder.Visibility = Visibility.Collapsed; // Placeholder gizlenir
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
                x.Sifre == sifre);

            if (kullanici == null)
            {
                BildirimGoster("Kullanici.kullaniciAdiSifreHatali");
                return;
            }

            _mainFrame.Navigate(new Anasayfa(_mainFrame));
        }

        private void cmbDil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_sayfaHazirMi || cmbDil == null || KullaniciPlaceholder == null || passwordPlaceholder == null || btnGiris == null)
                return;

            int dilIndex = cmbDil.SelectedIndex;

            if (dilIndex == 0)
            {
                LanguageConverter.DilYukle("tr");
                KullaniciPlaceholder.Text = "Kullanici Adı";
                passwordPlaceholder.Text = "Şifre";
                btnGiris.Content = "Giriş";
                chkBeniHatirla.Content = "Beni Hatırla";
            }
            else if (dilIndex == 1)
            {
                LanguageConverter.DilYukle("eng");
                KullaniciPlaceholder.Text = "User";
                passwordPlaceholder.Text = "Password";
                btnGiris.Content = "Login";
                chkBeniHatirla.Content = "Remember Me";
            }
        }
    }
}
