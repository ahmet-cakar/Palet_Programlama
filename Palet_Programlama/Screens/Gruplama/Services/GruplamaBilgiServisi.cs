using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Gruplama.Models;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using System.Collections.Generic;
using System.Linq;

namespace Palet_Programlama.Screens.Gruplama.Services
{
    public sealed class GruplamaBilgiServisi
    {
        private readonly Dictionary<int, List<GrupAtamaBilgisi>> _katBazliGrupAtamalari = new();

        public void UrunBilgisiniUygula(Urun urun, DizilimKayitModel kayit)
        {
            if (urun == null || kayit == null)
                return;

            urun.UrunAdi = kayit.UrunAdi;
            urun.UrunEn = kayit.UrunEn;
            urun.UrunBoy = kayit.UrunBoy;
            urun.UrunYukseklik = kayit.UrunYukseklik;
        }
        public void KatAtamalariniKaydet(int katNo, List<GrupAtamaBilgisi> atamalar)
        {
            _katBazliGrupAtamalari[katNo] = atamalar
                .Select(x => new GrupAtamaBilgisi
                {
                    KatNo = x.KatNo,
                    GrupNo = x.GrupNo,
                    GrupEkseni = x.GrupEkseni,
                    KoliAnahtari = x.KoliAnahtari
                })
                .ToList();
        }

        public void KatTemizle(int katNo)
        {
            if (_katBazliGrupAtamalari.ContainsKey(katNo))
                _katBazliGrupAtamalari.Remove(katNo);
        }

        public void TumunuTemizle()
        {
            _katBazliGrupAtamalari.Clear();
        }

        public List<GrupAtamaBilgisi> KatAtamalariniGetir(int katNo)
        {
            if (_katBazliGrupAtamalari.TryGetValue(katNo, out var liste))
            {
                return liste
                    .Select(x => new GrupAtamaBilgisi
                    {
                        KatNo = x.KatNo,
                        GrupNo = x.GrupNo,
                        GrupEkseni = x.GrupEkseni,
                        KoliAnahtari = x.KoliAnahtari
                    })
                    .ToList();
            }

            return new List<GrupAtamaBilgisi>();
        }

        public void PaletBilgisiniUygula(Palet palet, DizilimKayitModel kayit)
        {
            if (palet == null || kayit == null)
                return;

            palet.PaletAdi = kayit.PaletAdi;
            palet.PaletEn = kayit.PaletEn;
            palet.PaletBoy = kayit.PaletBoy;
            palet.PaletYukseklik = kayit.PaletYukseklik;
        }

        public string PaletMetniUret(Palet palet)
        {
            if (palet == null)
                return string.Empty;

            return $"{palet.PaletAdi} - {palet.PaletEn:0} mm X {palet.PaletBoy:0} mm X {palet.PaletYukseklik:0} mm";
        }

        public string PaletMetniUret(DizilimKayitModel kayit)
        {
            if (kayit == null)
                return string.Empty;

            return $"{kayit.PaletAdi} - {kayit.PaletEn:0} mm X {kayit.PaletBoy:0} mm X {kayit.PaletYukseklik:0} mm";
        }
    }
}