

namespace Palet_Programlama.Screens.Dizilim.Models
{

    public enum UrunYonu
    {
        Dikey = 0,
        Yatay = 1
    }

    public class DizilimUrunKayitModel
    {
        public string Yon { get; set; }
        public int KatNo { get; set; }
        public double MerkezX { get; set; }
        public double MerkezY { get; set; }
        public double MerkezZ { get; set; }
    }
}
