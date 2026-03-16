using Newtonsoft.Json;
using Palet_Programlama.Modeller;
using Palet_Programlama.Sınıflar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palet_Programlama.Sayfalar.Gruplama.Services
{
    public sealed class DizilimKayitServisi
    {
        public List<DizilimKayitModel> KayitlariYukle()
        {
            try
            {
                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                if (!File.Exists(dosyaYolu))
                    return new List<DizilimKayitModel>();

                string json = File.ReadAllText(dosyaYolu);

                return JsonConvert.DeserializeObject<List<DizilimKayitModel>>(json)
                       ?? new List<DizilimKayitModel>();
            }
            catch
            {
                return new List<DizilimKayitModel>();
            }
        }

        public List<string> UrunAdlariniGetir(List<DizilimKayitModel> kayitlar)
        {
            return kayitlar
                .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi))
                .Select(x => x.UrunAdi)
                .Distinct()
                .ToList();
        }

        public List<string> UruneGoreDizilimAdlariniGetir(List<DizilimKayitModel> kayitlar, string urunAdi)
        {
            return kayitlar
                .Where(x => string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(x => !string.IsNullOrWhiteSpace(x.DizilimAdi))
                .Select(x => x.DizilimAdi)
                .Distinct()
                .ToList();
        }

        public DizilimKayitModel KayitBul(List<DizilimKayitModel> kayitlar, string urunAdi, string dizilimAdi)
        {
            return kayitlar.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals((x.DizilimAdi ?? "").Trim(), (dizilimAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public DizilimKayitModel GelenKaydiBul(List<DizilimKayitModel> kayitlar, string urunAdi, string dizilimAdi)
        {
            return KayitBul(kayitlar, urunAdi, dizilimAdi);
        }
    }
}