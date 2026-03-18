using Palet_Programlama.Sayfalar.Gruplama.Models;
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

namespace Palet_Programlama.Popuplar
{
    public partial class ProgramKaydetPopup : Window
    {
        public ProgramKaydetGirdiModel Sonuc { get; private set; } = new();

        public ProgramKaydetPopup()
        {
            InitializeComponent();
        }

        private void BtnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void VerileriYukle(string programAdi, string aciklama)
        {
            TxtProgramAdi.Text = programAdi ?? "";
            TxtAciklama.Text = aciklama ?? "";
            TxtProgramAdi.IsReadOnly = true;
        }


        private void BtnKaydet_Click(object sender, RoutedEventArgs e)
        {
            string programAdi = TxtProgramAdi.Text?.Trim() ?? "";
            string aciklama = TxtAciklama.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(programAdi))
            {
                MessageBox.Show("Program adı boş olamaz.");
                return;
            }

            Sonuc = new ProgramKaydetGirdiModel
            {
                ProgramAdi = programAdi,
                Aciklama = aciklama
            };

            DialogResult = true;
            Close();
        }
    }
}
