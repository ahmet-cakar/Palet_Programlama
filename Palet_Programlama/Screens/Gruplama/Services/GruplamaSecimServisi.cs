using Palet_Programlama.Sayfalar.Gruplama.Helpers;
using Palet_Programlama.Sayfalar.Gruplama.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar.Gruplama.Services
{
    public sealed class GruplamaSecimServisi
    {
        private readonly Dictionary<Rectangle, GrupAtamaBilgisi> _grupAtamalari = new();

        public Dictionary<Rectangle, GrupAtamaBilgisi> GrupAtamalari => _grupAtamalari;



        public string KoliAnahtariUret(Rectangle kutu)
        {
            double left = Canvas.GetLeft(kutu);
            double top = Canvas.GetTop(kutu);
            double width = kutu.Width;
            double height = kutu.Height;

            string yon = width >= height ? "Yatay" : "Dikey";

            return string.Join("_",
                left.ToString("0.###", CultureInfo.InvariantCulture),
                top.ToString("0.###", CultureInfo.InvariantCulture),
                width.ToString("0.###", CultureInfo.InvariantCulture),
                height.ToString("0.###", CultureInfo.InvariantCulture),
                yon);
        }

        public bool KoliBuAktifGruptaMi(Rectangle kutu, int aktifKatNo, int aktifGrupNo)
        {
            return _grupAtamalari.TryGetValue(kutu, out var bilgi)
                   && bilgi.KatNo == aktifKatNo
                   && bilgi.GrupNo == aktifGrupNo;
        }

        public void KatAtamalariniYukle(
                int katNo,
                IEnumerable<Rectangle> kutular,
                List<GrupAtamaBilgisi> kayitlar,
                KoliGeometriYardimcisi geometri)
        {
            var digerKatlariKoru = GrupAtamalari
                .Where(x => x.Value.KatNo != katNo)
                .ToDictionary(x => x.Key, x => x.Value);

            GrupAtamalari.Clear();

            foreach (var kayit in digerKatlariKoru)
                GrupAtamalari[kayit.Key] = kayit.Value;

            foreach (var kutu in kutular)
            {
                string koliAnahtari = KoliAnahtariUret(kutu);

                var eslesenKayit = kayitlar.FirstOrDefault(x =>
                    x.KatNo == katNo &&
                    string.Equals(x.KoliAnahtari, koliAnahtari, StringComparison.OrdinalIgnoreCase));

                if (eslesenKayit == null)
                    continue;

                GrupAtamalari[kutu] = new GrupAtamaBilgisi
                {
                    KatNo = eslesenKayit.KatNo,
                    GrupNo = eslesenKayit.GrupNo,
                    GrupEkseni = eslesenKayit.GrupEkseni,
                    KoliAnahtari = eslesenKayit.KoliAnahtari
                };
            }
        }

        public List<Rectangle> AktifKattaAyniGruptakiKutulariGetir(
            IEnumerable<Rectangle> kutular,
            int aktifKatNo,
            int grupNo,
            Rectangle haricKutu = null)
        {
            return kutular
                .Where(kutu => kutu != haricKutu)
                .Where(kutu =>
                    _grupAtamalari.TryGetValue(kutu, out var bilgi) &&
                    bilgi.KatNo == aktifKatNo &&
                    bilgi.GrupNo == grupNo)
                .ToList();
        }

        public int AktifKattaGruptakiKoliSayisiniGetir(
            IEnumerable<Rectangle> kutular,
            int aktifKatNo,
            int grupNo,
            Rectangle haricKutu = null)
        {
            return kutular
                .Where(kutu => kutu != haricKutu)
                .Count(kutu =>
                    _grupAtamalari.TryGetValue(kutu, out var bilgi) &&
                    bilgi.KatNo == aktifKatNo &&
                    bilgi.GrupNo == grupNo);
        }

        public string AktifKattaGrubunEkseniniGetir(
            IEnumerable<Rectangle> kutular,
            int aktifKatNo,
            int grupNo,
            KoliGeometriYardimcisi geometri)
        {
            var gruptakiKutular = AktifKattaAyniGruptakiKutulariGetir(kutular, aktifKatNo, grupNo);

            if (gruptakiKutular.Count < 2)
                return string.Empty;

            return geometri.IkiKutuArasiEkseniBul(gruptakiKutular[0], gruptakiKutular[1]);
        }

        public void KoliyaGrupAta(
            Rectangle kutu,
            int aktifKatNo,
            int grupNo,
            IEnumerable<Rectangle> tumKutular,
            KoliGeometriYardimcisi geometri)
        {
            string grupEkseni = string.Empty;

            var gruptakiKutular = AktifKattaAyniGruptakiKutulariGetir(tumKutular, aktifKatNo, grupNo, kutu);

            if (gruptakiKutular.Count == 1)
            {
                grupEkseni = geometri.IkiKutuArasiEkseniBul(gruptakiKutular[0], kutu);
            }
            else if (gruptakiKutular.Count >= 2)
            {
                grupEkseni = AktifKattaGrubunEkseniniGetir(tumKutular, aktifKatNo, grupNo, geometri);
            }

            _grupAtamalari[kutu] = new GrupAtamaBilgisi
            {
                KatNo = aktifKatNo,
                GrupNo = grupNo,
                GrupEkseni = grupEkseni,
                KoliAnahtari = KoliAnahtariUret(kutu)
            };
        }

        public void KolininGrubunuKaldir(Rectangle kutu)
        {
            if (_grupAtamalari.ContainsKey(kutu))
                _grupAtamalari.Remove(kutu);
        }

        public void GruptakiTumKutularinEksenBilgisiniGuncelle(
            IEnumerable<Rectangle> tumKutular,
            int aktifKatNo,
            int grupNo,
            KoliGeometriYardimcisi geometri)
        {
            string grupEkseni = AktifKattaGrubunEkseniniGetir(tumKutular, aktifKatNo, grupNo, geometri);

            foreach (var kutu in AktifKattaAyniGruptakiKutulariGetir(tumKutular, aktifKatNo, grupNo))
            {
                if (_grupAtamalari.TryGetValue(kutu, out var bilgi))
                {
                    bilgi.GrupEkseni = grupEkseni;
                }
            }
        }

        public void TumGrupAtamalariniTemizle()
        {
            _grupAtamalari.Clear();
        }
    }
}