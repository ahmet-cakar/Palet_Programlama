using Palet_Programlama.Sınıflar;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Palet_Programlama.UserController
{
    public partial class UrunPaletSecimKutusu : Window
    {
        private sealed class ComboSecenek<T>
        {
            public string Text { get; set; } = string.Empty;
            public T? Value { get; set; }
        }

        public Urun? SecilenUrun { get; private set; }
        public Palet? SecilenPalet { get; private set; }

        public UrunPaletSecimKutusu(List<Urun> urunler, List<Palet> paletler)
        {
            InitializeComponent();

            urunler ??= new List<Urun>();
            paletler ??= new List<Palet>();

            urunComboBox.ItemsSource = urunler
                .Select(x => new ComboSecenek<Urun>
                {
                    Text = x.UrunAdi,
                    Value = x
                })
                .ToList();

            paletComboBox.ItemsSource = paletler
                .Select(x => new ComboSecenek<Palet>
                {
                    Text = x.PaletAdi,
                    Value = x
                })
                .ToList();

            urunComboBox.DisplayMemberPath = nameof(ComboSecenek<Urun>.Text);
            paletComboBox.DisplayMemberPath = nameof(ComboSecenek<Palet>.Text);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void urunComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (urunComboBox.SelectedItem is ComboSecenek<Urun> seciliUrun)
            {
                urunComboBox.Text = seciliUrun.Text;
            }
        }

        private void paletComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paletComboBox.SelectedItem is ComboSecenek<Palet> seciliPalet)
            {
                paletComboBox.Text = seciliPalet.Text;
            }
        }

        private void btnTamam_Click(object sender, RoutedEventArgs e)
        {
            if (urunComboBox.SelectedItem is not ComboSecenek<Urun> seciliUrun ||
                paletComboBox.SelectedItem is not ComboSecenek<Palet> seciliPalet ||
                seciliUrun.Value == null ||
                seciliPalet.Value == null)
            {
                BildirimKutusu bildirimKutusu = new BildirimKutusu();
                bildirimKutusu.MesajGonder("ButtonKey.btntamam", "HataMesajlari.secimyapiniz");
                bildirimKutusu.Show();
                return;
            }

            SecilenUrun = seciliUrun.Value;
            SecilenPalet = seciliPalet.Value;

            DialogResult = true;
            Close();
        }



        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}