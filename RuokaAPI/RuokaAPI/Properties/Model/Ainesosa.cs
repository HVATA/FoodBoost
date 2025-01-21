using System.Text.Json.Serialization;

namespace RuokaAPI.Properties.Model
{
    public class Ainesosa
    {
        public int Id { get; set; }
        public string Nimi { get; set; }
        [JsonIgnore]
        public List<Resepti> Reseptit { get; set; } = new();
    }
}
