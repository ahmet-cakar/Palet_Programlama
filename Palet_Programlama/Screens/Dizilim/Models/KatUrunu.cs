namespace Palet_Programlama.Screens.Dizilim.Models
{


    public sealed class KatUrunu
    {
        public double MerkezX { get; set; }
        public double MerkezY { get; set; }
        public double DikeyUzunluk { get; set; }   // dikey eksen boyunca (height gibi)
        public double YatayUzunluk { get; set; }   // yatay eksen boyunca (width gibi)

        public UrunYonu? Yon { get; set; } = UrunYonu.Dikey;

        public KatUrunu() { }

        public KatUrunu(double merkezX, double merkezY, double dikeyUzunluk, double yatayUzunluk, UrunYonu? yon)
        {
            MerkezX = merkezX;
            MerkezY = merkezY;
            DikeyUzunluk = dikeyUzunluk;
            YatayUzunluk = yatayUzunluk;
            Yon = yon;
        }
    }
}