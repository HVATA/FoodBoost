﻿using System.Text.Json.Serialization;

namespace FoodAPI.Properties.Model
{
    public class Kayttaja
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
    }

}
