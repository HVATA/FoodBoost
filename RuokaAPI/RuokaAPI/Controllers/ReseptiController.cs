using Microsoft.AspNetCore.Mvc;
using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;
using RuokaAPI.Repositories;

namespace RuokaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReseptiController : ControllerBase
    {
        private readonly ReseptiRepository _repository;

        public ReseptiController(ReseptiRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resepti>>> HaeReseptit(
            [FromQuery] string[]? ainesosat,
            [FromQuery] string[]? avainsanat)
        {
            var reseptit = await _repository.HaeReseptitAsync(ainesosat, avainsanat);
            return Ok(reseptit);
        }


        [HttpPost]
        public async Task<ActionResult<Resepti>> LisaaResepti(ReseptiDto reseptiDto)
        {
            var uusiResepti = await _repository.LisaaAsync(reseptiDto);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PaivitaResepti(int id, ReseptiDto resepti)
        {
            if (!await _repository.OnOlemassaAsync(id))
            {
                return NotFound();
            }

            await _repository.PaivitaAsync(id, resepti);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> PoistaResepti(int id)
        {
            if (!await _repository.OnOlemassaAsync(id))
            {
                return NotFound();
            }

            await _repository.PoistaAsync(id);
            return NoContent();
        }
    }
}
