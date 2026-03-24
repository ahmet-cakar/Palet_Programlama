using System;
using System.Collections.Generic;

namespace Palet_Programlama.Sayfalar.Gruplama.Models
{
    public sealed class ProgramKayitModel
    {
        public int Id { get; set; }
        public string ProgramAdi { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;

        public string UrunAdi { get; set; } = string.Empty;

        public string PaletAdi { get; set; } = string.Empty;

        public string DizilimAdi { get; set; } = string.Empty;

        public string OlusturmaTarihi { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

        public List<ProgramGrupModel> Gruplar { get; set; } = new();
    }
}