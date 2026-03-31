using Newtonsoft.Json;
using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Helpers;
using Palet_Programlama.Screens.Services;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Palet_Programlama.UserController
{
    public partial class UrunPaletSecimKutusu : Window
    {

        private readonly SonSecimServisi _sonSecimServisi = new SonSecimServisi();

        private sealed class ComboSecenek<T>
        {
            public string Text { get; set; } = string.Empty;
            public T Value { get; set; }
        }

        public Urun SecilenUrun { get; private set; }
        public Palet SecilenPalet { get; private set; }
        public string SecilenDizilimAdi { get; private set; }

        private string _dizilimAciklama;



        private void SonSecimiKaydet()
        {
            _sonSecimServisi.Kaydet(new SonSecimModel
            {
                UrunAdi = SecilenUrun?.UrunAdi,
                PaletAdi = SecilenPalet?.PaletAdi,
                DizilimAdi = SecilenDizilimAdi
            });
        }

        public UrunPaletSecimKutusu(List<Urun> urunler, List<Palet> paletler, string dizilimAciklama)
        {
            InitializeComponent();

            urunler = urunler ?? new List<Urun>();
            paletler = paletler ?? new List<Palet>();

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

            dizilimComboBox.ItemsSource = null;
            dizilimComboBox.Text = "Dizilim Seçiniz";
            _dizilimAciklama = dizilimAciklama;
            txtDizilimAciklama.Text = dizilimAciklama;

            SonSecimiYukle();
        }

        private void SonSecimiYukle()
        {

            try
            {
                var sonSecim = _sonSecimServisi.Yukle();
                if (sonSecim == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(sonSecim.UrunAdi))
                {
                    var urunItem = urunComboBox.Items
                        .OfType<ComboSecenek<Urun>>()
                        .FirstOrDefault(x =>
                            string.Equals((x.Value?.UrunAdi ?? "").Trim(),
                                          sonSecim.UrunAdi.Trim(),
                                          StringComparison.OrdinalIgnoreCase));

                    if (urunItem != null)
                    {
                        urunComboBox.SelectedItem = urunItem;
                    }
                }

                if (!string.IsNullOrWhiteSpace(sonSecim.PaletAdi))
                {
                    var paletItem = paletComboBox.Items
                        .OfType<ComboSecenek<Palet>>()
                        .FirstOrDefault(x =>
                            string.Equals((x.Value?.PaletAdi ?? "").Trim(),
                                          sonSecim.PaletAdi.Trim(),
                                          StringComparison.OrdinalIgnoreCase));

                    if (paletItem != null)
                    {
                        paletComboBox.SelectedItem = paletItem;
                    }
                }

                DizilimleriYukle();

                if (!string.IsNullOrWhiteSpace(sonSecim.DizilimAdi) &&
                    dizilimComboBox.ItemsSource is IEnumerable<string> dizilimler)
                {
                    var seciliDizilim = dizilimler.FirstOrDefault(x =>
                        string.Equals((x ?? "").Trim(),
                                      sonSecim.DizilimAdi.Trim(),
                                      StringComparison.OrdinalIgnoreCase));

                    if (seciliDizilim != null)
                    {
                        dizilimComboBox.SelectedItem = seciliDizilim;
                    }
                }
            }
            finally { }
               

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

            DizilimleriYukle();
        }

        private void paletComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paletComboBox.SelectedItem is ComboSecenek<Palet> seciliPalet)
            {
                paletComboBox.Text = seciliPalet.Text;
            }

            DizilimleriYukle();
        }

        private void dizilimComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dizilimComboBox.SelectedItem is string seciliDizilim)
            {
                if (seciliDizilim == "Dizilim Seçiniz")
                {
                    SecilenDizilimAdi = null;
                    dizilimComboBox.Text = "Dizilim Seçiniz";
                    return;
                }

                SecilenDizilimAdi = seciliDizilim;
                dizilimComboBox.Text = seciliDizilim;
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
                bildirimKutusu.MesajGonder("ButtonKey.btntamam", "MesajKutusu.zorunluAlanDoldur");
                bildirimKutusu.ShowDialog();
                return;
            }
            SecilenUrun = seciliUrun.Value;
            SecilenPalet = seciliPalet.Value;
            SecilenDizilimAdi = dizilimComboBox.SelectedItem as string;

            if (SecilenDizilimAdi == "Dizilim Seçiniz")
            {
                SecilenDizilimAdi = null;
            }


            if (_dizilimAciklama == "(* Zorunlu)" && SecilenDizilimAdi == null)
            {
                BildirimKutusu bildirimKutusu = new BildirimKutusu();
                bildirimKutusu.MesajGonder("ButtonKey.btntamam", "MesajKutusu.zorunluAlanDoldur");
                bildirimKutusu.ShowDialog();
                return;
            }

            SonSecimiKaydet();
            DialogResult = true;
            Close();






        }

        private void DizilimleriYukle()
        {
            dizilimComboBox.ItemsSource = null;
            dizilimComboBox.SelectedItem = null;
            dizilimComboBox.Text = "Dizilim Seçiniz";
            SecilenDizilimAdi = null;

            if (urunComboBox.SelectedItem is not ComboSecenek<Urun> seciliUrun ||
                paletComboBox.SelectedItem is not ComboSecenek<Palet> seciliPalet ||
                seciliUrun.Value == null ||
                seciliPalet.Value == null)
            {
                return;
            }

            try
            {
                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                if (!File.Exists(dosyaYolu))
                {
                    dizilimComboBox.Text = "Dizilim dosyası bulunamadı";
                    return;
                }

                string json = File.ReadAllText(dosyaYolu);

                var tumDizilimler = JsonConvert.DeserializeObject<List<DizilimKayitModel>>(json)
                                   ?? new List<DizilimKayitModel>();

                var uygunDizilimler = tumDizilimler
                  .Where(x =>
                      string.Equals((x.PaletAdi ?? "").Trim(), (seciliPalet.Value.PaletAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                      string.Equals((x.UrunAdi ?? "").Trim(), (seciliUrun.Value.UrunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                  .Select(x => x.DizilimAdi)
                  .Where(x => !string.IsNullOrWhiteSpace(x))
                  .Distinct()
                  .ToList();

                if (_dizilimAciklama != "(* Zorunlu)")
                {
                    uygunDizilimler.Insert(0, "Dizilim Seçiniz");
                }

                dizilimComboBox.ItemsSource = uygunDizilimler;

                if (uygunDizilimler.Count == 0)
                {
                    dizilimComboBox.Text = "Kayıtlı dizilim yok";
                }
            }
            catch (Exception)
            {
                dizilimComboBox.ItemsSource = null;
                dizilimComboBox.Text = "Dizilimler yüklenemedi";
            }
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}