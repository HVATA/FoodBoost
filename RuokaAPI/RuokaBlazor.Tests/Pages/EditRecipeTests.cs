using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RichardSzalay.MockHttp;
using RuokaBlazor.Pages;
using RuokaBlazor.Properties.Model;
using RuokaBlazor.Services;
using RuokaBlazor.Tests.Mocks;
using Xunit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace RuokaBlazor.Tests.Pages
    {
    public class EditRecipeTests : TestContext
        {
        private readonly ClaimsPrincipal _user;

        public EditRecipeTests ()
            {
            // Luodaan testikäyttäjä
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1001"),
                new Claim(ClaimTypes.GivenName, "TestGivenName"),
                new Claim(ClaimTypes.Surname, "TestSurname"),
                new Claim("Nimimerkki", "TestUser"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "user"),
                new Claim("Salasana", "testsalasana")
            };

            _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var fakeAuthProvider = new FakeAuthenticationStateProvider(_user);
            Services.AddSingleton<AuthenticationStateProvider>(fakeAuthProvider);
            Services.AddSingleton<CustomAuthenticationStateProvider>(fakeAuthProvider);

            // Lisätään Blazorin autentikointi ja auktorisointi
            Services.AddAuthorizationCore();

            // Määritellään JSInterop kutsut (esim. localStorage)
            JSInterop.Setup<string>("localStorage.getItem", "authUser")
                     .SetResult("{ \"id\": 1001, \"role\": \"user\" }");

            // Oletuksena HTTPClient avainsanoille
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Rekisteröidään oma NavigationManager (mock)
            Services.AddSingleton<NavigationManager, MockNavigationManager>();
            }

        [Fact]
        public async Task EditRecipeComponent_LoadsRecipeSuccessfully ()
            {
            // Arrange: Mokataan HTTP-pyynnöt, jotta komponentti saa kelvollisen reseptin ja avainsanat
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                Ainesosat = new AinesosanMaaraDto[0],
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Assert: Tarkistetaan, että resepti latautui
            Assert.NotNull(component.Instance.resepti);
            Assert.Null(component.Instance.virheViesti);
            Assert.Equal("Test Recipe", component.Instance.resepti.Nimi);
            }

        [Fact]
        public async Task EditRecipeComponent_LoadsKeywords ()
            {
            // Poistetaan mahdollisesti rekisteröity HttpClient
            Services.RemoveAll<HttpClient>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Renderöidään CreateRecipe-komponentti, jossa avainsanat näkyvät
            var component = RenderComponent<CreateRecipe>();

            // Odotetaan, että markup sisältää odotetut avainsanat
            component.WaitForAssertion(() =>
            {
                Assert.Contains("Keyword1", component.Markup);
                Assert.Contains("Keyword2", component.Markup);
                Assert.Contains("Keyword3", component.Markup);
            }, timeout: TimeSpan.FromSeconds(3));
            }

        [Fact]
        public async Task EditRecipeComponent_AddsAndRemovesIngredient_UI_Test ()
            {
            // Arrange: Mokataan HTTP-pyynnöt reseptin ja avainsanojen hakua varten
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                // Alustetaan listassa kaksi ainesosaa
                Ainesosat = new AinesosanMaaraDto[]
                {
                    new AinesosanMaaraDto(),
                    new AinesosanMaaraDto()
                },
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Assert: Alussa ainesosa-inputeja tulee olla 2
            Assert.Equal(2, component.Instance.resepti.Ainesosat.Length);

            // Act: Kutsutaan suoraan metodia lisätä ainesosa
            await component.InvokeAsync(() => component.Instance.LisaaAinesosa());
            component.Render();
            Assert.Equal(3, component.Instance.resepti.Ainesosat.Length);

            // Act: Kutsutaan suoraan metodia poistaa ainesosa
            await component.InvokeAsync(() => component.Instance.PoistaAinesosa(0));
            component.Render();
            Assert.Equal(2, component.Instance.resepti.Ainesosat.Length);
            }

        [Fact]
        public async Task EditRecipeComponent_AddsAndRemovesIngredient_DirectMethod_Test ()
            {
            // Arrange: Mokataan HTTP-pyynnöt reseptin ja avainsanojen hakua varten
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                Ainesosat = new AinesosanMaaraDto[0], // Aluksi tyhjä lista
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Assert: Tarkistetaan, että alussa ainesosia on 0
            Assert.Equal(0, component.Instance.resepti.Ainesosat.Length);

            // Act: Kutsutaan suoraan metodia lisätä ainesosa
            await component.InvokeAsync(() => component.Instance.LisaaAinesosa());
            component.Render();
            Assert.Equal(1, component.Instance.resepti.Ainesosat.Length);

            // Act: Kutsutaan suoraan metodia poistaa ainesosa
            await component.InvokeAsync(() => component.Instance.PoistaAinesosa(0));
            component.Render();
            Assert.Equal(0, component.Instance.resepti.Ainesosat.Length);
            }

        [Fact]
        public async Task EditRecipeComponent_AddsAndRemovesKeyword ()
            {
            // Arrange: Mokataan HTTP-pyynnöt reseptin ja avainsanojen hakua varten
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                Ainesosat = new AinesosanMaaraDto[0],
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Assert: Alussa avainsanoja tulee olla 0
            Assert.Equal(0, component.Instance.resepti.Avainsanat.Length);

            // Act: Kutsutaan suoraan metodia lisätä avainsana
            await component.InvokeAsync(() => component.Instance.LisaaAvainsana());
            component.Render();
            Assert.Equal(1, component.Instance.resepti.Avainsanat.Length);

            // Act: Kutsutaan suoraan metodia poistaa avainsana
            await component.InvokeAsync(() => component.Instance.PoistaAvainsana(0));
            component.Render();
            Assert.Equal(0, component.Instance.resepti.Avainsanat.Length);
            }

        [Fact]
        public async Task EditRecipeComponent_SavesRecipe ()
            {
            // Arrange: Mokataan HTTP-pyynnöt reseptin ja avainsanojen hakua varten
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                Ainesosat = new AinesosanMaaraDto[0],
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            mockHttp.When(HttpMethod.Put, "http://localhost/resepti/1")
                    .Respond(HttpStatusCode.OK);

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Act: Kutsutaan suoraan metodia tallentaa resepti
            await component.InvokeAsync(() => component.Instance.TallennaResepti());
            component.Render();

            // Assert: Tarkistetaan, että navigointi tapahtui
            var navigationManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
            Assert.NotNull(navigationManager);
            Assert.EndsWith("/recipe/1", navigationManager.Uri);
            }

        [Fact]
        public async Task EditRecipeComponent_ImagePreviewShowsOnFileSelect ()
            {
            // Arrange: Mokataan HTTP-pyynnöt reseptin ja avainsanojen hakua varten
            var mockHttp = new MockHttpMessageHandler();
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Test Recipe",
                Valmistuskuvaus = "Test Description",
                Ainesosat = new AinesosanMaaraDto[0],
                Avainsanat = new string[0],
                Kuva1 = "test.jpg",
                Katseluoikeus = "Julkinen"
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Act: Renderöidään komponentti
            var component = RenderComponent<EditRecipe>(parameters => parameters.Add(p => p.Id, 1));
            component.WaitForState(() => component.Instance.resepti != null);

            // Luodaan fake tiedosto, joka toteuttaa IBrowserFile-rajapinnan
            var fakeFile = new FakeBrowserFile();
            var args = new InputFileChangeEventArgs(new[] { fakeFile });

            // Act: Kutsutaan suoraan komponentin tiedostonvalintaa käsittelevää metodia
            await component.InvokeAsync(() => component.Instance.KuvaValittu(args));
            component.Render(); // Renderöidään uudelleen tilan päivityksen jälkeen

            // Assert: Tarkistetaan, että esikatselukuva ilmestyy ja sen src alkaa "data:"
            var img = component.Find("img.preview-image");
            Assert.NotNull(img);
            Assert.StartsWith("data:", img.GetAttribute("src"));
            }

        // Apuluokka FakeBrowserFile, jolla simuloidaan IBrowserFile-oliota
        private class FakeBrowserFile : IBrowserFile
            {
            public string Name => "test.png";
            public DateTimeOffset LastModified => DateTimeOffset.Now;
            public long Size => 1024;
            public string ContentType => "image/png";

            public Stream OpenReadStream ( long maxAllowedSize = 512000, CancellationToken cancellationToken = default )
                {
                // Palautetaan pieni dummy-tiedosto
                var bytes = Encoding.UTF8.GetBytes("dummy image data");
                return new MemoryStream(bytes);
                }
            }
        }
    }

