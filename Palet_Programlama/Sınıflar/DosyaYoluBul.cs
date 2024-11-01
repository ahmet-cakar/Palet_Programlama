using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palet_Programlama.Sınıflar
{
    public static class DosyaYoluBul
    {
        public static string DosyaGetir(string klasorAdi, string dosyaAdi)
        {
            string uygulamaDizini = Directory.GetCurrentDirectory(); //uygulama dizini 
            string uygulamaDizini2 = Path.GetDirectoryName(uygulamaDizini);  //ustdizin
            string uygulamaDizini3 = Path.GetDirectoryName(uygulamaDizini2);  //2 ust dizin
            string result = Path.Combine(uygulamaDizini3, klasorAdi + "\\" + dosyaAdi); //Data klasorunun icindeki dosya  yolu
            return result;
        }
        public static string DosyaGetir2(string klasorAdi, string dosyaAdi)
        {
            string uygulamaDizini = Directory.GetCurrentDirectory(); //uygulama dizini 
            string result = Path.Combine(uygulamaDizini,"Palet_Programlama"+"\\"+ klasorAdi + "\\" + dosyaAdi); //Data klasorunun icindeki dosya  yolu
            return result;
        }
    }
}
