

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Palet_Programlama.Sınıflar;
using Palet_Programlama.UserController;
namespace Palet_Programlama.Sayfalar
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
            string Dil = KullaniciDil.Dil;
            LanguageConverter.LoadLanguage($"{Dil}");
            InitializeComponent();
            this.MainFrame = Main;
            //string User = KullaniciDil.Kullaniciadi; 
            userpanelborder1.Visibility = Visibility.Hidden;
            userpanelborder2.Visibility = Visibility.Hidden;
        }

        private void UrunEklePage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UrunEkle(MainFrame));
        }

        private void DizilimPage_Click(object sender, RoutedEventArgs e)
        {
            var urunler = urunIslemler.UrunListesiniGetir();
            var paletler = paletIslemler.PaletListesiniGetir();

            UrunPaletSecimKutusu pencere = new UrunPaletSecimKutusu(urunler, paletler);
            bool? sonuc = pencere.ShowDialog();

            if (sonuc == true)
            {
                if (pencere.SecilenUrun != null && pencere.SecilenPalet != null)
                {
                    MainFrame.Navigate(new DizilimYap(MainFrame,pencere.SecilenUrun,pencere.SecilenPalet));
                }
            }
        }
        private void btn_program_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Programlar(MainFrame));
        }

        private void Kullaniciicon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textuser.Text = KullaniciDil.Kullaniciadi;
            if (userpanelborder1.Visibility==Visibility.Hidden)
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
            //BildirimKutusu kutusu=new BildirimKutusu();
            //kutusu.Show();
            //kutusu.MesajGonder("MesajKutusu.btncon1", "MesajKutusu.mesaj");
            
        }

        private void textlogout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Kullanici(MainFrame));
        }

        private void btnGruplamaYap_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GruplamaYap(MainFrame));
        }
    }
}
