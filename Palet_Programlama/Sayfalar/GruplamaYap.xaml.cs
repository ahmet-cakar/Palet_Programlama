using Palet_Programlama.Modeller;
using Palet_Programlama.Servisler.PaletMethod;
using Palet_Programlama.Sınıflar;
using Palet_Programlama.Sayfalar.Gruplama.Helpers;
using Palet_Programlama.Sayfalar.Gruplama.Services;
using Servisler.PaletMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for GruplamaYap.xaml
    /// </summary>
    public partial class GruplamaYap : Page
    {
        private readonly Frame MainFrame;

        private readonly KatYoneticisi _katYonetici = new();
        private readonly MesafeGostergesi _mesafe = new();

        private readonly KoliGeometriYardimcisi _geometri = new();
        private readonly KoliYonYardimcisi _yonYardimcisi = new();
        private readonly DizilimKayitServisi _dizilimKayitServisi = new();
        private readonly GruplamaBilgiServisi _bilgiServisi = new();
        private readonly GruplamaSecimServisi _secimServisi = new();
        private readonly GrupGorsellestirmeServisi _gorsellestirmeServisi = new();
        private readonly GrupKuralServisi _kuralServisi = new();

        private Urun _secilenUrun;
        private Palet _secilenPalet;
        private string _gelenDizilimAdi;

        private List<DizilimKayitModel> _dizilimKayitlari = new();
        private readonly List<Rect> _tiklananDigerKutular = new();

        private Rectangle sonSecilmisKutu = new Rectangle();
        private bool _sayfaYukleniyor;

        private const int BirGruptakiMaksimumKoliSayisi = 4;
        private const double GrupHizalamaToleransi = 5.0;
        private const double GrupKomsulukToleransi = 5.0;

        private double OlcekY => myCanvas.Width / _secilenPalet.PaletBoy;
        private double OlcekX => myCanvas.Height / _secilenPalet.PaletEn;

        private int AktifKatNo => _katYonetici.AktifKat;

        private int AktifGrupNo
        {
            get
            {
                if (int.TryParse(txtGrupValue.Text, out int grupNo) && grupNo > 0)
                    return grupNo;

                return 1;
            }
        }

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

            _dizilimKayitlari = _dizilimKayitServisi.KayitlariYukle();

            IlkVerileriHazirla();
            ComboBoxlariDoldurIlkAcilisIcin();

            _sayfaYukleniyor = false;

            SeciliDizilimiCanvasaYukle();
        }

        private void IlkVerileriHazirla()
        {
            if (string.IsNullOrWhiteSpace(_gelenDizilimAdi))
            {
                txtPaletOzellikleri.Text = _bilgiServisi.PaletMetniUret(_secilenPalet);
                return;
            }

            var gelenKayit = _dizilimKayitServisi.GelenKaydiBul(
                _dizilimKayitlari,
                _secilenUrun?.UrunAdi ?? "",
                _gelenDizilimAdi ?? "");

            if (gelenKayit == null)
            {
                txtPaletOzellikleri.Text = _bilgiServisi.PaletMetniUret(_secilenPalet);
                return;
            }

            KayitBilgileriniUygula(gelenKayit);
        }

        private void ComboBoxlariDoldurIlkAcilisIcin()
        {
            UrunleriDoldur();
            UrunSeciminiUygula();

            DizilimleriDoldur(_secilenUrun?.UrunAdi ?? "");
            DizilimSeciminiUygula();
        }

        private void UrunleriDoldur()
        {
            CboxUrunListesi.ItemsSource = _dizilimKayitServisi.UrunAdlariniGetir(_dizilimKayitlari);
        }

        private void DizilimleriDoldur(string urunAdi)
        {
            var dizilimAdlari = _dizilimKayitServisi.UruneGoreDizilimAdlariniGetir(_dizilimKayitlari, urunAdi);
            CboxDizilimListesi.ItemsSource = dizilimAdlari.Any() ? dizilimAdlari : null;
        }

        private void UrunSeciminiUygula()
        {
            var urunAdlari = CboxUrunListesi.ItemsSource as IEnumerable<string>;
            if (urunAdlari == null)
                return;

            if (!string.IsNullOrWhiteSpace(_secilenUrun?.UrunAdi) &&
                urunAdlari.Contains(_secilenUrun.UrunAdi))
            {
                CboxUrunListesi.SelectedItem = _secilenUrun.UrunAdi;
                return;
            }

            if (CboxUrunListesi.Items.Count > 0)
                CboxUrunListesi.SelectedIndex = 0;
        }

        private void DizilimSeciminiUygula()
        {
            var dizilimler = CboxDizilimListesi.ItemsSource as IEnumerable<string>;
            if (dizilimler == null)
                return;

            if (!string.IsNullOrWhiteSpace(_gelenDizilimAdi) &&
                dizilimler.Contains(_gelenDizilimAdi))
            {
                CboxDizilimListesi.SelectedItem = _gelenDizilimAdi;
                return;
            }

            if (CboxDizilimListesi.Items.Count > 0)
                CboxDizilimListesi.SelectedIndex = 0;
        }

        private DizilimKayitModel SeciliKaydiGetir()
        {
            return _dizilimKayitServisi.KayitBul(
                _dizilimKayitlari,
                CboxUrunListesi.SelectedItem?.ToString() ?? "",
                CboxDizilimListesi.SelectedItem?.ToString() ?? "");
        }

        private void KayitBilgileriniUygula(DizilimKayitModel kayit)
        {
            _gelenDizilimAdi = kayit.DizilimAdi;

            _bilgiServisi.UrunBilgisiniUygula(_secilenUrun, kayit);
            _bilgiServisi.PaletBilgisiniUygula(_secilenPalet, kayit);

            txtPaletOzellikleri.Text = _bilgiServisi.PaletMetniUret(kayit);
        }

        private void SeciliDizilimiCanvasaYukle()
        {
            var kayit = SeciliKaydiGetir();

            if (kayit == null)
            {
                CanvasiTemizle();
                return;
            }

            KayitBilgileriniUygula(kayit);

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

            GrupGorselleriniYenile();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle tiklananKutu)
                return;

            int aktifGrupNo = AktifGrupNo;
            var tumKutular = myCanvas.Children.OfType<Rectangle>();

            if (_secimServisi.KoliBuAktifGruptaMi(tiklananKutu, AktifKatNo, aktifGrupNo))
            {
                _secimServisi.KolininGrubunuKaldir(tiklananKutu);

                _secimServisi.GruptakiTumKutularinEksenBilgisiniGuncelle(
                    tumKutular,
                    AktifKatNo,
                    aktifGrupNo,
                    _geometri);

                _gorsellestirmeServisi.KutuGrupGorseliniGuncelle(
                    myCanvas,
                    tiklananKutu,
                    AktifKatNo,
                    _secimServisi.GrupAtamalari,
                    _geometri);
                return;
            }

            if (!_kuralServisi.GrupYonKuraliUygunMu(
                tiklananKutu,
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                _secimServisi,
                _yonYardimcisi))
            {
                GrupKuraliUyarisiGoster("Aynı grupta yatay ve dikey ürün birlikte bulunamaz.");
                return;
            }

            if (!_kuralServisi.GrupMaksimumKoliKuralinaUygunMu(
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                BirGruptakiMaksimumKoliSayisi,
                _secimServisi,
                tiklananKutu))
            {
                GrupKuraliUyarisiGoster("Aynı grupta en fazla 4 koli olabilir.");
                return;
            }

            if (!_kuralServisi.GrupEksenKuraliUygunMu(
                tiklananKutu,
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                GrupHizalamaToleransi,
                _secimServisi,
                _geometri))
            {
                GrupKuraliUyarisiGoster("Bir grup yalnızca tek yönde büyüyebilir. Aynı gruba hem yatay hem dikey doğrultuda ürün eklenemez.");
                return;
            }

            if (!_kuralServisi.GrupKomsulukKuralinaUygunMu(
                tiklananKutu,
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                GrupHizalamaToleransi,
                GrupKomsulukToleransi,
                _secimServisi,
                _geometri))
            {
                GrupKuraliUyarisiGoster("Aynı gruptaki ürünler bitişik olmalıdır. Aradaki ürünü atlayarak gruplama yapılamaz.");
                return;
            }

            _secimServisi.KoliyaGrupAta(
                tiklananKutu,
                AktifKatNo,
                aktifGrupNo,
                tumKutular,
                _geometri);

            _secimServisi.GruptakiTumKutularinEksenBilgisiniGuncelle(
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                _geometri);

            _gorsellestirmeServisi.KutuGrupGorseliniGuncelle(
                myCanvas,
                tiklananKutu,
                AktifKatNo,
                _secimServisi.GrupAtamalari,
                _geometri);
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            // Gruplama sayfasında sürükleme yok
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Gruplama sayfasında seçim MouseDown'da yönetiliyor
        }

        private void CboxUrunListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sayfaYukleniyor || CboxUrunListesi.SelectedItem == null)
                return;

            string seciliUrunAdi = CboxUrunListesi.SelectedItem.ToString();

            var urunKaydi = _dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), seciliUrunAdi.Trim(), StringComparison.OrdinalIgnoreCase));

            if (urunKaydi != null)
                _bilgiServisi.UrunBilgisiniUygula(_secilenUrun, urunKaydi);

            _gelenDizilimAdi = null;

            _sayfaYukleniyor = true;
            DizilimleriDoldur(seciliUrunAdi);
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
            if (_sayfaYukleniyor || CboxDizilimListesi.SelectedItem == null)
                return;

            _gelenDizilimAdi = CboxDizilimListesi.SelectedItem.ToString();
            SeciliDizilimiCanvasaYukle();
        }

        private void BtnKatEksi_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtKatValue.Text) == 1)
                return;

            KataGec(_katYonetici.AktifKat - 1);
        }

        private void BtnKatArti_Click(object sender, RoutedEventArgs e)
        {
            KataGec(_katYonetici.AktifKat + 1);
        }

        private void KataGec(int yeniKat)
        {
            SecimiTemizle();

            _katYonetici.KatDegistir(
                yeniKat,
                myCanvas,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
            GrupGorselleriniYenile();
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

        private void SecimiTemizle()
        {
            _tiklananDigerKutular.Clear();
            _mesafe.Gizle();
        }

        private void GrupGorselleriniYenile()
        {
            _gorsellestirmeServisi.AktifKattakiGrupEtiketleriniYenile(
                myCanvas,
                AktifKatNo,
                _secimServisi.GrupAtamalari,
                _geometri);

            _gorsellestirmeServisi.AktifKattakiTumKutuGorselleriniYenile(
                myCanvas,
                AktifKatNo,
                _secimServisi.GrupAtamalari,
                _geometri);
        }

        private void CanvasiTemizle()
        {
            myCanvas.Children.OfType<Rectangle>().ToList().ForEach(r => myCanvas.Children.Remove(r));

            _katYonetici.Temizle();
            _secimServisi.TumGrupAtamalariniTemizle();
            SecimiTemizle();

            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(myCanvas);

            txtKatValue.Text = "1";
        }

        private void GrupKuraliUyarisiGoster(string mesaj)
        {
            MessageBox.Show(mesaj, "Grup Kuralı", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}