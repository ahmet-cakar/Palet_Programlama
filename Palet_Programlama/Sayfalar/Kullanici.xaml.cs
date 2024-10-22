using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for Kullanici.xaml
    /// </summary>
    public partial class Kullanici : Page
    {
        private Frame MainFrame;
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
            MainFrame.Navigate(new Anasayfa(MainFrame));
        }
    }
}
