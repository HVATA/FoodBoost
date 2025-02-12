using System.Collections.Generic;

namespace RuokaAPI.Properties.Model;

public class Resepti
{
    public int Id { get; set; }
    public int Tekijäid { get; set; }
    public string Nimi { get; set; }
    //public string Kategoria { get; set; }
    public List<ReseptiAinesosa> AinesosanMaara { get; set; } = new();
    public string? Valmistuskuvaus { get; set; }
    public string? Kuva1 { get; set; }
    public string? Kuva2 { get; set; }
    public string? Kuva3 { get; set; }
    public string? Kuva4 { get; set; }
    public string? Kuva5 { get; set; }
    public string? Kuva6 { get; set; }
    public List<Avainsana> Avainsanat { get; set; } = new();
    public List<Arvostelu> Arvostelut { get; set; } = new();
    public string? Katseluoikeus { get; set; }
}