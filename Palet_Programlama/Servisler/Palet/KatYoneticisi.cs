using Palet_Programlama.Modeller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Palet_Programlama.Servisler.Palet
{
    public sealed class KatYoneticisi
    {
        private readonly Dictionary<int, List<KatUrunu>> _katlar = new();
        public int MaksKat { get; set; } = 10;

        public int AktifKat { get; private set; } = 1;

        public IReadOnlyDictionary<int, List<KatUrunu>> TumKatlar => _katlar;

        public void KatDegistir(
            int yeniKat,
            Canvas canvas,
            Rectangle? seciliKutu,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp)
        {
            if (yeniKat > MaksKat)
            {
                return;
            }

            if (yeniKat > AktifKat)
            {
                bool mevcutKatBosMu = !canvas.Children.OfType<Rectangle>().Any();
                if (mevcutKatBosMu)
                    return;
            }

            KatiKaydet(canvas);

            AktifKat = yeniKat < 1 ? 1 : yeniKat;

            if (seciliKutu is not null)
            {
                seciliKutu.Stroke = Brushes.Transparent;
                seciliKutu.StrokeThickness = 0;
            }

            KatiYukle(canvas, onDown, onMove, onUp);
        }

        public void KatiKaydetDisardan(Canvas canvas) => KatiKaydet(canvas);

        public void KatiYukleDisardan(
            Canvas canvas,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp) => KatiYukle(canvas, onDown, onMove, onUp);

        private void KatiKaydet(Canvas canvas)
        {
            var liste = canvas.Children.OfType<Rectangle>()
                .Select(r =>
                {
                    double left = Canvas.GetLeft(r); if (double.IsNaN(left)) left = 0;
                    double top = Canvas.GetTop(r); if (double.IsNaN(top)) top = 0;

                    double w = r.ActualWidth > 0 ? r.ActualWidth : r.Width;
                    double h = r.ActualHeight > 0 ? r.ActualHeight : r.Height;

                    double cx = left + w / 2.0;
                    double cy = top + h / 2.0;

                    var yon = r.Tag is UrunYonu t
                        ? t
                        : (w >= h ? UrunYonu.Yatay : UrunYonu.Dikey);

                    double dikeyUz = (yon == UrunYonu.Dikey) ? h : w;
                    double yatayUz = (yon == UrunYonu.Dikey) ? w : h;

                    return new KatUrunu(cx, cy, dikeyUz, yatayUz, yon);
                })
                .ToList();

            _katlar[AktifKat] = liste;
        }

        private void KatiYukle(
            Canvas canvas,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp)
        {
            var silinecekler = canvas.Children.OfType<Rectangle>().ToList();
            foreach (var r in silinecekler)
                canvas.Children.Remove(r);

            if (!_katlar.TryGetValue(AktifKat, out var liste))
                return;

            foreach (var u in liste)
            {
                double w = Genislik(u);
                double h = Yukseklik(u);

                double left = u.MerkezX - w / 2.0;
                double top = u.MerkezY - h / 2.0;

                string resim = u.Yon == UrunYonu.Dikey
                    ? "pack://application:,,,/Resimler/DizilimYap/dikey_kutu.png"
                    : "pack://application:,,,/Resimler/DizilimYap/yatay_kutu.png";

                var rect = new Rectangle
                {
                    Width = w,
                    Height = h,
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                    Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(resim, UriKind.Absolute))
                    },
                    Tag = u.Yon
                };

                rect.MouseDown += onDown;
                rect.MouseMove += onMove;
                rect.MouseUp += onUp;

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                canvas.Children.Add(rect);
            }
        }

        private static double Genislik(KatUrunu u) =>
            u.Yon == UrunYonu.Dikey ? u.YatayUzunluk : u.DikeyUzunluk;

        private static double Yukseklik(KatUrunu u) =>
            u.Yon == UrunYonu.Dikey ? u.DikeyUzunluk : u.YatayUzunluk;
    }
}