namespace varjoBlazor.Model
{
    public class SuosikitRequest
    {
        //ei viedä tätä luokkaa kantaan toimii vaan luokkana johon voi tallentaa yhdeksi objektiksi api/metodi kutjuja varten kun parametrinä ei voi välittää eikä Bodyssä montaa objektia

        public List<int>? Idlista { get; set; }
        public Kayttaja? Kayttaja { get; set; }




    }
}
