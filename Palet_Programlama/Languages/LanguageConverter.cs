using Newtonsoft.Json.Linq;
using Palet_Programlama.Screens.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
namespace Palet_Programlama.Languages
{
    public class LanguageConverter : IValueConverter
    {
        private static JObject _languageData;
        public LanguageConverter() { }

        // Dil dosyasını yüklemek için kullanılan metod

        public static void DilYukle(string languageCode)
        {
            string filePath = DosyaYoluBul.DosyaGetir("Languages", $"{languageCode}.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _languageData = JObject.Parse(json);
            }
            else
            {
                _languageData ??= new JObject();
            }
        }

        // Çeviriyi almak için kullanılan metod
        public static string GetString(string key)
        {
            if (_languageData == null)
            {
                DilYukle("tr");
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
