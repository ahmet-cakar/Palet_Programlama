
using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Gruplama.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace Palet_Programlama.Screens.Gruplama.Services
{
    public sealed class GrupKuralServisi
    {
        public bool GrupMaksimumKoliKuralinaUygunMu(
            IEnumerable<Rectangle> tumKutular,
            int aktifKatNo,
            int grupNo,
            int maksimumKoliSayisi,
            GruplamaSecimServisi secimServisi,
            Rectangle eklenecekKutu = null)
        {
            int koliSayisi = secimServisi.AktifKattaGruptakiKoliSayisiniGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                eklenecekKutu);

            return koliSayisi < maksimumKoliSayisi;
        }

        public bool GrupYonKuraliUygunMu(
            Rectangle eklenecekKutu,
            IEnumerable<Rectangle> tumKutular,
            int aktifKatNo,
            int grupNo,
            GruplamaSecimServisi secimServisi,
            KoliYonYardimcisi yonYardimcisi)
        {
            var ayniGruptakiKutular = secimServisi.AktifKattaAyniGruptakiKutulariGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                eklenecekKutu);

            if (!ayniGruptakiKutular.Any())
                return true;

            UrunYonu? yeniKutuYonu = yonYardimcisi.KutuYonunuGetir(eklenecekKutu);

            if (yeniKutuYonu == null)
                return false;

            foreach (var kutu in ayniGruptakiKutular)
            {
                UrunYonu? mevcutKutuYonu = yonYardimcisi.KutuYonunuGetir(kutu);

                if (mevcutKutuYonu == null)
                    return false;

                if (yeniKutuYonu != mevcutKutuYonu)
                    return false;
            }

            return true;
        }

        public bool GrupEksenKuraliUygunMu(
            Rectangle eklenecekKutu,
            IEnumerable<Rectangle> tumKutular,
            int aktifKatNo,
            int grupNo,
            double grupHizalamaToleransi,
            GruplamaSecimServisi secimServisi,
            KoliGeometriYardimcisi geometri)
        {
            var gruptakiKutular = secimServisi.AktifKattaAyniGruptakiKutulariGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                eklenecekKutu);

            if (!gruptakiKutular.Any())
                return true;

            if (gruptakiKutular.Count == 1)
                return true;

            string grupEkseni = secimServisi.AktifKattaGrubunEkseniniGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                geometri);

            if (string.IsNullOrWhiteSpace(grupEkseni))
                return true;

            if (string.Equals(grupEkseni, "X", StringComparison.OrdinalIgnoreCase))
            {
                return gruptakiKutular.All(kutu =>
                    geometri.AyniSatirdalarMi(kutu, eklenecekKutu, grupHizalamaToleransi));
            }

            if (string.Equals(grupEkseni, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return gruptakiKutular.All(kutu =>
                    geometri.AyniSutundalarMi(kutu, eklenecekKutu, grupHizalamaToleransi));
            }

            return true;
        }

        public bool GrupKomsulukKuralinaUygunMu(
            Rectangle eklenecekKutu,
            IEnumerable<Rectangle> tumKutular,
            int aktifKatNo,
            int grupNo,
            double grupHizalamaToleransi,
            double grupKomsulukToleransi,
            GruplamaSecimServisi secimServisi,
            KoliGeometriYardimcisi geometri)
        {
            var gruptakiKutular = secimServisi.AktifKattaAyniGruptakiKutulariGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                eklenecekKutu);

            if (!gruptakiKutular.Any())
                return true;

            if (gruptakiKutular.Count == 1)
            {
                Rectangle mevcutKutu = gruptakiKutular[0];
                string eksen = geometri.IkiKutuArasiEkseniBul(mevcutKutu, eklenecekKutu);

                if (string.Equals(eksen, "X", StringComparison.OrdinalIgnoreCase))
                {
                    return geometri.AyniSatirdalarMi(mevcutKutu, eklenecekKutu, grupHizalamaToleransi) &&
                           geometri.XYonundeKomsuMu(mevcutKutu, eklenecekKutu, grupKomsulukToleransi);
                }

                if (string.Equals(eksen, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    return geometri.AyniSutundalarMi(mevcutKutu, eklenecekKutu, grupHizalamaToleransi) &&
                           geometri.YYonundeKomsuMu(mevcutKutu, eklenecekKutu, grupKomsulukToleransi);
                }

                return false;
            }

            string grupEkseni = secimServisi.AktifKattaGrubunEkseniniGetir(
                tumKutular,
                aktifKatNo,
                grupNo,
                geometri);

            if (string.IsNullOrWhiteSpace(grupEkseni))
                return true;

            if (string.Equals(grupEkseni, "X", StringComparison.OrdinalIgnoreCase))
            {
                return gruptakiKutular.Any(kutu =>
                    geometri.AyniSatirdalarMi(kutu, eklenecekKutu, grupHizalamaToleransi) &&
                    geometri.XYonundeKomsuMu(kutu, eklenecekKutu, grupKomsulukToleransi));
            }

            if (string.Equals(grupEkseni, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return gruptakiKutular.Any(kutu =>
                    geometri.AyniSutundalarMi(kutu, eklenecekKutu, grupHizalamaToleransi) &&
                    geometri.YYonundeKomsuMu(kutu, eklenecekKutu, grupKomsulukToleransi));
            }

            return false;
        }
    }
}