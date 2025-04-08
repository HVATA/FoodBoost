using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace FoodAPI.Properties.Model
{
    public class Arvostelu
    {
        public int ArvosteluID { get; set; }

        public int ReseptiId { get; set; }


        [JsonIgnore]      
        public Resepti Resepti { get; set; } = new();


        //ei varmaan näytetä arvostelijanId:tä taulussa
        public int ArvostelijanId { get; set; }

        public string? ArvostelijanNimimerkki { get; set; }

        // 1-5 arviointi
        public int Numeroarvostelu { get; set; }

        public string Vapaateksti { get; set; }

    }
}
