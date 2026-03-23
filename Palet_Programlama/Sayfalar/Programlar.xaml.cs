using Palet_Programlama.Sayfalar.Gruplama.Models;
using Palet_Programlama.Sayfalar.ProgramlarSayfasi;
using Palet_Programlama.Sınıflar;
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
        private readonly UrunIslemler _urunServisi = new();
        private readonly PaletIslemler _paletServisi = new();


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
            
            Urun urun =_urunServisi.UrunGetir(seciliProgram.UrunAdi);
            txtUrunAdi.Text = urun.UrunAdi;
            txtUrunAdi.ToolTip = urun.UrunAdi;
            txtUrunGenislik.Text = urun.UrunEn.ToString();
            txtUrunUzunluk.Text = urun.UrunBoy.ToString();
            txtUrunBasinc.Text = urun.UrunBasinc.ToString();
            txtUrunYukseklik.Text = urun.UrunYukseklik.ToString();
            txtUrunAgirlik.Text = urun.UrunAgirlik.ToString();


            Palet palet = _paletServisi.PaletGetir(seciliProgram.PaletAdi);
            txtPaletAdi.Text = palet.PaletAdi;
            txtPaletAdi.ToolTip = palet.PaletAdi;
            txtPaletGenislik.Text = palet.PaletEn.ToString();
            txtPaletUzunluk.Text = palet.PaletBoy.ToString();
            txtPaletYukseklik.Text = palet.PaletYukseklik.ToString();

        }
        private void ProgramlariYukle()
        {
            _programKayitlari = _programListeServisi.ProgramlariGetir();
            ListBoxProgramlar.ItemsSource = _programKayitlari;
        }
    }
}
