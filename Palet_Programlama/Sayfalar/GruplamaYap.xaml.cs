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
using Palet_Programlama.Statics;

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
        private readonly GruplamaYuklemeServisi _yuklemeServisi = new();
        private readonly ObservableCollection<GruplamaListeItemModel> _gruplamaListesi = new();
        private readonly Dictionary<string, GrupGripperAyarlari> _grupAyarKayitlari = new();
        private readonly List<Rect> _tiklananDigerKutular = new();

        private GruplamaListeItemModel _secilenGruplamaItem;
        private Urun _secilenUrun;
        private Palet _secilenPalet;
        private string _gelenDizilimAdi;
        private List<ProgramKayitModel> _programKayitlari = new();
        private List<DizilimKayitModel> _dizilimKayitlari = new();
        private ProgramKayitModel _seciliProgramKaydi;
        private Rectangle sonSecilmisKutu = new Rectangle();
        private bool _sayfaYukleniyor;

        private const string ProgramSecinizMetni = "Güncelleme için seçim yapınız";

        private double OlcekY => canvasPalet.Width / _secilenPalet.PaletBoy;
        private double OlcekX => canvasPalet.Height / _secilenPalet.PaletEn;
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
        private void ProgramKaydetButonMetniniGuncelle()
        {
            BtnProgramKaydet.Content = _seciliProgramKaydi == null
                ? "Program Kaydet"
                : "Güncelle";
        }

        public GruplamaYap(Frame Main, Urun secilenUrun, Palet secilenPalet, string dizilimAdi)
        {
            InitializeComponent();

            MainFrame = Main;
            _secilenUrun = secilenUrun;
            _secilenPalet = secilenPalet;
            _gelenDizilimAdi = dizilimAdi;

            ListBoxGruplama.ItemsSource = _gruplamaListesi;
            _mesafe.Baslat(canvasPalet);

            SayfaVerileriniYukle();
        }

        private void SayfaVerileriniYukle()
        {
            YukleniyorGoster("Sayfa yükleniyor...");

            try
            {
                GruplamaYuklemeModel yuklemeModel = null;

                YuklemeModundaCalistir(() =>
                {
                    yuklemeModel = _yuklemeServisi.IlkAcilisVerisiniHazirla(
                        _secilenUrun,
                        _secilenPalet,
                        _gelenDizilimAdi);

                    _dizilimKayitlari = yuklemeModel.DizilimKayitlari;
                    _programKayitlari = yuklemeModel.ProgramKayitlari;

                    CboxUrunListesi.ItemsSource = yuklemeModel.UrunAdlari;
                    CboxUrunListesi.SelectedItem = yuklemeModel.SeciliUrunAdi;

                    CboxDizilimListesi.ItemsSource = yuklemeModel.DizilimAdlari.Any()
                        ? yuklemeModel.DizilimAdlari
                        : null;

                    if (!string.IsNullOrWhiteSpace(yuklemeModel.SeciliDizilimAdi))
                        CboxDizilimListesi.SelectedItem = yuklemeModel.SeciliDizilimAdi;
                    else if (CboxDizilimListesi.Items.Count > 0)
                        CboxDizilimListesi.SelectedIndex = 0;

                    txtPaletOzellikleri.Text = yuklemeModel.PaletOzellikMetni;

                    var programListe = new List<string> { ProgramSecinizMetni };
                    programListe.AddRange(yuklemeModel.ProgramAdlari);

                    CboxProgramListesi.ItemsSource = programListe;
                    CboxProgramListesi.SelectedIndex = 0;
                });

                SeciliDizilimiCanvasaYukle();
                ProgramSeciminiSifirla(false);
            }
            finally
            {
                YukleniyorGizle();
            }
        }
      
     
        private string SeciliUrunAdiniGetir()
        {
            return CboxUrunListesi.SelectedItem?.ToString() ?? _secilenUrun?.UrunAdi ?? "";
        }

        private string SeciliDizilimAdiniGetir()
        {
            return CboxDizilimListesi.SelectedItem?.ToString() ?? _gelenDizilimAdi ?? "";
        }

        private void ProgramlariGuncelSecimeGoreDoldur()
        {
            ProgramlariDoldur(SeciliUrunAdiniGetir(), SeciliDizilimAdiniGetir());
        }

        private void ProgramlariDoldur(string urunAdi, string dizilimAdi)
        {
            _programKayitlari = _programKayitServisi.KayitlariYukle();

            var programAdlari = _programKayitlari
                .Where(x =>
                    string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.DizilimAdi ?? "").Trim(), (dizilimAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.PaletAdi ?? "").Trim(), (_secilenPalet?.PaletAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                .Select(x => x.ProgramAdi)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var gosterilecekListe = new List<string> { ProgramSecinizMetni };
            gosterilecekListe.AddRange(programAdlari);

            YuklemeModundaCalistir(() =>
            {
                CboxProgramListesi.ItemsSource = null;
                CboxProgramListesi.ItemsSource = gosterilecekListe;
                CboxProgramListesi.SelectedIndex = 0;
            });
        }

        private DizilimKayitModel SeciliKaydiGetir()
        {
            return _yuklemeServisi.SeciliKaydiGetir(
                _dizilimKayitlari,
                CboxUrunListesi.SelectedItem?.ToString() ?? "",
                CboxDizilimListesi.SelectedItem?.ToString() ?? "");
        }

        private void KayitBilgileriniUygula(DizilimKayitModel kayit)
        {
            _gelenDizilimAdi = kayit?.DizilimAdi;
            _yuklemeServisi.KayitBilgileriniUygula(_secilenUrun, _secilenPalet, kayit);
            txtPaletOzellikleri.Text = _yuklemeServisi.PaletMetniGetir(_secilenPalet, kayit);
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
                canvasPalet,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
            GrupGorselleriniYenile();
        }

        private void DizilimDegisiminiUygula(string dizilimAdi)
        {
            _gelenDizilimAdi = dizilimAdi;
            GrupDurumunuTemizle();
            SeciliDizilimiCanvasaYukle();
            ProgramlariGuncelSecimeGoreDoldur();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle tiklananKutu)
                return;

            int aktifGrupNo = AktifGrupNo;

            if (GrupNoBaskaKattaKullanimdaMi(aktifGrupNo, AktifKatNo))
            {
                AktifGrupNumarasiniGuncelle();
                BildirimGoster("GruplamaYap.grupNoBaskaKattaKullaniliyor");
                return;
            }

            var tumKutular = canvasPalet.Children.OfType<Rectangle>().ToList();

            if (_secimServisi.KoliBuAktifGruptaMi(tiklananKutu, AktifKatNo, aktifGrupNo))
            {
                _secimServisi.KolininGrubunuKaldir(tiklananKutu);

                GrupDegisikligiSonrasiGuncelle(
                    tiklananKutu,
                    tumKutular,
                    AktifKatNo,
                    aktifGrupNo,
                    grupNumarasiniGuncelle: true);

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
                GrupSabitleri.BirGruptakiMaksimumKoliSayisi,
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
                GrupSabitleri.GrupHizalamaToleransi,
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
                GrupSabitleri.GrupHizalamaToleransi,
                GrupSabitleri.GrupKomsulukToleransi,
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

            GrupDegisikligiSonrasiGuncelle(
                tiklananKutu,
                tumKutular,
                AktifKatNo,
                aktifGrupNo,
                grupNumarasiniGuncelle: false);
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

            _gelenDizilimAdi = null;
            GrupDurumunuTemizle();
            ProgramSeciminiSifirla(true);

            var dizilimAdlari = _yuklemeServisi.UrunDegisimindeDizilimleriHazirla(
                _dizilimKayitlari,
                seciliUrunAdi,
                _secilenUrun);

            YuklemeModundaCalistir(() =>
            {
                CboxDizilimListesi.ItemsSource = dizilimAdlari.Any() ? dizilimAdlari : null;
            });

            if (CboxDizilimListesi.Items.Count > 0)
                CboxDizilimListesi.SelectedIndex = 0;
            else
                CanvasiTemizle();
        }
        private void CboxDizilimListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sayfaYukleniyor || CboxDizilimListesi.SelectedItem == null)
                return;

            DizilimDegisiminiUygula(CboxDizilimListesi.SelectedItem.ToString());
        }

        private void CboxProgramListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sayfaYukleniyor || CboxProgramListesi.SelectedItem == null)
                return;

            string seciliProgramAdi = CboxProgramListesi.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(seciliProgramAdi) || seciliProgramAdi == ProgramSecinizMetni)
            {
                ProgramSeciminiSifirla(false);
                GrupDurumunuTemizle();
                SeciliDizilimiCanvasaYukle();
                return;
            }

            var seciliProgram = _programKayitlari.FirstOrDefault(x =>
                string.Equals((x.ProgramAdi ?? "").Trim(), seciliProgramAdi.Trim(), StringComparison.OrdinalIgnoreCase));

            if (seciliProgram == null)
                return;

            _seciliProgramKaydi = seciliProgram;
            ProgramKaydetButonMetniniGuncelle();

            ProgramiSayfayaYukle(seciliProgram);
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

            KatiYukle(
                yeniKat,
                atamalariYukle: true,
                grupGorselleriniYenile: true,
                grupNumarasiniGuncelle: true);
        }

        private void KataGecSessiz(int yeniKat)
        {
            KatiYukle(
                yeniKat,
                atamalariYukle: false,
                grupGorselleriniYenile: false,
                grupNumarasiniGuncelle: false);
        }

        private void BtnGruplandirmaEksi_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtGrupValue.Text) != 1)
                txtGrupValue.Text = (Convert.ToInt32(txtGrupValue.Text) - 1).ToString();
        }

        private void BtnGruplandirmaArti_Click(object sender, RoutedEventArgs e)
        {
            txtGrupValue.Text = (Convert.ToInt32(txtGrupValue.Text) + 1).ToString();
        }

        private void BtnGruplariTemizle_Click(object sender, RoutedEventArgs e)
        {
            GrupDurumunuTemizle();
        }

        private void BtnProgramKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (!PalettekiTumUrunlerGrupluMu())
            {
                BildirimGoster("GruplamaYap.programKayitIcinTumUrunlerGrupluOlmali");
                return;
            }

            if (!TumGruplarinAyarlariTamamMi())
            {
                BildirimGoster("GruplamaYap.programKayitIcinTumGrupAyarlariYapilmali");
                return;
            }

            var popup = new ProgramKaydetPopup
            {
                Owner = Window.GetWindow(this)
            };

            if (_seciliProgramKaydi != null)
                popup.VerileriYukle(_seciliProgramKaydi.ProgramAdi, _seciliProgramKaydi.Aciklama);

            bool? sonuc = popup.ShowDialog();
            if (sonuc != true)
                return;

            string programAdi = popup.Sonuc.ProgramAdi;
            string aciklama = popup.Sonuc.Aciklama;

            YukleniyorGoster(_seciliProgramKaydi == null ? "Program kaydediliyor..." : "Program güncelleniyor...");

            try
            {
                if (_seciliProgramKaydi == null)
                {
                    if (_programKayitServisi.ProgramAdiVarMi(programAdi))
                    {
                        BildirimGoster("GruplamaYap.programAdiZatenKayitli");
                        return;
                    }

                    var yeniProgram = ProgramModeliOlustur(programAdi, aciklama);
                    yeniProgram.Id = _programKayitServisi.SonrakiIdGetir();

                    _programKayitServisi.Kaydet(yeniProgram);
                    KaydedilenProgramiUygula(yeniProgram);

                    BildirimGoster("GruplamaYap.programBasariIleKayitEdildi");
                    return;
                }

                var guncelProgram = ProgramModeliOlustur(programAdi, aciklama);
                guncelProgram.Id = _seciliProgramKaydi.Id;

                _programKayitServisi.Guncelle(guncelProgram);
                KaydedilenProgramiUygula(guncelProgram);

                BildirimGoster("GruplamaYap.programBasariIleGuncellendi");
            }
            finally
            {
                YukleniyorGizle();
            }
        }

        private void BtnGrupAyarlar_Click(object sender, RoutedEventArgs e)
        {
            if (_secilenGruplamaItem == null)
            {
                BildirimGoster("GruplamaYap.onceListedenGrupSecilmeli");
                return;
            }

            int katNo = _secilenGruplamaItem.KatNo;
            int grupNo = _secilenGruplamaItem.GrupNo;

            var mevcutAyar = GrupAyariniGetir(katNo, grupNo);

            var popup = new GrupAyarlariPopup(katNo, grupNo)
            {
                Owner = Window.GetWindow(this)
            };

            popup.AyarlariYukle(mevcutAyar);

            bool? sonuc = popup.ShowDialog();
            if (sonuc != true)
                return;

            var yeniAyar = popup.Sonuc;
            if (yeniAyar == null)
                return;

            GrupAyariniKaydet(katNo, grupNo, yeniAyar);

            _secilenGruplamaItem.IsaretliMi = yeniAyar.KayitEdildiMi;
            ListBoxGruplama.Items.Refresh();
        }

        private void ListBoxGruplama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _secilenGruplamaItem = ListBoxGruplama.SelectedItem as GruplamaListeItemModel;
        }

        private void ProgramiSayfayaYukle(ProgramKayitModel program)
        {
            if (program == null)
                return;

            YukleniyorGoster("Program yükleniyor...");
            try
            {
                YuklemeModundaCalistir(() =>
                {
                    if (!string.IsNullOrWhiteSpace(program.UrunAdi))
                        CboxUrunListesi.SelectedItem = program.UrunAdi;

                    if (!string.IsNullOrWhiteSpace(program.DizilimAdi))
                        CboxDizilimListesi.SelectedItem = program.DizilimAdi;

                    _gelenDizilimAdi = program.DizilimAdi;
                });

                SeciliDizilimiCanvasaYukle();
                ProgramGruplariniCanvasaUygula(program);
                MevcutGruplamaListesiniYenile();
            }
            finally
            {
                YukleniyorGizle();
            }
        }

        private void ProgramGruplariniCanvasaUygula(ProgramKayitModel program)
        {
            if (program?.Gruplar == null || !program.Gruplar.Any())
                return;

            _secimServisi.TumGrupAtamalariniTemizle();
            _grupAyarKayitlari.Clear();

            foreach (var grup in program.Gruplar.OrderBy(x => x.KatNo).ThenBy(x => x.GrupNo))
            {
                if (grup.GripperAyarlari != null)
                    GrupAyariniKaydet(grup.KatNo, grup.GrupNo, grup.GripperAyarlari);

                KataGecSessiz(grup.KatNo);

                var aktifKattakiKutular = canvasPalet.Children.OfType<Rectangle>().ToList();

                foreach (var urun in grup.Urunler)
                {
                    var eslesenKutu = EnYakinKutuyuBul(aktifKattakiKutular, urun.MerkezX, urun.MerkezY);

                    if (eslesenKutu == null)
                        continue;

                    _secimServisi.KoliyaGrupAta(
                        eslesenKutu,
                        grup.KatNo,
                        grup.GrupNo,
                        aktifKattakiKutular,
                        _geometri);
                }

                _secimServisi.GruptakiTumKutularinEksenBilgisiniGuncelle(
                    aktifKattakiKutular,
                    grup.KatNo,
                    grup.GrupNo,
                    _geometri);

                KattakiAtamalariKaydet(grup.KatNo);
            }

            KatiYukle(
                1,
                atamalariYukle: true,
                grupGorselleriniYenile: true,
                grupNumarasiniGuncelle: false);
        }

        private ProgramKayitModel ProgramModeliOlustur(string programAdi, string aciklama)
        {
            MevcutKatGruplariniKaydet();

            return new ProgramKayitModel
            {
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
                        GripperAyarlari = GrupAyariniGetir(katNo, grup.Key),
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
            var grupKutulari = GruptakiKutulariGetir(katNo, grupNo);

            if (!grupKutulari.Any())
                return UrunYonu.Yatay;

            return _yonYardimcisi.KutuYonunuGetir(grupKutulari.First());
        }

        private void GrupUrunleriniHesaplaVeUygula(ProgramGrupModel grupModel)
        {
            var kutular = GruptakiKutulariGetir(grupModel.KatNo, grupModel.GrupNo);
            grupModel.Urunler.Clear();

            if (!kutular.Any())
                return;

            double urunMerkezZ = KatIcinMerkezZGetir(grupModel.KatNo);

            foreach (var kutu in kutular)
            {
                Point gercekMerkez = _geometri.GercekMerkeziGetir(kutu, OlcekX, OlcekY);

                grupModel.Urunler.Add(new ProgramUrunModel
                {
                    MerkezX = gercekMerkez.X,
                    MerkezY = gercekMerkez.Y,
                    MerkezZ = urunMerkezZ
                });
            }
        }

        private void GrupMerkezleriniHesaplaVeUygula(ProgramGrupModel grupModel)
        {
            var kutular = GruptakiKutulariGetir(grupModel.KatNo, grupModel.GrupNo);

            if (!kutular.Any())
            {
                grupModel.GrupMerkezX = 0;
                grupModel.GrupMerkezY = 0;
                grupModel.GrupMerkezZ = 0;
                return;
            }

            var merkezler = kutular
                .Select(kutu => _geometri.KutuMerkeziniGetir(kutu))
                .ToList();

            double ortalamaCanvasX = merkezler.Average(x => x.X);
            double ortalamaCanvasY = merkezler.Average(x => x.Y);

            grupModel.GrupMerkezX = ortalamaCanvasY / OlcekX;
            grupModel.GrupMerkezY = ortalamaCanvasX / OlcekY;
            grupModel.GrupMerkezZ = KatIcinMerkezZGetir(grupModel.KatNo);
        }

        private List<Rectangle> GruptakiKutulariGetir(int katNo, int grupNo)
        {
            return _secimServisi.GrupAtamalari
                .Where(x => x.Value.KatNo == katNo && x.Value.GrupNo == grupNo)
                .Select(x => x.Key)
                .ToList();
        }

        private bool GrupNoBaskaKattaKullanimdaMi(int grupNo, int aktifKatNo)
        {
            return _secimServisi.GrupAtamalari.Values.Any(x =>
                x.GrupNo == grupNo && x.KatNo != aktifKatNo);
        }

        private int SonrakiBosGrupNoGetir()
        {
            var kullanilanGrupNolari = _secimServisi.GrupAtamalari.Values
                .Select(x => x.GrupNo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            int beklenen = 1;

            foreach (var grupNo in kullanilanGrupNolari)
            {
                if (grupNo != beklenen)
                    return beklenen;

                beklenen++;
            }

            return beklenen;
        }

        private void AktifGrupNumarasiniGuncelle()
        {
            txtGrupValue.Text = SonrakiBosGrupNoGetir().ToString();
        }

        private void GrupDegisikligiSonrasiGuncelle(
            Rectangle tiklananKutu,
            IEnumerable<Rectangle> tumKutular,
            int katNo,
            int grupNo,
            bool grupNumarasiniGuncelle)
        {
            _secimServisi.GruptakiTumKutularinEksenBilgisiniGuncelle(
                tumKutular,
                katNo,
                grupNo,
                _geometri);

            _gorsellestirmeServisi.KutuGrupGorseliniGuncelle(
                canvasPalet,
                tiklananKutu,
                katNo,
                _secimServisi.GrupAtamalari,
                _geometri);

            MevcutGruplamaListesiniYenile();

            if (grupNumarasiniGuncelle)
                AktifGrupNumarasiniGuncelle();
        }

        private List<GrupAtamaBilgisi> KattakiAtamalariOlustur(int katNo)
        {
            return _secimServisi.GrupAtamalari
                .Values
                .Where(x => x.KatNo == katNo)
                .Select(x => new GrupAtamaBilgisi
                {
                    KatNo = x.KatNo,
                    GrupNo = x.GrupNo,
                    GrupEkseni = x.GrupEkseni,
                    KoliAnahtari = x.KoliAnahtari
                })
                .ToList();
        }

        private void KattakiAtamalariKaydet(int katNo)
        {
            _bilgiServisi.KatAtamalariniKaydet(katNo, KattakiAtamalariOlustur(katNo));
        }

        private void MevcutKatGruplariniKaydet()
        {
            KattakiAtamalariKaydet(AktifKatNo);
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

        private bool TumGruplarinAyarlariTamamMi()
        {
            var gruplar = _secimServisi.GrupAtamalari.Values
                .GroupBy(x => new { x.KatNo, x.GrupNo })
                .ToList();

            if (!gruplar.Any())
                return false;

            foreach (var grup in gruplar)
            {
                var ayar = GrupAyariniGetir(grup.Key.KatNo, grup.Key.GrupNo);

                if (ayar == null || !ayar.KayitEdildiMi)
                    return false;
            }

            return true;
        }

        private double KatIcinMerkezZGetir(int katNo)
        {
            double paletYukseklik = _secilenPalet?.PaletYukseklik ?? 0;
            double urunYukseklik = _secilenUrun?.UrunYukseklik ?? 0;

            return paletYukseklik
                 + ((katNo - 1) * urunYukseklik)
                 + (urunYukseklik / 2.0);
        }

        private void ProgramSeciminiSifirla(bool comboboxiTemizle)
        {
            _seciliProgramKaydi = null;
            ProgramKaydetButonMetniniGuncelle();

            if (!comboboxiTemizle)
                return;

            YuklemeModundaCalistir(() =>
            {
                CboxProgramListesi.ItemsSource = new List<string> { ProgramSecinizMetni };
                CboxProgramListesi.SelectedIndex = 0;
            });
        }

        private void KaydedilenProgramiUygula(ProgramKayitModel program)
        {
            _seciliProgramKaydi = program;
            ProgramKaydetButonMetniniGuncelle();

            ProgramlariGuncelSecimeGoreDoldur();
            CboxProgramListesi.SelectedItem = program.ProgramAdi;
        }

        private void MevcutGruplamaListesiniYenile()
        {
            _gruplamaListesi.Clear();

            var grupKayitlari = _secimServisi.GrupAtamalari.Values
                .GroupBy(x => new { x.KatNo, x.GrupNo })
                .OrderBy(x => x.Key.KatNo)
                .ThenBy(x => x.Key.GrupNo)
                .ToList();

            foreach (var grup in grupKayitlari)
            {
                var ayar = GrupAyariniGetir(grup.Key.KatNo, grup.Key.GrupNo);

                _gruplamaListesi.Add(new GruplamaListeItemModel
                {
                    KatNo = grup.Key.KatNo,
                    GrupNo = grup.Key.GrupNo,
                    IsaretliMi = ayar?.KayitEdildiMi ?? false
                });
            }

            if (!_gruplamaListesi.Any())
                _secilenGruplamaItem = null;

            ListBoxGruplama.Items.Refresh();
        }

        private string GrupAyarAnahtari(int katNo, int grupNo)
        {
            return $"{katNo}_{grupNo}";
        }

        private GrupGripperAyarlari GrupAyariniGetir(int katNo, int grupNo)
        {
            string anahtar = GrupAyarAnahtari(katNo, grupNo);

            if (_grupAyarKayitlari.TryGetValue(anahtar, out var ayar))
                return ayar;

            ayar = new GrupGripperAyarlari();
            _grupAyarKayitlari[anahtar] = ayar;
            return ayar;
        }

        private void GrupAyariniKaydet(int katNo, int grupNo, GrupGripperAyarlari ayar)
        {
            string anahtar = GrupAyarAnahtari(katNo, grupNo);
            _grupAyarKayitlari[anahtar] = ayar;
        }

        private Rectangle EnYakinKutuyuBul(List<Rectangle> kutular, double gercekMerkezX, double gercekMerkezY)
        {
            Rectangle enYakinKutu = null;
            double enKucukFark = double.MaxValue;

            foreach (var kutu in kutular)
            {
                if (_secimServisi.GrupAtamalari.ContainsKey(kutu))
                    continue;

                Point kutuGercekMerkez = _geometri.GercekMerkeziGetir(kutu, OlcekX, OlcekY);
                double fark = Math.Abs(kutuGercekMerkez.X - gercekMerkezX)
                            + Math.Abs(kutuGercekMerkez.Y - gercekMerkezY);

                if (fark < enKucukFark)
                {
                    enKucukFark = fark;
                    enYakinKutu = kutu;
                }
            }

            return enYakinKutu;
        }

        private void GrupDurumunuTemizle()
        {
            _gruplamaListesi.Clear();
            _grupAyarKayitlari.Clear();
            _secilenGruplamaItem = null;

            _secimServisi.TumGrupAtamalariniTemizle();
            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(canvasPalet);

            foreach (var kat in _katYonetici.TumKatlar.Keys.ToList())
                _bilgiServisi.KatAtamalariniKaydet(kat, new List<GrupAtamaBilgisi>());

            foreach (var kutu in canvasPalet.Children.OfType<Rectangle>())
            {
                kutu.Stroke = System.Windows.Media.Brushes.Transparent;
                kutu.StrokeThickness = 0;
            }

            AktifGrupNumarasiniGuncelle();
            SecimiTemizle();
            MevcutGruplamaListesiniYenile();
        }

        private void CanvasiTemizle()
        {
            canvasPalet.Children.OfType<Rectangle>().ToList().ForEach(r => canvasPalet.Children.Remove(r));
            _katYonetici.Temizle();
            _secimServisi.TumGrupAtamalariniTemizle();
            SecimiTemizle();
            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(canvasPalet);
            txtKatValue.Text = "1";
        }

        private void SecimiTemizle()
        {
            _tiklananDigerKutular.Clear();
            _mesafe.Gizle();
        }

        private void GrupGorselleriniYenile()
        {
            _gorsellestirmeServisi.AktifKattakiGrupEtiketleriniYenile(
                canvasPalet,
                AktifKatNo,
                _secimServisi.GrupAtamalari,
                _geometri);

            _gorsellestirmeServisi.AktifKattakiTumKutuGorselleriniYenile(
                canvasPalet,
                AktifKatNo,
                _secimServisi.GrupAtamalari,
                _geometri);
        }

        private void KatiYukle(
            int yeniKat,
            bool atamalariYukle,
            bool grupGorselleriniYenile,
            bool grupNumarasiniGuncelle)
        {
            _katYonetici.KatDegistir(
                yeniKat,
                canvasPalet,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            if (atamalariYukle)
            {
                var kayitliAtamalar = _bilgiServisi.KatAtamalariniGetir(_katYonetici.AktifKat);

                _secimServisi.KatAtamalariniYukle(
                    _katYonetici.AktifKat,
                    canvasPalet.Children.OfType<Rectangle>(),
                    kayitliAtamalar,
                    _geometri);
            }

            txtKatValue.Text = _katYonetici.AktifKat.ToString();

            if (grupGorselleriniYenile)
                GrupGorselleriniYenile();

            if (grupNumarasiniGuncelle)
                AktifGrupNumarasiniGuncelle();
        }

        private void YuklemeModundaCalistir(Action action)
        {
            _sayfaYukleniyor = true;
            try
            {
                action?.Invoke();
            }
            finally
            {
                _sayfaYukleniyor = false;
            }
        }

        private void YukleniyorGoster(string mesaj = "Yükleniyor...")
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.YukleniyorGoster(mesaj);
        }

        private void YukleniyorGizle()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.YukleniyorGizle();
        }

        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }
    }
}