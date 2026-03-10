using System;
using System.Globalization;
using System.Windows;
using Palet_Programlama.Sınıflar;

namespace Palet_Programlama.UserController
{
    public partial class BildirimKutusu : Window
    {
        public BildirimKutusu()
        {
            InitializeComponent();
        }

        public void MesajGonder(string btnKey, string mesajKey)
        {
            btn1.Content = LanguageConverter.GetString(btnKey);
            mesaj.Text = LanguageConverter.GetString(mesajKey);
        }

        public void MesajGonderFormatli(string btnKey, string mesajKey, params object[] args)
        {
            btn1.Content = LanguageConverter.GetString(btnKey);

            string sablon = LanguageConverter.GetString(mesajKey);
            mesaj.Text = string.Format(CultureInfo.CurrentCulture, sablon, args);
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}