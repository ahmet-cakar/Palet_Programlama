using System;

public class GirisAyarlariModel
{
    public string SonGirenKullaniciAdi { get; set; } = "";
    public string SonGirenSifre { get; set; } = "";
    public bool BeniHatirla { get; set; } = false;
    public DateTime? SonBasariliGirisTarihi { get; set; }
}