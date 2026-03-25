using Palet_Programlama.Screens.Dizilim.Models;
using System.Collections.Generic;

namespace Palet_Programlama.Screens.Gruplama.Models
{
    public sealed class GruplamaYuklemeModel
    {
        public List<DizilimKayitModel> DizilimKayitlari { get; set; } = new();
        public List<ProgramKayitModel> ProgramKayitlari { get; set; } = new();

        public List<string> UrunAdlari { get; set; } = new();
        public List<string> DizilimAdlari { get; set; } = new();
        public List<string> ProgramAdlari { get; set; } = new();

        public string SeciliUrunAdi { get; set; } = "";
        public string SeciliDizilimAdi { get; set; } = "";
        public string PaletOzellikMetni { get; set; } = "";

        public DizilimKayitModel SeciliKayit { get; set; }
    }
}