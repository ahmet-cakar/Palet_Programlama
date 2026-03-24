using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for Kullanici.xaml
    /// </summary>
    public partial class Kullanici : Page
    {
        private readonly Frame MainFrame;

        public Kullanici(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
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

        private void giris_button_Click(object sender, RoutedEventArgs e)
        {
            if ((kullanici_textbox.Text == "" || kullanici_textbox.Text == "Admin") && passwordBox.Password == "")
            {
                KullaniciDil.Kullaniciadi = "Admin";
                ImageSource currentImage = DilBtn.Source;
                string currentUri = currentImage.ToString();
                if (currentUri.Contains("eng"))
                {
                    KullaniciDil.Dil = "eng";
                }
                else if (currentUri.Contains("turkce"))
                {
                    KullaniciDil.Dil = "tr";
                }
                MainFrame.Navigate(new Anasayfa(MainFrame));
            }

        }

        private void DilBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageSource currentImage = DilBtn.Source;
            // DilBtn'in mevcut kaynağını BitmapImage olarak al

            string currentUri = currentImage.ToString();
            // Eğer kaynak geçerli değilse işlem yapma
            if (currentImage == null) return;

            // Kaynağın URI'sini string olarak al



            // Yeni URI'yi hesaplayacak bir değişken
            string newUri = string.Empty;

            // Mevcut duruma göre yeni URI belirle
            switch (currentUri)
            {
                case "pack://application:,,,/Resimler/Kullanıcı/dil_turkce_kapali.png":
                    newUri = "pack://application:,,,/Resimler/Kullanıcı/dil_turkce_Acik.png";
                    break;

                case "pack://application:,,,/Resimler/Kullanıcı/dil_turkce_Acik.png":
                case "pack://application:,,,/Resimler/Kullanıcı/dil_eng_Acik.png":
                    // Tıklama konumunu al
                    Point clickPoint = e.GetPosition(DilBtn);
                    double imageHeight = DilBtn.ActualHeight;

                    // Üst tarafa tıklanmışsa
                    if (clickPoint.Y < imageHeight / 2)
                    {
                        newUri = (currentUri.Contains("turkce"))
                                 ? "pack://application:,,,/Resimler/Kullanıcı/dil_turkce_kapali.png"
                                 : "pack://application:,,,/Resimler/Kullanıcı/dil_eng.png";
                    }
                    // Alt tarafa tıklanmışsa
                    else
                    {
                        newUri = (currentUri.Contains("turkce"))
                                 ? "pack://application:,,,/Resimler/Kullanıcı/dil_eng.png"
                                 : "pack://application:,,,/Resimler/Kullanıcı/dil_turkce_kapali.png";
                    }
                    break;

                case "pack://application:,,,/Resimler/Kullanıcı/dil_eng.png":
                    newUri = "pack://application:,,,/Resimler/Kullanıcı/dil_eng_Acik.png";
                    break;
            }

            // Eğer yeni bir URI belirlendiyse kaynağı güncelle
            if (!string.IsNullOrEmpty(newUri))
            {
                DilBtn.Source = new BitmapImage(new Uri(newUri));
            }
            if (newUri.Contains("eng"))
            {
                KullaniciPlaceholder.Text = "User";
                passwordPlaceholder.Text = "Password";
                giris_button.Content = "Login";
            }
            else if (newUri.Contains("turkce_kapali"))
            {
                KullaniciPlaceholder.Text = "Kullanıcı Adı";
                passwordPlaceholder.Text = "Şifre";
                giris_button.Content = "Giriş";
            }

        }

    }
}
