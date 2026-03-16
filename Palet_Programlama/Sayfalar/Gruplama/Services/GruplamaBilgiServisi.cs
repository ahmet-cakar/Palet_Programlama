using Palet_Programlama.Modeller;
using Palet_Programlama.Sınıflar;

namespace Palet_Programlama.Sayfalar.Gruplama.Services
{
    public sealed class GruplamaBilgiServisi
    {
        public void UrunBilgisiniUygula(Urun urun, DizilimKayitModel kayit)
        {
            if (urun == null || kayit == null)
                return;

            urun.UrunAdi = kayit.UrunAdi;
            urun.UrunEn = kayit.UrunEn;
            urun.UrunBoy = kayit.UrunBoy;
            urun.UrunYukseklik = kayit.UrunYukseklik;
        }

        public void PaletBilgisiniUygula(Palet palet, DizilimKayitModel kayit)
        {
            if (palet == null || kayit == null)
                return;

            palet.PaletAdi = kayit.PaletAdi;
            palet.PaletEn = kayit.PaletEn;
            palet.PaletBoy = kayit.PaletBoy;
            palet.PaletYukseklik = kayit.PaletYukseklik;
        }

        public string PaletMetniUret(Palet palet)
        {
            if (palet == null)
                return string.Empty;

            return $"{palet.PaletAdi} - {palet.PaletEn:0} mm X {palet.PaletBoy:0} mm X {palet.PaletYukseklik:0} mm";
        }

        public string PaletMetniUret(DizilimKayitModel kayit)
        {
            if (kayit == null)
                return string.Empty;

            return $"{kayit.PaletAdi} - {kayit.PaletEn:0} mm X {kayit.PaletBoy:0} mm X {kayit.PaletYukseklik:0} mm";
        }
    }
}