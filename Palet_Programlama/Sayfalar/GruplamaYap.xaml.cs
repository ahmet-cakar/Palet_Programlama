using Palet_Programlama.Modeller;
using Palet_Programlama.Popuplar;
using Palet_Programlama.Sayfalar.Gruplama.Helpers;
using Palet_Programlama.Sayfalar.Gruplama.Models;
using Palet_Programlama.Sayfalar.Gruplama.Services;
using Palet_Programlama.Servisler.PaletMethod;
using Palet_Programlama.Sınıflar;
using Palet_Programlama.UserController;
using Servisler.PaletMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

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
        private readonly ProgramKayitServisi _programKayitServisi = new();
        private readonly ObservableCollection<GruplamaListeItemModel> _gruplamaListesi = new();

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
            ListBoxGruplama.ItemsSource = _gruplamaListesi;
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
                BildirimGoster("GruplamaYap.ayniGrupYatayDikeyUyarisi");
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
                BildirimGoster("GruplamaYap.ayniGrupMaksSayiUyarisi");
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
                BildirimGoster("GruplamaYap.ayniGrupYonUyarisi");
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
                BildirimGoster("GruplamaYap.ayniGrupBitisikUyarisi");

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
            MevcutKatGruplariniKaydet();
            SecimiTemizle();

            _katYonetici.KatDegistir(
                yeniKat,
                myCanvas,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);


            var kayitliAtamalar = _bilgiServisi.KatAtamalariniGetir(_katYonetici.AktifKat);

            _secimServisi.KatAtamalariniYukle(
                _katYonetici.AktifKat,
                myCanvas.Children.OfType<Rectangle>(),
                kayitliAtamalar,
                _geometri);

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

        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void BtnGruplariTemizle_Click(object sender, RoutedEventArgs e)
        {
            _secimServisi.TumGrupAtamalariniTemizle();
            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(myCanvas);

            foreach (var kutu in myCanvas.Children.OfType<Rectangle>())
            {
                kutu.Stroke = System.Windows.Media.Brushes.Transparent;
                kutu.StrokeThickness = 0;
            }

            txtGrupValue.Text = "1";
            SecimiTemizle();
        }

        private void MevcutKatGruplariniKaydet()
        {
            var mevcutAtamalar = _secimServisi.GrupAtamalari
                .Values
                .Where(x => x.KatNo == AktifKatNo)
                .Select(x => new GrupAtamaBilgisi
                {
                    KatNo = x.KatNo,
                    GrupNo = x.GrupNo,
                    GrupEkseni = x.GrupEkseni,
                    KoliAnahtari = x.KoliAnahtari
                })
                .ToList();

            _bilgiServisi.KatAtamalariniKaydet(AktifKatNo, mevcutAtamalar);
        }


        private bool PalettekiTumUrunlerGrupluMu()
        {
            MevcutKatGruplariniKaydet();

            var tumKatlar = _katYonetici.TumKatlar;

            if (tumKatlar == null || !tumKatlar.Any())
                return false;

            int palettekiToplamUrunSayisi = tumKatlar
                .Where(kat => kat.Value != null)
                .Sum(kat => kat.Value.Count);

            int toplamGrupluUrunSayisi = tumKatlar
                .SelectMany(kat => _bilgiServisi.KatAtamalariniGetir(kat.Key))
                .Where(x => !string.IsNullOrWhiteSpace(x.KoliAnahtari))
                .Select(x => $"{x.KatNo}_{x.KoliAnahtari}")
                .Distinct()
                .Count();

            return palettekiToplamUrunSayisi == toplamGrupluUrunSayisi;
        }

        private ProgramKayitModel ProgramModeliOlustur(string programAdi, string aciklama)
        {
            MevcutKatGruplariniKaydet();

            return new ProgramKayitModel
            {
                Id = _programKayitServisi.SonrakiIdGetir(),
                ProgramAdi = programAdi,
                Aciklama = aciklama,
                UrunAdi = _secilenUrun?.UrunAdi ?? "",
                PaletAdi = _secilenPalet?.PaletAdi ?? "",
                DizilimAdi = _gelenDizilimAdi ?? "",
                Gruplar = ProgramGruplariniOlustur()
            };
        }

        private List<ProgramGrupModel> ProgramGruplariniOlustur()
        {
            var sonuc = new List<ProgramGrupModel>();

            foreach (var kat in _katYonetici.TumKatlar.OrderBy(x => x.Key))
            {
                int katNo = kat.Key;

                var katAtamalari = _bilgiServisi.KatAtamalariniGetir(katNo)
                    .Where(x => x.KatNo == katNo)
                    .ToList();

                var gruplar = katAtamalari
                    .GroupBy(x => x.GrupNo)
                    .OrderBy(x => x.Key);

                foreach (var grup in gruplar)
                {
                    var grupModel = new ProgramGrupModel
                    {
                        KatNo = katNo,
                        GrupNo = grup.Key,
                        UrunSayisi = grup.Count(),
                        GripperAcisi = 360,
                        Yon = GrupYonuBul(katNo, grup.Key),
                        GrupMerkezX = 0,
                        GrupMerkezY = 0,
                        GrupMerkezZ = 0,
                        Urunler = new List<ProgramUrunModel>()
                    };

                    GrupMerkezleriniHesaplaVeUygula(grupModel);
                    GrupUrunleriniHesaplaVeUygula(grupModel);

                    sonuc.Add(grupModel);
                }
            }

            return sonuc;
        }

        private UrunYonu? GrupYonuBul(int katNo, int grupNo)
        {
            var grupKutulari = _secimServisi.GrupAtamalari
                .Where(x => x.Value.KatNo == katNo && x.Value.GrupNo == grupNo)
                .Select(x => x.Key)
                .ToList();

            if (!grupKutulari.Any())
                return UrunYonu.Yatay;

            var ilkKutu = grupKutulari.First();
           
            return _yonYardimcisi.KutuYonunuGetir(ilkKutu);
        }

        private void GrupUrunleriniHesaplaVeUygula(ProgramGrupModel grupModel)
        {
            var kutular = _secimServisi.GrupAtamalari
                .Where(x => x.Value.KatNo == grupModel.KatNo && x.Value.GrupNo == grupModel.GrupNo)
                .Select(x => x.Key)
                .ToList();

            grupModel.Urunler.Clear();

            if (!kutular.Any())
                return;

            double paletYukseklik = _secilenPalet?.PaletYukseklik ?? 0;
            double urunYukseklik = _secilenUrun?.UrunYukseklik ?? 0;

            double urunMerkezZ = paletYukseklik
                               + ((grupModel.KatNo - 1) * urunYukseklik)
                               + (urunYukseklik / 2.0);

            foreach (var kutu in kutular)
            {
                double left = Canvas.GetLeft(kutu);
                double top = Canvas.GetTop(kutu);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                double width = kutu.ActualWidth > 0 ? kutu.ActualWidth : kutu.Width;
                double height = kutu.ActualHeight > 0 ? kutu.ActualHeight : kutu.Height;

                double canvasMerkezX = left + width / 2.0;
                double canvasMerkezY = top + height / 2.0;

                // Canvas -> gerçek palet koordinatı dönüşümü
                double gercekMerkezX = canvasMerkezY / OlcekX;
                double gercekMerkezY = canvasMerkezX / OlcekY;

                grupModel.Urunler.Add(new ProgramUrunModel
                {
                    MerkezX = gercekMerkezX,
                    MerkezY = gercekMerkezY,
                    MerkezZ = urunMerkezZ
                });
            }
        }


        private void GrupMerkezleriniHesaplaVeUygula(ProgramGrupModel grupModel)
        {
            var kutular = _secimServisi.GrupAtamalari
                .Where(x => x.Value.KatNo == grupModel.KatNo && x.Value.GrupNo == grupModel.GrupNo)
                .Select(x => x.Key)
                .ToList();

            if (!kutular.Any())
            {
                grupModel.GrupMerkezX = 0;
                grupModel.GrupMerkezY = 0;
                grupModel.GrupMerkezZ = 0;
                return;
            }

            var merkezler = kutular
                .Select(kutu =>
                {
                    double left = Canvas.GetLeft(kutu);
                    double top = Canvas.GetTop(kutu);

                    double width = kutu.ActualWidth > 0 ? kutu.ActualWidth : kutu.Width;
                    double height = kutu.ActualHeight > 0 ? kutu.ActualHeight : kutu.Height;

                    double canvasMerkezX = left + width / 2.0;
                    double canvasMerkezY = top + height / 2.0;

                    return new
                    {
                        CanvasMerkezX = canvasMerkezX,
                        CanvasMerkezY = canvasMerkezY
                    };
                })
                .ToList();

            double ortalamaCanvasX = merkezler.Average(x => x.CanvasMerkezX);
            double ortalamaCanvasY = merkezler.Average(x => x.CanvasMerkezY);

            grupModel.GrupMerkezX = ortalamaCanvasY / OlcekX;
            grupModel.GrupMerkezY = ortalamaCanvasX / OlcekY;

            double paletYukseklik = _secilenPalet?.PaletYukseklik ?? 0;
            double urunYukseklik = _secilenUrun?.UrunYukseklik ?? 0;

            grupModel.GrupMerkezZ = paletYukseklik
                                  + ((grupModel.KatNo - 1) * urunYukseklik)
                                  + (urunYukseklik / 2.0);
        }


        private void GruplamaListesiniDoldur(ProgramKayitModel program)
        {
            _gruplamaListesi.Clear();

            if (program?.Gruplar == null || !program.Gruplar.Any())
                return;

            foreach (var grup in program.Gruplar.OrderBy(x => x.KatNo).ThenBy(x => x.GrupNo))
            {
                _gruplamaListesi.Add(new GruplamaListeItemModel
                {
                    KatNo = grup.KatNo,
                    GrupNo = grup.GrupNo,
                    IsaretliMi = false
                });
            }
        }

        private void BtnProgramKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (!PalettekiTumUrunlerGrupluMu())
            {
                BildirimGoster("GruplamaYap.programKayitIcinTumUrunlerGrupluOlmali");
                return;
            }

            var popup = new ProgramKaydetPopup
            {
                Owner = Window.GetWindow(this)
            };

            bool? sonuc = popup.ShowDialog();
            if (sonuc != true)
                return;

            string programAdi = popup.Sonuc.ProgramAdi;
            string aciklama = popup.Sonuc.Aciklama;

            if (_programKayitServisi.ProgramAdiVarMi(programAdi))
            {
                BildirimGoster("GruplamaYap.programAdiZatenKayitli");
                return;
            }

            var program = ProgramModeliOlustur(programAdi, aciklama);
            _programKayitServisi.Kaydet(program);
            GruplamaListesiniDoldur(program);
            BildirimGoster("GruplamaYap.programBasariIleKayitEdildi");
        }
    }


}