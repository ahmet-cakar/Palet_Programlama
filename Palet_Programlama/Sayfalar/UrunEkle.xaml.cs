using Palet_Programlama.Sınıflar;
using Palet_Programlama.UserController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for UrunEkle.xaml
    /// </summary>
    public partial class UrunEkle : Page
    {
        private Frame MainFrame;
        private List<TextBox> textBoxes;
        private List<TextBlock> placeholders;
        UrunIslemler urunIslemler=new UrunIslemler();
        PaletIslemler paletIslemler=new PaletIslemler();
        private List<Urun> urunlist;
        private List<Palet> paletlist;
        public UrunEkle(Frame Main)
        {
            InitializeComponent();
            textBoxes = new List<TextBox> { txtUrunAdi, txtUrunEn, txtUrunBoy, txtUrunYukseklik, txtUrunAgirlik, txtUrunBasinc, txtPaletAdi, txtPaletEn, txtPaletBoy, txtPaletYukseklik};
            placeholders = new List<TextBlock> { phUrunAdi, phUrunEn, phUrunBoy, phUrunYukseklik, phUrunAgirlik, phUrunBasinc, phPaletAdi, phPaletEn, phPaletBoy, phPaletYukseklik };
            this.MainFrame = Main;
            this.Loaded += Page_Loaded;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
                urunlistbox.Items.Clear();
                urunlist = urunIslemler.UrunListesiniGetir();
                foreach (var item in urunlist)
                {
                    urunlistbox.Items.Add(item.UrunAdi);
                }
                paletlistbox.Items.Clear();
                paletlist = paletIslemler.PaletListesiniGetir();
                foreach (var item in paletlist)
                {
                    paletlistbox.Items.Add(item.PaletAdi);
                }
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

        #region TextBox Controlleri
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Sadece sayılar ve tek bir nokta içeren sayısal formatı kontrol eden regex
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
        private readonly Dictionary<string, Tuple<string, string>> textBoxData = new Dictionary<string, Tuple<string, string>>
        {
            { "txtUrunEn", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/en-koli.png", "UrunEkle.preview1") },
            { "txtUrunBoy", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/boy-koli.png", "UrunEkle.preview2") },
            { "txtUrunYukseklik", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/yukseklik-koli.png", "UrunEkle.preview3") },
            { "txtUrunAgirlik", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/koli-agirlik.png", "UrunEkle.preview4") },
            { "txtUrunBasinc", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/koli-basinc.png", "UrunEkle.preview5") },
            { "txtPaletEn", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/en-pallet.png", "UrunEkle.preview1") },
            { "txtPaletBoy", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/boy-pallet.png", "UrunEkle.preview2") },
            { "txtPaletYukseklik", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/yukseklik-pallet.png", "UrunEkle.preview3") },
        };
        private void UrunPalet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox triggeredTextBox && textBoxData.TryGetValue(triggeredTextBox.Name, out var data))
            {
                // Resim kaynağını değiştir
                onizleımage.Source = new BitmapImage(new Uri(data.Item1));

                // TextBlock metnini güncelle
                priviewtextblock.Text = LanguageConverter.GetString(data.Item2);
            }

            UpdatePlaceholder();
        }

        private void UrunPalet_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholder();
        }
        private void UrunPalet_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholder();
        }
        private void UpdatePlaceholder()
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (string.IsNullOrEmpty(textBoxes[i].Text))
                {
                    placeholders[i].Visibility = Visibility.Visible; // Placeholder görünür
                }
                else
                {
                    placeholders[i].Visibility = Visibility.Collapsed; // Placeholder gizlenir
                }
            }
        }
       
        #region Ürün İşlemleri
        private void UrunEkleBtn_Click(object sender, RoutedEventArgs e)
        {
            var textBoxes = new (TextBox, string)[]
             {
                (txtUrunAdi, "HataMesajlari.urunadbos"),
                (txtUrunEn, "HataMesajlari.urunenbos"),
                (txtUrunBoy, "HataMesajlari.urunboybos"),
                (txtUrunYukseklik, "HataMesajlari.urunyukseklikbos"),
                (txtUrunAgirlik, "HataMesajlari.urunagirlikbos"),
                (txtUrunBasinc, "HataMesajlari.urunbasincbos")
             };

            // Boş olan bir TextBox varsa uyarı göster ve işlemi sonlandır
            foreach (var (textBox, message) in textBoxes)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    BildirimKutusu bildirimKutusu=new BildirimKutusu();
                    bildirimKutusu.MesajGonder("ButtonKey.btntamam",message);
                    bildirimKutusu.Show();
                    return;
                }
            }
            string urunAd = txtUrunAdi.Text;
            double urunEn = Convert.ToDouble(txtUrunEn.Text);
            double urunBoy = Convert.ToDouble(txtUrunBoy.Text);
            double urunYuseklik = Convert.ToDouble(txtUrunYukseklik.Text);
            double urunAgirlik = Convert.ToDouble(txtUrunAgirlik.Text);
            int urunBasinc = Convert.ToInt16(txtUrunBasinc.Text);
            var urunler = urunIslemler.UrunListesiniGetir();
            bool varMi = urunler.Any(x => x.UrunAdi == urunAd);
            if (varMi){ BildirimGoster("MesajKutusu.urunMevcut"); return; }
            urunIslemler.UrunKaydet(urunAd, urunEn, urunBoy, urunYuseklik, urunAgirlik,urunBasinc);
            urunlistbox.Items.Add(urunAd);
            
        }


        private void BildirimGoster(string mesajKey, string butonKey = "MesajKutusu.tamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void UrunSilBtn_Click(object sender, RoutedEventArgs e)
        { 
            if (urunlistbox.SelectedItem ==null)
            {
                //Mesaj kutusu Eklenecek
                return;
            }
            string SilinecekUrun=urunlistbox.SelectedItem.ToString();
            urunIslemler.UrunSil(SilinecekUrun);
            urunlistbox.Items.Remove(SilinecekUrun);
            urunlistbox.SelectedItem = null;
            txtUrunAdi.Clear();
            txtUrunEn.Clear();
            txtUrunBoy.Clear();
            txtUrunYukseklik.Clear();
            txtUrunAgirlik.Clear();
            txtUrunBasinc.Clear();


        }
        private void UrunList_SelectedItem(object sender, SelectionChangedEventArgs e)
        {

            if(urunlistbox.SelectedItem == null)
            {
                return;
            }

            urunlist = urunIslemler.UrunListesiniGetir();
            string SeciliUrun=urunlistbox.SelectedItem.ToString();
            foreach (var UrunAdBul in urunlist)
            {
                if (UrunAdBul.UrunAdi==SeciliUrun)
                {
                    txtUrunAdi.Text=UrunAdBul.UrunAdi;
                    txtUrunEn.Text=UrunAdBul.UrunEn.ToString();
                    txtUrunBoy.Text=UrunAdBul.UrunBoy.ToString();
                    txtUrunYukseklik.Text=UrunAdBul.UrunYukseklik.ToString();
                    txtUrunAgirlik.Text=UrunAdBul.UrunAgirlik.ToString();
                    txtUrunBasinc.Text=UrunAdBul.UrunBasinc.ToString();
                    return;
                }
            }
        }
        #endregion
        #region Palet İşlemleri
        private void PaletEkleBtn_Click(object sender, RoutedEventArgs e)
        {
            var textBoxes = new (TextBox, string)[]
             {
                (txtPaletAdi, "HataMesajlari.paletadbos"),
                (txtPaletEn, "HataMesajlari.paletenbos"),
                (txtPaletBoy, "HataMesajlari.paletboybos"),
                (txtPaletYukseklik, "HataMesajlari.paletyukseklikbos")

             };

            // Boş olan bir TextBox varsa uyarı göster ve işlemi sonlandır
            foreach (var (textBox, message) in textBoxes)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    BildirimKutusu bildirimKutusu = new BildirimKutusu();
                    bildirimKutusu.MesajGonder("ButtonKey.btntamam", message);
                    bildirimKutusu.Show();
                    return;
                }
            }
            string paletAd = txtPaletAdi.Text;
            double paletEn = Convert.ToDouble(txtPaletEn.Text);
            double paletBoy = Convert.ToDouble(txtPaletBoy.Text);
            double paletYuseklik = Convert.ToDouble(txtPaletYukseklik.Text);

            var paletler = paletIslemler.PaletListesiniGetir();
            bool varMi = paletler.Any(x => x.PaletAdi == paletAd);
            if (varMi) { BildirimGoster("MesajKutusu.paletMevcut"); return; }


            paletIslemler.PaletKaydet(paletAd, paletEn, paletBoy, paletYuseklik);
            paletlistbox.Items.Add(paletAd);
            
        }
        private void PaletSilBtn_Click(object sender, RoutedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
            {
                //Mesaj kutusu Eklenecek
                return;
            }
            string SilinecekPalet = paletlistbox.SelectedItem.ToString();
            paletIslemler.PaletSil(SilinecekPalet);
            paletlistbox.Items.Remove(SilinecekPalet);
            urunlistbox.SelectedItem = null;
            txtPaletAdi.Clear();
            txtPaletEn.Clear();
            txtPaletBoy.Clear();
            txtPaletYukseklik.Clear();


        }
        private void PaletDuzenleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
            {
                //Mesaj kutusu Eklenecek
                return;
            }


            var textBoxes = new (TextBox, string)[]
             {
                (txtPaletAdi, "HataMesajlari.paletadbos"),
                (txtPaletEn, "HataMesajlari.paletenbos"),
                (txtPaletBoy, "HataMesajlari.paletboybos"),
                (txtPaletYukseklik, "HataMesajlari.paletyukseklikbos")

             };

            // Boş olan bir TextBox varsa uyarı göster ve işlemi sonlandır
            foreach (var (textBox, message) in textBoxes)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    BildirimKutusu bildirimKutusu = new BildirimKutusu();
                    bildirimKutusu.MesajGonder("ButtonKey.btntamam", message);
                    bildirimKutusu.Show();
                    return;
                }
            }
            paletlist = paletIslemler.PaletListesiniGetir();
            string paletAd = txtPaletAdi.Text;
            double paletEn = Convert.ToDouble(txtPaletEn.Text);
            double paletBoy = Convert.ToDouble(txtPaletBoy.Text);
            double paletYuseklik = Convert.ToDouble(txtPaletYukseklik.Text);
            foreach (var item in paletlist)
            {
               if (item.PaletAdi == paletlistbox.SelectedItem.ToString())
                {
                    item.PaletAdi = paletAd;
                    item.PaletEn = paletEn;
                    item.PaletBoy = paletBoy;
                    item.PaletYukseklik = paletYuseklik;
                    paletIslemler.PaletListesiKaydet(paletlist);
                    Page_Loaded(this, new RoutedEventArgs());
                    break;
                }
            }
            
        }
        private void PaletList_SelectedItem(object sender, SelectionChangedEventArgs e)
        {
            if (paletlistbox.SelectedItem == null)
            {
                //Mesaj kutusu Eklenecek
                return;
            }


            paletlist = paletIslemler.PaletListesiniGetir();
            string SeciliPalet = paletlistbox.SelectedItem.ToString();
            foreach (var PaletAdBul in paletlist)
            {
                if (PaletAdBul.PaletAdi == SeciliPalet)
                {
                    txtPaletAdi.Text = PaletAdBul.PaletAdi;
                    txtPaletEn.Text = PaletAdBul.PaletEn.ToString();
                    txtPaletBoy.Text = PaletAdBul.PaletBoy.ToString();
                    txtPaletYukseklik.Text = PaletAdBul.PaletYukseklik.ToString();
                    return;
                }
            }
        }
        #endregion
    }
}
 