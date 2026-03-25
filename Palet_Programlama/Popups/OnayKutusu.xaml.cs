using System.Globalization;
using System.Windows;

namespace Palet_Programlama.UserController
{
    public partial class OnayKutusu : Window
    {
        public bool OnaylandiMi { get; private set; }

        public OnayKutusu()
        {
            InitializeComponent();
        }

        public void MesajGonder(string mesajMetni, string evetButonMetni = "Evet", string hayirButonMetni = "Hayır")
        {
            mesaj.Text = mesajMetni;
            btnEvet.Content = evetButonMetni;
            btnHayir.Content = hayirButonMetni;
        }

        public void MesajGonderFormatli(string sablon, params object[] args)
        {
            mesaj.Text = string.Format(CultureInfo.CurrentCulture, sablon, args);
            btnEvet.Content = "Evet";
            btnHayir.Content = "Hayır";
        }

        private void btnEvet_Click(object sender, RoutedEventArgs e)
        {
            OnaylandiMi = true;
            DialogResult = true;
            Close();
        }

        private void btnHayir_Click(object sender, RoutedEventArgs e)
        {
            OnaylandiMi = false;
            DialogResult = false;
            Close();
        }
    }
}