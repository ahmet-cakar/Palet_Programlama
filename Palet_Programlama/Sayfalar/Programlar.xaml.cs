using System.Windows;
using System.Windows.Controls;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for Programlar.xaml
    /// </summary>
    public partial class Programlar : Page
    {
        private readonly Frame MainFrame;
        public Programlar(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
        }
        private void recededuzenle_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReceteDuzenle());

        }
    }
}
