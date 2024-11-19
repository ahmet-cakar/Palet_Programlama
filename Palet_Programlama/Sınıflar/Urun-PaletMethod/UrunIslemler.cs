using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palet_Programlama.Sınıflar
{
    public class UrunIslemler
    {
        private readonly string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Urunler.json");
        private List<Urun> UrunListesi = new List<Urun>();

        public void UrunKaydet(string UrunAdi, double UrunEn, double UrunBoy, double UrunYukseklik, double UrunAgirlik, int UrunBasinc)
        {
            UrunListesi = DosyaOku();

            Urun urun = new Urun
            {
                UrunAdi = UrunAdi,
                UrunEn = UrunEn,
                UrunBoy = UrunBoy,
                UrunYukseklik = UrunYukseklik,
                UrunAgirlik = UrunAgirlik,
                UrunBasinc = UrunBasinc
            };

            UrunListesi.Add(urun);
            DosyaYaz(UrunListesi);
        }
        public void UrunSil(string UrunAdi)
        {
            UrunListesi = DosyaOku();

            Urun urun = UrunListesi.FirstOrDefault(u => u.UrunAdi == UrunAdi);
            if (urun != null)
            {
                UrunListesi.Remove(urun);
                DosyaYaz(UrunListesi);
            }
        }
        public List<Urun> UrunListesiniGetir()
        {
            return DosyaOku();
        }
        public void UrunListesiKaydet(List<Urun>UrunListesi)
        {
            DosyaYaz(UrunListesi);
        }

        private List<Urun> DosyaOku()
        {
            if (!File.Exists(dosyaYolu)) return new List<Urun>();

            string json = File.ReadAllText(dosyaYolu);
            return string.IsNullOrEmpty(json) ? new List<Urun>() : JsonConvert.DeserializeObject<List<Urun>>(json);
        }

        private void DosyaYaz(List<Urun> urunListesi)
        {
            string json = JsonConvert.SerializeObject(urunListesi, Formatting.Indented);
            File.WriteAllText(dosyaYolu, json);
        }
    }
}
