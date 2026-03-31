using Newtonsoft.Json;
using Palet_Programlama.Screens.Helpers;
using System;
using System.IO;

namespace Palet_Programlama.Screens.Services
{
    public class SonSecimServisi
    {
        private readonly string _dosyaYolu;

        public SonSecimServisi()
        {
            _dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "SonSecimler.json");
        }

        public SonSecimModel Yukle()
        {
            try
            {
                if (!File.Exists(_dosyaYolu))
                    return null;

                string json = File.ReadAllText(_dosyaYolu);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonConvert.DeserializeObject<SonSecimModel>(json);
            }
            catch
            {
                return null;
            }
        }

        public void Kaydet(SonSecimModel model)
        {
            try
            {
                string klasor = Path.GetDirectoryName(_dosyaYolu);
                if (!string.IsNullOrWhiteSpace(klasor) && !Directory.Exists(klasor))
                    Directory.CreateDirectory(klasor);

                string json = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(_dosyaYolu, json);
            }
            catch
            {
            }
        }
    }
}