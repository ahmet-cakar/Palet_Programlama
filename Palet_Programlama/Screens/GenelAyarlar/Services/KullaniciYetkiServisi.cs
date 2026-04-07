using Palet_Programlama.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Palet_Programlama.Services
{
    public class KullaniciYetkiServisi
    {

        public List<string> YetkileriGetir(KullaniciModel? kullanici)
        {
            if (kullanici == null || kullanici.YetkiliSayfalar == null)
                return new List<string>();

            return kullanici.YetkiliSayfalar.ToList();
        }

        public void YetkileriAta(KullaniciModel kullanici, List<string> yetkiler)
        {
            if (kullanici == null)
                return;

            kullanici.YetkiliSayfalar = yetkiler?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList()
                ?? new List<string>();
        }
        public bool SayfaYetkisiVarMi(KullaniciModel? kullanici, string sayfaAdi)
        {
            if (kullanici == null || !kullanici.AktifMi || string.IsNullOrWhiteSpace(sayfaAdi))
                return false;

            if (kullanici.Rol == "Admin")
                return true;

            return kullanici.YetkiliSayfalar != null &&
                   kullanici.YetkiliSayfalar.Contains(sayfaAdi);
        }

        public void YetkileriCheckboxlaraYansit(
            KullaniciModel? kullanici,
            CheckBox chkUrunEkle,
            CheckBox chkDizilimYap,
            CheckBox chkGruplamaYap,
            CheckBox chkProgramlar,
            CheckBox chkHizAyarlari,
            CheckBox chkAlarmlar,
            CheckBox chkIzleme,
            CheckBox chkAyarlar)
        {
            chkUrunEkle.IsChecked = false;
            chkDizilimYap.IsChecked = false;
            chkGruplamaYap.IsChecked = false;
            chkProgramlar.IsChecked = false;
            chkHizAyarlari.IsChecked = false;
            chkAlarmlar.IsChecked = false;
            chkIzleme.IsChecked = false;
            chkAyarlar.IsChecked = false;

            if (kullanici?.YetkiliSayfalar == null)
                return;

            chkUrunEkle.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.UrunEkle);
            chkDizilimYap.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.DizilimYap);
            chkGruplamaYap.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.GruplamaYap);
            chkProgramlar.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.Programlar);
            chkHizAyarlari.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.HizAyarlari);
            chkAlarmlar.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.Alarmlar);
            chkIzleme.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.Izleme);
            chkAyarlar.IsChecked = kullanici.YetkiliSayfalar.Contains(SayfaAdlari.Ayarlar);
        }


        public List<string> SeciliYetkileriTopla(
            CheckBox chkUrunEkle,
            CheckBox chkDizilimYap,
            CheckBox chkGruplamaYap,
            CheckBox chkProgramlar,
            CheckBox chkHizAyarlari,
            CheckBox chkAlarmlar,
            CheckBox chkIzleme,
            CheckBox chkAyarlar)
        {
            var yetkiler = new List<string>();

            if (chkUrunEkle.IsChecked == true) yetkiler.Add(SayfaAdlari.UrunEkle);
            if (chkDizilimYap.IsChecked == true) yetkiler.Add(SayfaAdlari.DizilimYap);
            if (chkGruplamaYap.IsChecked == true) yetkiler.Add(SayfaAdlari.GruplamaYap);
            if (chkProgramlar.IsChecked == true) yetkiler.Add(SayfaAdlari.Programlar);
            if (chkHizAyarlari.IsChecked == true) yetkiler.Add(SayfaAdlari.HizAyarlari);
            if (chkAlarmlar.IsChecked == true) yetkiler.Add(SayfaAdlari.Alarmlar);
            if (chkIzleme.IsChecked == true) yetkiler.Add(SayfaAdlari.Izleme);
            if (chkAyarlar.IsChecked == true) yetkiler.Add(SayfaAdlari.Ayarlar);

            return yetkiler;
        }
    }
}