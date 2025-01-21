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
        public async Task<ActionResult<IEnumerable<Resepti>>> HaeReseptit()
        {
            var reseptit = await _repository.HaeKaikkiAsync();
            return Ok(reseptit);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resepti>> HaeResepti(int id)
        {
            var resepti = await _repository.HaeIdllaAsync(id);
            if (resepti == null)
            {
                return NotFound();
            }
            return Ok(resepti);
        }

        [HttpPost]
        public async Task<ActionResult<Resepti>> LisaaResepti(ReseptiDto reseptiDto)
        {
            var uusiResepti = await _repository.LisaaAsync(reseptiDto);
            return CreatedAtAction(nameof(HaeResepti), new { id = uusiResepti.Id }, uusiResepti);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PaivitaResepti(int id, Resepti resepti)
        {
            if (id != resepti.Id)
            {
                return BadRequest();
            }

            if (!await _repository.OnOlemassaAsync(id))
            {
                return NotFound();
            }

            await _repository.PaivitaAsync(resepti);
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
