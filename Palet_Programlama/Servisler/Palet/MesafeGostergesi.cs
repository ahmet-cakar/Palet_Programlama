using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Palet_Programlama.Servisler.Palet
{
    public sealed class MesafeGostergesi
    {
        private readonly Line sol = new() { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        private readonly Line sag = new() { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        private readonly Line ust = new() { Stroke = Brushes.Magenta, StrokeThickness = 3 };
        private readonly Line alt = new() { Stroke = Brushes.Magenta, StrokeThickness = 3 };

        private readonly TextBlock solYazi = new() { Foreground = Brushes.Magenta, FontSize = 20, Background = Brushes.Black, Padding = new Thickness(4, 1, 4, 1) };
        private readonly TextBlock sagYazi = new() { Foreground = Brushes.Magenta, FontSize = 20, Background = Brushes.Black, Padding = new Thickness(4, 1, 4, 1) };
        private readonly TextBlock ustYazi = new() { Foreground = Brushes.Magenta, FontSize = 20, Background = Brushes.Black, Padding = new Thickness(4, 1, 4, 1) };
        private readonly TextBlock altYazi = new() { Foreground = Brushes.Magenta, FontSize = 20, Background = Brushes.Black, Padding = new Thickness(4, 1, 4, 1) };
        private Canvas? _canvas;

        public void Baslat(Canvas canvas)
        {
            _canvas = canvas;

            canvas.Children.Add(sol); canvas.Children.Add(sag); canvas.Children.Add(ust); canvas.Children.Add(alt);
            canvas.Children.Add(solYazi); canvas.Children.Add(sagYazi); canvas.Children.Add(ustYazi); canvas.Children.Add(altYazi);

            Panel.SetZIndex(sol, 999); Panel.SetZIndex(sag, 999); Panel.SetZIndex(ust, 999); Panel.SetZIndex(alt, 999);
            Panel.SetZIndex(solYazi, 1000); Panel.SetZIndex(sagYazi, 1000); Panel.SetZIndex(ustYazi, 1000); Panel.SetZIndex(altYazi, 1000);

            Gizle();
        }

        public void Goster()
        {
            SetVisible(Visibility.Visible);
        }

        public void Gizle()
        {
            SetVisible(Visibility.Collapsed);
        }


        public void Guncelle(Rect moving, IReadOnlyList<Rect> digerleri)
        {
            if (_canvas is null) return;

            double leftDistance = moving.Left;
            double rightDistance = _canvas.ActualWidth - moving.Right;
            double topDistance = moving.Top;
            double bottomDistance = _canvas.ActualHeight - moving.Bottom;

            foreach (var o in digerleri)
            {
                if (moving.Left > o.Right) leftDistance = Math.Min(leftDistance, moving.Left - o.Right);
                if (moving.Right < o.Left) rightDistance = Math.Min(rightDistance, o.Left - moving.Right);
                if (moving.Top > o.Bottom) topDistance = Math.Min(topDistance, moving.Top - o.Bottom);
                if (moving.Bottom < o.Top) bottomDistance = Math.Min(bottomDistance, o.Top - moving.Bottom);
            }

            sol.X1 = moving.Left; sol.Y1 = moving.Top + moving.Height / 2;
            sol.X2 = moving.Left - leftDistance; sol.Y2 = sol.Y1;
            solYazi.Text = (leftDistance * 2).ToString("0");
            YaziKonumla(solYazi, (sol.X1 + sol.X2) / 2, sol.Y1);

            sag.X1 = moving.Right; sag.Y1 = moving.Top + moving.Height / 2;
            sag.X2 = moving.Right + rightDistance; sag.Y2 = sag.Y1;
            sagYazi.Text = (rightDistance * 2).ToString("0");
            YaziKonumla(sagYazi, (sag.X1 + sag.X2) / 2, sag.Y1);

            ust.X1 = moving.Left + moving.Width / 2; ust.Y1 = moving.Top;
            ust.X2 = ust.X1; ust.Y2 = moving.Top - topDistance;
            ustYazi.Text = (topDistance * 2).ToString("0");
            YaziKonumla(ustYazi, ust.X1, (ust.Y1 + ust.Y2) / 2);

            alt.X1 = moving.Left + moving.Width / 2; alt.Y1 = moving.Bottom;
            alt.X2 = alt.X1; alt.Y2 = moving.Bottom + bottomDistance;
            altYazi.Text = (bottomDistance * 2).ToString("0");
            YaziKonumla(altYazi, alt.X1, (alt.Y1 + alt.Y2) / 2);
        }

        private void YaziKonumla(TextBlock tb, double x, double y)
        {
            if (_canvas is null) return;

            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double w = tb.DesiredSize.Width;
            double h = tb.DesiredSize.Height;

            double maxX = Math.Max(0, _canvas.ActualWidth - w);
            double maxY = Math.Max(0, _canvas.ActualHeight - h);

            x = Math.Max(0, Math.Min(x, maxX));
            y = Math.Max(0, Math.Min(y, maxY));

            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);
        }

        private void SetVisible(Visibility v)
        {
            sol.Visibility = v; sag.Visibility = v; ust.Visibility = v; alt.Visibility = v;
            solYazi.Visibility = v; sagYazi.Visibility = v; ustYazi.Visibility = v; altYazi.Visibility = v;
        }


    }


}
