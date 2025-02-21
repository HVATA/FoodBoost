using System.Text.Json.Serialization;

namespace RuokaAPI.Properties.Model
{
    public class Avainsana
    {
        public int Id { get; set; }
        public required string Sana { get; set; }


        [JsonIgnore]
        public List<Resepti> Reseptit { get; set; } = new();
    }
}
