using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palet_Programlama.Statics
{
    public static class ResimYollari
    {
        public static readonly ImageSource dikeyResim =
         new BitmapImage(new Uri("pack://application:,,,/Resimler/DizilimYap/dikey_kutu.png", UriKind.Absolute));

        public static readonly ImageSource yatayResim =
            new BitmapImage(new Uri("pack://application:,,,/Resimler/DizilimYap/yatay_kutu.png", UriKind.Absolute));
    }
}
