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
        public Kullanici(Frame Main)
        {
            InitializeComponent();
            this._mainFrame = Main;
            _kullanicilarServisi = new KullanicilarServisi();
            
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

        private void DilBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageSource currentImage = DilBtn.Source;
            // DilBtn'in mevcut kaynağını BitmapImage olarak al

            string currentUri = currentImage.ToString();
            // Eğer kaynak geçerli değilse işlem yapma
            if (currentImage == null) return;

  

            // Yeni URI'yi hesaplayacak bir değişken
            string newUri = string.Empty;

            // Mevcut duruma göre yeni URI belirle
            switch (currentUri)
            {
                case "pack://application:,,,/Images/Kullanici/dil_turkce_kapali.png":
                    newUri = "pack://application:,,,/Images/Kullanici/dil_turkce_Acik.png";
                    break;

                case "pack://application:,,,/Images/Kullanici/dil_turkce_Acik.png":
                case "pack://application:,,,/Images/Kullanici/dil_eng_Acik.png":
                    // Tıklama konumunu al
                    Point clickPoint = e.GetPosition(DilBtn);
                    double imageHeight = DilBtn.ActualHeight;

                    // Üst tarafa tıklanmışsa
                    if (clickPoint.Y < imageHeight / 2)
                    {
                        newUri = (currentUri.Contains("turkce"))
                                 ? "pack://application:,,,/Images/Kullanici/dil_turkce_kapali.png"
                                 : "pack://application:,,,/Images/Kullanici/dil_eng.png";
                    }
                    // Alt tarafa tıklanmışsa
                    else
                    {
                        newUri = (currentUri.Contains("turkce"))
                                 ? "pack://application:,,,/Images/Kullanici/dil_eng.png"
                                 : "pack://application:,,,/Images/Kullanici/dil_turkce_kapali.png";
                    }
                    break;

                case "pack://application:,,,/Images/Kullanici/dil_eng.png":
                    newUri = "pack://application:,,,/Images/Kullanici/dil_eng_Acik.png";
                    break;
            }

            // Eğer yeni bir URI belirlendiyse kaynağı güncelle
            if (!string.IsNullOrEmpty(newUri))
            {
                DilBtn.Source = new BitmapImage(new Uri(newUri));
            }
            if (newUri.Contains("eng"))
            {
                LanguageConverter.DilYukle("eng");
                KullaniciPlaceholder.Text = "User";
                passwordPlaceholder.Text = "Password";
                btnGiris.Content = "Login";
            }
            else if (newUri.Contains("turkce_kapali"))
            {
                LanguageConverter.DilYukle("tr");
                KullaniciPlaceholder.Text = "Kullanici Adı";
                passwordPlaceholder.Text = "Şifre";
                btnGiris.Content = "Giriş";
            }

        }

        private void btnGiris_Click(object sender, RoutedEventArgs e)
        {
          
            string kullaniciAdi = kullanici_textbox.Text.Trim();
            string sifre = passwordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                BildirimGoster("Kullanıcı adı ve şifre boş bırakılamaz.");
                return;
            }

            var kullanicilar = _kullanicilarServisi.TumKullanicilariGetir();

            var kullanici = kullanicilar.FirstOrDefault(x =>
                x.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase) &&
                x.Sifre == sifre);

            if (kullanici == null)
            {
                BildirimGoster("Kullanıcı adı veya şifre hatalı.");
                return;
            }

            _mainFrame.Navigate(new Anasayfa(_mainFrame));
        }
    }
}
