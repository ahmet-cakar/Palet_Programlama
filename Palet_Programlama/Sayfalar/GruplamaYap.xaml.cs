using Newtonsoft.Json;
using Palet_Programlama.Modeller;
using Palet_Programlama.Sınıflar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for GruplamaYap.xaml
    /// </summary>
    public partial class GruplamaYap : Page
    {
        private readonly Frame MainFrame;
        private Urun _secilenUrun;
        private Palet _secilenPalet;
        private string _gelenDizilimAdi;
        private readonly Servisler.Palet.KatYoneticisi _katYonetici = new();
        private List<DizilimKayitModel> _dizilimKayitlari = new();
        private readonly Servisler.Palet.MesafeGostergesi _mesafe = new();
        private Rectangle suruklenenKutu;
        private Rectangle sonSecilmisKutu = new Rectangle();
        private double OlcekY => myCanvas.Width / _secilenPalet.PaletBoy;
        private double OlcekX => myCanvas.Height / _secilenPalet.PaletEn;

        public GruplamaYap(Frame Main, Urun secilenUrun, Palet secilenPalet, string dizilimAdi)
        {
            InitializeComponent();
            MainFrame = Main;
            _secilenUrun = secilenUrun;
            _secilenPalet = secilenPalet;
            _gelenDizilimAdi = dizilimAdi;
            _mesafe.Baslat(myCanvas);
            txtPaletOzellikleri.Text =
                $"{_secilenPalet.PaletAdi} - {_secilenPalet.PaletEn:0} mm X {_secilenPalet.PaletBoy:0} mm X {_secilenPalet.PaletYukseklik:0} mm";

            SayfaVerileriniYukle();
        }

        private void SayfaVerileriniYukle()
        {
            DizilimKayitlariniYukle();
            DizilimComboBoxDoldur();
            UrunComboBoxDoldur();
            GelenDizilimiCanvasaYukle();
        }


        private Rect GetRect(Rectangle r)
        {
            double left = Canvas.GetLeft(r);
            double top = Canvas.GetTop(r);

            double w = (r.ActualWidth > 0) ? r.ActualWidth : r.Width;
            double h = (r.ActualHeight > 0) ? r.ActualHeight : r.Height;

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            return new Rect(left, top, w, h);
        }

        private List<Rect> DigerKutular(Rectangle hareketEden) =>
            myCanvas.Children.OfType<Rectangle>()
                .Where(r => r != hareketEden)
                .Select(GetRect)
                .ToList();

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            suruklenenKutu = (Rectangle)sender;
        }



        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            // Gruplama sayfasında sürükleme yok
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (suruklenenKutu == null)
                return;

            if (suruklenenKutu.StrokeThickness == 1)
            {
                suruklenenKutu.Stroke = Brushes.Transparent;
                suruklenenKutu.StrokeThickness = 0;
                _mesafe.Gizle();
            }
            else
            {
                var digerKutular = DigerKutular(suruklenenKutu);

                sonSecilmisKutu.Stroke = Brushes.Transparent;
                sonSecilmisKutu.StrokeThickness = 0;

                suruklenenKutu.Stroke = Brushes.Red;
                suruklenenKutu.StrokeThickness = 1;

                _mesafe.Guncelle(GetRect(suruklenenKutu), digerKutular);
                _mesafe.Goster();
            }

            sonSecilmisKutu = suruklenenKutu;
        }


        private void GelenDizilimiCanvasaYukle()
        {
            if (string.IsNullOrWhiteSpace(_gelenDizilimAdi))
                return;

            bool yuklendi = _katYonetici.DizilimYukle(
                _gelenDizilimAdi,
                _secilenUrun,
                _secilenPalet,
                OlcekX,
                OlcekY);

            if (!yuklendi)
                return;

            _katYonetici.KatiYukleDisardan(
                myCanvas,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
        }



        private void DizilimKayitlariniYukle()
        {
            try
            {
                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                if (!File.Exists(dosyaYolu))
                {
                    _dizilimKayitlari = new List<DizilimKayitModel>();
                    return;
                }

                string json = File.ReadAllText(dosyaYolu);

                _dizilimKayitlari = JsonConvert.DeserializeObject<List<DizilimKayitModel>>(json)
                                    ?? new List<DizilimKayitModel>();
            }
            catch (Exception)
            {
                _dizilimKayitlari = new List<DizilimKayitModel>();
            }
        }

        private void DizilimComboBoxDoldur()
        {
            var dizilimAdlari = _dizilimKayitlari
                .Where(x => !string.IsNullOrWhiteSpace(x.DizilimAdi))
                .Select(x => x.DizilimAdi)
                .Distinct()
                .ToList();

            CboxDizilimListesi.ItemsSource = dizilimAdlari;

            if (!string.IsNullOrWhiteSpace(_gelenDizilimAdi) && dizilimAdlari.Contains(_gelenDizilimAdi))
            {
                CboxDizilimListesi.SelectedItem = _gelenDizilimAdi;
            }
            else if (dizilimAdlari.Any())
            {
                CboxDizilimListesi.SelectedIndex = 0;
            }
        }

        private void UrunComboBoxDoldur()
        {
            var urunAdlari = _dizilimKayitlari
                .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi))
                .Select(x => x.UrunAdi)
                .Distinct()
                .ToList();

            if (_secilenUrun != null &&
                !string.IsNullOrWhiteSpace(_secilenUrun.UrunAdi) &&
                !urunAdlari.Contains(_secilenUrun.UrunAdi))
            {
                urunAdlari.Insert(0, _secilenUrun.UrunAdi);
            }

            CboxUrunListesi.ItemsSource = urunAdlari;

            if (_secilenUrun != null &&
                !string.IsNullOrWhiteSpace(_secilenUrun.UrunAdi) &&
                urunAdlari.Contains(_secilenUrun.UrunAdi))
            {
                CboxUrunListesi.SelectedItem = _secilenUrun.UrunAdi;
            }
            else if (urunAdlari.Any())
            {
                CboxUrunListesi.SelectedIndex = 0;
            }
        }

        private void BtnGruplandirmaEksi_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtGrupValue.Text) != 1)
            {
                txtGrupValue.Text = (Convert.ToInt32(txtGrupValue.Text) - 1).ToString();
            }
        }

        private void BtnGruplandirmaArti_Click(object sender, RoutedEventArgs e)
        {
            txtGrupValue.Text = (Convert.ToInt32(txtGrupValue.Text) + 1).ToString();
        }

        private void BtnKatEksi_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtKatValue.Text) != 1)
            {
                _mesafe.Gizle();

                int yeniKat = _katYonetici.AktifKat - 1;

                _katYonetici.KatDegistir(
                    yeniKat,
                    myCanvas,
                    sonSecilmisKutu,
                    Rectangle_MouseDown,
                    Rectangle_MouseMove,
                    Rectangle_MouseUp);

                txtKatValue.Text = _katYonetici.AktifKat.ToString();
            }
        }


        private void BtnKatArti_Click(object sender, RoutedEventArgs e)
        {
            _mesafe.Gizle();

            int yeniKat = _katYonetici.AktifKat + 1;

            _katYonetici.KatDegistir(
                yeniKat,
                myCanvas,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
        }

        private void CboxUrunListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboxUrunListesi.SelectedItem == null)
                return;

            string seciliUrunAdi = CboxUrunListesi.SelectedItem.ToString();

            var urunKaydi = _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase));

            if (urunKaydi != null)
            {
                _secilenUrun.UrunAdi = urunKaydi.UrunAdi;
                _secilenUrun.UrunEn = urunKaydi.UrunEn;
                _secilenUrun.UrunBoy = urunKaydi.UrunBoy;
                _secilenUrun.UrunYukseklik = urunKaydi.UrunYukseklik;
            }

            SeciliUruneGoreDizilimleriDoldur();
        }

        private void CboxDizilimListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboxDizilimListesi.SelectedItem == null)
                return;

            SeciliDizilimiCanvasaYukle();
        }


        private void SeciliDizilimiCanvasaYukle()
        {
            var kayit = SeciliDizilimKaydiniGetir();
            if (kayit == null)
            {
                myCanvas.Children.Clear();
                _katYonetici.Temizle();
                txtKatValue.Text = "1";
                return;
            }

            _secilenPalet.PaletAdi = kayit.PaletAdi;
            _secilenPalet.PaletEn = kayit.PaletEn;
            _secilenPalet.PaletBoy = kayit.PaletBoy;
            _secilenPalet.PaletYukseklik = kayit.PaletYukseklik;

            PaletBilgisiniGuncelle(kayit);

            bool yuklendi = _katYonetici.DizilimYukle(
                kayit.DizilimAdi,
                _secilenUrun,
                _secilenPalet,
                OlcekX,
                OlcekY);

            if (!yuklendi)
            {
                myCanvas.Children.Clear();
                _katYonetici.Temizle();
                txtKatValue.Text = "1";
                return;
            }

            sonSecilmisKutu = new Rectangle();
            _mesafe.Gizle();

            _katYonetici.KatiYukleDisardan(
                myCanvas,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
        }

        private void SeciliUruneGoreDizilimleriDoldur()
        {
            string seciliUrunAdi = CboxUrunListesi.SelectedItem?.ToString() ?? "";

            var dizilimAdlari = _dizilimKayitlari
                .Where(x => string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(x => !string.IsNullOrWhiteSpace(x.DizilimAdi))
                .Select(x => x.DizilimAdi)
                .Distinct()
                .ToList();

            CboxDizilimListesi.ItemsSource = dizilimAdlari;

            if (dizilimAdlari.Any())
                CboxDizilimListesi.SelectedIndex = 0;
            else
                CboxDizilimListesi.ItemsSource = null;
        }


        private void PaletBilgisiniGuncelle(DizilimKayitModel kayit)
        {
            if (kayit == null)
                return;

            txtPaletOzellikleri.Text =
                $"{kayit.PaletAdi} - {kayit.PaletEn:0} mm X {kayit.PaletBoy:0} mm X {kayit.PaletYukseklik:0} mm";
        }

        private DizilimKayitModel SeciliDizilimKaydiniGetir()
        {
            string seciliDizilimAdi = CboxDizilimListesi.SelectedItem?.ToString() ?? "";
            string seciliUrunAdi = CboxUrunListesi.SelectedItem?.ToString() ?? "";

            return _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.DizilimAdi ?? "").Trim(), seciliDizilimAdi.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}