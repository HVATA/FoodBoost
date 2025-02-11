using RuokaAPI.Properties.Model;
using System.Text.Json.Serialization;

public class ReseptiAinesosa
{
    public int ReseptiId { get; set; }
    [JsonIgnore]
    public Resepti Resepti { get; set; }
    public int AinesosaId { get; set; }
    [JsonIgnore]
    public Ainesosa Ainesosa { get; set; }
    public string Maara { get; set; } // New property
}
