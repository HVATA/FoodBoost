using System.Text.Json.Serialization;

namespace FoodBlazor.Properties.Model
{
    public class Ainesosa
    {
        public int Id { get; set; }
        public string Nimi { get; set; }

        // Lisätään IsChecked ominaisuus
        public bool IsChecked { get; set; } = false;

        [JsonIgnore]
        public List<Resepti> Reseptit { get; set; } = new();
    }


}
