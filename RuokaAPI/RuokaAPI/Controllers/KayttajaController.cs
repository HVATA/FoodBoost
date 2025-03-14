using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Properties.Model;
using RuokaAPI.Services;
using BCrypt.Net;
using Azure.Core;
using System.Globalization;




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




            List<Kayttaja> lista2 = new List<Kayttaja>();

            string email = x.Sahkopostiosoite;

            lista = _context.Kayttajat.Where(x => x.Sahkopostiosoite == email).ToList();

            string? salasana = null;


            //laitetaan tähän tuon sähköpostin tarkastuksen lisäksi nimimerkin tarkastus
            // string? nimimerkki = x.Nimimerkki;

            //Voiko olla että esim. 3 nimimerkillä Make99 arvostelee samaa reseptiä mutta eri emaileilla jos ei näytetä julkisesti sähköpostiosoitetta?


            //  lista2= _context.Kayttajat.Where(x=>x.Nimimerkki == nimimerkki).ToList();
            //== 0&&lista2.Count==0

            if (lista.Count == 0)
            {


                _context.Kayttajat.Add(x);
                await _context.SaveChangesAsync();

                return "Käyttäjä lisätty";
            }

            /*  if (lista2.Count > 0)
              {

                  return "Nimimerkki on jo käytössä!!";



             }
              */

            else
            {

                return "Sähköposti on jo käytössä!!!";


            }



        }


        [HttpGet("Tunnistautumistiedot/{salasana}/{sahkopostiosoite}")]
        public async Task<ActionResult<Kayttaja>> HaeKayttaja(string salasana, string sahkopostiosoite)
        {
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null)
            {
                return NotFound("Käyttäjää ei löytynyt.");
            }

            if (p.Salasana == salasana)
            {
                return Ok(p);
            }
            else
            {
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
            if (p != null && p.Kayttajataso.Equals(kayttajataso) && p.Salasana == salasana && p.Sahkopostiosoite == sahkopostiosoite)


            {
                kayttajat = await _context.Kayttajat.ToListAsync();
                return kayttajat;
            }

            else
            {

                return kayttajat;
            }
        }

        [HttpDelete("Poista/{poistettavanID}/{sahkopostiosoite}/{salasana}")]
        public async Task<ActionResult> PoistaKayttaja(int poistettavanID, string sahkopostiosoite, string salasana)
        {
            // Etsitään poistaja sähköpostin perusteella
            var poistaja = await _context.Kayttajat
                .FirstOrDefaultAsync(k => k.Sahkopostiosoite == sahkopostiosoite);

            if (poistaja == null)
            {
                return Unauthorized("Virheellinen sähköposti.");
            }

            // Tarkistetaan, onko salasana hajautettu ja validoidaan se


            if (poistaja.Salasana != salasana)
            {
                return Unauthorized("Virheellinen salasana.");
            }

            // Tarkistetaan, onko poistajalla oikeus poistaa ja oman Käyttäjän saa poistaa tässä versiossa vaikka ei olisi admin
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
        public async Task<ActionResult<Kayttaja>> Paivita(Kayttaja k)
        {
            Kayttaja? tt = _context.Kayttajat.Find(k.Id);

            if (tt != null)
            {


                tt.Etunimi = k.Etunimi;
                tt.Sukunimi = k.Sukunimi;
                tt.Sahkopostiosoite = k.Sahkopostiosoite;
                tt.Kayttajataso = k.Kayttajataso;
                tt.Salasana = k.Salasana;
                tt.Nimimerkki = k.Nimimerkki;



                _context.Kayttajat.Update(tt);
                await _context.SaveChangesAsync();

                return Ok("Onnistuu");
            }
            else
            {

                return BadRequest();

            }
        }


        //salasanan manuaaliseen vaihtoon käyttää KayttajaDTO luokkaa

        [HttpPut("Muutasalasana")]
        public async Task<ActionResult<Kayttaja>> MuutaSalasana(KayttajaDTO k)
        {
            Kayttaja? tt = _context.Kayttajat.Find(k.Id);


            string? uusisalasana = null;

            uusisalasana = k.Uusisalasana;


            if (tt != null && tt.Salasana == k.Salasana)
            {


                tt.Etunimi = k.Etunimi;
                tt.Sukunimi = k.Sukunimi;
                tt.Sahkopostiosoite = k.Sahkopostiosoite;
                tt.Kayttajataso = k.Kayttajataso;
                if (!string.IsNullOrEmpty(uusisalasana))
                {

                    tt.Salasana = k.Uusisalasana;
                }


                tt.Nimimerkki = k.Nimimerkki;



                _context.Kayttajat.Update(tt);
                await _context.SaveChangesAsync();

                return Ok("Onnistuu");
            }
            else
            {

                return BadRequest();

            }


        }


        //pyydetään sähköpostitse uutta salasanaa

        [HttpGet("Salasananpalautus/{email}")]
        public async Task<ActionResult> HaeUusiSalasana(string email)
        {
            // Tarkistetaan, löytyykö käyttäjä
            Kayttaja? tt = _context.Kayttajat.Where(x => x.Sahkopostiosoite == email).FirstOrDefault();
            if (tt == null)
            {
                return NotFound("Käyttäjää ei löydy!");
            }

            // Tarkistetaan sähköposti
            if (!tt.Sahkopostiosoite.Equals(email))
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

        [HttpPut("LahetaResepti/{reseptiId}/{vastaanottajanEmail}")]
        public async Task<IActionResult> LahetaResepti(int reseptiId, string vastaanottajanEmail, [FromBody] Kayttaja o)
        {
            string? kuvaString = "";

            try
            {
                // Tarkistetaan käyttäjä kannasta
                var kayttaja = await _context.Kayttajat.Where(x => x.Sahkopostiosoite == o.Sahkopostiosoite).FirstOrDefaultAsync();

                if (kayttaja == null)
                {
                    return Unauthorized("Käyttäjää ei löytynyt.");
                }



                if (kayttaja.Salasana != o.Salasana)
                {
                    return Unauthorized("Virheellinen salasana.");
                }

                // Haetaan resepti tietokannasta ID:n perusteella
                var resepti = await _context.Reseptit
                    .Include(r => r.AinesosanMaara).ThenInclude(am => am.Ainesosa) // Haetaan myös ainesosat
                    .Include(r => r.Avainsanat) // Haetaan myös avainsanat
                    .FirstOrDefaultAsync(r => r.Id == reseptiId);

                if (resepti == null)
                {
                    return NotFound("Reseptiä ei löytynyt.");
                }

                // Rakennetaan resepti stringiksi
                string reseptiString = $"Henkilö nimimerkillä: {kayttaja.Nimimerkki} halusi jakaa reseptin kanssasi BoostFood verkkopalvelusta \n\n" +
                                       $"Resepti: {resepti.Nimi}\n\n" +
                                       $"Valmistuskuvaus: {resepti.Valmistuskuvaus ?? "Ei kuvausta"}\n\n" +
                                       $"Ainesosat:\n";

                // Lisätään ainesosat listasta
                foreach (var ainesosa in resepti.AinesosanMaara.Select(a => a.Ainesosa))
                {
                    reseptiString += $"- {ainesosa.Nimi}\n";
                }

                reseptiString += $"\nAvainsanat:\n";
                foreach (var avainsana in resepti.Avainsanat)
                {
                    reseptiString += $"- {avainsana.Id}, ({avainsana.Sana})\n";
                }


                if (!string.IsNullOrEmpty(resepti.Kuva1)) //reseptiString += $"Kuva 1: {resepti.Kuva1}\n";
                {

                    kuvaString = resepti.Kuva1;
                }

                // Lähetetään sähköposti
                ReseptinLaheys reseptinlahetys = new ReseptinLaheys();

                bool lahetysOk = await reseptinlahetys.LahetaResepti(vastaanottajanEmail, "Jaettu resepti", reseptiString, kuvaString);

                if (!lahetysOk)
                {
                    return StatusCode(500, "Sähköpostin lähetys epäonnistui.");
                }

                return Ok("Resepti lähetetty sähköpostitse!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Virhe: {ex.Message}");
            }
        }

        [HttpPut("TallennaSuosikeiksiListallinen")]
        public async Task<string> TallennaSuosikeiksi([FromBody] SuosikitRequest request)
        {
            List<Suosikit> suosikkilista = request.Suosikitlista;
            Kayttaja kayttaja = request.Kayttaja;

            // Tarkistetaan käyttäjä kannasta
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == kayttaja.Sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null)
            {
                return "Käyttäjää ei löytynyt.";
            }


            if (p.Salasana != kayttaja.Salasana)
            {
                return "Virheellinen salasana.";
            }

            if (p.Salasana == kayttaja.Salasana)
            {
                // Haetaan käyttäjän aiemmat suosikit kannasta
                var templista = await _context.Suosikit
                    .Where(x => x.kayttajaID == p.Id)
                    .ToListAsync();

                foreach (var x in suosikkilista)
                {
                    // Tarkistetaan, onko sama resepti jo suosikeissa käyttäjän ID:llä ja reseptiID:llä
                    if (!templista.Any(s => s.kayttajaID == x.kayttajaID && s.reseptiID == x.reseptiID))
                    {
                        _context.Suosikit.Add(x);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return "Suosikit tallennettu onnistuneesti.";
        }

        [HttpPut("Haesuosikkireseptit")]
        public async Task<List<Resepti>> HaeSuosikkiReseptit(Kayttaja k)
        {

            List<Suosikit>? suosikit = new List<Suosikit>();

            List<Resepti> Suosikkireseptit = new List<Resepti>();


            Kayttaja? p = await _context.Kayttajat.Where(x => x.Sahkopostiosoite == k.Sahkopostiosoite).FirstOrDefaultAsync();

            if (p == null)
            {
                Suosikkireseptit = null;
                return Suosikkireseptit;
            }


            if (p.Salasana != k.Salasana)
            {
                return Suosikkireseptit = null;
            }

            if (p.Salasana == k.Salasana)
            {
                suosikit = await _context.Suosikit.Where(x => x.kayttajaID == p.Id).ToListAsync();

                foreach (var x in suosikit)
                {

                    Resepti? resepti = await _context.Reseptit
                                            .Include(r => r.AinesosanMaara).ThenInclude(am => am.Ainesosa)
                                            .Include(r => r.Avainsanat)
                                            .FirstOrDefaultAsync(r => r.Id == x.reseptiID);

                    Suosikkireseptit.Add(resepti);
                }

            }
            return Suosikkireseptit;
        }

        [HttpDelete("PoistaSuosikit")]
        public async Task<string> PoistaSuosikit([FromBody] Kayttaja request)
        {
            // Etsitään käyttäjä tietokannasta
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == request.Sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null)
            {
                return "Käyttäjää ei löytynyt.";
            }


            if (p.Salasana != request.Salasana)
            {
                return "Virheellinen salasana.";
            }

            // Poistetaan kaikki käyttäjän suosikit
            var suosikit = _context.Suosikit.Where(x => x.kayttajaID == p.Id);
            _context.Suosikit.RemoveRange(suosikit);

            await _context.SaveChangesAsync();
            return "Kaikki suosikit poistettu onnistuneesti.";
        }



        [HttpPost("Lisaasuosikki")]
        public async Task<string> LisaaSuosikki([FromBody] SuosikkiMuokkaus request)
        {
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == request.Kayttaja.Sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null) return "Käyttäjää ei löytynyt.";



            if (p.Salasana != request.Kayttaja.Salasana)
            {

                return "Virheellinen salasana.";
            }

            // Tarkistetaan, onko resepti jo suosikeissa
            bool onJoSuosikissa = await _context.Suosikit
                .AnyAsync(x => x.kayttajaID == p.Id && x.reseptiID == request.suosikki.reseptiID);

            if (onJoSuosikissa)
            {
                return "Resepti on jo suosikeissa.";
            }


            // Lisätään uusi suosikki
            _context.Suosikit.Add(new Suosikit
            {
                kayttajaID = p.Id,
                reseptiID = request.suosikki.reseptiID
            });

            await _context.SaveChangesAsync();
            return "Resepti lisätty suosikkeihin.";
        }



        [HttpDelete("Poistasuosikki")]
        public async Task<string> PoistaSuosikki([FromBody] SuosikkiMuokkaus? request)
        {
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == request.Kayttaja.Sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null) return "Käyttäjää ei löytynyt.";


            if (p.Salasana != request.Kayttaja.Salasana)
            {

                return "Virheellinen salasana.";

            }

            // Etsitään suosikki
            var suosikki = await _context.Suosikit
                .Where(x => x.kayttajaID == p.Id && x.reseptiID == request.suosikki.reseptiID)
                .FirstOrDefaultAsync();

            if (suosikki == null) return "Resepti ei ole suosikeissa.";

            // Poistetaan suosikki
            _context.Suosikit.Remove(suosikki);
            await _context.SaveChangesAsync();

            return "Resepti poistettu suosikeista.";
        }



        //Poistetaan kaikki käyttäjän tuotokset ja käyttäjä jos tekijä tai palvelun pitäjä sitä vaatii
        [HttpDelete("Poistatuotokset/{PoistettavanID}")]
        public async Task<string> PoistaKaiikiTuotokset(int PoistettavanID, [FromBody] Kayttaja poistaja)
        {
            Kayttaja? p = await _context.Kayttajat
                .Where(x => x.Sahkopostiosoite == poistaja.Sahkopostiosoite)
                .FirstOrDefaultAsync();

            if (p == null) return "Käyttäjää ei löytynyt.";


            if (p.Salasana != poistaja.Salasana)
            {

                return "Virheellinen salasana.";

            }

            if (p != null && p.Kayttajataso == "admin")
            {

                // Etsitään suosikit
                List<Suosikit>? suosikit = await _context.Suosikit
                    .Where(x => x.kayttajaID == PoistettavanID).ToListAsync();

                //ja poistetaan suosikit

                if (suosikit.Count > 0)
                {

                    _context.Suosikit.RemoveRange(suosikit);

                    await _context.SaveChangesAsync();


                   

                }
                //etsitään reseptit

                List<Resepti>? lista = await _context.Reseptit.Where(x => x.Tekijäid == PoistettavanID).ToListAsync();

                if (lista.Count > 0)
                {
                    //  Ja poistetaan ne
                    _context.Reseptit.RemoveRange(lista);


                    await _context.SaveChangesAsync();

                   

                }

                //poistetaan arvostelut

                List<Arvostelu>? arvostelut = await _context.Arvostelut.Where(x => x.ArvostelijanId == PoistettavanID).ToListAsync();

                if(arvostelut.Count > 0)
                {

                    _context.RemoveRange(arvostelut);
                    await _context.SaveChangesAsync();

                   

                }

                //poistetaan itse henkilö

                List<Kayttaja>? henkilo =await _context.Kayttajat.Where(x => x.Id == PoistettavanID).ToListAsync();

                if(henkilo.Count > 0)
                {

                    _context.RemoveRange(henkilo);
                    await _context.SaveChangesAsync();

             

                }

                return "Kaikki löydetyt käyttäjän tuotokset ja käyttäjä on poistettu palvelusta!!";

            }
            else
            {


                return "Käyttäjän tuotoksia ei löytynyt tai poistajalla ei ollut oikeuksia!!";



            }
        
        
        
        
        }
    }
}