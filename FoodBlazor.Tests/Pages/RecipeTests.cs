using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RichardSzalay.MockHttp;
using FoodBlazor.Pages;
using FoodBlazor.Properties.Model;
using FoodBlazor.Services;
using FoodBlazor.Tests.Mocks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using AntDesign;

namespace FoodBlazor.Tests.Pages
    {
    public class RecipeTests : TestContext
        {
        private readonly ClaimsPrincipal _user;
        private readonly Uri _baseUri = new Uri("http://localhost");

        public RecipeTests ()
            {
            // Rekisteröidään fake-palvelut, joita AntDesign-komponentit tarvitsevat:
            Services.AddSingleton<IComponentIdGenerator, FakeComponentIdGenerator>();
            Services.AddSingleton<IconService, FakeIconService>();

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
            Services.AddAuthorizationCore();

            // Määritellään JSInterop-kutsut (esim. localStorage)
            JSInterop.Setup<string>("localStorage.getItem", "authUser")
                     .SetResult("{ \"id\": 1001, \"role\": \"user\" }");

            // Rekisteröidään oma NavigationManager (mock)
            Services.AddSingleton<NavigationManager, MockNavigationManager>();
            }

        /// <summary>
        /// Asettaa HTTP-mokaukset yhteiselle HTTPClientille.
        /// </summary>
        /// <param name="mockHttp">MockHttpMessageHandler</param>
        private void SetupCommonHttpClient ( MockHttpMessageHandler mockHttp )
            {
            // Mokataan reseptin haku. TekijäId asetetaan, jotta käyttäjällä on oikeudet.
            var recipe = new ReseptiResponse
                {
                Id = 1,
                Nimi = "Delicious Pancakes",
                Valmistuskuvaus = "Mix flour, eggs, and milk. Fry until golden.",
                Katseluoikeus = "Julkinen",
                Kuva1 = "pancakes.jpg",
                TekijaId = 1001
                };

            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/1")
                    .Respond("application/json", JsonSerializer.Serialize(recipe));

            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");

            mockHttp.When(HttpMethod.Get, "http://localhost/Kayttaja/HaeNimimerkki/1001")
                    .Respond("text/plain", "TestUser");

            mockHttp.When(HttpMethod.Put, "http://localhost/Kayttaja/Haesuosikkireseptit")
                    .Respond("application/json", "[]");
            }

        /// <summary>
        /// Renderöi Recipe-komponentin, kun reseptiä ladataan onnistuneesti.
        /// </summary>
        /// <returns>IRenderedComponent of Recipe</returns>
        private IRenderedComponent<Recipe> RenderRecipeComponent ()
            {
            var mockHttp = new MockHttpMessageHandler();
            SetupCommonHttpClient(mockHttp);
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = _baseUri;
            Services.AddSingleton<HttpClient>(client);
            return RenderComponent<Recipe>(parameters => parameters.Add(p => p.Id, 1));
            }

        [Fact]
        public async Task RecipeComponent_LoadsRecipeSuccessfully ()
            {
            // Arrange
            var component = RenderRecipeComponent();

            // Act & Assert: Odotetaan, että reseptin tiedot latautuvat ja tarkistetaan näkyvyys
            component.WaitForAssertion(() =>
            {
                Assert.Contains("Delicious Pancakes", component.Markup);
            }, timeout: TimeSpan.FromSeconds(10));

            Assert.Contains("Mix flour, eggs, and milk. Fry until golden.", component.Markup);
            Assert.Contains("pancakes.jpg", component.Markup);
            }

        [Fact]
        public async Task RecipeComponent_ShowsError_WhenRecipeNotFound ()
            {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/resepti/99")
                    .Respond(HttpStatusCode.NotFound);

            mockHttp.When(HttpMethod.Put, "http://localhost/Kayttaja/Haesuosikkireseptit")
                    .Respond("application/json", "[]");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = _baseUri;
            Services.AddSingleton<HttpClient>(client);

            // Act
            var component = RenderComponent<Recipe>(parameters => parameters.Add(p => p.Id, 99));

            // Assert
            component.WaitForAssertion(() =>
            {
                Assert.Contains("Virhe reseptin tietojen lataamisessa:", component.Markup);
            }, timeout: TimeSpan.FromSeconds(5));
            }

        [Fact]
        public async Task RecipeComponent_DisplaysReviewFormAndActionButtons_WhenUserHasPermissions ()
            {
            // Arrange
            var component = RenderRecipeComponent();

            // Varmistetaan, että reseptin tiedot latautuvat
            component.WaitForAssertion(() =>
            {
                Assert.Contains("Delicious Pancakes", component.Markup);
            }, timeout: TimeSpan.FromSeconds(10));

            // Assert: Tarkistetaan, että arvostelulomake näkyy
            Assert.Contains("Jätä arvostelu", component.Markup);
            Assert.Contains("Arvosana (1-5):", component.Markup);
            Assert.Contains("Kommentti:", component.Markup);

            // Assert: Tarkistetaan, että muokkaus- ja poistonapit näkyvät
            Assert.Contains("Muokkaa", component.Markup);
            Assert.Contains("Poista", component.Markup);
            }

        [Fact]
        public async Task RecipeComponent_EditButton_NavigatesToEditPage ()
            {
            // Arrange
            var component = RenderRecipeComponent();
            var navManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
            Assert.NotNull(navManager);

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Delicious Pancakes", component.Markup);
            }, timeout: TimeSpan.FromSeconds(10));

            // Act: Klikataan "Muokkaa" -painiketta
            var editButton = component.FindAll("button")
                .FirstOrDefault(btn => btn.TextContent.Contains("Muokkaa"));
            Assert.NotNull(editButton);
            editButton.Click();

            // Assert: Varmistetaan, että navigaatio ohjaa muokkaussivulle
            Assert.Contains("/editRecipe/1", navManager.Uri);
            }

        [Fact]
        public async Task RecipeComponent_DeleteButton_NavigatesToHome ()
            {
            // Arrange: Mokaillaan JSInterop, jotta confirm() palauttaa true
            JSInterop.Setup<bool>("window.confirm", _ => true);
            var component = RenderRecipeComponent();
            var navManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
            Assert.NotNull(navManager);

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Delicious Pancakes", component.Markup);
            }, timeout: TimeSpan.FromSeconds(10));

            // Act: Klikataan "Poista" -painiketta
            var deleteButton = component.FindAll("button")
                .FirstOrDefault(btn => btn.TextContent.Contains("Poista"));
            Assert.NotNull(deleteButton);
            deleteButton.Click();

            // Assert: Varmistetaan, että navigaatio ohjaa etusivulle
            Assert.Contains("http://localhost/", navManager.Uri);
            }
        }
    }
