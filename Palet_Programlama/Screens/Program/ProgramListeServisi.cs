using Newtonsoft.Json;
using Palet_Programlama.Screens.Gruplama.Models;
using Palet_Programlama.Screens.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Palet_Programlama.Screens.Program
{
   

        public sealed class ProgramListeServisi
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

            public List<ProgramKayitModel> ProgramlariGetir()
            {
                return KayitlariYukle()
                    .Where(x => !string.IsNullOrWhiteSpace(x.ProgramAdi))
                    .OrderBy(x => x.ProgramAdi)
                    .ToList();
            }

            public bool ProgramSil(int programId)
            {
                string yol = DosyaYoluBul.DosyaGetir("Data", "Programlar.json");

                if (!File.Exists(yol))
                    return false;

                var kayitlar = KayitlariYukle();
                var silinecekKayit = kayitlar.FirstOrDefault(x => x.Id == programId);

                if (silinecekKayit == null)
                    return false;

                kayitlar.Remove(silinecekKayit);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(kayitlar, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(yol, json);

                return true;
            }
        }

}

