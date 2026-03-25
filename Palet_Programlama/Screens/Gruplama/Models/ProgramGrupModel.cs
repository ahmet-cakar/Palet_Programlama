using Palet_Programlama.Screens.Dizilim.Models;
using System.Collections.Generic;

namespace Palet_Programlama.Screens.Gruplama.Models
{
    public sealed class ProgramGrupModel
    {
        public int KatNo { get; set; }
        public int GrupNo { get; set; }

        public double GrupMerkezX { get; set; }
        public double GrupMerkezY { get; set; }
        public double GrupMerkezZ { get; set; }

        public GrupGripperAyarlari GripperAyarlari { get; set; } = new();

        public UrunYonu? Yon { get; set; }
        public int UrunSayisi { get; set; }

        public List<ProgramUrunModel> Urunler { get; set; } = new();
    }
}