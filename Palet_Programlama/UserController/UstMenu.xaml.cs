using Palet_Programlama.Screens;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Palet_Programlama.Languages;

namespace Palet_Programlama.UserController
{
    public partial class UstMenu : UserControl
    {
        private readonly List<MenuSayfaItem> _sayfalar = new();

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
            _sayfalar.Add(new MenuSayfaItem($"1-{LanguageConverter.GetString("UserControl.urunekle")}", "UrunEkle"));
            _sayfalar.Add(new MenuSayfaItem($"2-{LanguageConverter.GetString("UserControl.dizilimekle")}", "DizilimYap"));
            _sayfalar.Add(new MenuSayfaItem($"3-{LanguageConverter.GetString("UserControl.grupekle")}", "GruplamaYap"));
            _sayfalar.Add(new MenuSayfaItem($"4-{LanguageConverter.GetString("UserControl.program")}", "Programlar"));
            _sayfalar.Add(new MenuSayfaItem($"5-{LanguageConverter.GetString("UserControl.hizayar")}", "HizAyarlari"));
            _sayfalar.Add(new MenuSayfaItem($"6-{LanguageConverter.GetString("UserControl.alarm")}", "Alarmlar"));
            _sayfalar.Add(new MenuSayfaItem($"7-{LanguageConverter.GetString("UserControl.izle")}", "Izleme"));
            _sayfalar.Add(new MenuSayfaItem($"8-{LanguageConverter.GetString("UserControl.ayarlar")}", "Komut"));
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
                    return new UrunEkle();

                case "DizilimYap":
                    {
                        var urunler = mainWindow.UrunIslemler.UrunListesiniGetir();
                        var paletler = mainWindow.PaletIslemler.PaletListesiniGetir();

                        var secimKutusu = new UrunPaletSecimKutusu(
                        urunler,
                        paletler,
                        LanguageConverter.GetString("UrunPaletSecimPopup.dizilimZorunluDegil"));
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

                        var secimKutusu = new UrunPaletSecimKutusu(
                        urunler,
                        paletler,
                        LanguageConverter.GetString("UrunPaletSecimPopup.zorunluAlan"));
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