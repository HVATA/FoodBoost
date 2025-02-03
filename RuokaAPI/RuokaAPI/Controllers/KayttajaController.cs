using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Properties.Model;
using RuokaAPI.Services;
using BCrypt.Net;




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

            string? salasana =null;
            




            if (lista.Count == 0)
            {
                salasana = x.Salasana;
                string hash = BCrypt.Net.BCrypt.HashPassword(salasana);
                x.Salasana = hash;


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
            


         Kayttaja?    p = await _context.Kayttajat.Where (x => (x.Sahkopostiosoite==sahkopostiosoite)).FirstOrDefaultAsync();


            bool onHashattu = p.Salasana.StartsWith("$2a$") || p.Salasana.StartsWith("$2b$");


            if ((onHashattu && BCrypt.Net.BCrypt.Verify(salasana, p.Salasana)) || (!onHashattu && p.Salasana == salasana))
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
            if ((p != null && p.Kayttajataso.Equals(kayttajataso) &&
    (p.Salasana == salasana || (p.Salasana.StartsWith("$2a$") || p.Salasana.StartsWith("$2b$")) && BCrypt.Net.BCrypt.Verify(salasana, p.Salasana)) &&
    p.Sahkopostiosoite == sahkopostiosoite))
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
            // Etsitään poistaja sähköpostin perusteella
            var poistaja = await _context.Kayttajat
                .FirstOrDefaultAsync(k => k.Sahkopostiosoite == sahkopostiosoite);

            if (poistaja == null)
            {
                return Unauthorized("Virheellinen sähköposti.");
            }

            // Tarkistetaan, onko salasana hajautettu ja validoidaan se
            bool onHashattu = poistaja.Salasana.StartsWith("$2a$") || poistaja.Salasana.StartsWith("$2b$");
            bool salasanaOk = onHashattu ? BCrypt.Net.BCrypt.Verify(salasana, poistaja.Salasana) : (poistaja.Salasana == salasana);

            if (!salasanaOk)
            {
                return Unauthorized("Virheellinen salasana.");
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

            bool onHashattu = tt.Salasana.StartsWith("$2a$") || tt.Salasana.StartsWith("$2b$");
            bool salasanaOk = onHashattu ? BCrypt.Net.BCrypt.Verify(p.Salasana, tt.Salasana) : (tt.Salasana == p.Salasana);

            if (salasanaOk && tt.Sahkopostiosoite.Equals(p.Sahkopostiosoite))
            {

                string? kuva = null;




                tt.Etunimi = p.Etunimi;
                tt.Sukunimi = p.Sukunimi;
                tt.Sahkopostiosoite = p.Sahkopostiosoite;
                tt.Kayttajataso = p.Kayttajataso;
                tt.Salasana = BCrypt.Net.BCrypt.HashPassword(p.Salasana);
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

            tt.Salasana = BCrypt.Net.BCrypt.HashPassword(Uusisalasana);
            _context.Kayttajat.Update(tt);
            await _context.SaveChangesAsync();

            return Ok("Salasana vaihdettu.");
       
        }

        [HttpPut("LahetaResepti/{reseptiId}/{vastaanottajanEmail}")]
        public async Task<IActionResult> LahetaResepti(int reseptiId, string vastaanottajanEmail, [FromBody] Kayttaja o)
        {
            try
            {
                // Tarkistetaan käyttäjä kannasta
                var kayttaja = await _context.Kayttajat.Where(x => x.Sahkopostiosoite == o.Sahkopostiosoite).FirstOrDefaultAsync();

                if (kayttaja == null)
                {
                    return Unauthorized("Käyttäjää ei löytynyt.");
                }

                // Tarkistetaan salasana (hashattu tai ei)
                string salasana = o.Salasana;
                bool onHashattu = kayttaja.Salasana.StartsWith("$2a$") || kayttaja.Salasana.StartsWith("$2b$");
                bool salasanaOk = onHashattu ? BCrypt.Net.BCrypt.Verify(salasana, kayttaja.Salasana) : (kayttaja.Salasana == salasana);

                if (!salasanaOk)
                {
                    return Unauthorized("Virheellinen salasana.");
                }

                // Haetaan resepti tietokannasta ID:n perusteella
                var resepti = await _context.Reseptit
                    .Include(r => r.Ainesosat) // Haetaan myös ainesosat
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
                foreach (var ainesosa in resepti.Ainesosat)
                {
                    reseptiString += $"- {ainesosa.Nimi})\n";
                }

                reseptiString += $"\nAvainsanat:\n";
                foreach (var avainsana in resepti.Avainsanat)
                {
                    reseptiString += $"- {avainsana.Id}, ({avainsana.Sana})\n";
                }

                reseptiString += "\nKuvat:\n";
                if (!string.IsNullOrEmpty(resepti.Kuva1)) reseptiString += $"Kuva 1: {resepti.Kuva1}\n";
                if (!string.IsNullOrEmpty(resepti.Kuva2)) reseptiString += $"Kuva 2: {resepti.Kuva2}\n";
                if (!string.IsNullOrEmpty(resepti.Kuva3)) reseptiString += $"Kuva 3: {resepti.Kuva3}\n";
                if (!string.IsNullOrEmpty(resepti.Kuva4)) reseptiString += $"Kuva 4: {resepti.Kuva4}\n";
                if (!string.IsNullOrEmpty(resepti.Kuva5)) reseptiString += $"Kuva 5: {resepti.Kuva5}\n";
                if (!string.IsNullOrEmpty(resepti.Kuva6)) reseptiString += $"Kuva 6: {resepti.Kuva6}\n";

                // Lähetetään sähköposti
                ReseptinLaheys reseptinlahetys = new ReseptinLaheys();

                bool lahetysOk = await reseptinlahetys.LahetaResepti(vastaanottajanEmail, "Jaettu resepti", reseptiString);

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

        [HttpPost("Lisaasuokkkiehin/{sahkopostiosoite}")]
        public async Task<string> TallennaSuosikeiksi([FromBody] SuosikitRequest request, string sahkopostiosoite)
        {
            // Tarkistetaan käyttäjä kannasta
            Kayttaja? p = await _context.Kayttajat.Where(x => x.Sahkopostiosoite == sahkopostiosoite).FirstOrDefaultAsync();

            if (p == null)
            {
                return "Käyttäjää ei löytynyt.";
            }

            // Tarkistetaan salasana (hashattu tai ei)
            string ssana = p.Salasana;
            bool onHashattu = p.Salasana.StartsWith("$2a$") || p.Salasana.StartsWith("$2b$");

            bool salasanaOk = onHashattu ? BCrypt.Net.BCrypt.Verify(request.Kayttaja.Salasana, p.Salasana) : (p.Salasana == request.Kayttaja.Salasana);

            if (!salasanaOk)
            {
                return "Virheellinen salasana.";
            }

            if (salasanaOk)
            {
                // Lisätään reseptit suosikkeihin
                foreach (int ReseptiId in request.Idlista)
                {
                    _context.Suosikit.Add(new Suosikit
                    {
                        kayttajaID = p.Id,
                        reseptiID = ReseptiId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return "Suosikit tallennettu onnistuneesti.";
        }



    }
}    
