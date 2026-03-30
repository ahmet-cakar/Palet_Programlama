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

        private double MerkezZHesapla(
            int katNo,
            Palet secilenPalet,
            Urun secilenUrun,
            double separatorYukseklik,
            List<int> secilenSeparatorKatlari)
        {
            double normalMerkezZ =
                secilenPalet.PaletYukseklik
                + ((katNo - 1) * secilenUrun.UrunYukseklik)
                + (secilenUrun.UrunYukseklik / 2.0);

            int altindakiSeparatorSayisi = secilenSeparatorKatlari
                .Count(x => x < katNo);

            return normalMerkezZ + (altindakiSeparatorSayisi * separatorYukseklik);
        }


        public DizilimKayitModel KayitModelDizilimiOlustur(
            string dizilimAdi,
            Palet secilenPalet,
            Urun secilenUrun,
            IReadOnlyDictionary<int, List<KatUrunu>> tumKatlar,
            double olcekX,
            double olcekY,
            bool separatorKullanilacak,
            double separatorYukseklik,
            List<int> secilenSeparatorKatlari)
        {
            if (!separatorKullanilacak)
            {
                separatorYukseklik = 0;
                secilenSeparatorKatlari = new List<int>();
            }

            secilenSeparatorKatlari ??= new List<int>();

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
                UrunYukseklik = secilenUrun.UrunYukseklik,
                SeparatorYukseklik = separatorYukseklik
            };

            foreach (var kat in tumKatlar
                .Where(x => x.Value != null && x.Value.Any())
                .OrderBy(x => x.Key))
            {
                int katNo = kat.Key;

                int altindakiSeparatorSayisi = secilenSeparatorKatlari.Count(x => x < katNo);
                bool separatorVarMi = secilenSeparatorKatlari.Contains(katNo - 1); 

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
                        MerkezZ = MerkezZHesapla(
                            katNo,
                            secilenPalet,
                            secilenUrun,
                            separatorYukseklik,
                            secilenSeparatorKatlari),
                        SeparatorVarMi = separatorVarMi
                    });
                }
            }

            return model;
        }
    }
}