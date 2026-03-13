using Palet_Programlama.Sayfalar;
using Palet_Programlama.Sınıflar;
using System.Windows;

namespace Palet_Programlama
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public UrunIslemler UrunIslemler { get; } = new UrunIslemler();
        public PaletIslemler PaletIslemler { get; } = new PaletIslemler();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Bekleme(MainFrame));

        }




    }
}
