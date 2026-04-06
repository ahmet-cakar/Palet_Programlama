using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Dizilim.Services;
using Palet_Programlama.Screens.Gruplama.Helpers;
using Palet_Programlama.Screens.Gruplama.Models;
using Palet_Programlama.Screens.Gruplama.Services;
using Palet_Programlama.Screens.Program;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using Palet_Programlama.UserController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Palet_Programlama.Languages;

namespace Palet_Programlama.Screens
{
    /// <summary>
    /// Interaction logic for Programlar.xaml
    /// </summary>
    public partial class Programlar : Page
    {
        private readonly Frame MainFrame;
        private readonly ProgramListeServisi _programListeServisi = new();
        private readonly UrunIslemler _urunServisi = new();
        private readonly PaletIslemler _paletServisi = new();
        private readonly KatYoneticisi _katYonetici = new();
        private readonly KoliGeometriYardimcisi _geometri = new();
        private readonly GruplamaBilgiServisi _bilgiServisi = new();
        private readonly GruplamaSecimServisi _secimServisi = new();
        private readonly GrupGorsellestirmeServisi _gorsellestirmeServisi = new();
        private readonly DizilimKayitServisi _dizilimKayitServisi = new();

        private List<ProgramKayitModel> _programKayitlari = new();
        private List<DizilimKayitModel> _dizilimKayitlari = new();
        private ProgramKayitModel _seciliProgramKaydi;

        private readonly Rectangle sonSecilmisKutu = new Rectangle();

        private Urun urun;
        private Palet palet;

        private double OlcekY => canvasPalet.Width / palet.PaletBoy;
        private double OlcekX => canvasPalet.Height / palet.PaletEn;
        private int AktifKatNo => _katYonetici.AktifKat;


        private string ProgramSecilmediText => LanguageConverter.GetString("Program.Secilmedi");
        private string SecilmediText => LanguageConverter.GetString("Program.secilmedi");
        private string UrunSecilmediText => LanguageConverter.GetString("Program.urunSecilmedi");
        private string PaletSecilmediText => LanguageConverter.GetString("Program.paletSecilmedi");

        public Programlar(Frame Main)
        {
            InitializeComponent();
            MainFrame = Main;

            UstMenuControl.AktifSayfa = "Programlar";
            _dizilimKayitlari = _dizilimKayitServisi.KayitlariYukle();
            ProgramlariYukle();
        }

        private void ProgramlariYukle()
        {
            _programKayitlari = _programListeServisi.ProgramlariGetir();
            ListBoxProgramlar.ItemsSource = _programKayitlari;
        }

        private void ListBoxProgramlar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seciliProgram = ListBoxProgramlar.SelectedItem as ProgramKayitModel;
            if (seciliProgram == null)
                return;

            txtProgramAdi.Text = seciliProgram.ProgramAdi;
            txtProgramID.Text = seciliProgram.Id.ToString();
            txtProgramAdi.ToolTip = seciliProgram.ProgramAdi;

            urun = _urunServisi.UrunGetir(seciliProgram.UrunAdi);
            if (urun != null)
            {
                txtUrunAdi.Text = urun.UrunAdi;
                txtUrunAdi.ToolTip = urun.UrunAdi;
                txtUrunGenislik.Text = urun.UrunEn.ToString();
                txtUrunUzunluk.Text = urun.UrunBoy.ToString();
                txtUrunBasinc.Text = urun.UrunBasinc.ToString();
                txtUrunYukseklik.Text = urun.UrunYukseklik.ToString();
                txtUrunAgirlik.Text = urun.UrunAgirlik.ToString();
            }

            palet = _paletServisi.PaletGetir(seciliProgram.PaletAdi);
            if (palet != null)
            {
                txtPaletAdi.Text = palet.PaletAdi;
                txtPaletAdi.ToolTip = palet.PaletAdi;
                txtPaletGenislik.Text = palet.PaletEn.ToString();
                txtPaletUzunluk.Text = palet.PaletBoy.ToString();
                txtPaletYukseklik.Text = palet.PaletYukseklik.ToString();
            }

            _seciliProgramKaydi = seciliProgram;
            ProgramiCanvasaYukle(seciliProgram);
        }

        private void ProgramiCanvasaYukle(ProgramKayitModel program)
        {
            if (program == null || urun == null || palet == null)
                return;

            var kayit = _dizilimKayitServisi.KayitBul(
                _dizilimKayitlari,
                program.UrunAdi ?? "",
                program.DizilimAdi ?? "");

            if (kayit == null)
            {
                CanvasiTemizle();
                return;
            }

            bool yuklendi = _katYonetici.DizilimYukle(
                kayit.DizilimAdi,
                urun,
                palet,
                OlcekX,
                OlcekY);

            if (!yuklendi)
            {
                CanvasiTemizle();
                return;
            }

            _katYonetici.KatiYukleDisardan(
                canvasPalet,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            ProgramGruplariniCanvasaUygula(program);
            txtKatValue.Text = "1";
        }

        private void ProgramGruplariniCanvasaUygula(ProgramKayitModel program)
        {
            if (program?.Gruplar == null || !program.Gruplar.Any())
                return;

            _secimServisi.TumGrupAtamalariniTemizle();
            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(canvasPalet);

            foreach (var katNo in _katYonetici.TumKatlar.Keys.ToList())
            {
                _bilgiServisi.KatAtamalariniKaydet(katNo, new List<GrupAtamaBilgisi>());
            }

            foreach (var grup in program.Gruplar.OrderBy(x => x.KatNo).ThenBy(x => x.GrupNo))
            {
                KataGecSessiz(grup.KatNo);

                var aktifKattakiKutular = canvasPalet.Children.OfType<Rectangle>().ToList();

                foreach (var urunModel in grup.Urunler)
                {
                    var eslesenKutu = EnYakinKutuyuBul(
                        aktifKattakiKutular,
                        urunModel.MerkezX,
                        urunModel.MerkezY);

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

                _bilgiServisi.KatAtamalariniKaydet(
                    grup.KatNo,
                    _secimServisi.GrupAtamalari.Values
                        .Where(x => x.KatNo == grup.KatNo)
                        .Select(x => new GrupAtamaBilgisi
                        {
                            KatNo = x.KatNo,
                            GrupNo = x.GrupNo,
                            GrupEkseni = x.GrupEkseni,
                            KoliAnahtari = x.KoliAnahtari
                        })
                        .ToList());
            }

            KatiYukle(1, true, true);
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

        private void KatiYukle(int yeniKat, bool atamalariYukle, bool grupGorselleriniYenile)
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
        }

        private void KataGecSessiz(int yeniKat)
        {
            _katYonetici.KatDegistir(
                yeniKat,
                canvasPalet,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKatValue.Text = _katYonetici.AktifKat.ToString();
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

        private void CanvasiTemizle()
        {
            canvasPalet.Children.OfType<Rectangle>().ToList().ForEach(r => canvasPalet.Children.Remove(r));
            _katYonetici.Temizle();
            _secimServisi.TumGrupAtamalariniTemizle();
            _gorsellestirmeServisi.TumGrupEtiketleriniTemizle(canvasPalet);
            txtKatValue.Text = "1";
        }

        private void BtnKatArti_Click(object sender, RoutedEventArgs e)
        {
            if (_katYonetici.TumKatlar == null || !_katYonetici.TumKatlar.Any())
                return;

            int hedefKat = _katYonetici.AktifKat + 1;

            if (!_katYonetici.TumKatlar.ContainsKey(hedefKat))
                return;

            KatiYukle(hedefKat, true, true);
        }

        private void BtnKatEksi_Click(object sender, RoutedEventArgs e)
        {
            if (_katYonetici.AktifKat <= 1)
                return;

            KatiYukle(_katYonetici.AktifKat - 1, true, true);
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void BtnProgramSil_Click(object sender, RoutedEventArgs e)
        {
            var seciliProgram = ListBoxProgramlar.SelectedItem as ProgramKayitModel;
            if (seciliProgram == null)
            {
                var uyari = new BildirimKutusu();
                uyari.MesajGonder("MesajKutusu.tamam", "Program.programSecmedenSilme");
                uyari.ShowDialog();
                return;
            }

            var onayKutusu = new OnayKutusu
            {
                Owner = Window.GetWindow(this)
            };

            string mesaj = string.Format(
                LanguageConverter.GetString("Program.silmeOnayMesaj"),
                seciliProgram.ProgramAdi);

            onayKutusu.MesajGonder(
                mesaj,
                LanguageConverter.GetString("MesajKutusu.evet"),
                LanguageConverter.GetString("MesajKutusu.hayir"));

            bool? sonuc = onayKutusu.ShowDialog();
            if (sonuc != true || !onayKutusu.OnaylandiMi)
                return;

            bool silindi = _programListeServisi.ProgramSil(seciliProgram.Id);

            if (!silindi)
            {
                var hata = new BildirimKutusu();
                hata.MesajGonder("MesajKutusu.tamam", "Program.programSilinemedi");
                hata.ShowDialog();
                return;
            }

            ProgramlariYukle();
            BilgileriTemizle();
            CanvasiTemizle();

            var bilgi = new BildirimKutusu();
            bilgi.MesajGonder("MesajKutusu.tamam", "Program.basariylaSilindi");
            bilgi.ShowDialog();
        }

        private void BilgileriTemizle()
        {
            txtProgramAdi.Text = ProgramSecilmediText;
            txtProgramAdi.ToolTip = ProgramSecilmediText;
            txtProgramID.Text = SecilmediText;
            txtUrunAdi.Text = UrunSecilmediText;
            txtUrunAdi.ToolTip = UrunSecilmediText;
            txtPaletAdi.Text = PaletSecilmediText;
            txtPaletAdi.ToolTip = PaletSecilmediText;

            txtUrunGenislik.Text = "-";
            txtUrunUzunluk.Text = "-";
            txtUrunBasinc.Text = "-";
            txtUrunYukseklik.Text = "-";
            txtUrunAgirlik.Text = "-";
            txtPaletGenislik.Text = "-";
            txtPaletUzunluk.Text = "-";
            txtPaletYukseklik.Text = "-";

            _seciliProgramKaydi = null;
            urun = null;
            palet = null;
        }

        private void BtnProgramBaslat_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}