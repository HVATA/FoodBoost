namespace RuokaAPI.Dtos
{
    //tämä luokka kuvaa http-pyynnön body-lohkoa, joka eroaa hiukan tietokantataulusta, esim resepitaulun id:tä ei tarvita
    //lisätessä ja muokatessa id tulee osana url:ia. Kun tätä luokkaa käytetään, voidaan pyyntä tehdä siinä muodossa kuin halutaan
    //ilman, että tarvitsee välittää tietokantataulun rakenteesta
    public class ReseptiRequest
    {
        public int Id { get; set; }
        public int TekijaId { get; set; }
        public string Nimi { get; set; }
        public string Valmistuskuvaus { get; set; } = string.Empty;
        public AinesosanMaaraDto[] Ainesosat { get; set; } = Array.Empty<AinesosanMaaraDto>();
        public string[] Avainsanat { get; set; } = Array.Empty<string>();
        public string Kuva1 { get; set; } = string.Empty;
        public string Katseluoikeus { get; set; } = string.Empty;
    }}

