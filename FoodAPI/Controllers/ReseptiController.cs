using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodAPI.Data;
using FoodAPI.Dtos;
using FoodAPI.Properties.Model;
using FoodAPI.Repositories;
using System.Text.Json.Serialization;

namespace FoodAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // This controller handles all the operations related to recipes (Resepti),
    // including fetching, adding, updating, deleting recipes, and adding reviews (Arvostelu) to recipes.
    // Note: This class does not perform actual database queries. Instead, it delegates requests to the repository class.
    public class ReseptiController : ControllerBase
    {
        private readonly ReseptiRepository _repository;

        public ReseptiController(ReseptiRepository repository)
        {
            _repository = repository;
        }

        /////////////TESTI////////////////////////////////////////////////
        [HttpGet("TestiLista")]
        public async Task<ActionResult<IEnumerable<ReseptiListausTestiDto>>> HaeKaikkiReseptitTesti()
        {
            var reseptit = await _repository.HaeReseptitAsync(null, null, false);

            var dtoList = reseptit.Select(r => new ReseptiListausTestiDto
            {
                Id = r.Id,
                Nimi = r.Nimi,
                TekijaId = r.TekijaId,
                Katseluoikeus = r.Katseluoikeus ?? ""
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("KuvaTesti/{id}")]
        public async Task<ActionResult<string>> HaeKuvaTesti(int id)
        {
            var resepti = await _repository.HaeReseptiAsync(id); // k�yt� omaa metodia tai tee sellainen

            if (resepti == null || string.IsNullOrEmpty(resepti.Kuva1))
            {
                return NotFound("Kuvaa ei l�ytynyt.");
            }

            return Ok(resepti.Kuva1); // base64 string
        }
        ///////////////TESTI///////////////////////////////////////////////////
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Resepti>>> HaeKaikkiReseptit(
            [FromQuery] string[]? ainesosat, //Voi hakea tyhj�n� tai ainesosalla tai avainsanalla
            [FromQuery] string[]? avainsanat
            )
        {
            var reseptit = await _repository.HaeReseptitAsync(ainesosat, avainsanat, false);
            return Ok(reseptit);
        }

        [HttpGet("Julkiset")]
        public async Task<ActionResult<IEnumerable<Resepti>>> HaeJulkisetReseptit(
           [FromQuery] string[]? ainesosat, //Voi hakea tyhj�n� tai ainesosalla tai avainsanalla
           [FromQuery] string[]? avainsanat
           )
        {
            var reseptit = await _repository.HaeReseptitAsync(ainesosat, avainsanat, true);
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

        [HttpGet("omat/{userId}")]
        public async Task<ActionResult<IEnumerable<ReseptiResponse>>> HaeOmatReseptit ( int userId )
            {
            var reseptit = await _repository.HaeReseptitKayttajalleAsync(userId);
            if (reseptit == null || !reseptit.Any())
                {
                return NotFound("Ei l�ytynyt reseptej�.");
                }
            return Ok(reseptit);
            }

        [HttpGet("ainesosat")]
        public async Task<ActionResult<IEnumerable<string>>> HaeAinesosat()
        {
            var ainesosat = await _repository.HaeAinesosatAsync();
            var ainesosaNimet = ainesosat.Select(a => a.Nimi).ToList();
            return Ok(ainesosaNimet);
        }

        [HttpGet("avainsanat")]
        public async Task<ActionResult<IEnumerable<string>>> HaeAvainsanat()
        {
            var avainsanat = await _repository.HaeAvainsanatAsync();
            var avainsanalista = avainsanat.Select(a => a.Sana).ToList();
            return Ok(avainsanalista);
        }

        [HttpPost]
        public async Task<ActionResult<Resepti>> LisaaResepti(ReseptiRequest reseptiDto)
        {
            var validator = new ReseptiValidator();
            var validationMessages = validator.Validate(reseptiDto);

            if (validationMessages.Count > 0)
            {
                return BadRequest(validationMessages);
            }

            var uusiResepti = await _repository.LisaaAsync(reseptiDto);
            return CreatedAtAction(nameof(LisaaResepti), new { id = uusiResepti.Id }, uusiResepti);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PaivitaResepti(int id, ReseptiRequest reseptiDto)
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

        [HttpPost("{id}/arvostelut")]
        public async Task<IActionResult> LisaaArvostelu(int id, ArvosteluRequest request)
        {
            var validator = new ArvosteluValidator();
            var validationMessages = validator.Validate(request);

            if (validationMessages.Count > 0)
            {
                return BadRequest(validationMessages);
            }

            // Add the new Arvostelu to the database
            var lisaysOnnistui = await _repository.LisaaArvosteluAsync(id, request);
            if (!lisaysOnnistui)
            {
                return NotFound("Resepti not found.");
            }

            return Created();
        }
    }
}
