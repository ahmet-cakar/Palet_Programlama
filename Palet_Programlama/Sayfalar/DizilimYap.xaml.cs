using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        // Ürün ölçüsü (Canvas pixel) — şimdilik senin dikey kutu: 100x150
        private const double UrunW_Dikey = 100;
        private const double UrunH_Dikey = 150;
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

        public DizilimYap(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
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
                    if (_motor.TrySurukle(movingNow, hedef, myCanvas.ActualWidth, myCanvas.ActualHeight, digerKutular, out var sonuc))
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
            suruklenenKutu.ReleaseMouseCapture();


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
            {
                return;
            }
            // Eğer tıklanan yer bir Rectangle ise (mevcut ürün), ekleme yapma.
            // (İstersen yine de eklemesini istersen bunu kaldırırız.)
            if (e.OriginalSource is Rectangle) return;

            var p = e.GetPosition(myCanvas);

            double w = UrunW_Dikey;
            double h = UrunH_Dikey;

            // Yatay seçiliyse ölçü swap
            if (_eklemeYon == EklemeYon.Yatay)
                (w, h) = (h, w);

           
            // Tıklanan nokta ürünün sol-üst köşesi olsun
            double newLeft = p.X;
            double newTop = p.Y;

            // Canvas sınırına sıkıştır
            double maxLeft = myCanvas.ActualWidth - w;
            double maxTop = myCanvas.ActualHeight - h;
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
    }
}
