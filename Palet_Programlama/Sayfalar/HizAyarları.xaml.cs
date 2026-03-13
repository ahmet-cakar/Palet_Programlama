using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Palet_Programlama.Sayfalar
{
    /// <summary>
    /// Interaction logic for HizAyarları.xaml
    /// </summary>
    public partial class HizAyarları : Page
    {
        private Frame MainFrame;
        public HizAyarları(Frame Main)
        {
            InitializeComponent();
            InitializePercentageTextBoxes();
            this.MainFrame = Main;
        }
        private void InitializePercentageTextBoxes()
        {
            foreach (var control in FindVisualChildren<TextBox>(this))
            {
                // Başlangıç metnini % olarak ayarlayın
                if (string.IsNullOrWhiteSpace(control.Text) || control.Text == "% ")
                {
                    control.Text = "% ";
                }
                control.TextChanged += TextBox_TextChanged;
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.Text.StartsWith("% "))
                {
                    textBox.Text = "% " + textBox.Text.TrimStart('%').TrimStart();
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
