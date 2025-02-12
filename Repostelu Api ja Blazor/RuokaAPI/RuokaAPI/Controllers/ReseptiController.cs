using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Resepti>> GetReseptiById(int id)
        {
            var resepti = await _repository.HaeReseptiAsync(id); // Adjust according to your DbSet name

            if (resepti == null)
            {
                return NotFound();
            }

            return Ok(resepti);
        }


        [HttpPost]
        public async Task<ActionResult<Resepti>> LisaaResepti(ReseptiDto reseptiDto)
        {
            var validator = new ReseptiValidator();
            var validationMessages = validator.Validate(reseptiDto);

            if (validationMessages.Count > 0)
            {
                return BadRequest(validationMessages);
            }

            var uusiResepti = await _repository.LisaaAsync(reseptiDto);
            return CreatedAtAction(nameof(HaeReseptit), new { id = uusiResepti.Id }, uusiResepti);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PaivitaResepti(int id, ReseptiDto reseptiDto)
        {
            if (!await _repository.OnOlemassaAsync(id))
            {
                return NotFound();
            }

            var validator = new ReseptiValidator();
            var validationMessages = validator.Validate(reseptiDto);

            if (validationMessages.Count > 0)
            {
                return BadRequest(validationMessages);
            }

            await _repository.PaivitaAsync(id, reseptiDto);
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
