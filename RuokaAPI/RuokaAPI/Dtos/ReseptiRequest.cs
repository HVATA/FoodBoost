namespace RuokaAPI.Dtos
{
    //t�m� luokka kuvaa http-pyynn�n body-lohkoa, joka eroaa hiukan tietokantataulusta, esim reseptitaulun id:t� ei tarvita
    //lis�tess� ja muokatessa id tulee osana url:ia. Kun t�t� luokkaa k�ytet��n, voidaan pyynt� tehd� siin� muodossa kuin halutaan
    //ilman, ett� tarvitsee v�litt�� tietokantataulun rakenteesta
    public class ReseptiRequest
    {
        public int Id { get; set; }
        public int TekijaId { get; set; }
        public string Nimi { get; set; }
        public string Valmistuskuvaus { get; set; } = string.Empty;
        public AinesosanMaaraDto[] Ainesosat { get; set; } = Array.Empty<AinesosanMaaraDto>();
        public string[] Avainsanat { get; set; } = Array.Empty<string>();
        public string? Kuva1 { get; set; }
        public string Katseluoikeus { get; set; } = string.Empty;
    }}

