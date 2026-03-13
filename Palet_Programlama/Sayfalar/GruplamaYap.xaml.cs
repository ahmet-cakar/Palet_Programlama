using Newtonsoft.Json;
using Palet_Programlama.Modeller;
using Palet_Programlama.Servisler.PaletMethod;
using Palet_Programlama.Sınıflar;
using Servisler.PaletMethod;
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

        private readonly KatYoneticisi _katYonetici = new();
        private readonly MesafeGostergesi _mesafe = new();

        private List<DizilimKayitModel> _dizilimKayitlari = new();

        private Rectangle suruklenenKutu;
        private Rectangle sonSecilmisKutu = new Rectangle();
        private List<Rect> _tiklananDigerKutular = new();

        private Rect _sonSeciliRect;
        private bool _sonSeciliRectVar = false;
        private bool _sayfaYukleniyor = false;

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

            SayfaVerileriniYukle();
        }

        private void SayfaVerileriniYukle()
        {
            _sayfaYukleniyor = true;

            DizilimKayitlariniYukle();
            IlkVerileriHazirla();
            ComboBoxlariDoldurIlkAcılısIcin();

            _sayfaYukleniyor = false;

            SeciliDizilimiCanvasaYukle();
        }

        private void IlkVerileriHazirla()
        {
            if (string.IsNullOrWhiteSpace(_gelenDizilimAdi))
            {
                PaletMetniniGuncelle(_secilenPalet);
                return;
            }

            var gelenKayit = _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.DizilimAdi ?? "").Trim(), _gelenDizilimAdi.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals((x.UrunAdi ?? "").Trim(), (_secilenUrun.UrunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

            if (gelenKayit == null)
            {
                PaletMetniniGuncelle(_secilenPalet);
                return;
            }

            UrunBilgisiniUygula(gelenKayit);
            PaletBilgisiniUygula(gelenKayit);
            PaletBilgisiniGuncelle(gelenKayit);
        }

        private void ComboBoxlariDoldurIlkAcılısIcin()
        {
            UrunComboBoxDoldur();
            DizilimleriSeciliUruneGoreDoldur();

            if (!string.IsNullOrWhiteSpace(_secilenUrun?.UrunAdi))
                CboxUrunListesi.SelectedItem = _secilenUrun.UrunAdi;

            if (!string.IsNullOrWhiteSpace(_gelenDizilimAdi) &&
                CboxDizilimListesi.ItemsSource is IEnumerable<string> dizilimler &&
                dizilimler.Contains(_gelenDizilimAdi))
            {
                CboxDizilimListesi.SelectedItem = _gelenDizilimAdi;
            }
            else if (CboxDizilimListesi.Items.Count > 0)
            {
                CboxDizilimListesi.SelectedIndex = 0;
            }
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
            catch
            {
                _dizilimKayitlari = new List<DizilimKayitModel>();
            }
        }

        private void UrunComboBoxDoldur()
        {
            var urunAdlari = _dizilimKayitlari
                .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi))
                .Select(x => x.UrunAdi)
                .Distinct()
                .ToList();

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

        private void DizilimleriSeciliUruneGoreDoldur()
        {
            string seciliUrunAdi = _secilenUrun?.UrunAdi ?? "";

            var dizilimAdlari = _dizilimKayitlari
                .Where(x => string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase))
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
            else
            {
                CboxDizilimListesi.ItemsSource = null;
            }
        }

        private void CboxUrunListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sayfaYukleniyor)
                return;

            if (CboxUrunListesi.SelectedItem == null)
                return;

            string seciliUrunAdi = CboxUrunListesi.SelectedItem.ToString();

            var urunKaydi = _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase));

            if (urunKaydi != null)
            {
                UrunBilgisiniUygula(urunKaydi);
            }

            _gelenDizilimAdi = null;

            _sayfaYukleniyor = true;
            DizilimleriSeciliUruneGoreDoldur();
            _sayfaYukleniyor = false;

            if (CboxDizilimListesi.Items.Count > 0)
            {
                CboxDizilimListesi.SelectedIndex = 0;
            }
            else
            {
                CanvasiTemizle();
            }
        }

        private void CboxDizilimListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sayfaYukleniyor)
                return;

            if (CboxDizilimListesi.SelectedItem == null)
                return;

            _gelenDizilimAdi = CboxDizilimListesi.SelectedItem.ToString();
            SeciliDizilimiCanvasaYukle();
        }

        private void SeciliDizilimiCanvasaYukle()
        {
            var kayit = SeciliDizilimKaydiniGetir();

            if (kayit == null)
            {
                CanvasiTemizle();
                return;
            }

            _gelenDizilimAdi = kayit.DizilimAdi;

            UrunBilgisiniUygula(kayit);
            PaletBilgisiniUygula(kayit);
            PaletBilgisiniGuncelle(kayit);

            bool yuklendi = _katYonetici.DizilimYukle(
                kayit.DizilimAdi,
                _secilenUrun,
                _secilenPalet,
                OlcekX,
                OlcekY);

            if (!yuklendi)
            {
                CanvasiTemizle();
                return;
            }

            SecimiTemizle();

            _katYonetici.KatiYukleDisardan(
                myCanvas,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
        }

        private DizilimKayitModel SeciliDizilimKaydiniGetir()
        {
            string seciliDizilimAdi = CboxDizilimListesi.SelectedItem?.ToString() ?? "";
            string seciliUrunAdi = CboxUrunListesi.SelectedItem?.ToString() ?? "";

            return _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.DizilimAdi ?? "").Trim(), seciliDizilimAdi.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private void UrunBilgisiniUygula(DizilimKayitModel kayit)
        {
            _secilenUrun.UrunAdi = kayit.UrunAdi;
            _secilenUrun.UrunEn = kayit.UrunEn;
            _secilenUrun.UrunBoy = kayit.UrunBoy;
            _secilenUrun.UrunYukseklik = kayit.UrunYukseklik;
        }

        private void PaletBilgisiniUygula(DizilimKayitModel kayit)
        {
            _secilenPalet.PaletAdi = kayit.PaletAdi;
            _secilenPalet.PaletEn = kayit.PaletEn;
            _secilenPalet.PaletBoy = kayit.PaletBoy;
            _secilenPalet.PaletYukseklik = kayit.PaletYukseklik;
        }

        private void PaletBilgisiniGuncelle(DizilimKayitModel kayit)
        {
            if (kayit == null)
                return;

            txtPaletOzellikleri.Text =
                $"{kayit.PaletAdi} - {kayit.PaletEn:0} mm X {kayit.PaletBoy:0} mm X {kayit.PaletYukseklik:0} mm";
        }

        private void PaletMetniniGuncelle(Palet palet)
        {
            if (palet == null)
                return;

            txtPaletOzellikleri.Text =
                $"{palet.PaletAdi} - {palet.PaletEn:0} mm X {palet.PaletBoy:0} mm X {palet.PaletYukseklik:0} mm";
        }

        private void CanvasiTemizle()
        {
            myCanvas.Children.OfType<Rectangle>().ToList().ForEach(r => myCanvas.Children.Remove(r));
            _katYonetici.Temizle();
            SecimiTemizle();
            txtKatValue.Text = "1";
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

        private List<Rect> DigerKutular(Rectangle hareketEden)
        {
            return myCanvas.Children
                .OfType<Rectangle>()
                .Where(r => r != hareketEden)
                .Select(GetRect)
                .ToList();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle tiklananKutu)
                return;

            suruklenenKutu = tiklananKutu;

            if (suruklenenKutu == sonSecilmisKutu && sonSecilmisKutu.StrokeThickness == 1)
            {
                sonSecilmisKutu.Stroke = Brushes.Transparent;
                sonSecilmisKutu.StrokeThickness = 0;
                _mesafe.Gizle();

                sonSecilmisKutu = new Rectangle();
                _sonSeciliRectVar = false;
                return;
            }

            Rect seciliRect = GetRect(suruklenenKutu);

            if (sonSecilmisKutu == suruklenenKutu &&
                _sonSeciliRectVar &&
                Math.Abs(seciliRect.Left - _sonSeciliRect.Left) < 0.1 &&
                Math.Abs(seciliRect.Top - _sonSeciliRect.Top) < 0.1)
            {
                _mesafe.Goster();
                return;
            }

            if (sonSecilmisKutu != null)
            {
                sonSecilmisKutu.Stroke = Brushes.Transparent;
                sonSecilmisKutu.StrokeThickness = 0;
            }

            suruklenenKutu.Stroke = Brushes.Red;
            suruklenenKutu.StrokeThickness = 1;

            _tiklananDigerKutular = DigerKutular(suruklenenKutu);
            _mesafe.Guncelle(seciliRect, _tiklananDigerKutular);
            _mesafe.Goster();

            sonSecilmisKutu = suruklenenKutu;
            _sonSeciliRect = seciliRect;
            _sonSeciliRectVar = true;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            // Gruplama sayfasında sürükleme yok
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Gruplama sayfasında seçim MouseDown'da yönetiliyor
        }

        private void SecimiTemizle()
        {
            if (sonSecilmisKutu != null)
            {
                sonSecilmisKutu.Stroke = Brushes.Transparent;
                sonSecilmisKutu.StrokeThickness = 0;
            }

            sonSecilmisKutu = new Rectangle();
            suruklenenKutu = null;
            _tiklananDigerKutular.Clear();
            _sonSeciliRectVar = false;
            _mesafe.Gizle();
        }

        private void BtnKatEksi_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtKatValue.Text) == 1)
                return;

            SecimiTemizle();

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

        private void BtnKatArti_Click(object sender, RoutedEventArgs e)
        {
            SecimiTemizle();

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
    }
}