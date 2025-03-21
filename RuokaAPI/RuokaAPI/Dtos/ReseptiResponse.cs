using RuokaAPI.Properties.Model;

namespace RuokaAPI.Dtos
{
    // This class represents the response DTO for a recipe (Resepti).
    // It includes properties for the recipe's ID, author ID, name, description, ingredients, keywords, image,
    // visibility, and reviews.
    public class ReseptiResponse
    {
        public int Id { get; set; }
        public int TekijaId { get; set; }
        public string Nimi { get; set; }
        public string Valmistuskuvaus { get; set; } = string.Empty;
        public AinesosanMaaraDto[] Ainesosat { get; set; } = Array.Empty<AinesosanMaaraDto>();
        public string[] Avainsanat { get; set; } = Array.Empty<string>();
        public string? Kuva1 { get; set; }
        public string Katseluoikeus { get; set; } = string.Empty;
        public Arvostelu[] Arvostelut { get; set; } = Array.Empty<Arvostelu>();
        
        }
}

