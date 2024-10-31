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
    /// Interaction logic for UstMenu.xaml
    /// </summary>
    public partial class UstMenu : UserControl
    {
        private bool _isMouseDown = false;
        private Point _startPoint;
        private double _scrollStartOffset;

        public UstMenu()
        {
            InitializeComponent();
        }
        
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Fare tekerleği ile kaydırma işlemi
            if (e.Delta > 0)  // Yukarı kaydırma
            {
                myScrollViewer.ScrollToHorizontalOffset(myScrollViewer.HorizontalOffset - 50);
            }
            else  // Aşağı kaydırma
            {
                myScrollViewer.ScrollToHorizontalOffset(myScrollViewer.HorizontalOffset + 50);
            }

            e.Handled = true;  // Varsayılan kaydırmayı durdur
        }

        private void myScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Fare basılı tutulduğunda işlem başlar
            _isMouseDown = true;
            _startPoint = e.GetPosition(this);  // Başlangıç pozisyonunu al
            _scrollStartOffset = myScrollViewer.HorizontalOffset;  // O anki kaydırma noktasını al
            myScrollViewer.CaptureMouse();  // Fareyi yakala
        }

        private void myScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                // Fare hareket ettikçe kaydırma yapılır
                var currentPoint = e.GetPosition(this);
                double diff = currentPoint.X - _startPoint.X;  // Ne kadar hareket ettiğini hesapla

                // Kaydırıcıyı yeni pozisyona taşır
                myScrollViewer.ScrollToHorizontalOffset(_scrollStartOffset - diff);
            }
        }

        private void myScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Fare bırakıldığında kaydırmayı bitir
            _isMouseDown = false;
            myScrollViewer.ReleaseMouseCapture();
        }

        private void UrunEklePageBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Frame'in içeriğini değiştirme 
                mainWindow.MainFrame.Content = new UrunEkle(mainWindow.MainFrame);  
            }
        }

        private void DizilimPageBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Frame'in içeriğini değiştirme 
                mainWindow.MainFrame.Content = new DizilimYap(mainWindow.MainFrame);
            }
        }

        private void AnasayfaBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Frame'in içeriğini değiştirme 
                mainWindow.MainFrame.Content = new Anasayfa(mainWindow.MainFrame);
            }
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Frame'in içeriğini değiştirme 
                mainWindow.MainFrame.Content = new Kullanici(mainWindow.MainFrame);
            }
        }
    }
}
