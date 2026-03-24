using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar
{
    public partial class HizAyarları : Page
    {
        private readonly Frame MainFrame;
        private readonly Dictionary<string, List<Shape>> _okSekilleri = new();

        public HizAyarları(Frame main)
        {
            InitializeComponent();
            UstMenuControl.AktifSayfa = "HizAyarlari";
            MainFrame = main;

            InitializePercentageTextBoxes();
            OkYollariniCiz();
        }

        private void InitializePercentageTextBoxes()
        {
            foreach (var control in FindVisualChildren<TextBox>(this))
            {
                if (string.IsNullOrWhiteSpace(control.Text) || control.Text == "% ")
                {
                    control.Text = "% ";
                }

                control.TextChanged += TextBox_TextChanged;
            }
        }

        private void OkYollariniCiz()
        {
            OkCanvas.Children.Clear();
            _okSekilleri.Clear();

            var oklar = new List<(string Ad, Point Baslangic, Point Bitis)>
            {
                ("V60", new Point(1132, 157), new Point(284, 157)),
                ("V50", new Point(284, 157),  new Point(286, 306)),
                ("V51", new Point(286, 306),  new Point(290, 421)),
                ("V52", new Point(290, 421),  new Point(428, 521)),
                ("V53", new Point(428, 521),  new Point(427, 398)),
                ("V54", new Point(427, 398),  new Point(427, 272)),
                ("V55", new Point(427, 272),  new Point(996, 272)),
                ("V56", new Point(996, 272),  new Point(996, 388)),
                ("V57", new Point(996, 388),  new Point(1126, 520)),
                ("V58", new Point(1126, 520), new Point(1130, 411)),
                ("V59", new Point(1130, 411), new Point(1132, 157))
            };

            foreach (var ok in oklar)
            {
                OkCiz(ok.Ad, ok.Baslangic, ok.Bitis);
            }
        }

        private void OkCiz(string okAdi, Point baslangic, Point bitis)
        {
            const double kisaltma = 35.0;

            double dx = bitis.X - baslangic.X;
            double dy = bitis.Y - baslangic.Y;
            double uzunluk = Math.Sqrt(dx * dx + dy * dy);

            if (uzunluk <= kisaltma * 2)
                return;

            double birimX = dx / uzunluk;
            double birimY = dy / uzunluk;

            Point yeniBaslangic = new Point(
                baslangic.X + birimX * kisaltma,
                baslangic.Y + birimY * kisaltma);

            Point yeniBitis = new Point(
                bitis.X - birimX * kisaltma,
                bitis.Y - birimY * kisaltma);

            var line = new Line
            {
                X1 = yeniBaslangic.X,
                Y1 = yeniBaslangic.Y,
                X2 = yeniBitis.X,
                Y2 = yeniBitis.Y,
                Stroke = Brushes.White,
                StrokeThickness = 4,
                SnapsToDevicePixels = true
            };

            OkCanvas.Children.Add(line);

            var sekiller = new List<Shape> { line };
            OkBasiCiz(yeniBaslangic, yeniBitis, sekiller);

            _okSekilleri[okAdi] = sekiller;
        }

        private void OkBasiCiz(Point baslangic, Point bitis, List<Shape> sekiller)
        {
            double aci = Math.Atan2(bitis.Y - baslangic.Y, bitis.X - baslangic.X);

            double uzunluk = 18;
            double sapma = Math.PI / 7;

            Point sol = new Point(
                bitis.X - uzunluk * Math.Cos(aci - sapma),
                bitis.Y - uzunluk * Math.Sin(aci - sapma));

            Point sag = new Point(
                bitis.X - uzunluk * Math.Cos(aci + sapma),
                bitis.Y - uzunluk * Math.Sin(aci + sapma));

            var ok1 = new Line
            {
                X1 = bitis.X,
                Y1 = bitis.Y,
                X2 = sol.X,
                Y2 = sol.Y,
                Stroke = Brushes.White,
                StrokeThickness = 4,
                SnapsToDevicePixels = true
            };

            var ok2 = new Line
            {
                X1 = bitis.X,
                Y1 = bitis.Y,
                X2 = sag.X,
                Y2 = sag.Y,
                Stroke = Brushes.White,
                StrokeThickness = 4,
                SnapsToDevicePixels = true
            };

            OkCanvas.Children.Add(ok1);
            OkCanvas.Children.Add(ok2);

            sekiller.Add(ok1);
            sekiller.Add(ok2);
        }

        private void HizTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox textBox || textBox.Tag is not string secilenOk)
                return;

            foreach (var item in _okSekilleri)
            {
                Brush renk = item.Key == secilenOk ? Brushes.LimeGreen : Brushes.White;

                foreach (var sekil in item.Value)
                {
                    sekil.Stroke = renk;
                }
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

                    if (child is T t)
                        yield return t;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    
    
    }
}