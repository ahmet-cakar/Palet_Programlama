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
        
        public UrunEkle(Frame Main)
        {
            InitializeComponent();
            textBoxes = new List<TextBox> { myTextBox, myTextBox1, myTextBox2, myTextBox3, myTextBox4, myTextBox5, myTextBox6, myTextBox7, myTextBox8, myTextBox9 };
            placeholders = new List<TextBlock> { myPlace, myPlace1, myPlace2, myPlace3, myPlace4, myPlace5, myPlace6, myPlace7, myPlace8, myPlace9 };
            this.MainFrame = Main;
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
            { "myTextBox1", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/en-preview.png", "UrunEkle.preview1") },
            { "myTextBox2", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/boy-preview.png", "UrunEkle.preview2") },
            { "myTextBox3", Tuple.Create("pack://application:,,,/Resimler/UrunEkle/yukseklik-preview.png", "UrunEkle.preview3") }
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
    }
}
