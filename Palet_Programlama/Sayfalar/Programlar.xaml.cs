using Palet_Programlama.Sayfalar.Gruplama.Models;
using Palet_Programlama.Sayfalar.ProgramlarSayfasi;
using System.Collections.Generic;
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
        private readonly ProgramListeServisi _programListeServisi = new();
        private List<ProgramKayitModel> _programKayitlari = new();


        public Programlar(Frame Main)
        {
            InitializeComponent();
            MainFrame = Main;
            ProgramlariYukle();
        }

        private void ListBoxProgramlar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seciliProgram = ListBoxProgramlar.SelectedItem as ProgramKayitModel;
            if (seciliProgram == null)
                return;

            txtProgramAdi.Text = seciliProgram.ProgramAdi;
            txtProgramID.Text = seciliProgram.Id.ToString();
            txtProgramAdi.ToolTip = seciliProgram.ProgramAdi;
        }
        private void ProgramlariYukle()
        {
            _programKayitlari = _programListeServisi.ProgramlariGetir();
            ListBoxProgramlar.ItemsSource = _programKayitlari;
        }
    }
}
