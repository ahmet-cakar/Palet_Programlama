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
    }
}
