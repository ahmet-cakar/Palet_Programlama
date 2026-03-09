namespace Palet_Programlama.Modeller
{
    public enum UrunYonu
    {
        Dikey = 0,
        Yatay = 1
    }

    public sealed class KatUrunu
    {
        public double MerkezX { get; set; }
        public double MerkezY { get; set; }

        // Eksen bazlı ölçüler (Canvas px)
        public double DikeyUzunluk { get; set; }   // dikey eksen boyunca (height gibi)
        public double YatayUzunluk { get; set; }   // yatay eksen boyunca (width gibi)

        public UrunYonu Yon { get; set; } = UrunYonu.Dikey;

        public KatUrunu() { }

        public KatUrunu(double merkezX, double merkezY, double dikeyUzunluk, double yatayUzunluk, UrunYonu yon)
        {
            MerkezX = merkezX;
            MerkezY = merkezY;
            DikeyUzunluk = dikeyUzunluk;
            YatayUzunluk = yatayUzunluk;
            Yon = yon;
        }
    }
}