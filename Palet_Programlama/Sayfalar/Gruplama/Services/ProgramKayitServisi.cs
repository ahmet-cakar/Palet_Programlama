using Newtonsoft.Json;
using Palet_Programlama.Sayfalar.Gruplama.Models;
using Palet_Programlama.Sınıflar;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palet_Programlama.Sayfalar.Gruplama.Services
{
    public sealed class ProgramKayitServisi
    {
        private const string KlasorAdi = "Data";
        private const string DosyaAdi = "Programlar.json";

        public List<ProgramKayitModel> KayitlariYukle()
        {
            string yol = DosyaYoluBul.DosyaGetir(KlasorAdi, DosyaAdi);

            if (!File.Exists(yol))
                return new List<ProgramKayitModel>();

            string json = File.ReadAllText(yol);
            if (string.IsNullOrWhiteSpace(json))
                return new List<ProgramKayitModel>();

            return JsonConvert.DeserializeObject<List<ProgramKayitModel>>(json)
                   ?? new List<ProgramKayitModel>();
        }

        public bool ProgramAdiVarMi(string programAdi)
        {
            return KayitlariYukle().Any(x =>
                string.Equals((x.ProgramAdi ?? "").Trim(), programAdi.Trim(), System.StringComparison.OrdinalIgnoreCase));
        }

        public void Guncelle(ProgramKayitModel guncelProgram)
        {
            var kayitlar = KayitlariYukle();

            int index = kayitlar.FindIndex(x => x.Id == guncelProgram.Id);
            if (index < 0)
                return;

            kayitlar[index] = guncelProgram;

            string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Programlar.json");
            string json = JsonConvert.SerializeObject(kayitlar, Formatting.Indented);
            File.WriteAllText(dosyaYolu, json);
        }

        public int SonrakiIdGetir()
        {
            var kayitlar = KayitlariYukle();
            return kayitlar.Any() ? kayitlar.Max(x => x.Id) + 1 : 1;
        }

        public void Kaydet(ProgramKayitModel yeniKayit)
        {
            var kayitlar = KayitlariYukle();
            kayitlar.Add(yeniKayit);

            string yol = DosyaYoluBul.DosyaGetir(KlasorAdi, DosyaAdi);
            string json = JsonConvert.SerializeObject(kayitlar, Formatting.Indented);
            File.WriteAllText(yol, json);
        }
    }
}