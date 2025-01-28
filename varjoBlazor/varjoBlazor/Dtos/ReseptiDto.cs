namespace RuokaAPI.Dtos
{
    public class ReseptiDto
    {
        public int TekijaId { get; set; }
        public string Valmistuskuvaus { get; set; } = string.Empty;
        public string[] Ainesosat { get; set; } = Array.Empty<string>();
        public string[] Avainsanat { get; set; } = Array.Empty<string>();
        public string Kuva1 { get; set; } = string.Empty;
        public string Katseluoikeus { get; set; } = string.Empty;
    }
}
