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
            _context.Kayttajat.RemoveRange(_context.Kayttajat);
            _context.Reseptit.RemoveRange(_context.Reseptit);
            _context.Suosikit.RemoveRange(_context.Suosikit);
            await _context.SaveChangesAsync();

            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var resepti = new Resepti { Id = new Random().Next(1000, 2000), Nimi = "Testi Resepti", Tekijäid = kayttaja.Id };
            _context.Reseptit.Add(resepti);
            await _context.SaveChangesAsync();

            var suosikki = new Suosikit { Id = new Random().Next(2000, 3000), kayttajaID = kayttaja.Id, reseptiID = resepti.Id };
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
                Kayttajataso = "käyttäjä"
            };

            var result = await _controller.PoistaSuosikit(vääräKayttaja);

            Assert.Equal("Virheellinen salasana.", result);
        }

        [Fact]
        public async Task LisaaKayttaja_PalauttaaVirheenJosSahkopostiOnJoKaytossa()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.Lisaa(kayttaja);

            Assert.Equal("Sähköposti on jo käytössä!!!", result);
        }

        [Fact]
        public async Task Paivita_PalauttaaBadRequestJosKayttajaaEiLoydy()
        {
            var kayttaja = LuoKayttaja();
            kayttaja.Id = 999; // Käyttäjä, jota ei ole tietokannassa

            var result = await _controller.Paivita(kayttaja);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task MuutaSalasana_PalauttaaBadRequestJosVanhaSalasanaVaara()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var dto = new KayttajaDTO { Id = kayttaja.Id, Salasana = "vaara123", Uusisalasana = "uusisalasana" };
            var result = await _controller.MuutaSalasana(dto);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task HaeUusiSalasana_PalauttaaNotFoundJosSahkopostiaEiLoydy()
        {
            var result = await _controller.HaeUusiSalasana("tuntematon@example.com");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task LahetaResepti_PalauttaaUnauthorizedJosKayttajaaEiLoydy()
        {
            var result = await _controller.LahetaResepti(1, "vastaanottaja@example.com", new Kayttaja { Sahkopostiosoite = "tuntematon@example.com", Salasana = "salasana" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task LisaaSuosikki_PalauttaaVirheenJosSuosikkiOnJoLisatty()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var request = new SuosikkiMuokkaus
            {
                Kayttaja = kayttaja,
                suosikki = new Suosikit { kayttajaID = kayttaja.Id, reseptiID = 1 }
            };

            // Lisätään ensimmäinen suosikki normaalisti metodin kautta
            await _controller.LisaaSuosikki(request);

            // Yritetään lisätä sama suosikki uudestaan
            var result = await _controller.LisaaSuosikki(request);

            Assert.Equal("Resepti on jo suosikeissa.", result);
        }

        [Fact]
        public async Task PoistaSuosikki_PalauttaaVirheenJosReseptiEiOleSuosikeissa()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var request = new SuosikkiMuokkaus
            {
                Kayttaja = new Kayttaja
                {
                    Sahkopostiosoite = kayttaja.Sahkopostiosoite,
                    Salasana = "salasana123" // Oikea salasana
                },
                suosikki = new Suosikit { kayttajaID = kayttaja.Id, reseptiID = 99 }
            };

            var result = await _controller.PoistaSuosikki(request);

            Assert.Equal("Virheellinen salasana.", result);
        }

        [Fact]
        public async Task HaeKayttaja_PalauttaaKayttajanJosTunnistautuminenOnnistuu()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeKayttaja("salasana123", "test@example.com");

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task HaeKayttaja_PalauttaaNotFoundJosKayttajaaEiLoydy()
        {
            var result = await _controller.HaeKayttaja("salasana123", "tuntematon@example.com");

            Assert.NotNull(result);
            if (result == null)
            {
                throw new Exception("HaeKayttaja palautti null-arvon.");
            }

            Assert.NotNull(result.Result);
            if (result.Result == null)
            {
                throw new Exception("Result on null HaeKayttaja-palautuksessa.");
            }

            Assert.IsType<NotFoundObjectResult>(result.Result);

            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            if (notFoundResult == null)
            {
                throw new Exception("NotFoundObjectResult on null.");
            }

            Assert.NotNull(notFoundResult.Value);
            Assert.Equal("Käyttäjää ei löytynyt.", notFoundResult.Value?.ToString());
        }


        [Fact]
        public async Task HaeKayttaja_PalauttaaKayttajanJosTiedotOikein()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeKayttaja("salasana123", "test@example.com");

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsType<Kayttaja>(okResult.Value);
        }



        [Fact]
        public async Task HaeUusiSalasana_PalauttaaOkJosKayttajaLoytyy()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeUusiSalasana(kayttaja.Sahkopostiosoite);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task HaeUusiSalasana_PalauttaaNotFoundJosKayttajaaEiLoydy()
        {
            var result = await _controller.HaeUusiSalasana("tuntematon@example.com");

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal("Käyttäjää ei löydy!", notFoundResult.Value);
        }

        [Fact]
        public async Task LahetaResepti_PalauttaaOkJosLahetysOnnistui()
        {
            var kayttaja = LuoKayttaja();
            var resepti = new Resepti { Id = 1, Nimi = "Testi Resepti", Tekijäid = 1 };

            _context.Kayttajat.Add(kayttaja);
            _context.Reseptit.Add(resepti);
            await _context.SaveChangesAsync();

            var result = await _controller.LahetaResepti(1, "vastaanottaja@example.com", kayttaja);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task LahetaResepti_PalauttaaNotFoundJosReseptiaEiLoydy()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var missingReseptiId = 999;
            var result = await _controller.LahetaResepti(missingReseptiId, "vastaanottaja@example.com", kayttaja);

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
        }



        [Fact]
        public async Task HaeSuosikkiReseptit_PalauttaaTyhjanListanJosEiSuosikkeja()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeSuosikkiReseptit(kayttaja);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HaeSuosikkiReseptit_PalauttaaVirheenJosKayttajaaEiLoydy()
        {
            var tuntematonKayttaja = new Kayttaja { Sahkopostiosoite = "tuntematon@example.com", Salasana = "salasana123" };

            var result = await _controller.HaeSuosikkiReseptit(tuntematonKayttaja);

            Assert.Null(result);
        }

        [Fact]
        public async Task HaeSuosikkiReseptit_PalauttaaKaikkiSuosikitJosUseita()
        {
            _context.Kayttajat.RemoveRange(_context.Kayttajat);
            _context.Reseptit.RemoveRange(_context.Reseptit);
            _context.Suosikit.RemoveRange(_context.Suosikit);
            await _context.SaveChangesAsync();

            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var resepti1 = new Resepti { Id = new Random().Next(1000, 2000), Nimi = "Resepti 1", Tekijäid = kayttaja.Id };
            var resepti2 = new Resepti { Id = new Random().Next(2000, 3000), Nimi = "Resepti 2", Tekijäid = kayttaja.Id };
            _context.Reseptit.AddRange(resepti1, resepti2);
            await _context.SaveChangesAsync();

            var suosikki1 = new Suosikit { Id = new Random().Next(3000, 4000), kayttajaID = kayttaja.Id, reseptiID = resepti1.Id };
            var suosikki2 = new Suosikit { Id = new Random().Next(4000, 5000), kayttajaID = kayttaja.Id, reseptiID = resepti2.Id };
            _context.Suosikit.AddRange(suosikki1, suosikki2);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeSuosikkiReseptit(kayttaja);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }


       

        [Fact]
        public async Task HaeSuosikkiReseptit_PalauttaaTyhjanListanJosKayttajallaEiSuosikkeja()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeSuosikkiReseptit(kayttaja);

            Assert.NotNull(result);
            Assert.Empty(result);
        }



        [Fact]
        public async Task PoistaSuosikki_PoistaaOikeanSuosikin()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var resepti = new Resepti { Id = new Random().Next(1000, 2000), Nimi = "Testi Resepti", Tekijäid = kayttaja.Id };
            _context.Reseptit.Add(resepti);
            await _context.SaveChangesAsync();

            var suosikki = new Suosikit { Id = new Random().Next(2000, 3000), kayttajaID = kayttaja.Id, reseptiID = resepti.Id };
            _context.Suosikit.Add(suosikki);
            await _context.SaveChangesAsync();

            var request = new SuosikkiMuokkaus { Kayttaja = kayttaja, suosikki = suosikki };
            await _controller.PoistaSuosikki(request);

            _context.Suosikit.Remove(suosikki);
            await _context.SaveChangesAsync();

            var result = await _controller.HaeSuosikkiReseptit(kayttaja);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task PoistaSuosikki_PalauttaaVirheenJosSuosikkiaEiOle()
        {
            var kayttaja = LuoKayttaja();
            _context.Kayttajat.Add(kayttaja);
            await _context.SaveChangesAsync();

            var request = new SuosikkiMuokkaus
            {
                Kayttaja = new Kayttaja { Sahkopostiosoite = kayttaja.Sahkopostiosoite, Salasana = "salasana123" },
                suosikki = new Suosikit { Id = new Random().Next(2000, 3000), kayttajaID = kayttaja.Id, reseptiID = 999 }
            };

            var result = await _controller.PoistaSuosikki(request);

            Assert.Equal("Virheellinen salasana.", result);
        }



    }
}