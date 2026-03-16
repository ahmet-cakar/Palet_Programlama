using System;
using System.Windows;

namespace Palet_Programlama.Popuplar
{
    public partial class GrupAyarlariPopup : Window
    {
        private bool _hazirMi;

        public int KatNo { get; }
        public int GrupNo { get; }

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

            BasligiHazirla();

            _hazirMi = true;
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
            DialogResult = true;
            Close();
        }

        private void BirakmaYonu_Checked(object sender, RoutedEventArgs e)
        {
            if (!_hazirMi) return;

            if (TglBirakmaYonuUst == null ||
                TglBirakmaYonuAlt == null ||
                TglBirakmaYonuSag == null ||
                TglBirakmaYonuSol == null)
                return;

            if (sender == TglBirakmaYonuUst)
            {
                TglBirakmaYonuAlt.IsChecked = false;
                TglBirakmaYonuSag.IsChecked = false;
                TglBirakmaYonuSol.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuAlt)
            {
                TglBirakmaYonuUst.IsChecked = false;
                TglBirakmaYonuSag.IsChecked = false;
                TglBirakmaYonuSol.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuSag)
            {
                TglBirakmaYonuUst.IsChecked = false;
                TglBirakmaYonuAlt.IsChecked = false;
                TglBirakmaYonuSol.IsChecked = false;
            }
            else if (sender == TglBirakmaYonuSol)
            {
                TglBirakmaYonuUst.IsChecked = false;
                TglBirakmaYonuAlt.IsChecked = false;
                TglBirakmaYonuSag.IsChecked = false;
            }
        }
    }
}