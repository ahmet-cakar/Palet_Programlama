using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar.Gruplama.Helpers
{
    public sealed class KoliGeometriYardimcisi
    {
        public Rect GetRect(Rectangle r)
        {
            double left = Canvas.GetLeft(r);
            double top = Canvas.GetTop(r);

            double w = (r.ActualWidth > 0) ? r.ActualWidth : r.Width;
            double h = (r.ActualHeight > 0) ? r.ActualHeight : r.Height;

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            return new Rect(left, top, w, h);
        }

        public Point KutuMerkeziniGetir(Rectangle kutu)
        {
            Rect rect = GetRect(kutu);
            return new Point(
                rect.Left + rect.Width / 2.0,
                rect.Top + rect.Height / 2.0);
        }

        public string IkiKutuArasiEkseniBul(Rectangle birinci, Rectangle ikinci)
        {
            Point p1 = KutuMerkeziniGetir(birinci);
            Point p2 = KutuMerkeziniGetir(ikinci);

            double dx = Math.Abs(p1.X - p2.X);
            double dy = Math.Abs(p1.Y - p2.Y);

            if (dx >= dy)
                return "X";

            return "Y";
        }

        public bool AyniSatirdalarMi(Rectangle birinci, Rectangle ikinci, double tolerans)
        {
            Point p1 = KutuMerkeziniGetir(birinci);
            Point p2 = KutuMerkeziniGetir(ikinci);

            return Math.Abs(p1.Y - p2.Y) <= tolerans;
        }

        public bool AyniSutundalarMi(Rectangle birinci, Rectangle ikinci, double tolerans)
        {
            Point p1 = KutuMerkeziniGetir(birinci);
            Point p2 = KutuMerkeziniGetir(ikinci);

            return Math.Abs(p1.X - p2.X) <= tolerans;
        }

        public bool XYonundeKomsuMu(Rectangle birinci, Rectangle ikinci, double tolerans)
        {
            Rect r1 = GetRect(birinci);
            Rect r2 = GetRect(ikinci);

            double sagSolMesafe = Math.Abs(r1.Right - r2.Left);
            double solSagMesafe = Math.Abs(r2.Right - r1.Left);

            return sagSolMesafe <= tolerans || solSagMesafe <= tolerans;
        }

        public Point GercekMerkeziGetir(Rectangle kutu, double olcekX, double olcekY)
        {
            Point merkez = KutuMerkeziniGetir(kutu);

            return new Point(
                merkez.Y / olcekX,
                merkez.X / olcekY);
        }

        public bool YYonundeKomsuMu(Rectangle birinci, Rectangle ikinci, double tolerans)
        {
            Rect r1 = GetRect(birinci);
            Rect r2 = GetRect(ikinci);

            double altUstMesafe = Math.Abs(r1.Bottom - r2.Top);
            double ustAltMesafe = Math.Abs(r2.Bottom - r1.Top);

            return altUstMesafe <= tolerans || ustAltMesafe <= tolerans;
        }
    }
}