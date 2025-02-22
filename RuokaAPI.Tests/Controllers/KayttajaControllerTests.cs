using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokaAPI.Controllers;
using RuokaAPI.Properties.Model;
using RuokaAPI.Data;

namespace RuokaAPI.Tests
{
    public class KayttajaControllerTests
    {
        private readonly ruokaContext _context;
        private readonly KayttajaController _controller;


        //Testien teon aloitus, inmemory databasea käytetään näissä
        public KayttajaControllerTests()
        {
            var options = new DbContextOptionsBuilder<ruokaContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .EnableSensitiveDataLogging()  //Auttaa virheiden selvityksessä
                .Options;
            _context = new ruokaContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _controller = new KayttajaController(_context);
        }

        private Kayttaja LuoKayttaja()
        {
            return new Kayttaja
            {
                Id = 1,
                Sahkopostiosoite = "test@example.com",
                Salasana = "salasana123",
                Etunimi = "Matti",
                Sukunimi = "Meikäläinen",
                Nimimerkki = "Testaaja",
                Kayttajataso = "Peruskäyttäjä"
            };
        }

        [Fact]
        public async Task LisaaKayttaja_PalauttaaOkViestinKunKayttajaLisatty()
        {
            var kayttaja = LuoKayttaja();
            var result = await _controller.Lisaa(kayttaja);

            Assert.IsType<string>(result);
            Assert.Equal("Käyttäjä lisätty", result);
        }

        [Fact]
        public async Task HaeKaikkiKayttajat_PalauttaaListaKayttajista()
        {
            _context.Kayttajat.AddRange(new List<Kayttaja>
            {
                new Kayttaja { Id = 1, Sahkopostiosoite = "test1@example.com", Salasana = "salasana1", Etunimi = "Testi1", Sukunimi = "Käyttäjä1", Nimimerkki = "Testeri1", Kayttajataso = "admin" },
                new Kayttaja { Id = 2, Sahkopostiosoite = "test2@example.com", Salasana = "salasana2", Etunimi = "Testi2", Sukunimi = "Käyttäjä2", Nimimerkki = "Testeri2", Kayttajataso = "Peruskäyttäjä" }
            });

            await _context.SaveChangesAsync();

            var result = await _controller.HaeKayttajat(1, "salasana1", "test1@example.com");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task PoistaKayttaja_PalauttaaOkViestinKunKayttajaPoistettu()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.PoistaKayttaja(1, "test@example.com", "salasana123");

            

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task TallennaSuosikeiksi_PalauttaaOkViestiKunTallennusOnnistui()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var request = new SuosikitRequest
            {
                Kayttaja = kayttaja,
                Suosikitlista = new List<Suosikit> { new Suosikit { kayttajaID = 1, reseptiID = 10 } }
            };

            var result = await _controller.TallennaSuosikeiksi(request);

            Assert.Equal("Suosikit tallennettu onnistuneesti.", result);
        }

        [Fact]
        public async Task HaeSuosikkiReseptit_PalauttaaReseptilistanJosKayttajaLoytyy()
        {
            var kayttaja = LuoKayttaja();
            var resepti = new Resepti { Id = 1, Nimi = "Testi Resepti", Tekijäid = 1 };

            _context.Kayttajat.Add(kayttaja);
            _context.Reseptit.Add(resepti);
            await _context.SaveChangesAsync();

            var suosikki = new Suosikit { Id = 1, kayttajaID = kayttaja.Id, reseptiID = resepti.Id };
            _context.Suosikit.Add(suosikki);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeSuosikkiReseptit(kayttaja);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task PoistaSuosikit_PalauttaaOnnistumisviestinKunPoistoOnnistui()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var suosikki = new Suosikit { Id = 1, kayttajaID = 1, reseptiID = 10 };
            _context.Suosikit.Add(suosikki);
            await _context.SaveChangesAsync();

            var result = await _controller.PoistaSuosikit(kayttaja);

            Assert.Equal("Kaikki suosikit poistettu onnistuneesti.", result);
        }

        [Fact]
        public async Task PoistaSuosikit_PalauttaaVirheviestinJosKayttajaaEiLoydy()
        {
            var kayttaja = new Kayttaja
            {
                Sahkopostiosoite = "tuntematon@example.com",
                Salasana = "salasana123",
                Etunimi = "Testeri",
                Sukunimi = "Testaaja",
                Nimimerkki = "Tester123",
                Kayttajataso = "admin"
            };

            var result = await _controller.PoistaSuosikit(kayttaja);

            Assert.Equal("Käyttäjää ei löytynyt.", result);
        }

        [Fact]
        public async Task PoistaSuosikit_PalauttaaVirheviestinJosVaarasalasana()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var vääräKayttaja = new Kayttaja
            {
                Sahkopostiosoite = "test@example.com",
                Salasana = "vääräsalasana",
                Etunimi = "Matti",
                Sukunimi = "Meikäläinen",
                Nimimerkki = "Testaaja",
                Kayttajataso = "Peruskäyttäjä"
            };

            var result = await _controller.PoistaSuosikit(vääräKayttaja);

            Assert.Equal("Virheellinen salasana.", result);
        }
    }
}