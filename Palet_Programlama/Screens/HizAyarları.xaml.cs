using Palet_Programlama.Sayfalar.HizAyarlari.Models;
using Palet_Programlama.Sayfalar.HizAyarlari.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            HizVerileriniYukle();
        }

        private void HizVerileriniYukle()
        {
            var servis = new HizServisi();
            HizVerileri veri = servis.HizVerileriniYukle();

            if (veri.v50 > 0) TxtV50.Text = $"% {veri.v50}";
            if (veri.v51 > 0) TxtV51.Text = $"% {veri.v51}";
            if (veri.v52 > 0) TxtV52.Text = $"% {veri.v52}";
            if (veri.v53 > 0) TxtV53.Text = $"% {veri.v53}";
            if (veri.v54 > 0) TxtV54.Text = $"% {veri.v54}";
            if (veri.v55 > 0) TxtV55.Text = $"% {veri.v55}";
            if (veri.v56 > 0) TxtV56.Text = $"% {veri.v56}";
            if (veri.v57 > 0) TxtV57.Text = $"% {veri.v57}";
            if (veri.v58 > 0) TxtV58.Text = $"% {veri.v58}";
            if (veri.v59 > 0) TxtV59.Text = $"% {veri.v59}";
            if (veri.v60 > 0) TxtV60.Text = $"% {veri.v60}";
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

        private void BtnHizKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (!TextBoxDegeriGecerliMi(TxtV50) ||
                !TextBoxDegeriGecerliMi(TxtV51) ||
                !TextBoxDegeriGecerliMi(TxtV52) ||
                !TextBoxDegeriGecerliMi(TxtV53) ||
                !TextBoxDegeriGecerliMi(TxtV54) ||
                !TextBoxDegeriGecerliMi(TxtV55) ||
                !TextBoxDegeriGecerliMi(TxtV56) ||
                !TextBoxDegeriGecerliMi(TxtV57) ||
                !TextBoxDegeriGecerliMi(TxtV58) ||
                !TextBoxDegeriGecerliMi(TxtV59) ||
                !TextBoxDegeriGecerliMi(TxtV60))
            {
                MessageBox.Show("Tüm hız değerleri 0 ile 100 arasında olmalıdır.");
                return;
            }

            var servis = new HizServisi();
            var veri = HizVerileriniOlustur();

            servis.HizVerileriniKaydet(veri);

            MessageBox.Show("Hız ayarları kaydedildi.");
        }

        private void SadeceSayiGirisi_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void SadeceSayiGirisi_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string yapistirilanMetin = (string)e.DataObject.GetData(typeof(string));

            if (!Regex.IsMatch(yapistirilanMetin, "^[0-9]+$"))
            {
                e.CancelCommand();
            }
        }

        private bool TextBoxDegeriGecerliMi(TextBox textBox)
        {
            int deger = YuzdeDegeriniAl(textBox);
            return deger >= 0 && deger <= 100;
        }

        private HizVerileri HizVerileriniOlustur()
        {
            return new HizVerileri
            {
                v50 = YuzdeDegeriniAl(TxtV50),
                v51 = YuzdeDegeriniAl(TxtV51),
                v52 = YuzdeDegeriniAl(TxtV52),
                v53 = YuzdeDegeriniAl(TxtV53),
                v54 = YuzdeDegeriniAl(TxtV54),
                v55 = YuzdeDegeriniAl(TxtV55),
                v56 = YuzdeDegeriniAl(TxtV56),
                v57 = YuzdeDegeriniAl(TxtV57),
                v58 = YuzdeDegeriniAl(TxtV58),
                v59 = YuzdeDegeriniAl(TxtV59),
                v60 = YuzdeDegeriniAl(TxtV60)
            };
        }

        private int YuzdeDegeriniAl(TextBox textBox)
        {
            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
                return 0;

            string temizMetin = textBox.Text
                .Replace("%", "")
                .Trim();

            return int.TryParse(temizMetin, out int sonuc) ? sonuc : 0;
        }


        private bool HizVerileriGecerliMi(HizVerileri veri)
        {
            return DegerGecerliMi(veri.v50)
                && DegerGecerliMi(veri.v51)
                && DegerGecerliMi(veri.v52)
                && DegerGecerliMi(veri.v53)
                && DegerGecerliMi(veri.v54)
                && DegerGecerliMi(veri.v55)
                && DegerGecerliMi(veri.v56)
                && DegerGecerliMi(veri.v57)
                && DegerGecerliMi(veri.v58)
                && DegerGecerliMi(veri.v59)
                && DegerGecerliMi(veri.v60);
        }

        private bool DegerGecerliMi(int deger)
        {
            return deger >= 0 && deger <= 100;
        }
    }
}