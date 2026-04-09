using Palet_Programlama.Models;
using Palet_Programlama.Services;
using Palet_Programlama.UserController;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Palet_Programlama.Screens
{
    public partial class Ayarlar : Page
    {
        private readonly Frame _mainFrame;
        private readonly KullanicilarServisi _kullanicilarServisi;
        private readonly KullaniciYetkiServisi _kullaniciYetkiServisi;
        private readonly KullaniciFormServisi _kullaniciFormServisi;

        private List<KullaniciModel> _kullanicilar = new();
        private KullaniciModel? _seciliKullanici;
        private KullaniciModel? _seciliYetkiKullanici;
        private bool _sifreGorunurMu = false;

        public Ayarlar(Frame main)
        {
            InitializeComponent();
            _mainFrame = main;
            UstMenuControl.AktifSayfa = "Ayarlar";

            _kullanicilarServisi = new KullanicilarServisi();
            _kullaniciYetkiServisi = new KullaniciYetkiServisi();
            _kullaniciFormServisi = new KullaniciFormServisi();

            KullanicilariYukle();
            lstAyarMenusu.SelectedIndex = 0;
            cmbRolSecimi.SelectedIndex = 0;
        }

        private void KullanicilariYukle()
        {
            _kullanicilar = _kullanicilarServisi.TumKullanicilariGetir();
            lstKullaniciListesi.ItemsSource = null;
            lstKullaniciListesi.ItemsSource = _kullanicilar;
            txtKayiSayisi.Text = $"{_kullanicilar.Count} Kayıt";
            YetkiKullanicilariniYukle();
        }

        private void YetkiKullanicilariniYukle()
        {
            lstYetkiKullaniciListesi.ItemsSource = null;
            lstYetkiKullaniciListesi.ItemsSource = _kullanicilar;
        }

        private void lstYetkiKullaniciListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstYetkiKullaniciListesi.SelectedItem is not KullaniciModel secili)
                return;

            _seciliYetkiKullanici = secili;
            txtSeciliYetkiKullanici.Text = $"Seçili Kullanıcı: {secili.KullaniciAdi}";

            _kullaniciYetkiServisi.YetkileriCheckboxlaraYansit(
                secili,
                chkUrunEkle,
                chkDizilimYap,
                chkGruplamaYap,
                chkProgramlar,
                chkHizAyarlari,
                chkAlarmlar,
                chkIzleme,
                chkAyarlar);
        }

        private void lstKullaniciListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstKullaniciListesi.SelectedItem is not KullaniciModel secili)
                return;

            _seciliKullanici = secili;

            txtKullaniciAdi.Text = secili.KullaniciAdi;
            txtKullaniciSifre.Password = secili.Sifre;
            txtKullaniciSifreAcik.Text = secili.Sifre;

            if (secili.Rol == "Admin")
                cmbRolSecimi.SelectedIndex = 2;
            else if (secili.Rol == "Operator")
                cmbRolSecimi.SelectedIndex = 1;
            else
                cmbRolSecimi.SelectedIndex = 0;
        }

        private void AlanlariTemizle()
        {
            txtKullaniciAdi.Text = string.Empty;
            txtKullaniciSifre.Password = string.Empty;
            txtKullaniciSifreAcik.Text = string.Empty;

            txtKullaniciSifre.Visibility = Visibility.Visible;
            txtKullaniciSifreAcik.Visibility = Visibility.Collapsed;
            btnSifreGoster.Content = "👁";
            _sifreGorunurMu = false;

            lstKullaniciListesi.SelectedItem = null;
            _seciliKullanici = null;
            cmbRolSecimi.SelectedIndex = 0;
        }

        private string AktifSifreyiAl()
        {
            return txtKullaniciSifre.Visibility == Visibility.Visible
                ? txtKullaniciSifre.Password
                : txtKullaniciSifreAcik.Text;
        }

        private string SeciliRoluAl()
        {
            if (cmbRolSecimi.SelectedItem is ComboBoxItem seciliItem &&
                seciliItem.Content != null)
            {
                return seciliItem.Content.ToString() ?? "Seçim Yapılmadı";
            }

            return "Seçim Yapılmadı";
        }

        private void btnKullaniciEkle_Click(object sender, RoutedEventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = AktifSifreyiAl().Trim();
            string rol = SeciliRoluAl();

            string? mesajKey = _kullaniciFormServisi.DogrulamaMesajKeyiGetir(kullaniciAdi, sifre);
            if (mesajKey != null)
            {
                BildirimGoster(mesajKey);
                return;
            }

            if (rol == "Seçim Yapılmadı")
            {
                BildirimGoster("Ayarlar.rolSeciniz");
                return;
            }

            if (_kullanicilarServisi.KullaniciAdiVarMi(kullaniciAdi))
            {
                BildirimGoster("Ayarlar.kullaniciAdiMevcut");
                return;
            }

            var yeniKullanici = _kullaniciFormServisi.YeniKullaniciOlustur(kullaniciAdi, sifre, rol);

            bool eklendi = _kullanicilarServisi.KullaniciEkle(yeniKullanici);

            if (!eklendi)
            {
                BildirimGoster("Ayarlar.kullaniciEklemeBasarisiz");
                return;
            }

            BildirimGoster("Ayarlar.kullaniciEklemeBasarili");
            KullanicilariYukle();
            AlanlariTemizle();
        }

        private void btnKullaniciGuncelle_Click(object sender, RoutedEventArgs e)
        {
            if (_seciliKullanici == null)
            {
                BildirimGoster("Ayarlar.kullaniciSeciniz");
                return;
            }

            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = AktifSifreyiAl().Trim();
            string rol = SeciliRoluAl();

            if (!_kullaniciFormServisi.BilgilerGecerliMi(kullaniciAdi, sifre))
            {
                BildirimGoster("Ayarlar.kullaniciSifreZorunlu");
                return;
            }

            if (rol == "Seçim Yapılmadı")
            {
                BildirimGoster("Ayarlar.rolSeciniz");
                return;
            }

            if (_kullanicilarServisi.KullaniciAdiVarMi(kullaniciAdi, _seciliKullanici.Id))
            {
                BildirimGoster("Ayarlar.kullaniciAdiMevcut");
                return;
            }

            _kullaniciFormServisi.KullaniciyiGuncelle(_seciliKullanici, kullaniciAdi, sifre, rol);

            bool guncellendi = _kullanicilarServisi.KullaniciGuncelle(_seciliKullanici);

            if (!guncellendi)
            {
                BildirimGoster("Ayarlar.kullaniciGuncellemeBasarisiz");
                return;
            }

            BildirimGoster("Ayarlar.kullaniciGuncellemeBasarili");
            KullanicilariYukle();
            AlanlariTemizle();
        }

        private void btnKullaniciSil_Click(object sender, RoutedEventArgs e)
        {
            if (_seciliKullanici == null)
            {
                BildirimGoster("Ayarlar.kullaniciSeciniz");
                return;
            }

            bool silindi = _kullanicilarServisi.KullaniciSil(_seciliKullanici.Id);

            if (!silindi)
            {
                BildirimGoster("Ayarlar.kullaniciSilmeBasarisiz");
                return;
            }

            BildirimGoster("Ayarlar.kullaniciSilmeBasarili");
            KullanicilariYukle();
            AlanlariTemizle();
        }

        private void btnSifreGoster_Click(object sender, RoutedEventArgs e)
        {
            if (_sifreGorunurMu)
            {
                txtKullaniciSifre.Password = txtKullaniciSifreAcik.Text;
                txtKullaniciSifreAcik.Visibility = Visibility.Collapsed;
                txtKullaniciSifre.Visibility = Visibility.Visible;
                btnSifreGoster.Content = "👁";
                _sifreGorunurMu = false;
            }
            else
            {
                txtKullaniciSifreAcik.Text = txtKullaniciSifre.Password;
                txtKullaniciSifre.Visibility = Visibility.Collapsed;
                txtKullaniciSifreAcik.Visibility = Visibility.Visible;
                btnSifreGoster.Content = "🙈";
                _sifreGorunurMu = true;
            }
        }

        private void BildirimGoster(string mesajKey, string butonKey = "ButtonKey.btntamam")
        {
            var pencere = new BildirimKutusu();
            pencere.MesajGonder(butonKey, mesajKey);
            pencere.ShowDialog();
        }

        private void lstAyarMenusu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelKullaniciYonetimi.Visibility = Visibility.Collapsed;
            panelYetkiAyari.Visibility = Visibility.Collapsed;
            panelVarsayilanSistemAyarlari.Visibility = Visibility.Collapsed;
            panelYedekleme.Visibility = Visibility.Collapsed;

            switch (lstAyarMenusu.SelectedIndex)
            {
                case 0:
                    panelKullaniciYonetimi.Visibility = Visibility.Visible;
                    break;
                case 1:
                    panelYetkiAyari.Visibility = Visibility.Visible;
                    break;
                case 2:
                    panelVarsayilanSistemAyarlari.Visibility = Visibility.Visible;
                    break;
                case 3:
                    panelYedekleme.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnTumAyarlariKaydet_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnYetkileriKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (_seciliYetkiKullanici == null)
            {
                BildirimGoster("Ayarlar.kullaniciSeciniz");
                return;
            }

            var yetkiler = _kullaniciYetkiServisi.SeciliYetkileriTopla(
                chkUrunEkle,
                chkDizilimYap,
                chkGruplamaYap,
                chkProgramlar,
                chkHizAyarlari,
                chkAlarmlar,
                chkIzleme,
                chkAyarlar);

            _kullaniciYetkiServisi.YetkileriAta(_seciliYetkiKullanici, yetkiler);

            bool guncellendi = _kullanicilarServisi.KullaniciGuncelle(_seciliYetkiKullanici);

            if (!guncellendi)
            {
                BildirimGoster("Ayarlar.kullaniciGuncellemeBasarisiz");
                return;
            }

            BildirimGoster("Ayarlar.kullaniciGuncellemeBasarili");
            KullanicilariYukle();
            lstYetkiKullaniciListesi.SelectedItem = null;
            _seciliYetkiKullanici = null;
            txtSeciliYetkiKullanici.Text = "Seçili Kullanıcı:";
        }
    }
}