using Newtonsoft.Json;
using Palet_Programlama.Screens.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palet_Programlama.Services
{
    public class AyarlarServisi
    {
        private readonly string _dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Ayarlar.json");

        public string SeciliDiliGetir()
        {
            if (!File.Exists(_dosyaYolu))
                return LanguagesEnum.tr.ToString();

            var json = File.ReadAllText(_dosyaYolu);
            var ayarlar = JsonConvert.DeserializeObject<List<AyarModel>>(json);

            return ayarlar?.FirstOrDefault()?.SeciliDil ?? LanguagesEnum.tr.ToString();
        }

        public void SeciliDiliKaydet(string dil)
        {
            var ayarlar = new List<AyarModel>
            {
                new AyarModel { SeciliDil = dil }
            };

            string json = JsonConvert.SerializeObject(ayarlar, Formatting.Indented);
            File.WriteAllText(_dosyaYolu, json);
        }
    }
}