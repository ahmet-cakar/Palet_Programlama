using Newtonsoft.Json;
using Palet_Programlama.Screens.Dizilim.Models;
using Palet_Programlama.Screens.Helpers;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palet_Programlama.Screens.Dizilim.Services
{
    public sealed class DizilimKayitServisi
    {
        private readonly Dictionary<int, List<KatUrunu>> _katlar = new();
        public int MaksKat { get; set; } = 10;
        public int AktifKat { get; private set; } = 1;

        public List<DizilimKayitModel> KayitlariYukle()
        {
            try
            {
                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                if (!File.Exists(dosyaYolu))
                    return new List<DizilimKayitModel>();

                string json = File.ReadAllText(dosyaYolu);

                return JsonConvert.DeserializeObject<List<DizilimKayitModel>>(json)
                       ?? new List<DizilimKayitModel>();
            }
            catch
            {
                return new List<DizilimKayitModel>();
            }
        }

        public List<string> UrunAdlariniGetir(List<DizilimKayitModel> kayitlar)
        {
            return kayitlar
                .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi))
                .Select(x => x.UrunAdi)
                .Distinct()
                .ToList();
        }

        public List<string> UruneGoreDizilimAdlariniGetir(List<DizilimKayitModel> kayitlar, string urunAdi)
        {
            return kayitlar
                .Where(x => string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(x => !string.IsNullOrWhiteSpace(x.DizilimAdi))
                .Select(x => x.DizilimAdi)
                .Distinct()
                .ToList();
        }

        public DizilimKayitModel KayitBul(List<DizilimKayitModel> kayitlar, string urunAdi, string dizilimAdi)
        {
            return kayitlar.FirstOrDefault(x =>
                string.Equals((x.UrunAdi ?? "").Trim(), (urunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals((x.DizilimAdi ?? "").Trim(), (dizilimAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase));
        }


        public bool DizilimYukle(
        string dizilimAdi,
        Urun secilenUrun,
        Palet secilenPalet,
        double olcekX,
        double olcekY)
        {
            if (string.IsNullOrWhiteSpace(dizilimAdi))
                return false;

            try
            {
                string dosyaYolu = DosyaYoluBul.DosyaGetir("Data", "Dizilimler.json");

                if (!File.Exists(dosyaYolu))
                    return false;

                string json = File.ReadAllText(dosyaYolu);

                var tumDizilimler = JsonConvert.DeserializeObject<List<DizilimKayitModel>>(json)
                                   ?? new List<DizilimKayitModel>();

                var kayit = tumDizilimler.FirstOrDefault(x => x.DizilimAdi == dizilimAdi);



                if (kayit == null)
                    return false;

                _katlar.Clear();

                var katGruplari = kayit.Urunler
                    .GroupBy(x => x.KatNo)
                    .OrderBy(x => x.Key);

                foreach (var katGrubu in katGruplari)
                {
                    var liste = new List<KatUrunu>();


                    foreach (var urunKayit in katGrubu)
                    {
                        UrunYonu yon = string.Equals(urunKayit.Yon, "Yatay", StringComparison.OrdinalIgnoreCase)
                            ? UrunYonu.Yatay
                            : UrunYonu.Dikey;

                        double gercekX = urunKayit.MerkezX;
                        double gercekY = urunKayit.MerkezY;

                        double canvasMerkezX = gercekY * olcekY;
                        double canvasMerkezY = gercekX * olcekX;

                        double dikeyUzunluk;
                        double yatayUzunluk;

                        if (yon == UrunYonu.Dikey)
                        {
                            dikeyUzunluk = secilenUrun.UrunBoy * olcekX;
                            yatayUzunluk = secilenUrun.UrunEn * olcekY;
                        }
                        else
                        {
                            dikeyUzunluk = secilenUrun.UrunBoy * olcekY;
                            yatayUzunluk = secilenUrun.UrunEn * olcekX;
                        }

                        liste.Add(new KatUrunu(
                            canvasMerkezX,
                            canvasMerkezY,
                            dikeyUzunluk,
                            yatayUzunluk,
                            yon));
                    }

                    _katlar[katGrubu.Key] = liste;
                }

                AktifKat = _katlar.Any() ? _katlar.Keys.Min() : 1;
                return true;
            }
            catch
            {
                return false;
            }
        }



        private double MerkezZHesapla(int katNo, Palet secilenPalet, Urun secilenUrun)
        {
            return secilenPalet.PaletYukseklik
                   + (katNo * secilenUrun.UrunYukseklik)
                   - (secilenUrun.UrunYukseklik / 2.0);
        }

        public DizilimKayitModel KayitModelDizilimiOlustur(
            string dizilimAdi,
            Palet secilenPalet,
            Urun secilenUrun,
            IReadOnlyDictionary<int, List<KatUrunu>> tumKatlar,
            double olcekX,
            double olcekY)
        {
            var model = new DizilimKayitModel
            {
                DizilimAdi = dizilimAdi,
                PaletAdi = secilenPalet.PaletAdi,
                PaletEn = secilenPalet.PaletEn,
                PaletBoy = secilenPalet.PaletBoy,
                PaletYukseklik = secilenPalet.PaletYukseklik,
                UrunAdi = secilenUrun.UrunAdi,
                UrunEn = secilenUrun.UrunEn,
                UrunBoy = secilenUrun.UrunBoy,
                UrunYukseklik = secilenUrun.UrunYukseklik
            };

            foreach (var kat in tumKatlar
                .Where(x => x.Value != null && x.Value.Any())
                .OrderBy(x => x.Key))
            {
                int katNo = kat.Key;

                foreach (var urun in kat.Value)
                {
                    double gercekMerkezX = urun.MerkezY / olcekX;
                    double gercekMerkezY = urun.MerkezX / olcekY;

                    model.Urunler.Add(new DizilimUrunKayitModel
                    {
                        Yon = urun.Yon.ToString(),
                        KatNo = katNo,
                        MerkezX = gercekMerkezX,
                        MerkezY = gercekMerkezY,
                        MerkezZ = MerkezZHesapla(katNo, secilenPalet, secilenUrun)
                    });
                }
            }

            return model;
        }
    }
}