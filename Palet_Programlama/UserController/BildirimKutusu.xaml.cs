using Palet_Programlama.Sınıflar;
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
using System.Windows.Shapes;

namespace Palet_Programlama.UserController
{
    /// <summary>
    /// Interaction logic for BildirimKutusu.xaml
    /// </summary>
    public partial class BildirimKutusu : Window
    {
        public BildirimKutusu()
        {
            InitializeComponent();
        }
        public void MesajGonder(string btnkey, string mesajkey)
        {
            btn1.Content = LanguageConverter.GetString($"{btnkey}");
            mesaj.Text = LanguageConverter.GetString($"{mesajkey}");
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
             this.Close();
        }
    }
}
