using Newtonsoft.Json;
using Palet_Programlama.Modeller;
using Palet_Programlama.Sınıflar;
using Palet_Programlama.UserController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for DizilimYap.xaml
    /// </summary>
    public partial class DizilimYap : Page
    {

        private readonly Servisler.Palet.YerlesimMotoru _motor = new();
        private readonly Servisler.Palet.MesafeGostergesi _mesafe = new();
        private readonly Servisler.Palet.KatYoneticisi _katYonetici = new();
        private enum EklemeYon { Dikey, Yatay }
        private EklemeYon? _eklemeYon = null;

        private Urun _secilenUrun;
        private Palet _secilenPalet;

        private double OlcekY => myCanvas.Width / _secilenPalet.PaletBoy;
        private double OlcekX => myCanvas.Height / _secilenPalet.PaletEn;
        private string? _gelenDizilimAdi;
        private Point ilkTiklamaPozisyonu;
        private bool suruklemeBasladi = false;
        private const double suruklemeEsigi = 1.0;
        private bool surukleniyor = false;
        private Point tiklamaOffset;
        private Rectangle sonSecilmisKutu = new Rectangle();//palet içindeki son seçilmiş kolinin seçimini kaldırmak için yazdık
        private List<Rect> DigerKutular(Rectangle hareketEden) =>
        myCanvas.Children.OfType<Rectangle>()
            .Where(r => r != hareketEden)
            .Select(GetRect)
        .ToList();
        Rectangle suruklenenKutu; // Hareket ettirilen Rectangle
        private Frame MainFrame;

        public DizilimYap(Frame Main, Urun secilenUrun, Palet secilenPalet, string? dizilimAdi)
        {
            InitializeComponent();
            this.MainFrame = Main;

            _secilenPalet = secilenPalet;
            _secilenUrun = secilenUrun;
            txtUrunOzellikleri.Text = $"{_secilenUrun.UrunAdi} - {_secilenUrun.UrunEn} mm x {_secilenUrun.UrunBoy} mm x {_secilenUrun.UrunYukseklik} mm";
            txtPaletOzellikleri.Text = $"{_secilenPalet.PaletAdi} - {_secilenPalet.PaletEn} mm x {_secilenPalet.PaletBoy} mm x {_secilenPalet.PaletYukseklik} mm";
            _mesafe.Baslat(myCanvas);
            _motor.SnapEsigi = 3.0;
            _motor.CakismaEpsilon = 0.5;
            txtKat.Text = _katYonetici.AktifKat.ToString();
            _gelenDizilimAdi = dizilimAdi;
            if (!string.IsNullOrWhiteSpace(_gelenDizilimAdi))
            {
                bool yüklendi = _katYonetici.DizilimYukle(
                    _gelenDizilimAdi,
                    _secilenUrun,
                    _secilenPalet,
                    OlcekX,
                    OlcekY);

                if (yüklendi)
                {
                    _katYonetici.KatiYukleDisardan(
                        myCanvas,
                        Rectangle_MouseDown,
                        Rectangle_MouseMove,
                        Rectangle_MouseUp);

                    txtKat.Text = _katYonetici.AktifKat.ToString();
                }
                else
                {
                    BildirimGoster("MesajKutusu.dizilimBulunamadi");
                }
            }
        }





        private Rect GetRect(Rectangle r)
        {
            double left = Canvas.GetLeft(r);
            double top = Canvas.GetTop(r);

            // Actual 0 ise Width/Height kullan
            double w = (r.ActualWidth > 0) ? r.ActualWidth : r.Width;
            double h = (r.ActualHeight > 0) ? r.ActualHeight : r.Height;

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            return new Rect(left, top, w, h);
        }


        // MouseDown olayında sürükleme başlat
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            suruklenenKutu = (Rectangle)sender;

            _mesafe.Goster();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                surukleniyor = true;
                suruklemeBasladi = false;
                ilkTiklamaPozisyonu = e.GetPosition(suruklenenKutu);
                tiklamaOffset = e.GetPosition(suruklenenKutu);
                suruklenenKutu.CaptureMouse();
                var digerKutular = DigerKutular(suruklenenKutu);
            }
        }

        // MouseMove olayında öğeyi hareket ettir
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (surukleniyor && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentMousePosition = e.GetPosition(suruklenenKutu);

                // Farenin ilk tıklama pozisyonundan yeterince uzaklaşıp uzaklaşmadığını kontrol edin
                if (!suruklemeBasladi && (Math.Abs(currentMousePosition.X - ilkTiklamaPozisyonu.X) > suruklemeEsigi ||
                                          Math.Abs(currentMousePosition.Y - ilkTiklamaPozisyonu.Y) > suruklemeEsigi))
                {
                    // Sürükleme başladı
                    suruklemeBasladi = true;
                }

                if (suruklemeBasladi)
                {


                    var movingNow = GetRect(suruklenenKutu);
                    double w = movingNow.Width;
                    double h = movingNow.Height;


                    sonSecilmisKutu.Stroke = Brushes.Transparent;
                    sonSecilmisKutu.StrokeThickness = 0;
                    var position = e.GetPosition(myCanvas);

                    // mouse offset ile hedef sol-üst
                    var hedef = new Point(position.X - tiklamaOffset.X, position.Y - tiklamaOffset.Y);

                    // diğer kutular
                    var digerKutular = DigerKutular(suruklenenKutu);

                    // motor hesaplasın
                    if (_motor.TrySurukle(movingNow, hedef, myCanvas.Width, myCanvas.Height, digerKutular, out var sonuc))
                    {
                        Canvas.SetLeft(suruklenenKutu, sonuc.Left);
                        Canvas.SetTop(suruklenenKutu, sonuc.Top);
                        _mesafe.Guncelle(sonuc, digerKutular);
                    }
                    else
                    {
                        // hareket yok ama mesafe göstergesi güncel kalsın
                        _mesafe.Guncelle(movingNow, digerKutular);
                    }

                }
            }



        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (suruklemeBasladi)
            {
                _mesafe.Gizle();
            }
            else
            {
                if (suruklenenKutu == null) { return; }

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
            suruklemeBasladi = false;
            surukleniyor = false;
            suruklenenKutu?.ReleaseMouseCapture();


        }

        private void dikeyKutu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_eklemeYon == EklemeYon.Dikey)
            {
                // toggle off
                _eklemeYon = null;
                dikeyKutu.StrokeThickness = 0;
                dikeyKutu.Stroke = Brushes.Transparent;
                return;
            }

            dikeyKutu.StrokeThickness = 1;
            yatayKutu.StrokeThickness = 0;
            dikeyKutu.Stroke = Brushes.Red;
            yatayKutu.Stroke = Brushes.Transparent;
            _eklemeYon = EklemeYon.Dikey;
        }


        private void yatayKutu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_eklemeYon == EklemeYon.Yatay)
            {
                // toggle off
                _eklemeYon = null;
                yatayKutu.StrokeThickness = 0;
                yatayKutu.Stroke = Brushes.Transparent;
                return;
            }

            dikeyKutu.StrokeThickness = 0;
            yatayKutu.StrokeThickness = 1;
            dikeyKutu.Stroke = Brushes.Transparent;
            yatayKutu.Stroke = Brushes.Red;
            _eklemeYon = EklemeYon.Yatay;  // yatayda
        }

        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_eklemeYon is null)
                return;

            if (e.OriginalSource is Rectangle) return;

            var p = e.GetPosition(myCanvas);

            var (w, h) = CanvasUrunBoyutuGetir(_eklemeYon.Value);

            double newLeft = p.X;
            double newTop = p.Y;

            double maxLeft = myCanvas.Width - w;
            double maxTop = myCanvas.Height - h;
            newLeft = Math.Max(0, Math.Min(newLeft, maxLeft));
            newTop = Math.Max(0, Math.Min(newTop, maxTop));

            var candidate = new Rect(newLeft, newTop, w, h);

            if (_motor.CakisiyorMu(candidate, myCanvas.Children.OfType<Rectangle>().Select(GetRect)))
            {
                BildirimGoster("MesajKutusu.urunlerUstUsteGelemez");
                return;
            }

            var rect = YeniUrunOlustur(w, h, _eklemeYon);
            rect.Tag = (_eklemeYon == EklemeYon.Dikey)
                ? Modeller.UrunYonu.Dikey
                : Modeller.UrunYonu.Yatay;

            Canvas.SetLeft(rect, newLeft);
            Canvas.SetTop(rect, newTop);
            myCanvas.Children.Add(rect);
        }


        private Rectangle YeniUrunOlustur(double w, double h, EklemeYon? yon)
        {
            var resim = yon == EklemeYon.Dikey
                ? "pack://application:,,,/Resimler/DizilimYap/dikey_kutu.png"
                : "pack://application:,,,/Resimler/DizilimYap/yatay_kutu.png";

            var rect = new Rectangle
            {
                Width = w,
                Height = h,
                Stroke = Brushes.Transparent,
                StrokeThickness = 0,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(resim, UriKind.Absolute))
                }
            };

            rect.MouseDown += Rectangle_MouseDown;
            rect.MouseMove += Rectangle_MouseMove;
            rect.MouseUp += Rectangle_MouseUp;

            return rect;
        }

        private void btnKatArti_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (!myCanvas.Children.OfType<Rectangle>().Any())
            {
                BildirimGoster("MesajKutusu.katBosOlamaz");
                return;
            }

            if (_katYonetici.AktifKat >= _katYonetici.MaksKat)
            {
                BildirimGoster("MesajKutusu.maksKatUyari");
                return;
            }


            _mesafe.Gizle();

            int yeniKat = _katYonetici.AktifKat + 1;

            _katYonetici.KatDegistir(
                yeniKat,
                myCanvas,
                sonSecilmisKutu,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            txtKat.Text = _katYonetici.AktifKat.ToString();
        }

        private void btnKatEksi_MouseDown(object sender, MouseButtonEventArgs e)
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

            txtKat.Text = _katYonetici.AktifKat.ToString();

        }

        private void BtnKopyalaArti_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt32(txtKopyalaValue.Text) + 1 == Convert.ToInt32(txtYapisValue.Text))
            {
                txtKopyalaValue.Text = Convert.ToString(Convert.ToInt32(txtKopyalaValue.Text) + 2);
                return;
            }
            txtKopyalaValue.Text = Convert.ToString(Convert.ToInt32(txtKopyalaValue.Text) + 1);
        }


        private void BtnYapistirArti_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (Convert.ToInt32(txtKopyalaValue.Text) == Convert.ToInt32(txtYapisValue.Text) + 1)
            {
                txtYapisValue.Text = Convert.ToString(Convert.ToInt32(txtYapisValue.Text) + 2);
                return;
            }

            txtYapisValue.Text = Convert.ToString(Convert.ToInt32(txtYapisValue.Text) + 1);
        }


        private void BtnKopyalaEksi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt32(txtKopyalaValue.Text) == 1)
            {
                return;
            }

            if (Convert.ToInt32(txtKopyalaValue.Text) - 1 == Convert.ToInt32(txtYapisValue.Text))
            {
                if (Convert.ToInt32(txtKopyalaValue.Text) - 2 == 0) { return; }
                txtKopyalaValue.Text = Convert.ToString(Convert.ToInt32(txtKopyalaValue.Text) - 2);
                return;
            }
            txtKopyalaValue.Text = Convert.ToString(Convert.ToInt32(txtKopyalaValue.Text) - 1);

        }



        private void BtnYapistirEksi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt32(txtYapisValue.Text) == 1)
            {
                return;
            }
            if (Convert.ToInt32(txtKopyalaValue.Text) == Convert.ToInt32(txtYapisValue.Text) - 1)
            {

                if (Convert.ToInt32(txtYapisValue.Text) - 2 == 0) { return; }

                txtYapisValue.Text = Convert.ToString(Convert.ToInt32(txtYapisValue.Text) - 2);
                return;
            }


            txtYapisValue.Text = Convert.ToString(Convert.ToInt32(txtYapisValue.Text) - 1);
        }



        private void BtnHareketMiktariArti_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtHareketMiktari.Text = Convert.ToString(Convert.ToInt32(txtHareketMiktari.Text) + 1);
        }

        private void BtnHareketMiktariEksi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt32(txtHareketMiktari.Text) == 1)
            {
                return;
            }
            txtHareketMiktari.Text = Convert.ToString(Convert.ToInt32(txtHareketMiktari.Text) - 1);
        }

        private DizilimKayitModel DizilimKayitModeliOlustur(string dizilimAdi)
        {
            var model = new DizilimKayitModel
            {
                DizilimAdi = dizilimAdi,
                PaletAdi = _secilenPalet.PaletAdi,
                PaletEn = _secilenPalet.PaletEn,
                PaletBoy = _secilenPalet.PaletBoy,
                PaletYukseklik = _secilenPalet.PaletYukseklik,
                UrunAdi = _secilenUrun.UrunAdi,
                UrunEn = _secilenUrun.UrunEn,
                UrunBoy = _secilenUrun.UrunBoy,
                UrunYukseklik = _secilenUrun.UrunYukseklik
            };

            foreach (var kat in _katYonetici.TumKatlar
                .Where(x => x.Value != null && x.Value.Any())
                .OrderBy(x => x.Key))
            {
                int katNo = kat.Key;

                foreach (var urun in kat.Value)
                {
                    double gercekMerkezX = CanvasMerkezDikeydenGercekX(urun.MerkezY);
                    double gercekMerkezY = CanvasMerkezYataydanGercekY(urun.MerkezX);

                    model.Urunler.Add(new DizilimUrunKayitModel
                    {
                        Yon = urun.Yon.ToString(),
                        KatNo = katNo,
                        MerkezX = gercekMerkezX,
                        MerkezY = gercekMerkezY,
                        MerkezZ = MerkezZHesapla(katNo)
                    });
                }
            }

            return model;
        }


        private void DizilimiJsonDosyasinaKaydet()
        {
            try
            {
                _katYonetici.KatiKaydetDisardan(myCanvas);

                bool hicUrunYokMu = !_katYonetici.TumKatlar.Any(kat => kat.Value != null && kat.Value.Any());
                if (hicUrunYokMu)
                {
                    BildirimGoster("MesajKutusu.kayitIcinUrunGerekli");
                    return;
                }

                string varsayilanAd = string.IsNullOrWhiteSpace(_gelenDizilimAdi)
                    ? ""
                    : _gelenDizilimAdi;

                var pencere = new MetinGirisKutusu();
                pencere.Ayarla(
                    "Dizilim Kaydet",
                    "Kaydetmek istediğiniz dizilim için bir isim giriniz.",
                    varsayilanAd,
                    "Kaydet",
                    "İptal");

                bool? sonuc = pencere.ShowDialog();
                if (sonuc != true)
                {
                    return;
                }

                string dizilimAdi = (pencere.GirilenMetin ?? "").Trim();
                if (string.IsNullOrWhiteSpace(dizilimAdi))
                {
                    BildirimGoster("MesajKutusu.kayitIcinIsimGerekli");
                    return;
                }

                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                List<DizilimKayitModel> tumDizilimler;

                if (!File.Exists(dosyaYolu))
                {
                    tumDizilimler = new List<DizilimKayitModel>();
                }
                else
                {
                    string mevcutJson = File.ReadAllText(dosyaYolu);

                    if (string.IsNullOrWhiteSpace(mevcutJson))
                    {
                        tumDizilimler = new List<DizilimKayitModel>();
                    }
                    else
                    {
                        tumDizilimler = JsonConvert.DeserializeObject<List<DizilimKayitModel>>(mevcutJson)
                                        ?? new List<DizilimKayitModel>();
                    }
                }

                bool guncellemeModu = !string.IsNullOrWhiteSpace(_gelenDizilimAdi);

                if (guncellemeModu)
                {
                    var eskiKayit = tumDizilimler.FirstOrDefault(x =>
                        string.Equals(x.DizilimAdi, _gelenDizilimAdi, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(x.PaletAdi, _secilenPalet.PaletAdi, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(x.UrunAdi, _secilenUrun.UrunAdi, StringComparison.OrdinalIgnoreCase));

                    if (eskiKayit == null)
                    {
                        BildirimGoster("MesajKutusu.dizilimBulunamadi");
                        return;
                    }

                    bool baskaKayittaAyniAdVarMi = tumDizilimler.Any(x =>
                        !object.ReferenceEquals(x, eskiKayit) &&
                        string.Equals(x.DizilimAdi, dizilimAdi, StringComparison.OrdinalIgnoreCase));

                    if (baskaKayittaAyniAdVarMi)
                    {
                        BildirimGoster("MesajKutusu.farkliDizilimAdiGir");
                        return;
                    }

                    var yeniDizilim = DizilimKayitModeliOlustur(dizilimAdi);

                    int index = tumDizilimler.IndexOf(eskiKayit);
                    tumDizilimler[index] = yeniDizilim;

                    string yeniJson = JsonConvert.SerializeObject(tumDizilimler, Formatting.Indented);
                    File.WriteAllText(dosyaYolu, yeniJson);

                    _gelenDizilimAdi = dizilimAdi;

                    BildirimGoster("MesajKutusu.guncellemeBasarili");
                    return;
                }
                else
                {
                    bool ayniIsimdeVarMi = tumDizilimler.Any(x =>
                        string.Equals(x.DizilimAdi, dizilimAdi, StringComparison.OrdinalIgnoreCase));

                    if (ayniIsimdeVarMi)
                    {
                        BildirimGoster("MesajKutusu.farkliDizilimAdiGir");
                        return;
                    }

                    var yeniDizilim = DizilimKayitModeliOlustur(dizilimAdi);
                    tumDizilimler.Add(yeniDizilim);

                    string yeniJson = JsonConvert.SerializeObject(tumDizilimler, Formatting.Indented);
                    File.WriteAllText(dosyaYolu, yeniJson);

                    _gelenDizilimAdi = dizilimAdi;

                    BildirimGoster("MesajKutusu.kayitBasarili");
                }
            }
            catch (Exception ex)
            {
                BildirimGosterFormatli("MesajKutusu.kayitHatasiFormat", ex.Message);
            }
        }

        private void BildirimGosterFormatli(string mesajKey, params object[] args)
        {
            var pencere = new Palet_Programlama.UserController.BildirimKutusu();
            pencere.MesajGonderFormatli("MesajKutusu.tamam", mesajKey, args);
            pencere.ShowDialog();
        }

        private double MerkezZHesapla(int katNo)
        {
            return _secilenPalet.PaletYukseklik
                   + (katNo * _secilenUrun.UrunYukseklik)
                   - (_secilenUrun.UrunYukseklik / 2.0);
        }

        private double CanvasMerkezYataydanGercekY(double canvasMerkezYatay)
        {
            return canvasMerkezYatay / OlcekY;
        }

        private double CanvasMerkezDikeydenGercekX(double canvasMerkezDikey)
        {
            return canvasMerkezDikey / OlcekX;
        }

        private (double w, double h) CanvasUrunBoyutuGetir(EklemeYon yon)
        {
            double urunEn = _secilenUrun.UrunEn;
            double urunBoy = _secilenUrun.UrunBoy;

            double olcekX = OlcekX;
            double olcekY = OlcekY;

            if (yon == EklemeYon.Dikey)
            {
                double w = urunEn * olcekY;
                double h = urunBoy * olcekX;
                return (w, h);
            }
            else
            {
                double w = urunBoy * olcekY;
                double h = urunEn * olcekX;
                return (w, h);
            }
        }



        private void Dizilimi_Kaydet_Click(object sender, RoutedEventArgs e)
        {

            DizilimiJsonDosyasinaKaydet();
        }




        private void AynalamaYazisiniGuncelle()
        {
            string dilAnahtari =
                (cbYEkseni.IsChecked == true || cbXEkseni.IsChecked == true)
                ? "DizilimYap.txtAynalamaAcik"
                : "DizilimYap.txtAynalama";

            var converter = (LanguageConverter)FindResource("LanguageConverter");
            txtAynamala.Text = converter.Convert(dilAnahtari, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture)?.ToString();
        }


        private void cbYEkseni_Click(object sender, RoutedEventArgs e)
        {
            AynalamaYazisiniGuncelle();
        }

        private void cbXEkseni_Click(object sender, RoutedEventArgs e)
        {
            AynalamaYazisiniGuncelle();
        }

        private void Btn_KatKopayala_Click(object sender, RoutedEventArgs e)
        {
            _katYonetici.KatiKaydetDisardan(myCanvas);

            int kopyalanacakKat = Convert.ToInt32(txtKopyalaValue.Text);
            int yapistirilacakKat = Convert.ToInt32(txtYapisValue.Text);

            if (kopyalanacakKat == yapistirilacakKat)
            {
                BildirimGoster("MesajKutusu.katKendineKopyalanamaz");
                return;
            }

            bool xAynala = cbXEkseni.IsChecked == true;
            bool yAynala = cbYEkseni.IsChecked == true;

            bool basarili = _katYonetici.KatKopyala(
                kopyalanacakKat,
                yapistirilacakKat,
                xAynala,
                yAynala,
                myCanvas.Width,
                myCanvas.Height);

            if (!basarili)
            {
                BildirimGoster("MesajKutusu.katKopyalamaBasarisiz");
                return;
            }
            BildirimGoster("MesajKutusu.katKopyalamaBasarili");
        }

        private void BtnYukari_Click(object sender, RoutedEventArgs e)
        {

            if (CokluEklemeModuAcikMi())
            {
                var secili = SeciliKutuyuGetir();
                if (secili == null)
                {
                    BildirimGoster("MesajKutusu.urunSecimiGerekli");
                    return;
                }

                SeciliUrundenCokluKopyaOlustur(0, -secili.Height);
                return;
            }

            if (HizalamaModuAcikMi())
            {
                SeciliUrunuHizala("Yukari");
                return;
            }

            if (KatTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketMm2))
                    return;

                double paletDeltaTopPx = -(hareketMm2 * OlcekX);
                AktifKattakiTumUrunleriTasi(0, paletDeltaTopPx);
                return;
            }

            if (PaletTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketPalet))
                    return;

                double paletDeltaLeftPx = hareketPalet * OlcekY;
                PalettekiTumKatlariTasi(0, -(hareketPalet * OlcekX));
                return;
            }


            if (!HareketMiktariniAl(out double hareketMm))
                return;

            double deltaTopPx = -(hareketMm * OlcekX);
            SeciliUrunuHareketEttir(0, deltaTopPx);
        }

        private void BtnAsagi_Click(object sender, RoutedEventArgs e)
        {

            if (CokluEklemeModuAcikMi())
            {
                var secili = SeciliKutuyuGetir();
                if (secili == null)
                {
                    BildirimGoster("MesajKutusu.urunSecimiGerekli");
                    return;
                }

                SeciliUrundenCokluKopyaOlustur(0, secili.Height);
                return;
            }

            if (HizalamaModuAcikMi())
            {
                SeciliUrunuHizala("Asagi");
                return;
            }


            if (KatTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketMm2))
                    return;

                double paletDeltaTopPx = hareketMm2 * OlcekX;
                AktifKattakiTumUrunleriTasi(0, paletDeltaTopPx);
                return;
            }

            if (PaletTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketPalet))
                    return;

                double paletDeltaLeftPx = hareketPalet * OlcekY;
                PalettekiTumKatlariTasi(0, hareketPalet * OlcekX);
                return;
            }

            if (!HareketMiktariniAl(out double hareketMm))
                return;

            double deltaTopPx = hareketMm * OlcekX;
            SeciliUrunuHareketEttir(0, deltaTopPx);
        }

        private void BtnSag_Click(object sender, RoutedEventArgs e)
        {

            if (CokluEklemeModuAcikMi())
            {
                var secili = SeciliKutuyuGetir();
                if (secili == null)
                {
                    BildirimGoster("MesajKutusu.urunSecimiGerekli");
                    return;
                }

                SeciliUrundenCokluKopyaOlustur(secili.Width, 0);
                return;
            }

            if (HizalamaModuAcikMi())
            {
                SeciliUrunuHizala("Sag");
                return;
            }


            if (KatTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketMm2))
                    return;

                double paletDeltaLeftPx = hareketMm2 * OlcekY;
                AktifKattakiTumUrunleriTasi(paletDeltaLeftPx, 0);
                return;
            }

            if (PaletTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketPalet))
                    return;

                double paletDeltaLeftPx = hareketPalet * OlcekY;
                PalettekiTumKatlariTasi(paletDeltaLeftPx, 0);
                return;
            }


            if (!HareketMiktariniAl(out double hareketMm))
                return;

            double deltaLeftPx = hareketMm * OlcekY;
            SeciliUrunuHareketEttir(deltaLeftPx, 0);
        }

        private void BtnSol_Click(object sender, RoutedEventArgs e)
        {

            if (CokluEklemeModuAcikMi())
            {
                var secili = SeciliKutuyuGetir();
                if (secili == null)
                {

                    BildirimGoster("MesajKutusu.urunSecimiGerekli");
                    return;
                }

                SeciliUrundenCokluKopyaOlustur(-secili.Width, 0);
                return;
            }

            if (HizalamaModuAcikMi())
            {
                SeciliUrunuHizala("Sol");
                return;
            }


            if (KatTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketMm2))
                    return;

                double paletDeltaLeftPx = -(hareketMm2 * OlcekY);
                AktifKattakiTumUrunleriTasi(paletDeltaLeftPx, 0);
                return;
            }


            if (PaletTasimaModuAcikMi())
            {
                if (!HareketMiktariniAl(out double hareketPalet))
                    return;

                double paletDeltaLeftPx = hareketPalet * OlcekY;
                PalettekiTumKatlariTasi(-(hareketPalet * OlcekY), 0);
                return;
            }



            if (!HareketMiktariniAl(out double hareketMm))
                return;

            double deltaLeftPx = -(hareketMm * OlcekY);
            SeciliUrunuHareketEttir(deltaLeftPx, 0);
        }


        private Rectangle? SeciliKutuyuGetir()
        {
            if (sonSecilmisKutu == null)
                return null;

            if (!myCanvas.Children.OfType<Rectangle>().Contains(sonSecilmisKutu))
                return null;

            if (sonSecilmisKutu.StrokeThickness <= 0)
                return null;

            return sonSecilmisKutu;
        }

        private bool HareketMiktariniAl(out double hareketMm)
        {
            hareketMm = 0;

            if (!double.TryParse(txtHareketMiktari.Text, out hareketMm))
                return false;

            if (hareketMm <= 0)
                return false;

            return true;
        }

        private void SeciliUrunuHareketEttir(double deltaLeftPx, double deltaTopPx)
        {
            var secili = SeciliKutuyuGetir();
            if (secili == null)
            {
                BildirimGoster("MesajKutusu.urunSecimiGerekli");
                return;
            }

            var mevcut = GetRect(secili);

            double yeniLeft = mevcut.Left + deltaLeftPx;
            double yeniTop = mevcut.Top + deltaTopPx;

            // Palet dışına çıkma kontrolü
            if (yeniLeft < 0 ||
                yeniTop < 0 ||
                yeniLeft + mevcut.Width > myCanvas.Width ||
                yeniTop + mevcut.Height > myCanvas.Height)
            {
                return;
            }

            var candidate = new Rect(yeniLeft, yeniTop, mevcut.Width, mevcut.Height);

            // Çarpışma kontrolü
            var digerKutular = DigerKutular(secili);
            if (_motor.CakisiyorMu(candidate, digerKutular))
            {
                return;
            }

            Canvas.SetLeft(secili, yeniLeft);
            Canvas.SetTop(secili, yeniTop);

            _mesafe.Guncelle(candidate, digerKutular);
            _mesafe.Goster();
        }

        private void cbCokluEkleme_Click(object sender, RoutedEventArgs e)
        {
            TekModSec(cbCokluEkleme);
        }

        private void cbHizalama_Click(object sender, RoutedEventArgs e)
        {

            TekModSec(cbHizalama);
        }

        private void cbPaletTasima_Click(object sender, RoutedEventArgs e)
        {

            TekModSec(cbPaletTasima);
        }

        private void cbKatTasima_Click(object sender, RoutedEventArgs e)
        {
            TekModSec(cbKatTasima);
        }

        private Rectangle SeciliKutudanKopyaOlustur(Rectangle kaynak)
        {
            double w = kaynak.Width;
            double h = kaynak.Height;

            var yon = kaynak.Tag is Modeller.UrunYonu t
                ? t
                : (w >= h ? Modeller.UrunYonu.Yatay : Modeller.UrunYonu.Dikey);

            string resim = yon == Modeller.UrunYonu.Dikey
                ? "pack://application:,,,/Resimler/DizilimYap/dikey_kutu.png"
                : "pack://application:,,,/Resimler/DizilimYap/yatay_kutu.png";

            var rect = new Rectangle
            {
                Width = w,
                Height = h,
                Stroke = Brushes.Transparent,
                StrokeThickness = 0,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(resim, UriKind.Absolute))
                },
                Tag = yon
            };

            rect.MouseDown += Rectangle_MouseDown;
            rect.MouseMove += Rectangle_MouseMove;
            rect.MouseUp += Rectangle_MouseUp;

            return rect;
        }


        private bool CokluEklemeModuAcikMi()
        {
            return cbCokluEkleme.IsChecked == true;
        }

        private void ModYazilariniGuncelle()
        {
            var converter = (LanguageConverter)FindResource("LanguageConverter");

            txtCokluEklemeMod.Text =
                converter.Convert(
                    cbCokluEkleme.IsChecked == true
                        ? "DizilimYap.txtCokluEkleModAcik"
                        : "DizilimYap.txtCokluEkleMod",
                    typeof(string),
                    null,
                    System.Globalization.CultureInfo.CurrentCulture)?.ToString();

            txtHizalamaMod.Text =
                converter.Convert(
                    cbHizalama.IsChecked == true
                        ? "DizilimYap.txtHizalamaModAcik"
                        : "DizilimYap.txtHizalamaMod",
                    typeof(string),
                    null,
                    System.Globalization.CultureInfo.CurrentCulture)?.ToString();

            txtPaletTasimaMod.Text =
                converter.Convert(
                    cbPaletTasima.IsChecked == true
                        ? "DizilimYap.txtPaletTasimaModAcik"
                        : "DizilimYap.txtPaletTasimaMod",
                    typeof(string),
                    null,
                    System.Globalization.CultureInfo.CurrentCulture)?.ToString();

            txtKatTasimaMod.Text =
                converter.Convert(
                    cbKatTasima.IsChecked == true
                        ? "DizilimYap.txtKatTasimaModAcik"
                        : "DizilimYap.txtKatTasimaMod",
                    typeof(string),
                    null,
                    System.Globalization.CultureInfo.CurrentCulture)?.ToString();
        }

        private void SeciliUrundenCokluKopyaOlustur(double adimLeft, double adimTop)
        {
            var secili = SeciliKutuyuGetir();
            if (secili == null)
            {
                BildirimGoster("MesajKutusu.urunSecimiGerekli");
                return;
            }

            var kaynakRect = GetRect(secili);

            double w = kaynakRect.Width;
            double h = kaynakRect.Height;

            double yeniLeft = kaynakRect.Left + adimLeft;
            double yeniTop = kaynakRect.Top + adimTop;

            while (true)
            {
                // palet dışı kontrol
                if (yeniLeft < 0 ||
                    yeniTop < 0 ||
                    yeniLeft + w > myCanvas.Width ||
                    yeniTop + h > myCanvas.Height)
                {
                    break;
                }

                var candidate = new Rect(yeniLeft, yeniTop, w, h);

                // mevcut tüm ürünlerle çarpışma kontrolü
                var tumKutular = myCanvas.Children
                    .OfType<Rectangle>()
                    .Select(GetRect)
                    .ToList();

                if (_motor.CakisiyorMu(candidate, tumKutular))
                {
                    break;
                }

                var yeniRect = SeciliKutudanKopyaOlustur(secili);
                Canvas.SetLeft(yeniRect, yeniLeft);
                Canvas.SetTop(yeniRect, yeniTop);
                myCanvas.Children.Add(yeniRect);

                yeniLeft += adimLeft;
                yeniTop += adimTop;
            }
        }

        private void TekModSec(CheckBox secilen)
        {
            if (secilen.IsChecked == true)
            {
                if (secilen != cbCokluEkleme) cbCokluEkleme.IsChecked = false;
                if (secilen != cbHizalama) cbHizalama.IsChecked = false;
                if (secilen != cbPaletTasima) cbPaletTasima.IsChecked = false;
                if (secilen != cbKatTasima) cbKatTasima.IsChecked = false;
            }

            ModYazilariniGuncelle();
        }



        private bool HizalamaModuAcikMi()
        {
            return cbHizalama.IsChecked == true;
        }


        private bool DikeyAralikCakisiyor(Rect a, Rect b)
        {
            return a.Top < b.Bottom && a.Bottom > b.Top;
        }

        private bool YatayAralikCakisiyor(Rect a, Rect b)
        {
            return a.Left < b.Right && a.Right > b.Left;
        }

        private void SeciliUrunuHizala(string yon)
        {
            var secili = SeciliKutuyuGetir();
            if (secili == null)
            {
                BildirimGoster("MesajKutusu.urunSecimiGerekli");
                return;
            }

            var mevcut = GetRect(secili);
            var digerKutular = DigerKutular(secili);

            double yeniLeft = mevcut.Left;
            double yeniTop = mevcut.Top;

            if (yon == "Sag")
            {
                double hedefLeft = myCanvas.Width - mevcut.Width;

                foreach (var diger in digerKutular)
                {
                    if (!DikeyAralikCakisiyor(mevcut, diger))
                        continue;

                    // sağdaki engeller
                    if (diger.Left >= mevcut.Right)
                    {
                        double adayLeft = diger.Left - mevcut.Width;
                        if (adayLeft < hedefLeft)
                            hedefLeft = adayLeft;
                    }
                }

                yeniLeft = hedefLeft;
            }
            else if (yon == "Sol")
            {
                double hedefLeft = 0;

                foreach (var diger in digerKutular)
                {
                    if (!DikeyAralikCakisiyor(mevcut, diger))
                        continue;

                    // soldaki engeller
                    if (diger.Right <= mevcut.Left)
                    {
                        double adayLeft = diger.Right;
                        if (adayLeft > hedefLeft)
                            hedefLeft = adayLeft;
                    }
                }

                yeniLeft = hedefLeft;
            }
            else if (yon == "Asagi")
            {
                double hedefTop = myCanvas.Height - mevcut.Height;

                foreach (var diger in digerKutular)
                {
                    if (!YatayAralikCakisiyor(mevcut, diger))
                        continue;

                    // aşağıdaki engeller
                    if (diger.Top >= mevcut.Bottom)
                    {
                        double adayTop = diger.Top - mevcut.Height;
                        if (adayTop < hedefTop)
                            hedefTop = adayTop;
                    }
                }

                yeniTop = hedefTop;
            }
            else if (yon == "Yukari")
            {
                double hedefTop = 0;

                foreach (var diger in digerKutular)
                {
                    if (!YatayAralikCakisiyor(mevcut, diger))
                        continue;

                    // yukarıdaki engeller
                    if (diger.Bottom <= mevcut.Top)
                    {
                        double adayTop = diger.Bottom;
                        if (adayTop > hedefTop)
                            hedefTop = adayTop;
                    }
                }

                yeniTop = hedefTop;
            }

            var candidate = new Rect(yeniLeft, yeniTop, mevcut.Width, mevcut.Height);

            if (_motor.CakisiyorMu(candidate, digerKutular))
                return;

            Canvas.SetLeft(secili, yeniLeft);
            Canvas.SetTop(secili, yeniTop);

            _mesafe.Guncelle(candidate, digerKutular);
            _mesafe.Goster();
        }



        private void PalettekiTumKatlariTasi(double deltaLeftPx, double deltaTopPx)
        {
            _katYonetici.KatiKaydetDisardan(myCanvas);

            bool basarili = _katYonetici.TumKatlariTasi(
                deltaLeftPx,
                deltaTopPx,
                myCanvas.Width,
                myCanvas.Height);

            if (!basarili)
                return;

            _katYonetici.KatiYukleDisardan(
                myCanvas,
                Rectangle_MouseDown,
                Rectangle_MouseMove,
                Rectangle_MouseUp);

            var secili = SeciliKutuyuGetir();
            if (secili != null)
            {
                var seciliRect = GetRect(secili);
                var digerKutular = DigerKutular(secili);
                _mesafe.Guncelle(seciliRect, digerKutular);
                _mesafe.Goster();
            }
            else
            {
                _mesafe.Gizle();
            }
        }


        private bool KatTasimaModuAcikMi()
        {
            return cbKatTasima.IsChecked == true;
        }

        private bool PaletTasimaModuAcikMi()
        {
            return cbPaletTasima.IsChecked == true;
        }

        private void AktifKattakiTumUrunleriTasi(double deltaLeftPx, double deltaTopPx)
        {
            var kutular = myCanvas.Children.OfType<Rectangle>().ToList();

            if (!kutular.Any())
            {
                BildirimGoster("MesajKutusu.tasinacakUrunYok");
                return;
            }

            // Önce kontrol: herhangi biri dışarı çıkacak mı?
            foreach (var kutu in kutular)
            {
                var rect = GetRect(kutu);

                double yeniLeft = rect.Left + deltaLeftPx;
                double yeniTop = rect.Top + deltaTopPx;

                if (yeniLeft < 0 ||
                    yeniTop < 0 ||
                    yeniLeft + rect.Width > myCanvas.Width ||
                    yeniTop + rect.Height > myCanvas.Height)
                {
                    return;
                }
            }

            // Hepsi uygunsa birlikte taşı
            foreach (var kutu in kutular)
            {
                var rect = GetRect(kutu);

                Canvas.SetLeft(kutu, rect.Left + deltaLeftPx);
                Canvas.SetTop(kutu, rect.Top + deltaTopPx);
            }

            // Seçili kutu varsa mesafeyi güncelle
            var secili = SeciliKutuyuGetir();
            if (secili != null)
            {
                var seciliRect = GetRect(secili);
                var digerKutular = DigerKutular(secili);
                _mesafe.Guncelle(seciliRect, digerKutular);
                _mesafe.Goster();
            }
            else
            {
                _mesafe.Gizle();
            }
        }



        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new UserController.BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void SeciliUrunuSil()
        {
            var secili = SeciliKutuyuGetir();
            if (secili == null)
            {
                BildirimGoster("MesajKutusu.urunSecimiGerekli");
                return;
            }

            myCanvas.Children.Remove(secili);

            sonSecilmisKutu = new Rectangle();
            _mesafe.Gizle();

            _katYonetici.KatiKaydetDisardan(myCanvas);
        }

        private void AktifKatiTemizle()
        {
            _katYonetici.KatiKaydetDisardan(myCanvas);

            if (_katYonetici.AraKatMiVeSilinemez())
            {
                BildirimGoster("MesajKutusu.araKatSilinemez");
                return;
            }

            _katYonetici.AktifKatiTemizle(myCanvas);

            sonSecilmisKutu = new Rectangle();
            _mesafe.Gizle();
        }

        private void TumPaletiTemizle()
        {
            _katYonetici.TumKatlariTemizle(myCanvas);

            txtKat.Text = _katYonetici.AktifKat.ToString();
            sonSecilmisKutu = new Rectangle();
            _mesafe.Gizle();
        }

        private void BtnUrunSil_Click(object sender, RoutedEventArgs e)
        {
            SeciliUrunuSil();
        }

        private void BtnPaletTemizle_Click(object sender, RoutedEventArgs e)
        {
            TumPaletiTemizle();
        }

        private void BtnKatTemizle_Click(object sender, RoutedEventArgs e)
        {
            AktifKatiTemizle();
        }
    }
}
