using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Properties.Model;
using RuokaAPI.Services;

namespace RuokaAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class KayttajaController : ControllerBase
    {

        private readonly ruokaContext _context;

        public KayttajaController(ruokaContext context)
        {
            _context = context;
        }

        [HttpPost("LisaaKayttaja")]
        public async Task<string> Lisaa(Kayttaja x)
        {
            //saako kukatahansa lisätä käyttäjän jos saa missä vaiheessa tarkistetaan että käyttäjä ei lisää kayttäjätaso = "admin"?


            List<Kayttaja> lista = new List<Kayttaja>();




            string email = x.Sahkopostiosoite;

            lista = _context.Kayttajat.Where(x => x.Sahkopostiosoite ==email).ToList();

            if (lista.Count == 0)
            {

                _context.Kayttajat.Add(x);
                await _context.SaveChangesAsync();

                return "Käyttäjä lisätty";
            }

            else
            {

                return "Sähköposti on jo käytössä!!!";


            }



        }

        [HttpGet("Tunnistautumistiedot/{salasana}/{sahkopostiosoite}")]
        public async Task<ActionResult<Kayttaja>> HaeKayttaja(string salasana,string sahkopostiosoite)
        {



            
        

            //Haetaan front kutsusta käyttäjä salasanan ja sähköpostiosoitteen perusteella
            


         Kayttaja?    p = await _context.Kayttajat.Where (x => (x.Sahkopostiosoite==sahkopostiosoite&&x.Salasana==salasana)).FirstOrDefaultAsync();


            if (p != null)
            {
                return Ok(p);


            }

            else {



                return NotFound("Käyttäjää ei löytynyt.");
            
            
            
            }


            

        }


        [HttpGet("Haekaikki/{id}/{salasana}/{sahkopostiosoite}")]
        public async Task<List<Kayttaja>> HaeKayttajat(int id, string salasana, string sahkopostiosoite)
        {

            List<Kayttaja> kayttajat = new List<Kayttaja>();

            // Etsitään käyttäjä tietokannasta
            Kayttaja? p = await _context.Kayttajat.FindAsync(id);

            if (p == null)
            {
                return kayttajat;
            }

            string kayttajataso = "admin";

            // Tarkistetaan, että käyttäjä on admin ja tunnistetiedot ovat oikein
            if (p.Kayttajataso.Equals(kayttajataso) && p.Salasana == salasana && p.Sahkopostiosoite == sahkopostiosoite)
            {
                kayttajat = await _context.Kayttajat.ToListAsync();
                return kayttajat;
            }

            else {

                return kayttajat;
                 } 
        }

        [HttpDelete("Poista/{poistettavanID}/{sahkopostiosoite}/{salasana}")]
        public async Task<ActionResult> PoistaKayttaja(int poistettavanID, string sahkopostiosoite, string salasana)
        {
            // Etsitään poistaja sähköpostin perusteella ja tarkistetaan täsmääkö salasana
            var poistaja = await _context.Kayttajat
                .FirstOrDefaultAsync(k => k.Sahkopostiosoite == sahkopostiosoite && k.Salasana == salasana);

            if (poistaja == null)
            {
                return Unauthorized("Virheellinen sähköposti tai salasana.");
            }

            // Tarkistetaan, onko poistajalla oikeus poistaa
            if (poistaja.Kayttajataso == "admin" || poistaja.Id == poistettavanID)
            {
                var poistettava = await _context.Kayttajat.FindAsync(poistettavanID);
                if (poistettava == null)
                {
                    return NotFound("Poistettavaa käyttäjää ei löytynyt.");
                }

                
                _context.Kayttajat.Remove(poistettava);
                await _context.SaveChangesAsync();

                return Ok("Käyttäjä poistettu.");
            }
            else
            {
                return Forbid("Poistamiseen ei ole oikeuksia.");
            }
        }

        [HttpPut("PaivitaTietoja")]
        public async Task<ActionResult<Kayttaja>> Paivita(Kayttaja p)
        {






            var tt = _context.Kayttajat.Find(p.Id);

            if (tt.Salasana.Equals(p.Salasana) && tt.Sahkopostiosoite.Equals(p.Sahkopostiosoite))
            {

                string? kuva = null;


                tt.Etunimi = p.Etunimi;
                tt.Sukunimi = p.Sukunimi;
                tt.Sahkopostiosoite = p.Sahkopostiosoite;
                tt.Kayttajataso = p.Kayttajataso;
                tt.Salasana = p.Salasana;
                tt.Nimimerkki = p.Nimimerkki;



                _context.Kayttajat.Update(tt);
                await _context.SaveChangesAsync();

                return Ok(tt);
            }
            else
            {

                Kayttaja k = new Kayttaja();

                return Ok(k);

            }


        }
        [HttpPut("Salasananpalautus")]
        public async Task<ActionResult> HaeUusiSalasana(Kayttaja p)
        {
            // Tarkistetaan, löytyykö käyttäjä
            var tt = _context.Kayttajat.Find(p.Id);
            if (tt == null)
            {
                return NotFound("Käyttäjää ei löydy!");
            }

            // Tarkistetaan sähköposti
            if (!tt.Sahkopostiosoite.Equals(p.Sahkopostiosoite))
            {
                return BadRequest("Sähköposti tai ID on väärin.");
            }

            // Lähetetään uusi salasana
            Salasananlahetys salasananlahetys = new Salasananlahetys();
            string? Uusisalasana = await salasananlahetys.LahetaSalasana(tt.Sahkopostiosoite);

            if (Uusisalasana == null)
            {
                return StatusCode(500, "Jotain meni pieleen!");
            }

            // Päivitetään salasana
            tt.Salasana = Uusisalasana;
            _context.Kayttajat.Update(tt);
            await _context.SaveChangesAsync();

            return Ok("Salasana vaihdettu.");
        }
    }
}    
