using RuokaAPI.Properties.Model;

namespace RuokaAPI.Dtos
{
    public class ReseptiResponse
    {
        public int Id { get; set; }
        public int TekijaId { get; set; }
        public string Nimi { get; set; }
        public string Valmistuskuvaus { get; set; } = string.Empty;
        public AinesosanMaaraDto[] Ainesosat { get; set; } = Array.Empty<AinesosanMaaraDto>();
        public string[] Avainsanat { get; set; } = Array.Empty<string>();
        public string Kuva1 { get; set; } = string.Empty;
        public string Katseluoikeus { get; set; } = string.Empty;
        public Arvostelu[] Arvostelut { get; set; } = Array.Empty<Arvostelu>();
    }
}

