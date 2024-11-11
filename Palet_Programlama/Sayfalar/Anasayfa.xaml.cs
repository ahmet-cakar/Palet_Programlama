
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
            MainFrame.Navigate(new DizilimYap(MainFrame));
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
    }
}
