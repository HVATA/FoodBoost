﻿namespace RuokaAPI.Properties.Model
{
    public class SuosikkiMuokkaus
    {
        //ei kantaan, luokkana joka pitää kerralla sisällään kaksi muuta luokkaa suosikki muokkaustoimintoja varten

       public Kayttaja? Kayttaja {  get; set; } 

       public Resepti? resepti { get; set; }

    }
}
