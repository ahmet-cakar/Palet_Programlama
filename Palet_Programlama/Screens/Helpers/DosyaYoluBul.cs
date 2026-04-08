using System;
using System.IO;

namespace Palet_Programlama.Screens.Helpers
{
    public static class DosyaYoluBul
    {
        public static string DosyaGetir(string klasorAdi, string dosyaAdi)
        {
            DirectoryInfo klasor = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (klasor != null)
            {
                string adayYol = Path.Combine(klasor.FullName, klasorAdi, dosyaAdi);
                if (File.Exists(adayYol))
                    return adayYol;

                klasor = klasor.Parent;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, klasorAdi, dosyaAdi);
        }
    }
}