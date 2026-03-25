using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Dizilim.Services;
using Palet_Programlama.Screens.Gruplama.Models;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Palet_Programlama.Screens.Gruplama.Services
{
    public sealed class GruplamaYuklemeServisi
    {
        private readonly DizilimKayitServisi _dizilimKayitServisi = new();
        private readonly ProgramKayitServisi _programKayitServisi = new();
        private readonly GruplamaBilgiServisi _bilgiServisi = new();

        public GruplamaYuklemeModel IlkAcilisVerisiniHazirla(
            Urun secilenUrun,
            Palet secilenPalet,
            string gelenDizilimAdi)
        {
            var model = new GruplamaYuklemeModel
            {
                DizilimKayitlari = _dizilimKayitServisi.KayitlariYukle()
            };

            model.UrunAdlari = _dizilimKayitServisi.UrunAdlariniGetir(model.DizilimKayitlari);

            model.SeciliUrunAdi = IlkSeciliUrunAdiniBul(model.UrunAdlari, secilenUrun?.UrunAdi);

            model.DizilimAdlari = _dizilimKayitServisi.UruneGoreDizilimAdlariniGetir(
                model.DizilimKayitlari,
                model.SeciliUrunAdi);

            model.SeciliDizilimAdi = IlkSeciliDizilimAdiniBul(model.DizilimAdlari, gelenDizilimAdi);

            model.SeciliKayit = _dizilimKayitServisi.KayitBul(
                model.DizilimKayitlari,
                model.SeciliUrunAdi,
                model.SeciliDizilimAdi);

            if (model.SeciliKayit != null)
            {
                _bilgiServisi.UrunBilgisiniUygula(secilenUrun, model.SeciliKayit);
                _bilgiServisi.PaletBilgisiniUygula(secilenPalet, model.SeciliKayit);
                model.PaletOzellikMetni = _bilgiServisi.PaletMetniUret(model.SeciliKayit);
            }
            else
            {
                model.PaletOzellikMetni = _bilgiServisi.PaletMetniUret(secilenPalet);
            }

            model.ProgramKayitlari = _programKayitServisi.KayitlariYukle();
            model.ProgramAdlari = ProgramAdlariniGetir(
                model.ProgramKayitlari,
                model.SeciliUrunAdi,
                model.SeciliDizilimAdi,
                secilenPalet?.PaletAdi);

            return model;
        }

        public List<string> UrunDegisimindeDizilimleriHazirla(
            List<DizilimKayitModel> dizilimKayitlari,
            string seciliUrunAdi,
            Urun secilenUrun)
        {
            var urunKaydi = dizilimKayitlari.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), (seciliUrunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

            if (urunKaydi != null && secilenUrun != null)
                _bilgiServisi.UrunBilgisiniUygula(secilenUrun, urunKaydi);

            return _dizilimKayitServisi.UruneGoreDizilimAdlariniGetir(dizilimKayitlari, seciliUrunAdi);
        }

        public DizilimKayitModel SeciliKaydiGetir(
            List<DizilimKayitModel> dizilimKayitlari,
            string urunAdi,
            string dizilimAdi)
        {
            return _dizilimKayitServisi.KayitBul(dizilimKayitlari, urunAdi, dizilimAdi);
        }

        public List<string> ProgramAdlariniGetir(
            List<ProgramKayitModel> programKayitlari,
            string urunAdi,
            string dizilimAdi,
            string paletAdi)
        {
            return programKayitlari
                .Where(x =>
                    string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.DizilimAdi ?? "").Trim(), (dizilimAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.PaletAdi ?? "").Trim(), (paletAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                .Select(x => x.ProgramAdi)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public void KayitBilgileriniUygula(Urun urun, Palet palet, DizilimKayitModel kayit)
        {
            if (kayit == null)
                return;

            _bilgiServisi.UrunBilgisiniUygula(urun, kayit);
            _bilgiServisi.PaletBilgisiniUygula(palet, kayit);
        }

        public string PaletMetniGetir(Palet palet, DizilimKayitModel kayit)
        {
            if (kayit != null)
                return _bilgiServisi.PaletMetniUret(kayit);

            return _bilgiServisi.PaletMetniUret(palet);
        }

        private string IlkSeciliUrunAdiniBul(List<string> urunAdlari, string varsayilanUrunAdi)
        {
            if (!string.IsNullOrWhiteSpace(varsayilanUrunAdi) && urunAdlari.Contains(varsayilanUrunAdi))
                return varsayilanUrunAdi;

            return urunAdlari.FirstOrDefault() ?? "";
        }

        private string IlkSeciliDizilimAdiniBul(List<string> dizilimAdlari, string varsayilanDizilimAdi)
        {
            if (!string.IsNullOrWhiteSpace(varsayilanDizilimAdi) && dizilimAdlari.Contains(varsayilanDizilimAdi))
                return varsayilanDizilimAdi;

            return dizilimAdlari.FirstOrDefault() ?? "";
        }
    }
}