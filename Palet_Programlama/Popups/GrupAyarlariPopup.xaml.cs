using Palet_Programlama.Screens.Gruplama.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace Palet_Programlama.Popuplar
{
    public partial class GrupAyarlariPopup : Window
    {
        private bool _hazirMi;
        public int KatNo { get; }
        public int GrupNo { get; }

        public GrupGripperAyarlari Sonuc { get; private set; }

        public GrupAyarlariPopup(int katNo, int grupNo)
        {
            if (katNo <= 0)
                throw new ArgumentOutOfRangeException(nameof(katNo), "Kat no 0'dan büyük olmalıdır.");

            if (grupNo <= 0)
                throw new ArgumentOutOfRangeException(nameof(grupNo), "Grup no 0'dan büyük olmalıdır.");

            KatNo = katNo;
            GrupNo = grupNo;

            _hazirMi = false;
            InitializeComponent();
            Loaded += (s, e) =>
            {
                double ekranGenisligi = SystemParameters.WorkArea.Width;
                double ekranYuksekligi = SystemParameters.WorkArea.Height;

                double solYariGenisligi = ekranGenisligi / 2.0;

                Left = (solYariGenisligi - Width) / 2.0;
                Top = (ekranYuksekligi - Height) / 2.0;
            };
            BasligiHazirla();

            _hazirMi = true;
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BasligiHazirla()
        {
            Title = $"{KatNo}. Kat - {GrupNo}. Grup Ayarları";

            if (TxtBaslik != null)
                TxtBaslik.Text = $"{KatNo}. Kat - {GrupNo}. Grup Ayarları";
        }

        private void BtnKapat_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (!SeciliGripperAcisiniBul(out GripperAcisiEnum seciliAci))
            {
                MessageBox.Show("Lütfen bir bırakma yönü seçiniz.");
                return;
            }

            if (!Int32.TryParse(TxtOffsetX.Text, out int offsetX))
            {
                MessageBox.Show("Offset X değeri geçersiz.");
                return;
            }

            if (!Int32.TryParse(TxtOffsetY.Text, out int offsetY))
            {
                MessageBox.Show("Offset Y değeri geçersiz.");
                return;
            }

            if (!Int32.TryParse(TxtOffsetZ.Text, out int offsetZ))
            {
                MessageBox.Show("Offset Z değeri geçersiz.");
                return;
            }

            Sonuc = new GrupGripperAyarlari
            {
                KayitEdildiMi = true,
                GripperAcisi = (int)seciliAci,
                OffsetX = offsetX,
                OffsetY = offsetY,
                OffsetZ = offsetZ
            };

            DialogResult = true;
            Close();
        }


        public void AyarlariYukle(GrupGripperAyarlari ayar)
        {
            if (ayar == null)
                return;

            _hazirMi = false;

            TxtOffsetX.Text = ayar.OffsetX.ToString();
            TxtOffsetY.Text = ayar.OffsetY.ToString();
            TxtOffsetZ.Text = ayar.OffsetZ.ToString();

            TglBirakmaYonuEksiX.IsChecked = false;
            TglBirakmaYonuArtiX.IsChecked = false;
            TglBirakmaYonuArtiY.IsChecked = false;
            TglBirakmaYonuEksiY.IsChecked = false;

            switch (ayar.GripperAcisi)
            {
                case (int)GripperAcisiEnum.ArtiX:
                    TglBirakmaYonuArtiX.IsChecked = true;
                    break;

                case (int)GripperAcisiEnum.EksiX:
                    TglBirakmaYonuEksiX.IsChecked = true;
                    break;

                case (int)GripperAcisiEnum.ArtiY:
                    TglBirakmaYonuArtiY.IsChecked = true;
                    break;

                case (int)GripperAcisiEnum.EksiY:
                    TglBirakmaYonuEksiY.IsChecked = true;
                    break;

                default:
                    TglBirakmaYonuEksiX.IsChecked = true;
                    break;
            }

            _hazirMi = true;
        }


        private bool SeciliGripperAcisiniBul(out GripperAcisiEnum gripperAcisi)
        {


            if (TglBirakmaYonuArtiX?.IsChecked == true)
            {
                gripperAcisi = GripperAcisiEnum.ArtiX;
                return true;
            }

            if (TglBirakmaYonuEksiX?.IsChecked == true)
            {
                gripperAcisi = GripperAcisiEnum.EksiX;
                return true;
            }

            if (TglBirakmaYonuArtiY?.IsChecked == true)
            {
                gripperAcisi = GripperAcisiEnum.ArtiY;
                return true;
            }

            if (TglBirakmaYonuEksiY?.IsChecked == true)
            {
                gripperAcisi = GripperAcisiEnum.EksiY;
                return true;
            }

            gripperAcisi = GripperAcisiEnum.EksiX;
            return false;
        }


        private void BirakmaYonu_Checked(object sender, RoutedEventArgs e)
        {
            if (!_hazirMi) return;

            if (TglBirakmaYonuEksiX == null ||
                TglBirakmaYonuArtiX == null ||
                TglBirakmaYonuArtiY == null ||
                TglBirakmaYonuEksiY == null)
                return;

            if (sender == TglBirakmaYonuEksiX)
            {
                TglBirakmaYonuArtiX.IsChecked = false;
                TglBirakmaYonuArtiY.IsChecked = false;
                TglBirakmaYonuEksiY.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuArtiX)
            {
                TglBirakmaYonuEksiX.IsChecked = false;
                TglBirakmaYonuArtiY.IsChecked = false;
                TglBirakmaYonuEksiY.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuArtiY)
            {
                TglBirakmaYonuEksiX.IsChecked = false;
                TglBirakmaYonuArtiX.IsChecked = false;
                TglBirakmaYonuEksiY.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuEksiY)
            {
                TglBirakmaYonuEksiX.IsChecked = false;
                TglBirakmaYonuArtiX.IsChecked = false;
                TglBirakmaYonuArtiY.IsChecked = false;
            }
        }

     
        
    }
}