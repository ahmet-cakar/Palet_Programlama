using Palet_Programlama.Screens;
using Palet_Programlama.Screens.UrunPaletEkle.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Palet_Programlama
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public UrunIslemler UrunIslemler { get; } = new UrunIslemler();
        public PaletIslemler PaletIslemler { get; } = new PaletIslemler();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Bekleme(MainFrame));


        }
        public void SayfayaGit(Page yeniSayfa)
        {
            if (yeniSayfa == null)
                return;

            MainFrame.Content = yeniSayfa;
        }

        public void YukleniyorGoster(string mesaj = "Yükleniyor...")
        {
            LoadingOverlayControl.Goster(mesaj);
        }

        public void YukleniyorGizle()
        {
            LoadingOverlayControl.Gizle();
        }

        public void SayfayaKayarakGit(Page yeniSayfa, bool solaDogru)
        {
            if (yeniSayfa == null)
                return;

            double mesafe = MainFrame.ActualWidth;
            if (mesafe <= 0)
                mesafe = 1400;

            if (MainFrame.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                MainFrame.RenderTransform = transform;
            }

            var ease = new QuarticEase
            {
                EasingMode = EasingMode.EaseOut
            };

            var cikisAnim = new DoubleAnimation
            {
                From = 0,
                To = solaDogru ? mesafe : -mesafe,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = ease
            };

            cikisAnim.Completed += (s, e) =>
            {
                MainFrame.Content = yeniSayfa;
                transform.X = solaDogru ? -mesafe : mesafe;

                var girisAnim = new DoubleAnimation
                {
                    From = -transform.X,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(320),
                    EasingFunction = ease
                };

                transform.BeginAnimation(TranslateTransform.XProperty, girisAnim);
            };

            transform.BeginAnimation(TranslateTransform.XProperty, cikisAnim);
        }

      
    }
}
