using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Palet_Programlama.Sınıflar
{
    public class PaletIslemler
    {
        private readonly string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Paletler.json");
        private List<Palet> PaletListesi = new List<Palet>();

        public void PaletKaydet(string PaletAdi, double PaletEn, double PaletBoy, double PaletYukseklik)
        {
            PaletListesi = DosyaOku();

            Palet Palet = new Palet
            {
                PaletAdi = PaletAdi,
                PaletEn = PaletEn,
                PaletBoy = PaletBoy,
                PaletYukseklik = PaletYukseklik,
            };

            PaletListesi.Add(Palet);
            DosyaYaz(PaletListesi);
        }
        public void PaletSil(string PaletAdi)
        {
            PaletListesi = DosyaOku();

            Palet Palet = PaletListesi.FirstOrDefault(u => u.PaletAdi == PaletAdi);
            if (Palet != null)
            {
                PaletListesi.Remove(Palet);
                DosyaYaz(PaletListesi);
            }
        }
        public List<Palet> PaletListesiniGetir()
        {
            return DosyaOku();
        }

        private List<Palet> DosyaOku()
        {
            if (!File.Exists(dosyaYolu)) return new List<Palet>();

            string json = File.ReadAllText(dosyaYolu);
            return string.IsNullOrEmpty(json) ? new List<Palet>() : JsonConvert.DeserializeObject<List<Palet>>(json);
        }

        private void DosyaYaz(List<Palet> PaletListesi)
        {
            string json = JsonConvert.SerializeObject(PaletListesi, Formatting.Indented);
            File.WriteAllText(dosyaYolu, json);
        }
    }
}
