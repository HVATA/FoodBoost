using System.Text.Json.Serialization;

namespace RuokaAPI.Properties.Model
{
    public class KayttajaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("etunimi")]
        public string Etunimi { get; set; }

        [JsonPropertyName("sukunimi")]
        public string Sukunimi { get; set; }

        [JsonPropertyName("nimimerkki")]
        public string Nimimerkki { get; set; }

        [JsonPropertyName("sahkopostiosoite")]
        public string Sahkopostiosoite { get; set; }

        [JsonPropertyName("salasana")]
        public string Salasana { get; set; }

        [JsonPropertyName("kayttajataso")]
        public string Kayttajataso { get; set; }

        [JsonPropertyName("uusisalasana")]
        public string Uusisalasana { get; set; }




    }
}
