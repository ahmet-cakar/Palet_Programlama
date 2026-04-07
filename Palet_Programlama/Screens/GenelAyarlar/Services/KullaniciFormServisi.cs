using System;
using System.Collections.Generic;
using Palet_Programlama.Models;

namespace Palet_Programlama.Services
{
    public class KullaniciFormServisi
    {
        public bool BilgilerGecerliMi(string kullaniciAdi, string sifre)
        {
            return !string.IsNullOrWhiteSpace(kullaniciAdi)
                && !string.IsNullOrWhiteSpace(sifre);
        }

        public KullaniciModel YeniKullaniciOlustur(string kullaniciAdi, string sifre)
        {
            return new KullaniciModel
            {
                Id = Guid.NewGuid().ToString(),
                KullaniciAdi = kullaniciAdi.Trim(),
                Sifre = sifre.Trim(),
                Rol = "Operator",
                AktifMi = true,
                YetkiliSayfalar = new List<string>()
            };
        }

        public void KullaniciyiGuncelle(KullaniciModel kullanici, string kullaniciAdi, string sifre)
        {
            if (kullanici == null)
                return;

            kullanici.KullaniciAdi = kullaniciAdi.Trim();
            kullanici.Sifre = sifre.Trim();
        }
    }
}