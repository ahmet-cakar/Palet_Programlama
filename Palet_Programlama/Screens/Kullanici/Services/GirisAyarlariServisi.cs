using Newtonsoft.Json;
using Palet_Programlama.Models;
using Palet_Programlama.Screens.Helpers;
using System;
using System.IO;

namespace Palet_Programlama.Services
{
    public class GirisAyarlariServisi
    {
        private readonly string _dosyaYolu;

        public GirisAyarlariServisi()
        {
            _dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "GirisAyarlari.json");
        }

        public GirisAyarlariModel AyarlariYukle()
        {
            try
            {
                if (!File.Exists(_dosyaYolu))
                    return new GirisAyarlariModel();

                string json = File.ReadAllText(_dosyaYolu);

                if (string.IsNullOrWhiteSpace(json))
                    return new GirisAyarlariModel();

                return JsonConvert.DeserializeObject<GirisAyarlariModel>(json)
                       ?? new GirisAyarlariModel();
            }
            catch
            {
                return new GirisAyarlariModel();
            }
        }

        public void AyarlariKaydet(GirisAyarlariModel model)
        {
            string klasor = Path.GetDirectoryName(_dosyaYolu)!;
            if (!Directory.Exists(klasor))
                Directory.CreateDirectory(klasor);

            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            File.WriteAllText(_dosyaYolu, json);
        }

        public void BasariliGirisiKaydet(string kullaniciAdi, string sifre, bool beniHatirla)
        {
            var model = new GirisAyarlariModel
            {
                SonGirenKullaniciAdi = beniHatirla ? kullaniciAdi : "",
                SonGirenSifre = beniHatirla ? sifre : "",
                BeniHatirla = beniHatirla,
                SonBasariliGirisTarihi = DateTime.Now
            };

            AyarlariKaydet(model);
        }

        public void HatirlananGirisiTemizle()
        {
            var model = new GirisAyarlariModel
            {
                SonGirenKullaniciAdi = "",
                SonGirenSifre = "",
                BeniHatirla = false,
                SonBasariliGirisTarihi = null
            };

            AyarlariKaydet(model);
        }
    }
}