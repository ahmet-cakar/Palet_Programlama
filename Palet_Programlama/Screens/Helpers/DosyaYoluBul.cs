using System.IO;

namespace Palet_Programlama.Screens.Helpers
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
       
    }
}
