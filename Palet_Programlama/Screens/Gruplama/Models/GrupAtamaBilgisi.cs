namespace Palet_Programlama.Screens.Gruplama.Models
{
    public sealed class GrupAtamaBilgisi
    {
        public int KatNo { get; set; }
        public int GrupNo { get; set; }
        public string GrupEkseni { get; set; } = "";

        public string KoliAnahtari { get; set; } = string.Empty;
    }
}