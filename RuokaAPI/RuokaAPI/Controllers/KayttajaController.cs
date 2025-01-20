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

        [HttpGet]
        public async Task<IEnumerable<Kayttaja>> HaeKäyttajat()
        {




            var lista = await _context.Kayttajaset.ToListAsync();


            return lista;

        }


    }
}
