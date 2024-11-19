using Palet_Programlama.Sınıflar;
using Palet_Programlama.UserController;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            textBoxes = new List<TextBox> { myTextBox, myTextBox1, myTextBox2, myTextBox3, myTextBox4, myTextBox5, myTextBox6, myTextBox7, myTextBox8, myTextBox9 };
            placeholders = new List<TextBlock> { myPlace, myPlace1, myPlace2, myPlace3, myPlace4, myPlace5, myPlace6, myPlace7, myPlace8, myPlace9 };
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
            myTextBox.CaretBrush = Brushes.White;
            myTextBox1.CaretBrush = Brushes.White;
            myTextBox2.CaretBrush = Brushes.White;
            myTextBox3.CaretBrush = Brushes.White;
            myTextBox4.CaretBrush = Brushes.White;
            myTextBox5.CaretBrush = Brushes.White;
            myTextBox6.CaretBrush = Brushes.White;
            myTextBox7.CaretBrush = Brushes.White;
            myTextBox8.CaretBrush = Brushes.White;
            myTextBox9.CaretBrush = Brushes.White;

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
            { "myTextBox1", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/en-koli.png", "UrunEkle.preview1") },
            { "myTextBox2", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/boy-koli.png", "UrunEkle.preview2") },
            { "myTextBox3", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/yukseklik-koli.png", "UrunEkle.preview3") },
            { "myTextBox4", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/koli-agirlik.png", "UrunEkle.preview4") },
            { "myTextBox5", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/koli-basinc.png", "UrunEkle.preview5") },
            { "myTextBox7", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/en-pallet.png", "UrunEkle.preview1") },
            { "myTextBox8", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/boy-pallet.png", "UrunEkle.preview2") },
            { "myTextBox9", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/yukseklik-pallet.png", "UrunEkle.preview3") },
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
                (myTextBox, "HataMesajlari.urunadbos"),
                (myTextBox1, "HataMesajlari.urunenbos"),
                (myTextBox2, "HataMesajlari.urunboybos"),
                (myTextBox3, "HataMesajlari.urunyukseklikbos"),
                (myTextBox4, "HataMesajlari.urunagirlikbos"),
                (myTextBox5, "HataMesajlari.urunbasincbos")
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
            string urunAd = myTextBox.Text;
            double urunEn = Convert.ToDouble(myTextBox1.Text);
            double urunBoy = Convert.ToDouble(myTextBox2.Text);
            double urunYuseklik = Convert.ToDouble(myTextBox3.Text);
            double urunAgirlik = Convert.ToDouble(myTextBox4.Text);
            int urunBasinc = Convert.ToInt16(myTextBox5.Text);
            urunIslemler.UrunKaydet(urunAd, urunEn, urunBoy, urunYuseklik, urunAgirlik,urunBasinc);
            urunlistbox.Items.Add(urunAd);
            
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
            foreach (var item in urunlistbox.Items)
            {
                if (item.ToString() == SilinecekUrun)
                {
                    urunlistbox.Items.Remove(item);
                    break;
                }
            }
        }
        private void UrunList_SelectedItem(object sender, SelectionChangedEventArgs e)
        {
            urunlist = urunIslemler.UrunListesiniGetir();
            string SeciliUrun=urunlistbox.SelectedItem.ToString();
            foreach (var UrunAdBul in urunlist)
            {
                if (UrunAdBul.UrunAdi==SeciliUrun)
                {
                    myTextBox.Text=UrunAdBul.UrunAdi;
                    myTextBox1.Text=UrunAdBul.UrunEn.ToString();
                    myTextBox2.Text=UrunAdBul.UrunBoy.ToString();
                    myTextBox3.Text=UrunAdBul.UrunYukseklik.ToString();
                    myTextBox4.Text=UrunAdBul.UrunAgirlik.ToString();
                    myTextBox5.Text=UrunAdBul.UrunBasinc.ToString();
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
                (myTextBox6, "HataMesajlari.paletadbos"),
                (myTextBox7, "HataMesajlari.paletenbos"),
                (myTextBox8, "HataMesajlari.paletboybos"),
                (myTextBox9, "HataMesajlari.paletyukseklikbos")

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
            string paletAd = myTextBox6.Text;
            double paletEn = Convert.ToDouble(myTextBox7.Text);
            double paletBoy = Convert.ToDouble(myTextBox8.Text);
            double paletYuseklik = Convert.ToDouble(myTextBox9.Text);

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
            foreach (var item in paletlistbox.Items)
            {
                if (item.ToString() == SilinecekPalet)
                {
                    paletlistbox.Items.Remove(item);
                    break;
                }
            }
        }
        private void PaletDuzenleBtn_Click(object sender, RoutedEventArgs e)
        {
            var textBoxes = new (TextBox, string)[]
             {
                (myTextBox6, "HataMesajlari.paletadbos"),
                (myTextBox7, "HataMesajlari.paletenbos"),
                (myTextBox8, "HataMesajlari.paletboybos"),
                (myTextBox9, "HataMesajlari.paletyukseklikbos")

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
            string paletAd = myTextBox6.Text;
            double paletEn = Convert.ToDouble(myTextBox7.Text);
            double paletBoy = Convert.ToDouble(myTextBox8.Text);
            double paletYuseklik = Convert.ToDouble(myTextBox9.Text);
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
            paletlist = paletIslemler.PaletListesiniGetir();
            string SeciliPalet = paletlistbox.SelectedItem.ToString();
            foreach (var PaletAdBul in paletlist)
            {
                if (PaletAdBul.PaletAdi == SeciliPalet)
                {
                    myTextBox6.Text = PaletAdBul.PaletAdi;
                    myTextBox7.Text = PaletAdBul.PaletEn.ToString();
                    myTextBox8.Text = PaletAdBul.PaletBoy.ToString();
                    myTextBox9.Text = PaletAdBul.PaletYukseklik.ToString();
                    return;
                }
            }
        }
        #endregion
    }
}
 