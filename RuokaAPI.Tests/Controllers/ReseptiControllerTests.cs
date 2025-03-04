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
            var result = await _sut.HaeKaikkiReseptit(["Kukkakaali"], ["Aamupala","Herkku"]);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);//resultin pitäisi olla OkObjectResult

            var okResult = result.Result as OkObjectResult;//muutetaan Action result OkObjectResultiksi
            var reseptit = okResult.Value as List<ReseptiResponse>;//muutetaan OkObjectResult IEnumerableksi
            Assert.Equal(2, reseptit.Count());
        }

    }
}
