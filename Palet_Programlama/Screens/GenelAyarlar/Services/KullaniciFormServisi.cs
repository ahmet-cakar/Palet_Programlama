using System;
using System.Collections.Generic;
using Palet_Programlama.Models;

namespace Palet_Programlama.Services
{
    public class KullaniciFormServisi
    {
        private const int MinimumKullaniciAdiUzunlugu = 3;
        private const int MinimumSifreUzunlugu = 4;

        public bool BilgilerGecerliMi(string kullaniciAdi, string sifre)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi))
                return false;

            if (string.IsNullOrWhiteSpace(sifre))
                return false;

            if (kullaniciAdi.Trim().Length < MinimumKullaniciAdiUzunlugu)
                return false;

            if (sifre.Trim().Length < MinimumSifreUzunlugu)
                return false;

            return true;
        }

        public string? DogrulamaMesajKeyiGetir(string kullaniciAdi, string sifre)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
                return "Ayarlar.kullaniciSifreZorunlu";

            if (kullaniciAdi.Trim().Length < MinimumKullaniciAdiUzunlugu)
                return "Ayarlar.kullaniciAdiMin3Karakter";

            if (sifre.Trim().Length < MinimumSifreUzunlugu)
                return "Ayarlar.sifreMin4Karakter";

            return null;
        }

        public KullaniciModel YeniKullaniciOlustur(string kullaniciAdi, string sifre, string rol)
        {
            return new KullaniciModel
            {
                Id = Guid.NewGuid().ToString(),
                KullaniciAdi = kullaniciAdi.Trim(),
                Sifre = sifre.Trim(),
                Rol = rol,
                AktifMi = true,
                YetkiliSayfalar = new List<string>()
            };
        }

        public void KullaniciyiGuncelle(KullaniciModel kullanici, string kullaniciAdi, string sifre, string rol)
        {
            if (kullanici == null)
                return;

            kullanici.KullaniciAdi = kullaniciAdi.Trim();
            kullanici.Sifre = sifre.Trim();
            kullanici.Rol = rol;
        }
    }
}