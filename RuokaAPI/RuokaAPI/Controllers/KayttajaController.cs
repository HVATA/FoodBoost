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

            lista = _context.Kayttajat.Where(x => x.Sahkopostiosoite == email).ToList();

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


        [HttpGet("/Haekaikki/{Id}/{Salasana}/{Sahkopostiosoite}")]
        public async Task<IEnumerable<Kayttaja>> HaeKayttajat(int Id, string Salasana, string Sahkopostiosoite)
        {

            //Vain admin voi hakea kaikki

            Kayttaja? p = await _context.Kayttajat.FindAsync(Id);


            List<Kayttaja> kayttajat = new List<Kayttaja>();

            string kayttäjätaso = "admin";


            if (p.Kayttajataso.Equals(kayttäjätaso) && p.Salasana.Equals(Salasana) && p.Sahkopostiosoite.Equals(Sahkopostiosoite))
            {




                kayttajat = await _context.Kayttajat.ToListAsync();


                return kayttajat;

            }
            else
            {
                kayttajat = null;

                return kayttajat;



            }

        }
        [HttpDelete("poista/{poistajanID}/{poistettavanID}/{salasana}/{sahkopostiosoite}")]
        async Task<ActionResult> PoistaKayttaja(int poistajanID, int poistettavanID, string salasana, string sahkopostiosoite)
        {


            var poistaja = _context.Kayttajat.Find(poistajanID);

            if (poistaja.Salasana.Equals(salasana) && poistaja.Sahkopostiosoite.Equals(sahkopostiosoite) && poistaja.Kayttajataso.Equals("admin"))
            {

                var x = _context.Kayttajat.Find(poistettavanID);

                _context.Kayttajat.Remove(x);
                await _context.SaveChangesAsync();

                return Ok(x);
            }
            else { return Ok("Poisto ei onnistunut!!!"); }



        }

        [HttpPut("PaivitaTietoja/{id}/{Salasana}/{Sahkoposti}")]
        public async Task<ActionResult<Kayttaja>> Paivita(int id, Kayttaja p, string Salasana, string Sahkoposti)
        {






            var tt = _context.Kayttajat.Find(id);

            if (tt.Salasana.Equals(Salasana) && tt.Sahkopostiosoite.Equals(Sahkoposti))
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
        [HttpPut("Salasananpalautus/{id}/{Sahkoposti}")]
        public async Task<ActionResult> HaeUusiSalasana(int id, string Sahkoposti)
        {

            Boolean k = false;

            string? Uusisalasana = null;


            var tt = _context.Kayttajat.Find(id);

            if (tt.Sahkopostiosoite.Equals(Sahkoposti))
            {

                Salasananlahetys salasananlahetys = new Salasananlahetys();
                Uusisalasana = await salasananlahetys.LahetaSalasana(Sahkoposti);




            }
            if (!tt.Sahkopostiosoite.Equals(Sahkoposti))
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
