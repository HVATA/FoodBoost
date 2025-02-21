using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace RuokaBlazor.Properties.Model
{
    public class Avainsana
    {
        public int Id { get; set; }
        public string Sana { get; set; }

        // Lisätään IsChecked ominaisuus
        public bool IsChecked { get; set; } = false;

        [JsonIgnore]
        public List<Resepti> Reseptit { get; set; } = new();
    }

}
