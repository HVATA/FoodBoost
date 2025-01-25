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

        [HttpPost]
        public async Task<string> LisaaKayttaja(Kayttaja x)
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

        [HttpGet("Tunnistautumistiedot")]
        public async Task<ActionResult<Kayttaja>> HaeKayttaja([FromQuery] string salasana,
    [FromQuery] string sahkopostiosoite




            )
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


        [HttpGet("Haekaikki")]
        public async Task<ActionResult<IEnumerable<Kayttaja>>> HaeKayttajat(
    [FromQuery] int id,
    [FromQuery] string salasana,
    [FromQuery] string sahkopostiosoite)
        {
            // Etsitään käyttäjä tietokannasta
            Kayttaja? p = await _context.Kayttajat.FindAsync(id);

            if (p == null)
            {
                return NotFound("Käyttäjää ei löytynyt.");
            }

            string kayttajataso = "admin";

            // Tarkistetaan, että käyttäjä on admin ja tunnistetiedot ovat oikein
            if (p.Kayttajataso.Equals(kayttajataso) && p.Salasana == salasana && p.Sahkopostiosoite == sahkopostiosoite)
            {
                var kayttajat = await _context.Kayttajat.ToListAsync();
                return Ok(kayttajat);
            }

            return Unauthorized("Käyttäjällä ei ole oikeuksia.");
        }

        [HttpDelete("Poista/{poistettavanID}")]
       public async Task<ActionResult> PoistaKayttaja(Kayttaja k,int poistettavanID)
        {


            var poistaja = _context.Kayttajat.Find(k.Id);

            if (poistaja.Salasana.Equals(k.Salasana) && poistaja.Sahkopostiosoite.Equals(k.Sahkopostiosoite) && poistaja.Kayttajataso.Equals("admin")||poistaja.Id==poistettavanID&&poistaja.Sahkopostiosoite==k.Sahkopostiosoite&&poistaja.Salasana==k.Salasana)
            {

                var x = _context.Kayttajat.Find(poistettavanID);

                _context.Kayttajat.Remove(x);
                await _context.SaveChangesAsync();

                return Ok(x);
            }
            else { return Ok("Poisto ei onnistunut!!!"); }



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

            Boolean k = false;

            string? Uusisalasana = null;


            var tt = _context.Kayttajat.Find(p.Id);

            if (tt.Sahkopostiosoite.Equals(p.Sahkopostiosoite))
            {

                Salasananlahetys salasananlahetys = new Salasananlahetys();
                Uusisalasana = await salasananlahetys.LahetaSalasana(tt.Sahkopostiosoite);




            }
            if (!tt.Sahkopostiosoite.Equals(p.Sahkopostiosoite))
            {

                return Ok("Sähköposti tai Id on väärin");

            }

            if (Uusisalasana != null)
            {

                tt.Salasana = Uusisalasana;



                _context.Kayttajat.Update(tt);
                await _context.SaveChangesAsync();

                return Ok("Salasana vaihdettu");


            }

            else {


                return Ok("Jotain meni pieleen!!!");
            
            
            }

            
        
        
        
        }
    }
}    
