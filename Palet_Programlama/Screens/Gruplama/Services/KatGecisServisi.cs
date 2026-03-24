using Servisler.PaletMethod;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Palet_Programlama.Sayfalar.Gruplama.Services
{
    public sealed class KatGecisServisi
    {
        public int KatDegistir(
            int yeniKat,
            KatYoneticisi katYonetici,
            Canvas canvas,
            Rectangle sonSecilmisKutu,
            MouseButtonEventHandler mouseDown,
            MouseEventHandler mouseMove,
            MouseButtonEventHandler mouseUp)
        {
            katYonetici.KatDegistir(
                yeniKat,
                canvas,
                sonSecilmisKutu,
                mouseDown,
                mouseMove,
                mouseUp);

            return katYonetici.AktifKat;
        }
    }
}
