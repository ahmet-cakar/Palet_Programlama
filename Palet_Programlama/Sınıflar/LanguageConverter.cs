using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
namespace Palet_Programlama.Sınıflar
{
    public static class LanguageConverter
    {
        public static JObject languageData;

        public static void LoadLanguage(string languageCode)
        {
            string filePath = $"C:\\Users\\yunusemre.kara\\source\\repos\\Palet_Programlama\\Palet_Programlama\\Dil_Paketleri\\{languageCode}.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                languageData = JObject.Parse(json);
            }
            else
            {
                throw new FileNotFoundException($"Dil dosyası bulunamadı: {filePath}");
            }
        }

        public static string GetString(string key)
        {
            var tokens = key.Split('.'); // "Anasayfa.textblokurun" gibi anahtarları parçalara ayırır
            JToken currentToken = languageData;

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
    }
}
