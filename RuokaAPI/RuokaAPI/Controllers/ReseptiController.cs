using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Properties.Model;



namespace RuokaAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ReseptiController : ControllerBase
    {

        private readonly ruokaContext _context;

        public ReseptiController(ruokaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Resepti>> HaeReseptit()
        {
            
            


            var lista = await _context.Reseptit.ToListAsync();


            return lista;

        }


    }
}
