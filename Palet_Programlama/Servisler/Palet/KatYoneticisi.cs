using Newtonsoft.Json;
using Palet_Programlama.Modeller;
using Palet_Programlama.Sınıflar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Palet_Programlama.Servisler.Palet
{
    public sealed class KatYoneticisi
    {
        private readonly Dictionary<int, List<KatUrunu>> _katlar = new();
        public int MaksKat { get; set; } = 10;

        public int AktifKat { get; private set; } = 1;

        public IReadOnlyDictionary<int, List<KatUrunu>> TumKatlar => _katlar;



        public bool TumKatlariTasi(double deltaLeftPx, double deltaTopPx, double canvasGenislik, double canvasYukseklik)
        {
            var doluKatlar = _katlar
                .Where(k => k.Value != null && k.Value.Any())
                .ToList();

            if (!doluKatlar.Any())
                return false;

            // Önce kontrol: herhangi bir üründe sınır ihlali olacak mı?
            foreach (var kat in doluKatlar)
            {
                foreach (var urun in kat.Value)
                {
                    double w = Genislik(urun);
                    double h = Yukseklik(urun);

                    double left = urun.MerkezX - w / 2.0;
                    double top = urun.MerkezY - h / 2.0;

                    double yeniLeft = left + deltaLeftPx;
                    double yeniTop = top + deltaTopPx;

                    if (yeniLeft < 0 ||
                        yeniTop < 0 ||
                        yeniLeft + w > canvasGenislik ||
                        yeniTop + h > canvasYukseklik)
                    {
                        return false;
                    }
                }
            }

            // Hepsi uygunsa taşı
            foreach (var kat in doluKatlar)
            {
                foreach (var urun in kat.Value)
                {
                    urun.MerkezX += deltaLeftPx;
                    urun.MerkezY += deltaTopPx;
                }
            }

            return true;
        }

        private static KatUrunu AynalanmisKopyaOlustur(
             KatUrunu kaynak,
             bool xEksenineGoreAynala,
             bool yEksenineGoreAynala,
             double canvasGenislik,
             double canvasYukseklik)
        {
            double yeniMerkezX = kaynak.MerkezX; // canvas yatay merkez
            double yeniMerkezY = kaynak.MerkezY; // canvas dikey merkez

            // Kullanıcının ekrandaki eksen beklentisine göre:
            // X ekseni seçiliyse sağ-sol simetri uygula
            if (xEksenineGoreAynala)
                yeniMerkezX = canvasGenislik - kaynak.MerkezX;

            // Y ekseni seçiliyse üst-alt simetri uygula
            if (yEksenineGoreAynala)
                yeniMerkezY = canvasYukseklik - kaynak.MerkezY;

            return new KatUrunu(
                yeniMerkezX,
                yeniMerkezY,
                kaynak.DikeyUzunluk,
                kaynak.YatayUzunluk,
                kaynak.Yon);
        }


        public void AktifKatiTemizle(Canvas canvas)
        {
            var silinecekler = canvas.Children.OfType<Rectangle>().ToList();
            foreach (var r in silinecekler)
                canvas.Children.Remove(r);

            _katlar[AktifKat] = new List<KatUrunu>();
        }

        public void TumKatlariTemizle(Canvas canvas)
        {
            var silinecekler = canvas.Children.OfType<Rectangle>().ToList();
            foreach (var r in silinecekler)
                canvas.Children.Remove(r);

            _katlar.Clear();
            AktifKat = 1;
        }

        public bool AraKatMiVeSilinemez()
        {
            // Aktif kattan sonra dolu bir kat varsa, bu kat ara kattır
            return _katlar.Any(k =>
                k.Key > AktifKat &&
                k.Value != null &&
                k.Value.Any());
        }


        public void Temizle()
        {
            _katlar.Clear();
            AktifKat = 1;
        }

        public bool DizilimYukle(
                string dizilimAdi,
                Urun secilenUrun,
                Sınıflar.Palet secilenPalet,
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

                var kayit = tumDizilimler.FirstOrDefault(x =>
                    string.Equals((x.DizilimAdi ?? "").Trim(), dizilimAdi.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.PaletAdi ?? "").Trim(), (secilenPalet.PaletAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((x.UrunAdi ?? "").Trim(), (secilenUrun.UrunAdi ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

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

        public void KatDegistir(
            int yeniKat,
            Canvas canvas,
            Rectangle? seciliKutu,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp)
        {
            if (yeniKat > MaksKat)
            {
                return;
            }

            if (yeniKat > AktifKat)
            {
                bool mevcutKatBosMu = !canvas.Children.OfType<Rectangle>().Any();
                if (mevcutKatBosMu)
                    return;
            }

            KatiKaydet(canvas);

            AktifKat = yeniKat < 1 ? 1 : yeniKat;

            if (seciliKutu is not null)
            {
                seciliKutu.Stroke = Brushes.Transparent;
                seciliKutu.StrokeThickness = 0;
            }

            KatiYukle(canvas, onDown, onMove, onUp);
        }


        public bool KatKopyala(
    int kaynakKat,
    int hedefKat,
    bool xEksenineGoreAynala,
    bool yEksenineGoreAynala,
    double canvasGenislik,
    double canvasYukseklik)
        {
            if (kaynakKat < 1 || hedefKat < 1)
                return false;

            if (kaynakKat == hedefKat)
                return false;

            // 1) Kaynak kat dolu olmalı
            if (!_katlar.TryGetValue(kaynakKat, out var kaynakListe) || kaynakListe == null || !kaynakListe.Any())
                return false;

            // 2) Hedef kat boş olmalı
            if (_katlar.TryGetValue(hedefKat, out var hedefListe) && hedefListe != null && hedefListe.Any())
                return false;

            // 3) Hedef katın bir önceki katı boş olmamalı
            if (hedefKat > 1)
            {
                int oncekiKat = hedefKat - 1;

                if (!_katlar.TryGetValue(oncekiKat, out var oncekiListe) || oncekiListe == null || !oncekiListe.Any())
                    return false;
            }

            _katlar[hedefKat] = kaynakListe
                .Select(u => AynalanmisKopyaOlustur(
                    u,
                    xEksenineGoreAynala,
                    yEksenineGoreAynala,
                    canvasGenislik,
                    canvasYukseklik))
                .ToList();

            return true;
        }


        public void KatiKaydetDisardan(Canvas canvas) => KatiKaydet(canvas);

        public void KatiYukleDisardan(
            Canvas canvas,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp) => KatiYukle(canvas, onDown, onMove, onUp);

        private void KatiKaydet(Canvas canvas)
        {
            var liste = canvas.Children.OfType<Rectangle>()
                .Select(r =>
                {
                    double left = Canvas.GetLeft(r); if (double.IsNaN(left)) left = 0;
                    double top = Canvas.GetTop(r); if (double.IsNaN(top)) top = 0;

                    double w = r.ActualWidth > 0 ? r.ActualWidth : r.Width;
                    double h = r.ActualHeight > 0 ? r.ActualHeight : r.Height;

                    double cx = left + w / 2.0;
                    double cy = top + h / 2.0;

                    var yon = r.Tag is UrunYonu t
                        ? t
                        : (w >= h ? UrunYonu.Yatay : UrunYonu.Dikey);

                    double dikeyUz = (yon == UrunYonu.Dikey) ? h : w;
                    double yatayUz = (yon == UrunYonu.Dikey) ? w : h;

                    return new KatUrunu(cx, cy, dikeyUz, yatayUz, yon);
                })
                .ToList();

            _katlar[AktifKat] = liste;
        }

        private void KatiYukle(
            Canvas canvas,
            MouseButtonEventHandler onDown,
            MouseEventHandler onMove,
            MouseButtonEventHandler onUp)
        {
            var silinecekler = canvas.Children.OfType<Rectangle>().ToList();
            foreach (var r in silinecekler)
                canvas.Children.Remove(r);

            if (!_katlar.TryGetValue(AktifKat, out var liste))
                return;

            foreach (var u in liste)
            {
                double w = Genislik(u);
                double h = Yukseklik(u);

                double left = u.MerkezX - w / 2.0;
                double top = u.MerkezY - h / 2.0;

                string resim = u.Yon == UrunYonu.Dikey
                    ? "pack://application:,,,/Resimler/DizilimYap/dikey_kutu.png"
                    : "pack://application:,,,/Resimler/DizilimYap/yatay_kutu.png";

                var rect = new Rectangle
                {
                    Width = w,
                    Height = h,
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                    Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(resim, UriKind.Absolute))
                    },
                    Tag = u.Yon
                };

                rect.MouseDown += onDown;
                rect.MouseMove += onMove;
                rect.MouseUp += onUp;

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                canvas.Children.Add(rect);
            }
        }

        private static double Genislik(KatUrunu u) =>
            u.Yon == UrunYonu.Dikey ? u.YatayUzunluk : u.DikeyUzunluk;

        private static double Yukseklik(KatUrunu u) =>
            u.Yon == UrunYonu.Dikey ? u.DikeyUzunluk : u.YatayUzunluk;
    }
}