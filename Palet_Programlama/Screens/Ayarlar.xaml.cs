using Palet_Programlama.Models;
using Palet_Programlama.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Palet_Programlama.Screens
{
    /// <summary>
    /// Interaction logic for Ayarlar.xaml
    /// </summary>
    public partial class Ayarlar : Page
    {
        private readonly Frame _mainFrame;
        private readonly KullanicilarServisi _kullanicilarServisi;
        private List<KullaniciModel> _kullanicilar = new();
        private KullaniciModel? _seciliKullanici;
        private bool _sifreGorunurMu = false;

        public Ayarlar(Frame main)
        {
            InitializeComponent();
            _mainFrame = main;
            _kullanicilarServisi = new KullanicilarServisi();
            KullanicilariYukle();

        }

        private void KullanicilariYukle()
        {
            _kullanicilar = _kullanicilarServisi.TumKullanicilariGetir();
            lstKullaniciListesi.ItemsSource = null;
            lstKullaniciListesi.ItemsSource = _kullanicilar;
            txtKayiSayisi.Text = _kullanicilar.Count.ToString() + " Kayıt";
        }

        private void lstKullaniciListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstKullaniciListesi.SelectedItem is not KullaniciModel secili)
                return;

            _seciliKullanici = secili;

            txtKullaniciAdi.Text = secili.KullaniciAdi;
            txtKullaniciSifre.Password = secili.Sifre;
            txtKullaniciSifreAcik.Text = secili.Sifre;
        }

        private void AlanlariTemizle()
        {
            txtKullaniciAdi.Text = string.Empty;
            txtKullaniciSifre.Password = string.Empty;
            txtKullaniciSifreAcik.Text = string.Empty;

            lstKullaniciListesi.SelectedItem = null;
            _seciliKullanici = null;
        }

        private string AktifSifreyiAl()
        {
            return txtKullaniciSifre.Visibility == Visibility.Visible
                ? txtKullaniciSifre.Password
                : txtKullaniciSifreAcik.Text;
        }


        private void btnKullaniciEkle_Click(object sender, RoutedEventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = AktifSifreyiAl().Trim();

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.");
                return;
            }

            if (_kullanicilarServisi.KullaniciAdiVarMi(kullaniciAdi))
            {
                MessageBox.Show("Bu kullanıcı adı zaten mevcut.");
                return;
            }

            var yeniKullanici = new KullaniciModel
            {
                KullaniciAdi = kullaniciAdi,
                Sifre = sifre,
                Rol = "Operator",
                AktifMi = true,
                YetkiliSayfalar = new List<string>()
            };

            bool eklendi = _kullanicilarServisi.KullaniciEkle(yeniKullanici);

            if (!eklendi)
            {
                MessageBox.Show("Kullanıcı eklenemedi.");
                return;
            }

            KullanicilariYukle();
            AlanlariTemizle();
        }

        private void btnKullaniciGuncelle_Click(object sender, RoutedEventArgs e)
        {
            if (_seciliKullanici == null)
            {
                MessageBox.Show("Lütfen güncellenecek kullanıcıyı seçiniz.");
                return;
            }

            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = AktifSifreyiAl().Trim();

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.");
                return;
            }

            if (_kullanicilarServisi.KullaniciAdiVarMi(kullaniciAdi, _seciliKullanici.Id))
            {
                MessageBox.Show("Bu kullanıcı adı zaten mevcut.");
                return;
            }

            _seciliKullanici.KullaniciAdi = kullaniciAdi;
            _seciliKullanici.Sifre = sifre;

            bool guncellendi = _kullanicilarServisi.KullaniciGuncelle(_seciliKullanici);

            if (!guncellendi)
            {
                MessageBox.Show("Kullanıcı güncellenemedi.");
                return;
            }

            KullanicilariYukle();
            AlanlariTemizle();
        }

        private void btnKullaniciSil_Click(object sender, RoutedEventArgs e)
        {
            if (_seciliKullanici == null)
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı seçiniz.");
                return;
            }

            bool silindi = _kullanicilarServisi.KullaniciSil(_seciliKullanici.Id);

            if (!silindi)
            {
                MessageBox.Show("Kullanıcı silinemedi.");
                return;
            }

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
    }
}
