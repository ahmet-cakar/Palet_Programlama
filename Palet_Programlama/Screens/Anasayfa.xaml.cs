using Palet_Programlama.Languages;
using Palet_Programlama.Screens.UrunPaletEkle.Models;

using Palet_Programlama.UserController;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Palet_Programlama.Screens
{
    /// <summary>
    /// Interaction logic for Anasayfa.xaml
    /// </summary>
    public partial class Anasayfa : Page
    {
        private readonly Frame MainFrame;
        UrunIslemler urunIslemler = new UrunIslemler();
        PaletIslemler paletIslemler = new PaletIslemler();

        public Anasayfa(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
            userpanelborder1.Visibility = Visibility.Hidden;
            userpanelborder2.Visibility = Visibility.Hidden;
        }

        private void UrunEklePage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UrunEkle());
        }

        private void DizilimPage_Click(object sender, RoutedEventArgs e)
        {
            var urunler = urunIslemler.UrunListesiniGetir();
            var paletler = paletIslemler.PaletListesiniGetir();

            UrunPaletSecimKutusu pencere = new UrunPaletSecimKutusu(
                urunler,
                paletler,
                LanguageConverter.GetString("UrunPaletSecimPopup.dizilimZorunluDegil"));

            bool? sonuc = pencere.ShowDialog();

            if (sonuc == true)
            {
                if (pencere.SecilenUrun != null && pencere.SecilenPalet != null)
                {
                    MainFrame.Navigate(new DizilimYap(MainFrame, pencere.SecilenUrun, pencere.SecilenPalet, pencere.SecilenDizilimAdi));
                }
            }
        }

        private void btn_program_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Programlar(MainFrame));
        }

        private void Kullaniciicon_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (userpanelborder1.Visibility == Visibility.Hidden)
            {
                userpanelborder1.Visibility = Visibility.Visible;
                userpanelborder2.Visibility = Visibility.Visible;
            }
            else
            {
                userpanelborder1.Visibility = Visibility.Hidden;
                userpanelborder2.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HizAyarları(MainFrame));
        }

        private void textlogout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Kullanici(MainFrame));
        }

        private void btnGruplamaYap_Click(object sender, RoutedEventArgs e)
        {
            var urunler = urunIslemler.UrunListesiniGetir();
            var paletler = paletIslemler.PaletListesiniGetir();

            UrunPaletSecimKutusu pencere = new UrunPaletSecimKutusu(
                urunler,
                paletler,
                LanguageConverter.GetString("UrunPaletSecimPopup.zorunluAlan"));

            bool? sonuc = pencere.ShowDialog();

            if (sonuc == true)
            {
                if (pencere.SecilenUrun != null && pencere.SecilenPalet != null)
                {
                    MainFrame.Navigate(new GruplamaYap(MainFrame, pencere.SecilenUrun, pencere.SecilenPalet, pencere.SecilenDizilimAdi));
                }
            }
        }

        private void btnAyarlar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Ayarlar(MainFrame));
        }
    }
}