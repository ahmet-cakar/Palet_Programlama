using Newtonsoft.Json;
using Palet_Programlama.Sayfalar.HizAyarlari.Models;
using Palet_Programlama.Sınıflar;
using System.IO;

namespace Palet_Programlama.Sayfalar.HizAyarlari.Services
{
    public sealed class HizServisi
    {
        private const string KlasorAdi = "Data";
        private const string DosyaAdi = "HizAyarlari.json";

        public HizVerileri HizVerileriniYukle()
        {
            string yol = DosyaYoluBul.DosyaGetir(KlasorAdi, DosyaAdi);

            if (!File.Exists(yol))
                return new HizVerileri();

            string json = File.ReadAllText(yol);

            if (string.IsNullOrWhiteSpace(json))
                return new HizVerileri();

            return JsonConvert.DeserializeObject<HizVerileri>(json) ?? new HizVerileri();
        }

        public void HizVerileriniKaydet(HizVerileri veri)
        {
            string yol = DosyaYoluBul.DosyaGetir(KlasorAdi, DosyaAdi);

            string? klasor = Path.GetDirectoryName(yol);
            if (!string.IsNullOrWhiteSpace(klasor) && !Directory.Exists(klasor))
            {
                Directory.CreateDirectory(klasor);
            }

            string json = JsonConvert.SerializeObject(veri, Formatting.Indented);
            File.WriteAllText(yol, json);
        }
    }
}