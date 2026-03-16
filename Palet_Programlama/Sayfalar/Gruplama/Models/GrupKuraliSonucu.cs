public sealed class GrupKuraliSonucu
{
    public bool Basarili { get; set; }
    public string Mesaj { get; set; }

    public static GrupKuraliSonucu Gecerli()
        => new GrupKuraliSonucu { Basarili = true };

    public static GrupKuraliSonucu Gecersiz(string mesaj)
        => new GrupKuraliSonucu { Basarili = false, Mesaj = mesaj };
}