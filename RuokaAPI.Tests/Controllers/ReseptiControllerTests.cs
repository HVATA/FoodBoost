using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Controllers;
using RuokaAPI.Data;
using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;
using RuokaAPI.Repositories;
using RuokaAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RuokaAPI.Tests.Controllers
{
    public class ReseptiControllerTests
    {
        private readonly ruokaContext _context;
        private readonly ReseptiController _sut;

        public ReseptiControllerTests()
        {
            _context = new DatabaseContextBuilder()
                .SeedDatabase()                
                .Build();  //luodaan konteksti, joka käyttää muistissa olevaa tietokantaa
            var repository = new ReseptiRepository(_context);//luodaan repository, joka käyttää kontekstia
            _sut = new ReseptiController(repository);//luodaan controller, joka käyttää repositorya
        }

        [Fact] //Testataan GetReseptit-metodi
        public async Task PitaisiPalauttaaKaikkiReseptit()
        {

            //Act
            var result = await _sut.HaeKaikkiReseptit(null, null);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);//resultin pitäisi olla OkObjectResult

            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var reseptit = okResult.Value as List<ReseptiResponse>;//muutetaan OkObjectResult IEnumerableksi
            Assert.Equal(2, reseptit.Count());
        }

        [Fact] //Testataan GetReseptiById-metodi
        public async Task PitaisiPalauttaaReseptiIdlla()
        {
            //Arrange
            var id = DatabaseContextBuilder.JulkinenReseptiTietokannassa.Id;

            //Act
            var result = await _sut.GetReseptiById(id);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var resepti = okResult.Value as ReseptiResponse;
            Assert.Equal(id, resepti.Id);
        }


        [Fact]//
        public async Task PitaisiPalauttaaOmatReseptit()
        {
            //Arrange

            var userId = DatabaseContextBuilder.YksityinenReseptiTietokannassa.Tekijäid;

            //Act
            var result = await _sut.HaeOmatReseptit(userId);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var reseptit = okResult.Value as List<ReseptiResponse>;
            Assert.NotNull(reseptit);//??
            Assert.All(reseptit, r => Assert.Equal(userId, r.TekijaId));

        }

        [Fact]
        public async Task PitaisiPalauttaaJulkisetReseptit()//palautta kun 
        {
            //Act
            var result = await _sut.HaeJulkisetReseptit(null, null);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var reseptit = okResult.Value as List<ReseptiResponse>;
            Assert.NotNull(reseptit);
            Assert.Single(reseptit);
            Assert.All(reseptit, r => Assert.Equal("Julkinen", r.Katseluoikeus, ignoreCase: true));
        }

        [Fact]
        public async Task PitaisiPalauttaaAinesosat()//pitäisi palauttaa ainesosat string-listauksena
        {
            var result = await _sut.HaeAinesosat();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var ainesosat = okResult.Value as List<string>;
            Assert.NotNull(ainesosat);
            Assert.Equal(2, ainesosat.Count());
            var odotetutAinesosat = new List<string> { "Kukkakaali", "Palsternakka" };
            Assert.Equal(odotetutAinesosat, ainesosat);
        }

        [Fact]
        public async Task PitaisiPalauttaaAvainsanat()//pitäisi palauttaa avainsanata string-listauksena
        {
            var result = await _sut.HaeAvainsanat();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var avainsanat = okResult.Value as List<string>;
            Assert.NotNull(avainsanat);
            Assert.Equal(3, avainsanat.Count());
        }

        [Fact] //Testataan GetReseptit-metodi
        public async Task PitaisiPalauttaaReseptitAinesosalla()
        {

            //Act
            var result = await _sut.HaeKaikkiReseptit(["Kukkakaali"], null);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);//resultin pitäisi olla OkObjectResult

            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var reseptit = okResult.Value as List<ReseptiResponse>;//muutetaan OkObjectResult IEnumerableksi
            Assert.Equal(2, reseptit.Count());
        }

        [Fact] //Testataan GetReseptit-metodi
        public async Task PitaisiPalauttaaReseptitAvainsanalla()
        {

            //Act
            var result = await _sut.HaeKaikkiReseptit(null, ["Aamupala"]);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);//resultin pitäisi olla OkObjectResult

            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var reseptit = okResult.Value as List<ReseptiResponse>;//muutetaan OkObjectResult IEnumerableksi
            Assert.Equal(2, reseptit.Count());
        }

        [Fact] //Testataan GetReseptit-metodi
        public async Task PitaisiPalauttaaReseptitAinesosallajaAvainsanalla()
        {

            //Act
            var result = await _sut.HaeKaikkiReseptit(["Kukkakaali"], ["Aamupala", "Herkku"]);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);//resultin pitäisi olla OkObjectResult

            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var reseptit = okResult.Value as List<ReseptiResponse>;//muutetaan OkObjectResult IEnumerableksi
            Assert.Equal(2, reseptit.Count());
        }

        [Fact]
        public async Task PitaisiLisataResepti()
        {
            //Arrange
            var builder = new ReseptiBuilder();
            var uusiReseptiRequest = builder.BuildRequest();


            //Act
            var result = await _sut.LisaaResepti(uusiReseptiRequest);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(3, _context.Reseptit.Count());

        }

        [Fact]
        public async Task PitaisiPaivittaaResepti()
        {
            //Arrange
            var id = 5;
            var builder = new ReseptiBuilder();
            var resepti = builder.WithId(id).Build();
            _context.Reseptit.Add(resepti);          
            _context.SaveChanges();
            var paivitettyReseptiRequest = builder.WithId(id).WithNimi("Paivitetty").BuildRequest();

            //Act
            var result = await _sut.PaivitaResepti(id, paivitettyReseptiRequest);

            Assert.IsType<NoContentResult>(result);
            var paivitettyResepti = await _context.Reseptit.FindAsync(id);
            Assert.Equal("Paivitetty", paivitettyResepti.Nimi);
        }

        [Fact]
        public async Task PitaisiPoistaaResepti()
        {
            //Arrange
            var id = DatabaseContextBuilder.JulkinenReseptiTietokannassa.Id;
            //Act
            var result = await _sut.PoistaResepti(id);

            Assert.IsType<NoContentResult>(result);
            var poistettuResepti = await _context.Reseptit.FindAsync(id);
            Assert.Null(poistettuResepti);
        }

        [Fact]
        public async Task PitaisiPalauttaaNotFoundJosReseptiaEiLoydy()
        {
            //Arrange
            var id = 1000;
            //Act
            var result = await _sut.GetReseptiById(id);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PitaisiPalauttaaBadRequestJosReseptiEiKelpaa()
        {
            //Arrange
            var builder = new ReseptiBuilder();
            var uusiReseptiRequest = builder.WithNimi(null).BuildRequest();

            //Act
            var result = await _sut.LisaaResepti(uusiReseptiRequest);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PitaisiLisataArvostelu()
        {
            //Arrange
            var id = DatabaseContextBuilder.JulkinenReseptiTietokannassa.Id;
            var builder = new ArvosteluBuilder();
            var arvosteluRequest = builder.Build();
            //Act
            var result = await _sut.LisaaArvostelu(id, arvosteluRequest);

            Assert.IsType<CreatedResult>(result);
            Assert.Equal(1, _context.Arvostelut.Count());
        }
    }
}
