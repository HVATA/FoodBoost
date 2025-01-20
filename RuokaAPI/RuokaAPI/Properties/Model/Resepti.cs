﻿namespace RuokaAPI.Properties.Model
{
    public class Resepti
    {
       public int Id {  get; set; } 

        public int Tekijäid { get; set; }

        // Ei tarpeen nimet mutta kun tai tarvitsee koostaa yhteenvetotiedon niin saa suoraan luokasta, ei tarvitse erikseen hakea tietokannasta
        // Voidaan myös hoitaa tunnistautuminen tarvittaessa yhdellä luokalla kun fronttipuoli saa noidin datat, voidaan toki karsia jos tarvis.

        public string Sahkopostiosoite { get; set; } //voidaan ottaa tuosta tarvittaessa suoraan email ja alemmasta Salasana jos halutaan hakea käyttäjän oikeuksia muokat reseptejä suoraan tästä

        //tehdään noita tunnistautumistietoja varten tästä DTO luokka jossa nuokin ylläolevat tiedot onja siitä yhdistää vaan perus Resepti luokkaan joka viedään kantaan.


        public string Salasana { get; set; }
        public string? Kayttajataso { get; set; }
        public string? Etunimi { get; set; }
        public string? Sukunimi { get; set; }

        public string? Nimimerkki { get; set; }

      public string? Ainesosat {  get; set; }

        public string? Valmistuskuvaus { get; set; }

        public string? Kuva1 { get; set; }
        public string? Kuva2 { get; set; }
        public string? Kuva3 { get; set; }
        public string? Kuva4 { get; set; }
        public string? Kuva5 { get; set; }
        public string? Kuva6 { get; set; }

       public string? Avainsanat {  get; set; }  

        public string? Katseluoikeus { get; set; }






    }
}
