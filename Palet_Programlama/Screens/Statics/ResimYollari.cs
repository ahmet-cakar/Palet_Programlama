using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palet_Programlama.Screens.Statics { 
    public static class ResimYollari
    {
        public static readonly ImageSource dikeyResim =
         new BitmapImage(new Uri("pack://application:,,,/Images/DizilimYap/dikey_kutu.png", UriKind.Absolute));

        public static readonly ImageSource yatayResim =
            new BitmapImage(new Uri("pack://application:,,,/Images/DizilimYap/yatay_kutu.png", UriKind.Absolute));
    }
}
