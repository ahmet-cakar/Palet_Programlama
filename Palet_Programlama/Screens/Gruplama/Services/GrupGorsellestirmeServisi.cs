using Palet_Programlama.Screens.Gruplama.Helpers;
using Palet_Programlama.Screens.Gruplama.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Palet_Programlama.Screens.Gruplama.Services
{
    public sealed class GrupGorsellestirmeServisi
    {
        private readonly Dictionary<Rectangle, TextBlock> _grupEtiketleri = new();

        public void GrupEtiketiniGuncelle(
            Canvas canvas,
            Rectangle kutu,
            int aktifKatNo,
            Dictionary<Rectangle, GrupAtamaBilgisi> grupAtamalari,
            KoliGeometriYardimcisi geometri)
        {
            if (_grupEtiketleri.TryGetValue(kutu, out var eskiEtiket))
            {
                canvas.Children.Remove(eskiEtiket);
                _grupEtiketleri.Remove(kutu);
            }

            if (!grupAtamalari.TryGetValue(kutu, out var bilgi))
                return;

            if (bilgi.KatNo != aktifKatNo)
                return;

            Rect rect = geometri.GetRect(kutu);

            var etiket = new TextBlock
            {
                Text = bilgi.GrupNo.ToString(),
                Foreground = Brushes.White,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(190, 0, 0, 0)),
                Padding = new Thickness(6, 2, 6, 2),
                IsHitTestVisible = false
            };

            canvas.Children.Add(etiket);
            etiket.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double left = rect.Left + (rect.Width - etiket.DesiredSize.Width) / 2;
            double top = rect.Top + (rect.Height - etiket.DesiredSize.Height) / 2;

            Canvas.SetLeft(etiket, left);
            Canvas.SetTop(etiket, top);
            Panel.SetZIndex(etiket, 999);

            _grupEtiketleri[kutu] = etiket;
        }

        public void TumGrupEtiketleriniTemizle(Canvas canvas)
        {
            foreach (var etiket in _grupEtiketleri.Values.ToList())
            {
                canvas.Children.Remove(etiket);
            }

            _grupEtiketleri.Clear();
        }

        public void AktifKattakiGrupEtiketleriniYenile(
             Canvas canvas,
             int aktifKatNo,
             Dictionary<Rectangle, GrupAtamaBilgisi> grupAtamalari,
             KoliGeometriYardimcisi geometri)
        {
            TumGrupEtiketleriniTemizle(canvas);

            var kutular = canvas.Children.OfType<Rectangle>().ToList();

            foreach (var kutu in kutular)
            {
                if (grupAtamalari.TryGetValue(kutu, out var bilgi) && bilgi.KatNo == aktifKatNo)
                {
                    GrupEtiketiniGuncelle(canvas, kutu, aktifKatNo, grupAtamalari, geometri);
                }
            }
        }

        public void KutuGrupGorseliniGuncelle(
            Canvas canvas,
            Rectangle kutu,
            int aktifKatNo,
            Dictionary<Rectangle, GrupAtamaBilgisi> grupAtamalari,
            KoliGeometriYardimcisi geometri)
        {
            if (grupAtamalari.TryGetValue(kutu, out var bilgi) && bilgi.KatNo == aktifKatNo)
            {
                kutu.Stroke = Brushes.DeepSkyBlue;
                kutu.StrokeThickness = 2;
                GrupEtiketiniGuncelle(canvas, kutu, aktifKatNo, grupAtamalari, geometri);
            }
            else
            {
                kutu.Stroke = Brushes.Transparent;
                kutu.StrokeThickness = 0;

                if (_grupEtiketleri.TryGetValue(kutu, out var etiket))
                {
                    canvas.Children.Remove(etiket);
                    _grupEtiketleri.Remove(kutu);
                }
            }
        }

        public void AktifKattakiTumKutuGorselleriniYenile(
          Canvas canvas,
          int aktifKatNo,
          Dictionary<Rectangle, GrupAtamaBilgisi> grupAtamalari,
          KoliGeometriYardimcisi geometri)
        {
            var kutular = canvas.Children.OfType<Rectangle>().ToList();

            foreach (var kutu in kutular)
            {
                KutuGrupGorseliniGuncelle(canvas, kutu, aktifKatNo, grupAtamalari, geometri);
            }
        }
    }
}