using Bunit;
using Xunit;
using RuokaBlazor.Pages;
using RuokaBlazor.Properties.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using RuokaBlazor.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using RuokaBlazor.Tests.Mocks;
using AntDesign;

public class HomeTests : TestContext
{
    public HomeTests()
    {
        // Luo testikäyttäjä
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1001"),
            new Claim(ClaimTypes.GivenName, "TestGivenName"),
            new Claim(ClaimTypes.Surname, "TestSurname"),
            new Claim("Nimimerkki", "TestUser"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "user"),
            new Claim("Salasana", "testsalasana")
        }, "mock"));

        // Lisää CustomAuthenticationStateProvider, jos Blazor-sovellus sitä käyttää
        Services.AddSingleton<CustomAuthenticationStateProvider>();

        // Lisää mahdolliset muut Blazor-palvelut, joita komponentti voi tarvita
        Services.AddAuthorizationCore();

        // Mockataan JSInterop vastaamaan localStorage-kutsuun
        JSInterop.Setup<string>("localStorage.getItem", "authUser")
                 .SetResult("{ \"id\": 1001, \"role\": \"user\" }"); // Simuloidaan käyttäjätiedot
    }

    [Fact]
    public void HomeComponent_ShowsRecipes_WhenRecipesAreMocked()
    {
        // Arrange
        var recipes = new List<ReseptiRequest>
        {
            new ReseptiRequest
            {
                Id = 1,
                TekijaId = 1001,
                Nimi = "Testi Resepti",
                Valmistuskuvaus = "Tämä on testikuvaus",
                Avainsanat = new[] { "pasta", "helppo" },
                Ainesosat = new[]
                {
                    new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" }
                },
                Katseluoikeus = "Yksityinen"  // 🔹 Siirrä tämä ulos Ainesosat-listasta!
            }
        };

        //Act
        var component = RenderComponent<Home>();

        // 🔹 Pakotetaan komponentti käyttämään mockattuja reseptejä
        component.Instance.GetType()
            .GetProperty("recipes")?
            .SetValue(component.Instance, recipes);

        component.Render(); // Pakotetaan UI päivitys

        // Debug: Tulosta HTML jos epäonnistuu
        Console.WriteLine(component.Markup);

        // Assert
        Assert.Contains("Testi Resepti", component.Markup);
    }

    [Fact]
    public void HomeComponent_ShowsLoadingMessage_WhenRecipesAreNull()
    {
        // Arrange: ei anneta reseptejä (jolloin niiden oletetaan olevan null)

        // Renderöidään komponentti ilman dataa
        var component = RenderComponent<Home>();

        // Assert: Tarkistetaan, että latausviesti näkyy
        Assert.Contains("Ladataan reseptejä...", component.Markup);
    }

    [Fact]
    public async Task HomeComponent_Search_ReturnsCorrectRecipes()
    {
        // 🔹 Mockattu reseptilista
        var allRecipes = new List<ReseptiRequest>
        {
            new ReseptiRequest
            {
                Id = 1,
                TekijaId = 1001,
                Nimi = "Pasta Carbonara",
                Valmistuskuvaus = "Herkullinen pasta",
                Avainsanat = new[] { "Pasta", "Italialainen" },
                Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" } }
            },
            new ReseptiRequest
            {
                Id = 2,
                TekijaId = 1002,
                Nimi = "Marjapiirakka",
                Valmistuskuvaus = "Makea herkku",
                Avainsanat = new[] { "Leivonta", "Jälkiruoka" },
                Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Mustikka", Maara = "100g" } }
            }
        };

        var ingredients = new List<Ainesosa>
        {
            new Ainesosa { Nimi = "Spaghetti", IsChecked = false },
            new Ainesosa { Nimi = "Mustikka", IsChecked = false }
        };

        var keywords = new List<Avainsana>
        {
            new Avainsana { Sana = "Pasta", IsChecked = false },
            new Avainsana { Sana = "Leivonta", IsChecked = false }
        };

        // 🔹 Mockataan HttpClient vastaamaan API-pyyntöihin oikein
        var mockHttp = new MockHttpMessageHandler();

        // Jos API-kutsussa on "ainesosat" tai "avainsanat", palautetaan vain täsmäävät reseptit
        mockHttp.When(HttpMethod.Get, "/Resepti*")
        .Respond(req =>
        {
            var uri = req.RequestUri;
            if (uri == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

            // Haetaan kaikki "ainesosat" ja "avainsanat" parametrit
            var requestedIngredients = queryParams.GetValues("ainesosat") ?? Array.Empty<string>();
            var requestedKeywords = queryParams.GetValues("avainsanat") ?? Array.Empty<string>();

            // Suodatetaan reseptit, jotka sisältävät KAIKKI haetut ainesosat JA vähintään yhden avainsanan
            var filteredRecipes = allRecipes
                .Where(r =>
                    (requestedIngredients.Length == 0 || requestedIngredients.All(ingredient =>
                        r.Ainesosat.Any(a => a.Ainesosa.Equals(ingredient, StringComparison.OrdinalIgnoreCase))))
                    &&
                    (requestedKeywords.Length == 0 || requestedKeywords.Any(keyword =>
                        r.Avainsanat.Any(a => a.Equals(keyword, StringComparison.OrdinalIgnoreCase)))))
                .ToList();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(filteredRecipes)
            };
        });



        var httpClient = new HttpClient(mockHttp) { BaseAddress = new Uri("https://localhost") };

        Services.AddSingleton(httpClient);

        // 🔹 Testikomponentin alustaminen mockatulla HTTP-palvelulla
        var component = RenderComponent<Home>();

        // 🛠 **Varmistetaan, että `recipes`, `ingredients` ja `keywords` eivät ole null**
        component.Instance.recipes ??= new List<ReseptiRequest>();
        component.Instance.ingredients ??= new List<Ainesosa>();
        component.Instance.keywords ??= new List<Avainsana>();

        // 🔹 Pakotetaan komponentti käyttämään mockattua dataa
        component.Instance.recipes = allRecipes;
        component.Instance.ingredients = ingredients;
        component.Instance.keywords = keywords;
        component.Render();

        // 🔹 Syötetään hakukenttään "Pasta"
        var input = component.Find("input");
        input.Input("Spaghetti");

        // 🔹 Kutsutaan haun päivitys
        await component.InvokeAsync(() => component.Instance.SearchQueryChanged());

        // 🔹 Pakotetaan UI-päivitys ja odotetaan
        await Task.Delay(500);
        component.Render();

        // ✅ Debug: Tulosta komponentin HTML-markup
        Console.WriteLine("Lopullinen Markup:");
        Console.WriteLine(component.Markup);

        // ✅ Varmistetaan, että "Pasta Carbonara" näkyy, mutta "Marjapiirakka" ei
        Assert.Contains("Pasta Carbonara", component.Markup);
        Assert.DoesNotContain("Marjapiirakka", component.Markup);
    }

    [Fact]
    public void HomeComponent_SelectRecipe_NavigatesToCorrectUrl()
    {
        // 🔹 Alustetaan Blazorin NavigationManager kunnolla
        var mockNavMan = new Mock<NavigationManager>(MockBehavior.Loose);
        Services.AddScoped<NavigationManager, MockNavigationManager>(); // Käytetään mockia
        var navMan = Services.GetRequiredService<NavigationManager>();
        Assert.NotNull(navMan);

        // 🔹 Luodaan testidata (resepti)
        var recipe = new ReseptiRequest
        {
            Id = 123,
            Nimi = "Testi Resepti",
            Valmistuskuvaus = "Testikuvaus",
            Kuva1 = "testikuva.jpg"
        };

        // 🔹 Renderöidään komponentti (pakollista, jotta NavigationManager toimii)
        var component = RenderComponent<Home>();

        // 🔹 Haetaan `SelectRecipe`-metodi reflektiolla, koska se voi olla private
        var method = component.Instance.GetType().GetMethod("SelectRecipe",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // 🔹 Kutsutaan SelectRecipe-metodia
        method?.Invoke(component.Instance, new object[] { recipe });

        // 🔹 Tarkistetaan, että navigointi on kutsuttu oikealla URL:lla
        Assert.EndsWith($"/{recipe.Id}", navMan.Uri);
    }



}
