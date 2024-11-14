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
        private Point ilkTiklamaPozisyonu;
        private bool suruklemeBasladi = false;
        private const double suruklemeEsigi = 1.0;
        private bool surukleniyor = false;
        private Point tiklamaOffset;
        private Rectangle sonSecilmisKutu=new Rectangle();//palet içindeki son seçilmiş kolinin seçimini kaldırmak için yazdık

        Rectangle suruklenenKutu; // Hareket ettirilen Rectangle
        List<Rect> digerKutular = new List<Rect>(); // Diğer Rectangle'ların kapladığı alanları saklayan liste

        // Mesafe göstergesi için çizgileri ve etiketleri tanımla
        Line solCizgi = new Line { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        Line sagCizgi = new Line { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        Line ustCizgi = new Line { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        Line altCizgi = new Line { Stroke = Brushes.Magenta, StrokeThickness = 3 };

        TextBlock solMesafe = new TextBlock { Foreground = Brushes.Magenta, FontSize = 20 };
        TextBlock sagMesafe = new TextBlock { Foreground = Brushes.Magenta, FontSize = 20 };
        TextBlock ustMesafe = new TextBlock { Foreground = Brushes.Magenta, FontSize = 20 };
        TextBlock altMesafe = new TextBlock { Foreground = Brushes.Magenta, FontSize = 20 };
        private Frame MainFrame;

        public DizilimYap(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
            paleteMesafeCizgileriEkle();
        }

        private void paleteMesafeCizgileriEkle()
        {
            // Çizgileri ve metinleri Canvas'a ekle

            myCanvas.Children.Add(solCizgi);
            myCanvas.Children.Add(sagCizgi);
            myCanvas.Children.Add(ustCizgi);
            myCanvas.Children.Add(altCizgi);

            myCanvas.Children.Add(solMesafe);
            myCanvas.Children.Add(sagMesafe);
            myCanvas.Children.Add(ustMesafe);
            myCanvas.Children.Add(altMesafe);
        }
        private void mesafeCizgileriniGizle()
        {
            solCizgi.Visibility = Visibility.Collapsed;
            sagCizgi.Visibility = Visibility.Collapsed;
            ustCizgi.Visibility = Visibility.Collapsed;
            altCizgi.Visibility = Visibility.Collapsed;

            solMesafe.Visibility = Visibility.Collapsed;
            sagMesafe.Visibility = Visibility.Collapsed;
            ustMesafe.Visibility = Visibility.Collapsed;
            altMesafe.Visibility = Visibility.Collapsed;
        }
        private void mesafeCizgileriniGoster()
        {
            // Çizgileri ve etiketleri gizle
            solCizgi.Visibility = Visibility.Visible;
            sagCizgi.Visibility = Visibility.Visible;
            ustCizgi.Visibility = Visibility.Visible;
            altCizgi.Visibility = Visibility.Visible;

            solMesafe.Visibility = Visibility.Visible;
            sagMesafe.Visibility = Visibility.Visible;
            ustMesafe.Visibility = Visibility.Visible;
            altMesafe.Visibility = Visibility.Visible;
        }
        private List<Rect> paletIcindekiKutulariBul(Rectangle suruklenenKutu)
        {
            // Diğer Rectangle'ların kapladığı alanları hesapla ve listeye ekle
            digerKutular.Clear();
            foreach (var child in myCanvas.Children)
            {
                if (child is Rectangle otherRectangle && otherRectangle != suruklenenKutu)
                {
                    double otherLeft = Canvas.GetLeft(otherRectangle);
                    double otherTop = Canvas.GetTop(otherRectangle);
                    Rect otherRect = new Rect(otherLeft, otherTop, otherRectangle.ActualWidth, otherRectangle.ActualHeight);
                    digerKutular.Add(otherRect);
                }
            }
            return digerKutular;
        }

        // MouseDown olayında sürükleme başlat
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            suruklenenKutu = (Rectangle)sender;
            
            mesafeCizgileriniGoster();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                surukleniyor = true;
                suruklemeBasladi = false;
                ilkTiklamaPozisyonu = e.GetPosition(suruklenenKutu);
                tiklamaOffset = e.GetPosition(suruklenenKutu);
                suruklenenKutu.CaptureMouse();
                digerKutular = paletIcindekiKutulariBul(suruklenenKutu);
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
                    sonSecilmisKutu.Stroke = Brushes.Transparent;
                    sonSecilmisKutu.StrokeThickness = 0;
                    var position = e.GetPosition(myCanvas);

                    // Yeni X ve Y pozisyonlarını hesapla
                    double newLeft = position.X - tiklamaOffset.X;
                    double newTop = position.Y - tiklamaOffset.Y;

                    // Canvas sınırları içinde kalmasını sağla
                    double canvasLeft = 0;
                    double canvasTop = 0;
                    double canvasRight = myCanvas.ActualWidth - suruklenenKutu.ActualWidth;
                    double canvasBottom = myCanvas.ActualHeight - suruklenenKutu.ActualHeight;

                    newLeft = Math.Max(canvasLeft, Math.Min(newLeft, canvasRight));
                    newTop = Math.Max(canvasTop, Math.Min(newTop, canvasBottom));

                    // Yeni pozisyonun Rectangle alanı
                    Rect newMovingRect = new Rect(newLeft, newTop, suruklenenKutu.ActualWidth, suruklenenKutu.ActualHeight);

                    // Diğer Rectangle'larla çarpışma kontrolü
                    bool canMove = true;
                    foreach (var otherRect in digerKutular)
                    {
                        if (newMovingRect.IntersectsWith(otherRect))
                        {
                            // Çarpışmanın hangi eksende olduğunu belirlemek için yatay ve dikey mesafeleri hesapla
                            double deltaX = Math.Min(Math.Abs(newLeft + suruklenenKutu.ActualWidth - otherRect.Left), Math.Abs(newLeft - otherRect.Right));
                            double deltaY = Math.Min(Math.Abs(newTop + suruklenenKutu.ActualHeight - otherRect.Top), Math.Abs(newTop - otherRect.Bottom));

                            if (deltaX < deltaY)
                            {
                                if (newLeft < otherRect.Left)
                                {
                                    if (otherRect.Left - suruklenenKutu.ActualWidth >= canvasLeft)
                                    {
                                        newLeft = otherRect.Left - suruklenenKutu.ActualWidth;
                                    }
                                    else
                                    {
                                        canMove = false;
                                    }
                                }
                                else
                                {
                                    if (otherRect.Right <= canvasRight)
                                    {
                                        newLeft = otherRect.Right;
                                    }
                                    else
                                    {
                                        canMove = false;
                                    }
                                }
                            }
                            else
                            {
                                if (newTop < otherRect.Top)
                                {
                                    if (otherRect.Top - suruklenenKutu.ActualHeight >= canvasTop)
                                    {
                                        newTop = otherRect.Top - suruklenenKutu.ActualHeight;
                                    }
                                    else
                                    {
                                        canMove = false;
                                    }
                                }
                                else
                                {
                                    if (otherRect.Bottom <= canvasBottom)
                                    {
                                        newTop = otherRect.Bottom;
                                    }
                                    else
                                    {
                                        canMove = false;
                                    }
                                }
                            }

                            Rect tempRect = new Rect(newLeft, newTop, suruklenenKutu.ActualWidth, suruklenenKutu.ActualHeight);
                            foreach (var checkRect in digerKutular)
                            {

                                bool isIntersecting =
                                tempRect.Left < checkRect.Right &&
                                tempRect.Right > checkRect.Left &&
                                tempRect.Top < checkRect.Bottom &&
                                tempRect.Bottom > checkRect.Top &&
                                // Kenarların yalnızca temas ettiği durumları hariç tutma
                                !(tempRect.Right == checkRect.Left || tempRect.Left == checkRect.Right ||
                                  tempRect.Bottom == checkRect.Top || tempRect.Top == checkRect.Bottom);


                                if (isIntersecting && checkRect != otherRect)
                                {


                                    canMove = false;
                                    break;
                                }
                            }

                            if (!canMove) return;

                            newMovingRect = new Rect(newLeft, newTop, suruklenenKutu.ActualWidth, suruklenenKutu.ActualHeight);
                        }
                    }

                    if (canMove)
                    {
                        Canvas.SetLeft(suruklenenKutu, newLeft);
                        Canvas.SetTop(suruklenenKutu, newTop);
                    }

                    UpdateDistanceIndicators(newMovingRect);
                }
            }


           
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (suruklemeBasladi)
            {
                mesafeCizgileriniGizle();
            }
            else
            {
                
                if (suruklenenKutu.StrokeThickness == 1)
                {
                    suruklenenKutu.Stroke = Brushes.Transparent;
                    suruklenenKutu.StrokeThickness = 0;
                    mesafeCizgileriniGizle();

                }
                else
                {
                    
                    sonSecilmisKutu.Stroke = Brushes.Transparent;
                    sonSecilmisKutu.StrokeThickness = 0;
                    suruklenenKutu.Stroke = Brushes.Red;
                    suruklenenKutu.StrokeThickness = 1;
                    UpdateDistanceIndicators(new Rect(Canvas.GetLeft(suruklenenKutu), Canvas.GetTop(suruklenenKutu), suruklenenKutu.ActualWidth, suruklenenKutu.ActualHeight));
                    mesafeCizgileriniGoster();

                }
                sonSecilmisKutu = suruklenenKutu;

            }
            suruklemeBasladi = false;
            surukleniyor = false;
            suruklenenKutu.ReleaseMouseCapture();





        }
        private void UpdateDistanceIndicators(Rect movingRect)
        {
            double leftDistance = movingRect.Left;
            double rightDistance = myCanvas.ActualWidth - movingRect.Right;
            double topDistance = movingRect.Top;
            double bottomDistance = myCanvas.ActualHeight - movingRect.Bottom;

            foreach (var otherRect in digerKutular)
            {
                if (movingRect.Left > otherRect.Right)
                    leftDistance = Math.Min(leftDistance, movingRect.Left - otherRect.Right);
                if (movingRect.Right < otherRect.Left)
                    rightDistance = Math.Min(rightDistance, otherRect.Left - movingRect.Right);
                if (movingRect.Top > otherRect.Bottom)
                    topDistance = Math.Min(topDistance, movingRect.Top - otherRect.Bottom);
                if (movingRect.Bottom < otherRect.Top)
                    bottomDistance = Math.Min(bottomDistance, otherRect.Top - movingRect.Bottom);
            }

            solCizgi.X1 = movingRect.Left;
            solCizgi.Y1 = movingRect.Top + movingRect.Height / 2;
            solCizgi.X2 = movingRect.Left - leftDistance;
            solCizgi.Y2 = solCizgi.Y1;
            solMesafe.Text = (leftDistance * 2).ToString("0");
            Canvas.SetLeft(solMesafe, (solCizgi.X1 + solCizgi.X2) / 2);
            Canvas.SetTop(solMesafe, solCizgi.Y1);

            sagCizgi.X1 = movingRect.Right;
            sagCizgi.Y1 = movingRect.Top + movingRect.Height / 2;
            sagCizgi.X2 = movingRect.Right + rightDistance;
            sagCizgi.Y2 = sagCizgi.Y1;
            sagMesafe.Text = (rightDistance * 2).ToString("0");
            Canvas.SetLeft(sagMesafe, (sagCizgi.X1 + sagCizgi.X2) / 2);
            Canvas.SetTop(sagMesafe, sagCizgi.Y1);

            ustCizgi.X1 = movingRect.Left + movingRect.Width / 2;
            ustCizgi.Y1 = movingRect.Top;
            ustCizgi.X2 = ustCizgi.X1;
            ustCizgi.Y2 = movingRect.Top - topDistance;
            ustMesafe.Text = (topDistance * 2).ToString("0");
            Canvas.SetLeft(ustMesafe, ustCizgi.X1);
            Canvas.SetTop(ustMesafe, (ustCizgi.Y1 + ustCizgi.Y2) / 2);

            altCizgi.X1 = movingRect.Left + movingRect.Width / 2;
            altCizgi.Y1 = movingRect.Bottom;
            altCizgi.X2 = altCizgi.X1;
            altCizgi.Y2 = movingRect.Bottom + bottomDistance;
            altMesafe.Text = (bottomDistance * 2).ToString("0");
            Canvas.SetLeft(altMesafe, altCizgi.X1);
            Canvas.SetTop(altMesafe, (altCizgi.Y1 + altCizgi.Y2) / 2);
        }

        private void dikeyKutu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dikeyKutu.StrokeThickness = 1;
            yatayKutu.StrokeThickness = 0;
            dikeyKutu.Stroke = Brushes.Red;
            yatayKutu.Stroke = Brushes.Transparent;
        }

        private void yatayKutu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dikeyKutu.StrokeThickness = 0;
            yatayKutu.StrokeThickness = 1;
            dikeyKutu.Stroke = Brushes.Transparent;
            yatayKutu.Stroke = Brushes.Red;
        }

      
    }
}
