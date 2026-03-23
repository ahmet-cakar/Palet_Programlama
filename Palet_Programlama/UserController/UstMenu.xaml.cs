using Palet_Programlama.UserController;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Palet_Programlama.Sayfalar
{
    public partial class UstMenu : UserControl
    {
        private readonly List<MenuSayfaItem> _sayfalar = new()
        {
            new MenuSayfaItem("1-Ürün/Palet Ekle", "UrunEkle"),
            new MenuSayfaItem("2-Dizilim Oluştur", "DizilimYap"),
            new MenuSayfaItem("3-Hız Ayarları", "HizAyarlari"),
            new MenuSayfaItem("4-Gruplama Oluştur", "GruplamaYap"),
            new MenuSayfaItem("5-Programlar", "Programlar"),
            new MenuSayfaItem("6-Alarmlar", "Alarmlar"),
            new MenuSayfaItem("7-İzleme", "Izleme"),
            new MenuSayfaItem("8-Komut", "Komut")
        };

      

        private string _aktifSayfa = "UrunEkle";

        public string AktifSayfa
        {
            get => _aktifSayfa;
            set => _aktifSayfa = value;
        }

        public UstMenu()
        {
            InitializeComponent();
            Loaded += UstMenu_Loaded;
        }

        private void UstMenu_Loaded(object sender, RoutedEventArgs e)
        {
            MenuyuHazirla();
        }

        private void MenuyuHazirla()
        {
            int aktifIndex = _sayfalar.FindIndex(x => x.SayfaKodu == AktifSayfa);
            if (aktifIndex < 0)
                aktifIndex = 0;

            MenuSayfaItem onceki = aktifIndex > 0 ? _sayfalar[aktifIndex - 1] : null;
            MenuSayfaItem mevcut = _sayfalar[aktifIndex];
            MenuSayfaItem sonraki = aktifIndex < _sayfalar.Count - 1 ? _sayfalar[aktifIndex + 1] : null;

            BtnOnceki.Content = onceki?.Baslik ?? "";
            BtnOnceki.Tag = onceki?.SayfaKodu;
            BtnOnceki.Visibility = onceki == null ? Visibility.Hidden : Visibility.Visible;

            BtnMevcut.Content = mevcut.Baslik;
            BtnMevcut.Tag = mevcut.SayfaKodu;
            BtnMevcut.Visibility = Visibility.Visible;

            BtnSonraki.Content = sonraki?.Baslik ?? "";
            BtnSonraki.Tag = sonraki?.SayfaKodu;
            BtnSonraki.Visibility = sonraki == null ? Visibility.Hidden : Visibility.Visible;

            BtnOnceki.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CFCFCF"));
            BtnOnceki.FontWeight = FontWeights.Normal;
            BtnOnceki.FontSize = 22;

            BtnMevcut.Foreground = Brushes.White;
            BtnMevcut.FontWeight = FontWeights.Bold;
            BtnMevcut.FontSize = 26;

            BtnSonraki.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CFCFCF"));
            BtnSonraki.FontWeight = FontWeights.Normal;
            BtnSonraki.FontSize = 22;
        }

       

       

        private void BtnOnceki_Click(object sender, RoutedEventArgs e)
        {
            if (BtnOnceki.Tag is not string sayfaKodu)
                return;

            Page hedefSayfa = HedefSayfayiHazirla(sayfaKodu);
            if (hedefSayfa == null)
                return;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
                return;

            mainWindow.SayfayaKayarakGit(hedefSayfa, true);
        }

        private void BtnSonraki_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSonraki.Tag is not string sayfaKodu)
                return;

            Page hedefSayfa = HedefSayfayiHazirla(sayfaKodu);
            if (hedefSayfa == null)
                return;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
                return;

            mainWindow.SayfayaKayarakGit(hedefSayfa, false);
        }

        private Page HedefSayfayiHazirla(string sayfaKodu)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
                return null;

            switch (sayfaKodu)
            {
                case "UrunEkle":
                    return new UrunEkle(mainWindow.MainFrame);

                case "DizilimYap":
                    {
                        var urunler = mainWindow.UrunIslemler.UrunListesiniGetir();
                        var paletler = mainWindow.PaletIslemler.PaletListesiniGetir();

                        var secimKutusu = new UrunPaletSecimKutusu(urunler, paletler, "( Dizilim zorunlu değil.)");
                        bool? sonuc = secimKutusu.ShowDialog();

                        if (sonuc == true &&
                            secimKutusu.SecilenUrun != null &&
                            secimKutusu.SecilenPalet != null)
                        {
                            return new DizilimYap(
                                mainWindow.MainFrame,
                                secimKutusu.SecilenUrun,
                                secimKutusu.SecilenPalet,
                                secimKutusu.SecilenDizilimAdi);
                        }

                        return null;
                    }

                case "HizAyarlari":
                    return new HizAyarları(mainWindow.MainFrame);

                case "GruplamaYap":
                    {
                        var urunler = mainWindow.UrunIslemler.UrunListesiniGetir();
                        var paletler = mainWindow.PaletIslemler.PaletListesiniGetir();

                        var secimKutusu = new UrunPaletSecimKutusu(urunler, paletler, "(* Zorunlu)");
                        bool? sonuc = secimKutusu.ShowDialog();

                        if (sonuc == true &&
                            secimKutusu.SecilenUrun != null &&
                            secimKutusu.SecilenPalet != null)
                        {
                            return new GruplamaYap(
                                mainWindow.MainFrame,
                                secimKutusu.SecilenUrun,
                                secimKutusu.SecilenPalet,
                                secimKutusu.SecilenDizilimAdi);
                        }

                        return null;
                    }

                case "Programlar":
                    return new Programlar(mainWindow.MainFrame);

                default:
                    return null;
            }
        }


    
        private void AnasayfaBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
                mainWindow.SayfayaGit(new Anasayfa(mainWindow.MainFrame));
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
                mainWindow.SayfayaGit(new Kullanici(mainWindow.MainFrame));
        }
    }

    public class MenuSayfaItem
    {
        public string Baslik { get; set; }
        public string SayfaKodu { get; set; }

        public MenuSayfaItem(string baslik, string sayfaKodu)
        {
            Baslik = baslik;
            SayfaKodu = sayfaKodu;
        }
    }
}