using System.Collections.Generic;

namespace Palet_Programlama.Modeller
{
    public class DizilimKayitModel
    {
        public string DizilimAdi { get; set; }
        public string PaletAdi { get; set; }
        public double PaletEn { get; set; }
        public double PaletBoy { get; set; }
        public double PaletYukseklik { get; set; }

        public string UrunAdi { get; set; }
        public double UrunEn { get; set; }
        public double UrunBoy { get; set; }
        public double UrunYukseklik { get; set; }

        public List<DizilimUrunKayitModel> Urunler { get; set; } = new();
    }
}