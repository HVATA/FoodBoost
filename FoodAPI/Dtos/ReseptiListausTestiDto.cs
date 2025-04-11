namespace FoodAPI.Dtos
{
    public class ReseptiListausTestiDto
    {
        public int Id { get; set; }
        public string Nimi { get; set; } = string.Empty;
        public int TekijaId { get; set; }
        public string Katseluoikeus { get; set; } = string.Empty;
        // Ei kuvaa tai muita raskaita tietoja
    }
}
