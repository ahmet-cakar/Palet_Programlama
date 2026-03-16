using Palet_Programlama.Modeller;
using System.Collections.Generic;

namespace Palet_Programlama.Sayfalar.Gruplama.Models
{
    public sealed class ProgramGrupModel
    {
        public int KatNo { get; set; }
        public int GrupNo { get; set; }

        public double GrupMerkezX { get; set; }
        public double GrupMerkezY { get; set; }
        public double GrupMerkezZ { get; set; }

        public int GripperAcisi { get; set; } = 360;

        public UrunYonu Yon { get; set; }
        public int UrunSayisi { get; set; }

        public List<ProgramUrunModel> Urunler { get; set; } = new();
    }
}