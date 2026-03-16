using System;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar.Gruplama.Helpers
{
    public sealed class KoliYonYardimcisi
    {
        public string KutuYonunuGetir(Rectangle kutu)
        {
            if (kutu == null)
                return string.Empty;

            if (kutu.Tag != null)
            {
                string tagDegeri = kutu.Tag.ToString()?.Trim() ?? string.Empty;

                if (string.Equals(tagDegeri, "Yatay", StringComparison.OrdinalIgnoreCase))
                    return "Yatay";

                if (string.Equals(tagDegeri, "Dikey", StringComparison.OrdinalIgnoreCase))
                    return "Dikey";

                if (string.Equals(tagDegeri, "Horizontal", StringComparison.OrdinalIgnoreCase))
                    return "Yatay";

                if (string.Equals(tagDegeri, "Vertical", StringComparison.OrdinalIgnoreCase))
                    return "Dikey";
            }

            double genislik = kutu.ActualWidth > 0 ? kutu.ActualWidth : kutu.Width;
            double yukseklik = kutu.ActualHeight > 0 ? kutu.ActualHeight : kutu.Height;

            return genislik >= yukseklik ? "Yatay" : "Dikey";
        }
    }
}