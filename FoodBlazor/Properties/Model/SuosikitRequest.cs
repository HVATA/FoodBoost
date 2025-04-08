namespace FoodBlazor.Properties.Model
{
    public class SuosikitRequest
    {
        //ei viedä tätä luokkaa kantaan toimii vaan luokkana johon voi tallentaa yhdeksi objektiksi api/metodi kutsuja varten kun parametrinä ei voi välittää eikä Bodyssä montaa objektia

        public List<Suosikit>? Suosikitlista { get; set; }
        public Kayttaja? Kayttaja { get; set; }

    }
}
