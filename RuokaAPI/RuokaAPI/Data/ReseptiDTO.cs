namespace RuokaAPI.Data
{
    public class ReseptiDTO
    {
        public int Id { get; set; }

        public int Tekijäid { get; set; }

        // Ei tarpeen nimet/tunnistautumistiedot/käyttäjätasot mutta kun tai tarvitsee koostaa yhteenvetotiedon niin saa suoraan luokasta, ei tarvitse erikseen hakea tietokannasta
        public string? Etunimi { get; set; }
        public string? Sukunimi { get; set; }

        public string? Nimimerkki { get; set; }

        public string? Sahkopostiosoite { get; set; }

        public string? Salasana { get; set; }

        public string? Kayttajataso { get; set; }

        public string? Ainesosat { get; set; }



        public string? Valmistuskuvaus { get; set; }

        public string? Kuva1 { get; set; }
        public string? Kuva2 { get; set; }
        public string? Kuva3 { get; set; }
        public string? Kuva4 { get; set; }
        public string? Kuva5 { get; set; }
        public string? Kuva6 { get; set; }

        public string? Avainsanat { get; set; }

        public string? Katseluoikeus { get; set; }






    }
}
