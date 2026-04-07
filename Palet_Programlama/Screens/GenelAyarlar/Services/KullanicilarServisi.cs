using Newtonsoft.Json;
using Palet_Programlama.Models;
using Palet_Programlama.Screens.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palet_Programlama.Services
{
    public class KullanicilarServisi
    {
        private readonly string _dosyaYolu;

        public KullanicilarServisi()
        {
            _dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Kullanicilar.json");
        }

        public List<KullaniciModel> TumKullanicilariGetir()
        {
            try
            {
                if (!File.Exists(_dosyaYolu))
                    return new List<KullaniciModel>();

                var json = File.ReadAllText(_dosyaYolu);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<KullaniciModel>();

                return JsonConvert.DeserializeObject<List<KullaniciModel>>(json) ?? new List<KullaniciModel>();
            }
            catch
            {
                return new List<KullaniciModel>();
            }
        }

        public KullaniciModel? IdyeGoreGetir(string id)
        {
            return TumKullanicilariGetir()
                .FirstOrDefault(x => x.Id == id);
        }

        public KullaniciModel? KullaniciAdinaGoreGetir(string kullaniciAdi)
        {
            return TumKullanicilariGetir()
                .FirstOrDefault(x => x.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase));
        }

        public bool KullaniciAdiVarMi(string kullaniciAdi, string? haricId = null)
        {
            return TumKullanicilariGetir().Any(x =>
                x.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase) &&
                x.Id != haricId);
        }

        public bool KullaniciEkle(KullaniciModel kullanici)
        {
            var kullanicilar = TumKullanicilariGetir();

            if (KullaniciAdiVarMi(kullanici.KullaniciAdi))
                return false;

            if (string.IsNullOrWhiteSpace(kullanici.Id))
                kullanici.Id = Guid.NewGuid().ToString();

            kullanicilar.Add(kullanici);
            Kaydet(kullanicilar);
            return true;
        }

        public bool KullaniciGuncelle(KullaniciModel guncelKullanici)
        {
            var kullanicilar = TumKullanicilariGetir();

            if (KullaniciAdiVarMi(guncelKullanici.KullaniciAdi, guncelKullanici.Id))
                return false;

            var eskiKayit = kullanicilar.FirstOrDefault(x => x.Id == guncelKullanici.Id);
            if (eskiKayit == null)
                return false;

            eskiKayit.KullaniciAdi = guncelKullanici.KullaniciAdi;
            eskiKayit.Sifre = guncelKullanici.Sifre;
            eskiKayit.Rol = guncelKullanici.Rol;
            eskiKayit.AktifMi = guncelKullanici.AktifMi;
            eskiKayit.YetkiliSayfalar = guncelKullanici.YetkiliSayfalar ?? new List<string>();

            Kaydet(kullanicilar);
            return true;
        }

        public bool KullaniciSil(string id)
        {
            var kullanicilar = TumKullanicilariGetir();
            var silinecek = kullanicilar.FirstOrDefault(x => x.Id == id);

            if (silinecek == null)
                return false;

            kullanicilar.Remove(silinecek);
            Kaydet(kullanicilar);
            return true;
        }

        public KullaniciModel? GirisDogrula(string kullaniciAdi, string sifre)
        {
            return TumKullanicilariGetir().FirstOrDefault(x =>
                x.AktifMi &&
                x.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase) &&
                x.Sifre == sifre);
        }

        public bool SayfayaErisimVarMi(KullaniciModel? kullanici, string sayfaAdi)
        {
            if (kullanici == null || !kullanici.AktifMi)
                return false;

            if (kullanici.Rol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            return kullanici.YetkiliSayfalar != null &&
                   kullanici.YetkiliSayfalar.Contains(sayfaAdi);
        }

        private void Kaydet(List<KullaniciModel> kullanicilar)
        {
            var klasor = Path.GetDirectoryName(_dosyaYolu);
            if (!string.IsNullOrWhiteSpace(klasor) && !Directory.Exists(klasor))
                Directory.CreateDirectory(klasor);

            var json = JsonConvert.SerializeObject(kullanicilar, Formatting.Indented);
            File.WriteAllText(_dosyaYolu, json);
        }
    }
}