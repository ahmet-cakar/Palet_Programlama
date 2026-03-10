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

        public DizilimYap(Frame Main,Urun secilenUrun, Palet secilenPalet)
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
                MessageBox.Show("Bu konumda başka bir ürün var. Üst üste gelemez.", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Kat boş. Ürün eklemeden yeni kata geçemezsin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_katYonetici.AktifKat >= _katYonetici.MaksKat)
            {
                MessageBox.Show($"Maksimum kat sayısı: {_katYonetici.MaksKat}", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
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

            if(Convert.ToInt32(txtKopyalaValue.Text) == Convert.ToInt32(txtYapisValue.Text) + 1)
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

        private void BtnKatKopyala_MouseDown(object sender, MouseButtonEventArgs e)
        {

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

        private void DizilimiJsonDosyasinaKaydet(string dizilimAdi)
        {
            try
            {
                _katYonetici.KatiKaydetDisardan(myCanvas);
                bool hicUrunYokMu = !_katYonetici.TumKatlar.Any(kat => kat.Value != null && kat.Value.Any());
                if (hicUrunYokMu)
                {
                    MessageBox.Show("Kaydedilecek ürün yok. Önce palete ürün eklemelisiniz.",
                        "Uyarı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }



                var yeniDizilim = DizilimKayitModeliOlustur(dizilimAdi);

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



                bool ayniIsimdeVarMi = tumDizilimler.Any(x =>
                string.Equals(x.DizilimAdi, dizilimAdi, StringComparison.OrdinalIgnoreCase));

                if (ayniIsimdeVarMi)
                {
                    MessageBox.Show("Bu isimde bir dizilim zaten var. Lütfen farklı bir dizilim adı girin.",
                        "Uyarı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }



                tumDizilimler.Add(yeniDizilim);

                string yeniJson = JsonConvert.SerializeObject(tumDizilimler, Formatting.Indented);
                File.WriteAllText(dosyaYolu, yeniJson);

                MessageBox.Show($"Dizilim kaydedildi:\n{dosyaYolu}", "Bilgi",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt hatası:\n{ex.Message}");
            }
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
            string dizilimAdi = "Dizilim_1";
            DizilimiJsonDosyasinaKaydet(dizilimAdi);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
