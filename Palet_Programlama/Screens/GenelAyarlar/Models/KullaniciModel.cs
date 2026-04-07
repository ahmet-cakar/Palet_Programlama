using System;
using System.Collections.Generic;

namespace Palet_Programlama.Models
{
    public class KullaniciModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operator";
        public bool AktifMi { get; set; } = true;

        public List<string> YetkiliSayfalar { get; set; } = new();
    }
}