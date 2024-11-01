using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
namespace Palet_Programlama.Sınıflar
{
    public class LanguageConverter : IValueConverter
    {
        private static JObject _languageData;
        public LanguageConverter() { }

        // Dil dosyasını yüklemek için kullanılan metod
        public static void LoadLanguage(string languageCode)
        {
            //string filePath = DosyaYoluBul.DosyaGetir("DilPaketleri",$"{languageCode}.json");
            string filePath = $"C:\\Users\\yunusemre.kara\\source\\repos\\Palet_Programlama\\Palet_Programlama\\DilPaketleri\\{languageCode}.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _languageData = JObject.Parse(json);
            }
            else
            {
                throw new FileNotFoundException($"Dil dosyası bulunamadı: {filePath}");
            }
        }
        public static void LoadLanguage2(string languageCode)
        {
            string filePath = DosyaYoluBul.DosyaGetir2("DilPaketleri", $"{languageCode}.json");
            //string filePath = $"C:\\Users\\yunusemre.kara\\source\\repos\\Palet_Programlama\\Palet_Programlama\\DilPaketleri\\{languageCode}.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _languageData = JObject.Parse(json);
            }
            else
            {
                throw new FileNotFoundException($"Dil dosyası bulunamadı: {filePath}");
            }
        }

        // Çeviriyi almak için kullanılan metod
        public static string GetString(string key)
        {
            if (_languageData == null)
            {
                LoadLanguage2("eng");
            }

            var tokens = key.Split('.'); // Anahtarları parçalara ayırıyoruz
            JToken currentToken = _languageData;

            foreach (var token in tokens)
            {
                if (currentToken[token] != null)
                {
                    currentToken = currentToken[token];
                }
                else
                {
                    return key; // Anahtar bulunamazsa, orijinal anahtarı döndür
                }
            }

            return currentToken.ToString();
        }

        // IValueConverter'ın Convert metodunu uygulayın
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string key)
            {
                return GetString(key);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
