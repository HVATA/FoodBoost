using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Properties.Model;

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

        
        [HttpGet("/Haekaikki/{Id}")]
        public async Task<IEnumerable<Kayttaja>> HaeKayttajat(int Id)
        {

            //Vain admin voi hakea kaikki

            Kayttaja? p =await _context.Kayttajat.FindAsync(Id);


                List<Kayttaja> kayttajat = new List<Kayttaja>();

                string kayttäjätaso = "admin";


            if (p.Kayttajataso.Equals(kayttäjätaso)) { 



                
                    kayttajat = await _context.Kayttajat.ToListAsync();


                    return kayttajat;

               }
            else
            {
                kayttajat = null;

                return kayttajat;



            }

            }
        }
    }
