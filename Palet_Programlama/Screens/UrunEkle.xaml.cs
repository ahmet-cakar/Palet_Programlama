using Palet_Programlama.Languages;
using Palet_Programlama.Screens.Services;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using Palet_Programlama.UserController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palet_Programlama.Screens
{
    /// <summary>
    /// Interaction logic for UrunEkle.xaml
    /// </summary>
    public partial class UrunEkle : Page
    {
        private Frame MainFrame;
        private List<TextBox> textBoxes;
        private List<TextBlock> placeholders;
        private readonly SonSecimServisi _sonSecimServisi = new SonSecimServisi();
        private readonly UrunIslemler urunIslemler = new UrunIslemler();
        private readonly PaletIslemler paletIslemler = new PaletIslemler();

        private List<Urun> urunlist;
        private List<Palet> paletlist;

        private (TextBox TextBox, string MesajKey)[] UrunZorunluAlanlar => new[]
        {
            (txtUrunAdi, "HataMesajlari.urunadbos"),
            (txtUrunEn, "HataMesajlari.urunenbos"),
            (txtUrunBoy, "HataMesajlari.urunboybos"),
            (txtUrunYukseklik, "HataMesajlari.urunyukseklikbos"),
            (txtUrunAgirlik, "HataMesajlari.urunagirlikbos"),
            (txtUrunBasinc, "HataMesajlari.urunbasincbos")
        };

        private (TextBox TextBox, string MesajKey)[] PaletZorunluAlanlar => new[]
        {
            (txtPaletAdi, "HataMesajlari.paletadbos"),
            (txtPaletEn, "HataMesajlari.paletenbos"),
            (txtPaletBoy, "HataMesajlari.paletboybos"),
            (txtPaletYukseklik, "HataMesajlari.paletyukseklikbos")
        };

        private readonly Dictionary<string, Tuple<string, string>> textBoxData = new Dictionary<string, Tuple<string, string>>
        {
            { "txtUrunAdi", Tuple.Create("pack://application:,,,/Images/UrunEkle/home-koli.png", "UrunEkle.previewHomeKoli") },
            { "txtPaletAdi", Tuple.Create("pack://application:,,,/Images/UrunEkle/home-pallet.png", "UrunEkle.previewHomePalet") },
            { "txtUrunEn", Tuple.Create("pack://application:,,,/Images/UrunEkle/en-koli.png", "UrunEkle.previewUrunEn") },
            { "txtUrunBoy", Tuple.Create("pack://application:,,,/Images/UrunEkle/boy-koli.png", "UrunEkle.previewUrunBoy") },
            { "txtUrunYukseklik", Tuple.Create("pack://application:,,,/Images/UrunEkle/yukseklik-koli.png", "UrunEkle.previewUrunYukseklik") },
            { "txtUrunAgirlik", Tuple.Create("pack://application:,,,/Images/UrunEkle/koli-agirlik.png", "UrunEkle.previewUrunAgirlik") },
            { "txtUrunBasinc", Tuple.Create("pack://application:,,,/Images/UrunEkle/koli-basinc.png", "UrunEkle.previewUrunBasinc") },
            { "txtPaletEn", Tuple.Create("pack://application:,,,/Images/UrunEkle/en-pallet.png", "UrunEkle.previewPaletEn") },
            { "txtPaletBoy", Tuple.Create("pack://application:,,,/Images/UrunEkle/boy-pallet.png", "UrunEkle.previewPaletBoy") },
            { "txtPaletYukseklik", Tuple.Create("pack://application:,,,/Images/UrunEkle/yukseklik-pallet.png", "UrunEkle.previewPaletYukseklik") },
        };

        public UrunEkle()
        {
            InitializeComponent();

            textBoxes = new List<TextBox>
            {
                txtUrunAdi, txtUrunEn, txtUrunBoy, txtUrunYukseklik, txtUrunAgirlik, txtUrunBasinc,
                txtPaletAdi, txtPaletEn, txtPaletBoy, txtPaletYukseklik
            };

            placeholders = new List<TextBlock>
            {
                phUrunAdi, phUrunEn, phUrunBoy, phUrunYukseklik, phUrunAgirlik, phUrunBasinc,
                phPaletAdi, phPaletEn, phPaletBoy, phPaletYukseklik
            };

            Loaded += Page_Loaded;
            UstMenuControl.AktifSayfa = "UrunEkle";
        }


        private void SonSecimiGuncelle(string urunAdi = null, string paletAdi = null, bool dizilimTemizle = false)
        {
            var sonSecim = _sonSecimServisi.Yukle() ?? new SonSecimModel();

            if (!string.IsNullOrWhiteSpace(urunAdi))
                sonSecim.UrunAdi = urunAdi;

            if (!string.IsNullOrWhiteSpace(paletAdi))
                sonSecim.PaletAdi = paletAdi;

            if (dizilimTemizle)
                sonSecim.DizilimAdi = null;

            _sonSecimServisi.Kaydet(sonSecim);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListeleriYukle();
        }

        private void myTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            txtUrunAdi.CaretBrush = Brushes.White;
            txtUrunEn.CaretBrush = Brushes.White;
            txtUrunBoy.CaretBrush = Brushes.White;
            txtUrunYukseklik.CaretBrush = Brushes.White;
            txtUrunAgirlik.CaretBrush = Brushes.White;
            txtUrunBasinc.CaretBrush = Brushes.White;
            txtPaletAdi.CaretBrush = Brushes.White;
            txtPaletEn.CaretBrush = Brushes.White;
            txtPaletBoy.CaretBrush = Brushes.White;
            txtPaletYukseklik.CaretBrush = Brushes.White;
        }

        private bool OnayAl(string mesaj)
        {
            var onayKutusu = new OnayKutusu
            {
                Owner = Window.GetWindow(this)
            };

            onayKutusu.MesajGonder(mesaj, "Evet", "Hayır");
            bool? sonuc = onayKutusu.ShowDialog();

            return sonuc == true && onayKutusu.OnaylandiMi;
        }

        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private bool ZorunluAlanlariKontrolEt((TextBox TextBox, string MesajKey)[] alanlar)
        {
            foreach (var (textBox, mesajKey) in alanlar)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    BildirimGoster(mesajKey);
                    return false;
                }
            }

            return true;
        }

        private void ListeleriYukle()
        {
            urunlist = urunIslemler.UrunListesiniGetir();
            paletlist = paletIslemler.PaletListesiniGetir();

            urunlistbox.Items.Clear();
            foreach (var item in urunlist)
            {
                urunlistbox.Items.Add(item.UrunAdi);
            }

            paletlistbox.Items.Clear();
            foreach (var item in paletlist)
            {
                paletlistbox.Items.Add(item.PaletAdi);
            }
        }

        private void UrunFormunuTemizle()
        {
            txtUrunAdi.Clear();
            txtUrunEn.Clear();
            txtUrunBoy.Clear();
            txtUrunYukseklik.Clear();
            txtUrunAgirlik.Clear();
            txtUrunBasinc.Clear();
        }

        private void PaletFormunuTemizle()
        {
            txtPaletAdi.Clear();
            txtPaletEn.Clear();
            txtPaletBoy.Clear();
            txtPaletYukseklik.Clear();
        }

        private Urun UrunFormundanOku()
        {
            return new Urun
            {
                UrunAdi = txtUrunAdi.Text.Trim(),
                UrunEn = Convert.ToDouble(txtUrunEn.Text),
                UrunBoy = Convert.ToDouble(txtUrunBoy.Text),
                UrunYukseklik = Convert.ToDouble(txtUrunYukseklik.Text),
                UrunAgirlik = Convert.ToDouble(txtUrunAgirlik.Text),
                UrunBasinc = Convert.ToInt32(txtUrunBasinc.Text)
            };
        }

        private Palet PaletFormundanOku()
        {
            return new Palet
            {
                PaletAdi = txtPaletAdi.Text.Trim(),
                PaletEn = Convert.ToDouble(txtPaletEn.Text),
                PaletBoy = Convert.ToDouble(txtPaletBoy.Text),
                PaletYukseklik = Convert.ToDouble(txtPaletYukseklik.Text)
            };
        }

        private void UrunFormaYaz(Urun urun)
        {
            txtUrunAdi.Text = urun.UrunAdi;
            txtUrunEn.Text = urun.UrunEn.ToString();
            txtUrunBoy.Text = urun.UrunBoy.ToString();
            txtUrunYukseklik.Text = urun.UrunYukseklik.ToString();
            txtUrunAgirlik.Text = urun.UrunAgirlik.ToString();
            txtUrunBasinc.Text = urun.UrunBasinc.ToString();
        }

        private void PaletFormaYaz(Palet palet)
        {
            txtPaletAdi.Text = palet.PaletAdi;
            txtPaletEn.Text = palet.PaletEn.ToString();
            txtPaletBoy.Text = palet.PaletBoy.ToString();
            txtPaletYukseklik.Text = palet.PaletYukseklik.ToString();
        }

        #region TextBox Controlleri

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            var regex = new System.Text.RegularExpressions.Regex(@"^\d*\.?\d*$");
            e.Handled = !regex.IsMatch(currentText);
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string pastedText = (string)e.DataObject.GetData(DataFormats.Text);
                TextBox textBox = sender as TextBox;
                string newText = textBox.Text.Insert(textBox.SelectionStart, pastedText);

                var regex = new System.Text.RegularExpressions.Regex(@"^\d*\.?\d*$");
                if (!regex.IsMatch(newText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        #endregion

        private void UrunPalet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox triggeredTextBox && textBoxData.TryGetValue(triggeredTextBox.Name, out var data))
            {
                OnizlemeTextKonumuAyarla(triggeredTextBox.Name);
                onIzlemeImage.Source = new BitmapImage(new Uri(data.Item1));
                priviewtextblock.Text = LanguageConverter.GetString(data.Item2);
            }

            UpdatePlaceholder();
        }

        private void OnizlemeTextKonumuAyarla(string name, bool textChange = true)
        {
            TextBox _textblock = FindName(name) as TextBox;
            string tamMetin;
            if (textChange)
            {
                switch (name)
                {
                    case "txtUrunAdi":
                        Canvas.SetLeft(txtOnizlemeKutu, 111);
                        Canvas.SetTop(txtOnizlemeKutu, 15);
                        txtOnizlemeKutu.FontSize = 18;
                        tamMetin = _textblock.Text ?? "";

                        txtOnizlemeKutu.Text = tamMetin.Length > 22
                            ? tamMetin.Substring(0, 22) + "..."
                            : tamMetin;
                        txtOnizlemeKutu.ToolTip = tamMetin;
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;

                    case "txtPaletAdi":
                        Canvas.SetLeft(txtOnizlemeKutu, 111);
                        Canvas.SetTop(txtOnizlemeKutu,12);
                        txtOnizlemeKutu.FontSize = 18;
                        tamMetin = _textblock.Text ?? "";

                        txtOnizlemeKutu.Text = tamMetin.Length > 22
                            ? tamMetin.Substring(0, 22) + "..."
                            : tamMetin;
                        txtOnizlemeKutu.ToolTip = tamMetin;
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;

                    case "txtUrunEn":
                        Canvas.SetLeft(txtOnizlemeKutu, 280);
                        Canvas.SetTop(txtOnizlemeKutu, 214);
                        txtOnizlemeKutu.FontSize = 18;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtUrunBoy":
                        Canvas.SetLeft(txtOnizlemeKutu, 145);
                        Canvas.SetTop(txtOnizlemeKutu, 214);
                        txtOnizlemeKutu.FontSize = 18;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtUrunYukseklik":
                        Canvas.SetLeft(txtOnizlemeKutu, 220);
                        Canvas.SetTop(txtOnizlemeKutu, 110.5);
                        txtOnizlemeKutu.FontSize = 18;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtUrunAgirlik":
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Hidden;
                        txtOnizlemeKutu.FontSize = 18;
                        break;
                    case "txtUrunBasinc":

                        Canvas.SetLeft(txtOnizlemeKutu, 204);
                        Canvas.SetTop(txtOnizlemeKutu, 134);
                        txtOnizlemeKutu.FontSize = 24;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtPaletEn":
                        Canvas.SetLeft(txtOnizlemeKutu, 274);
                        Canvas.SetTop(txtOnizlemeKutu, 205);
                        txtOnizlemeKutu.FontSize = 18;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtPaletBoy":
                        Canvas.SetLeft(txtOnizlemeKutu, 151);
                        Canvas.SetTop(txtOnizlemeKutu, 199);
                        txtOnizlemeKutu.FontSize = 18;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;
                    case "txtPaletYukseklik":
                        Canvas.SetLeft(txtOnizlemeKutu, 215);
                        Canvas.SetTop(txtOnizlemeKutu, 125);
                        txtOnizlemeKutu.FontSize = 22;
                        txtOnizlemeKutu.Text = _textblock.Text;
                        txtOnizlemeKutu.ToolTip = "";
                        txtOnizlemeKutu.Visibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                if(name == "txtUrunAdi" || name == "txtPaletAdi")
                {
                    tamMetin = _textblock.Text ?? "";

                    txtOnizlemeKutu.Text = tamMetin.Length > 22
                        ? tamMetin.Substring(0, 22) + "..."
                        : tamMetin;
                    txtOnizlemeKutu.ToolTip = tamMetin;
                }
                else
                {
                    txtOnizlemeKutu.Text = _textblock.Text;
                }
            }
        }

        private void UrunPalet_LostFocus(object sender, RoutedEventArgs e)
        {
            OnizlemeTextKonumuAyarla(((TextBox)sender).Name, false);
            UpdatePlaceholder();
        }

        private void UrunPalet_Changed(object sender, RoutedEventArgs e)
        {
            OnizlemeTextKonumuAyarla(((TextBox)sender).Name, false);
            UpdatePlaceholder();
        }

        private void UpdatePlaceholder()
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                placeholders[i].Visibility =
                    string.IsNullOrEmpty(textBoxes[i].Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void SonSecimdenSilinenleriTemizle(string silinenUrunAdi = null, string silinenPaletAdi = null)
        {
            var sonSecim = _sonSecimServisi.Yukle();
            if (sonSecim == null)
                return;

            bool degistiMi = false;

            if (!string.IsNullOrWhiteSpace(silinenUrunAdi) &&
                string.Equals(sonSecim.UrunAdi, silinenUrunAdi, StringComparison.OrdinalIgnoreCase))
            {
                sonSecim.UrunAdi = null;
                sonSecim.DizilimAdi = null;
                degistiMi = true;
            }

            if (!string.IsNullOrWhiteSpace(silinenPaletAdi) &&
                string.Equals(sonSecim.PaletAdi, silinenPaletAdi, StringComparison.OrdinalIgnoreCase))
            {
                sonSecim.PaletAdi = null;
                sonSecim.DizilimAdi = null;
                degistiMi = true;
            }

            if (degistiMi)
            {
                _sonSecimServisi.Kaydet(sonSecim);
            }
        }

        #region Ürün İşlemleri

        private void UrunEkleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ZorunluAlanlariKontrolEt(UrunZorunluAlanlar))
                return;

            var urun = UrunFormundanOku();
            var urunler = urunIslemler.UrunListesiniGetir();

            bool varMi = urunler.Any(x => x.UrunAdi == urun.UrunAdi);
            if (varMi)
            {
                BildirimGoster("MesajKutusu.urunMevcut");
                return;
            }

            urunIslemler.UrunKaydet(
                urun.UrunAdi,
                urun.UrunEn,
                urun.UrunBoy,
                urun.UrunYukseklik,
                urun.UrunAgirlik,
                urun.UrunBasinc);

            SonSecimiGuncelle(urunAdi: urun.UrunAdi, dizilimTemizle: true);
            urunlistbox.Items.Add(urun.UrunAdi);
            BildirimGoster("MesajKutusu.urunBasariliEklendi");
        }

        private void UrunSilBtn_Click(object sender, RoutedEventArgs e)
        {
            if (urunlistbox.SelectedItem == null)
            {
                BildirimGoster("HataMesajlari.urunseciniz");
                return;
            }

            string silinecekUrun = urunlistbox.SelectedItem.ToString();

            if (!OnayAl($"'{silinecekUrun}' adlı ürünü silmek istiyor musunuz?"))
                return;

            urunIslemler.UrunSil(silinecekUrun);
            urunlistbox.Items.Remove(silinecekUrun);
            urunlistbox.SelectedItem = null;
            SonSecimdenSilinenleriTemizle(silinenUrunAdi: silinecekUrun);
            BildirimGoster("MesajKutusu.urunBasariliSilindi");
            UrunFormunuTemizle();
        }

        private void BtnUrunDuzenle_Click(object sender, RoutedEventArgs e)
        {
            if (urunlistbox.SelectedItem == null)
            {
                BildirimGoster("HataMesajlari.urunseciniz");
                return;
            }

            if (!ZorunluAlanlariKontrolEt(UrunZorunluAlanlar))
                return;

            string seciliUrunAdi = urunlistbox.SelectedItem.ToString();

            if (!OnayAl($"'{seciliUrunAdi}' adlı ürünü düzenlemek istiyor musunuz?"))
                return;

            urunlist = urunIslemler.UrunListesiniGetir();
            var yeniUrun = UrunFormundanOku();

            bool ayniIsimVarMi = urunlist.Any(x =>
                x.UrunAdi.Equals(yeniUrun.UrunAdi, StringComparison.OrdinalIgnoreCase) &&
                !x.UrunAdi.Equals(seciliUrunAdi, StringComparison.OrdinalIgnoreCase));

            if (ayniIsimVarMi)
            {
                BildirimGoster("MesajKutusu.urunMevcut");
                return;
            }

            foreach (var item in urunlist)
            {
                if (item.UrunAdi == seciliUrunAdi)
                {
                    item.UrunAdi = yeniUrun.UrunAdi;
                    item.UrunEn = yeniUrun.UrunEn;
                    item.UrunBoy = yeniUrun.UrunBoy;
                    item.UrunYukseklik = yeniUrun.UrunYukseklik;
                    item.UrunAgirlik = yeniUrun.UrunAgirlik;
                    item.UrunBasinc = yeniUrun.UrunBasinc;

                    urunIslemler.UrunListesiKaydet(urunlist);
                    SonSecimiGuncelle(urunAdi: yeniUrun.UrunAdi, dizilimTemizle: true);
                    Page_Loaded(this, new RoutedEventArgs());
                    urunlistbox.SelectedItem = yeniUrun.UrunAdi;
                    BildirimGoster("MesajKutusu.urunBasariliGuncellendi");
                    break;
                }
            }

        }

        private void UrunList_SelectedItem(object sender, SelectionChangedEventArgs e)
        {
            if (urunlistbox.SelectedItem == null)
                return;

            urunlist = urunIslemler.UrunListesiniGetir();
            string seciliUrun = urunlistbox.SelectedItem.ToString();

            var urun = urunlist.FirstOrDefault(x => x.UrunAdi == seciliUrun);
            if (urun != null)
            {
                UrunFormaYaz(urun);
            }
        }

        #endregion

        #region Palet İşlemleri

        private void PaletEkleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ZorunluAlanlariKontrolEt(PaletZorunluAlanlar))
                return;

            var palet = PaletFormundanOku();
            var paletler = paletIslemler.PaletListesiniGetir();

            bool varMi = paletler.Any(x => x.PaletAdi == palet.PaletAdi);
            if (varMi)
            {
                BildirimGoster("MesajKutusu.paletMevcut");
                return;
            }

            paletIslemler.PaletKaydet(
                palet.PaletAdi,
                palet.PaletEn,
                palet.PaletBoy,
                palet.PaletYukseklik);
            SonSecimiGuncelle(paletAdi: palet.PaletAdi, dizilimTemizle: true);
            BildirimGoster("MesajKutusu.paletBasariliEklendi");
            paletlistbox.Items.Add(palet.PaletAdi);
        }

        private void PaletSilBtn_Click(object sender, RoutedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
            {
                BildirimGoster("HataMesajlari.paletSeciniz");
                return;
            }

            string silinecekPalet = paletlistbox.SelectedItem.ToString();

            if (!OnayAl($"'{silinecekPalet}' adlı paleti silmek istiyor musunuz?"))
                return;

            paletIslemler.PaletSil(silinecekPalet);
            paletlistbox.Items.Remove(silinecekPalet);
            paletlistbox.SelectedItem = null;
            SonSecimdenSilinenleriTemizle(silinenPaletAdi: silinecekPalet);
            BildirimGoster("MesajKutusu.paletBasariliSilindi");
            PaletFormunuTemizle();
        }

        private void PaletDuzenleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
            {
                BildirimGoster("HataMesajlari.paletSeciniz");
                return;
            }

            if (!ZorunluAlanlariKontrolEt(PaletZorunluAlanlar))
                return;

            string seciliPaletAdi = paletlistbox.SelectedItem.ToString();

            if (!OnayAl($"'{seciliPaletAdi}' adlı paleti düzenlemek istiyor musunuz?"))
                return;

            paletlist = paletIslemler.PaletListesiniGetir();
            var yeniPalet = PaletFormundanOku();

            foreach (var item in paletlist)
            {
                if (item.PaletAdi == seciliPaletAdi)
                {
                    item.PaletAdi = yeniPalet.PaletAdi;
                    item.PaletEn = yeniPalet.PaletEn;
                    item.PaletBoy = yeniPalet.PaletBoy;
                    item.PaletYukseklik = yeniPalet.PaletYukseklik;

                    paletIslemler.PaletListesiKaydet(paletlist);
                    SonSecimiGuncelle(paletAdi: yeniPalet.PaletAdi, dizilimTemizle: true);
                    BildirimGoster("MesajKutusu.paletBasariliGuncellendi");
                    Page_Loaded(this, new RoutedEventArgs());
                    paletlistbox.SelectedItem = yeniPalet.PaletAdi;
                    break;
                }
            }
        }

        private void PaletList_SelectedItem(object sender, SelectionChangedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
                return;

            paletlist = paletIslemler.PaletListesiniGetir();
            string seciliPalet = paletlistbox.SelectedItem.ToString();

            var palet = paletlist.FirstOrDefault(x => x.PaletAdi == seciliPalet);
            if (palet != null)
            {
                PaletFormaYaz(palet);
            }
        }


        #endregion

       
    }
}