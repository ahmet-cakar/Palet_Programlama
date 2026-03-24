using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Palet_Programlama.Servisler.PaletMethod
{
    public sealed class YerlesimMotoru
    {
        public double SnapEsigi { get; set; } = 3.0;        // px
        public double CakismaEpsilon { get; set; } = 0.5;   // px

        public bool CakisiyorMu(Rect aday, IEnumerable<Rect> digerleri)
        => digerleri.Any(r => Cakisiyor(aday, r));

        public Rect SiniraSikistir(Rect r, double alanGenislik, double alanYukseklik)
        {
            var left = Math.Max(0, Math.Min(r.Left, alanGenislik - r.Width));
            var top = Math.Max(0, Math.Min(r.Top, alanYukseklik - r.Height));
            return new Rect(Math.Round(left), Math.Round(top), r.Width, r.Height);
        }

        public void SnapUygula(ref double left, ref double top, double w, double h, double alanGen, double alanYuk, IReadOnlyList<Rect> digerleri)
        {
            if (Math.Abs(left) <= SnapEsigi) left = 0;
            if (Math.Abs(top) <= SnapEsigi) top = 0;

            var sagBosluk = alanGen - (left + w);
            var altBosluk = alanYuk - (top + h);

            if (Math.Abs(sagBosluk) <= SnapEsigi) left = alanGen - w;
            if (Math.Abs(altBosluk) <= SnapEsigi) top = alanYuk - h;

            foreach (var o in digerleri)
            {
                if (Math.Abs(left - o.Right) <= SnapEsigi) left = o.Right;
                if (Math.Abs((left + w) - o.Left) <= SnapEsigi) left = o.Left - w;

                if (Math.Abs(top - o.Bottom) <= SnapEsigi) top = o.Bottom;
                if (Math.Abs((top + h) - o.Top) <= SnapEsigi) top = o.Top - h;
            }

            left = Math.Round(left);
            top = Math.Round(top);
        }


        public bool TrySurukle(
    Rect mevcut,
    Point hedefSolUst,
    double alanGen,
    double alanYuk,
    IReadOnlyList<Rect> digerleri,
    out Rect sonuc)
        {
            // hedef: mouse’a göre sol-üst
            double newLeft = hedefSolUst.X;
            double newTop = hedefSolUst.Y;

            // sınır clamp
            double canvasRight = alanGen - mevcut.Width;
            double canvasBottom = alanYuk - mevcut.Height;

            newLeft = Math.Max(0, Math.Min(newLeft, canvasRight));
            newTop = Math.Max(0, Math.Min(newTop, canvasBottom));

            // ilk snap
            SnapUygula(ref newLeft, ref newTop, mevcut.Width, mevcut.Height, alanGen, alanYuk, digerleri);

            var moving = new Rect(newLeft, newTop, mevcut.Width, mevcut.Height);

            // çarpışma çöz (senin mevcut mantığın: en yakın eksende it)
            foreach (var o in digerleri)
            {
                if (!Cakisiyor(moving, o))
                    continue;

                double deltaX = Math.Min(Math.Abs(newLeft + mevcut.Width - o.Left), Math.Abs(newLeft - o.Right));
                double deltaY = Math.Min(Math.Abs(newTop + mevcut.Height - o.Top), Math.Abs(newTop - o.Bottom));

                bool canMove = true;

                if (deltaX < deltaY)
                {
                    if (newLeft < o.Left)
                    {
                        if (o.Left - mevcut.Width >= 0) newLeft = o.Left - mevcut.Width;
                        else canMove = false;
                    }
                    else
                    {
                        if (o.Right <= canvasRight) newLeft = o.Right;
                        else canMove = false;
                    }
                }
                else
                {
                    if (newTop < o.Top)
                    {
                        if (o.Top - mevcut.Height >= 0) newTop = o.Top - mevcut.Height;
                        else canMove = false;
                    }
                    else
                    {
                        if (o.Bottom <= canvasBottom) newTop = o.Bottom;
                        else canMove = false;
                    }
                }

                if (!canMove)
                {
                    sonuc = mevcut; // hareket yok
                    return false;
                }

                var temp = new Rect(newLeft, newTop, mevcut.Width, mevcut.Height);

                // diğer tüm rect’lerle çakışmayı tekrar kontrol et
                foreach (var check in digerleri)
                {
                    if (ReferenceEquals(check, o)) continue;
                    if (Cakisiyor(temp, check))
                    {
                        sonuc = mevcut;
                        return false;
                    }
                }

                moving = temp;
            }

            // final clamp + snap + round
            newLeft = Math.Max(0, Math.Min(newLeft, canvasRight));
            newTop = Math.Max(0, Math.Min(newTop, canvasBottom));

            SnapUygula(ref newLeft, ref newTop, mevcut.Width, mevcut.Height, alanGen, alanYuk, digerleri);

            newLeft = Math.Round(newLeft);
            newTop = Math.Round(newTop);

            sonuc = new Rect(newLeft, newTop, mevcut.Width, mevcut.Height);
            return true;
        }

        public bool Cakisiyor(Rect a, Rect b)
        {
            var eps = CakismaEpsilon;
            return a.Left < b.Right - eps &&
                   a.Right > b.Left + eps &&
                   a.Top < b.Bottom - eps &&
                   a.Bottom > b.Top + eps;
        }
    }
}
