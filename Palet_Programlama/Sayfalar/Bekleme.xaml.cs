using System.Windows.Controls;
using System.Windows.Input;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for Bekleme.xaml
    /// </summary>
    public partial class Bekleme : Page
    {
        private Frame MainFrame;
        public Bekleme(Frame Main)
        {
            InitializeComponent();
            this.MainFrame = Main;
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Kullanici(MainFrame));
        }


    }
}
