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

        // Lisää FakeAuthenticationStateProvider testipalveluihin
        Services.AddSingleton<AuthenticationStateProvider>(new FakeAuthenticationStateProvider(user));

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
                Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" } }
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
        Assert.Contains("Tämä on testikuvaus", component.Markup);
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
    public async void HomeComponent_Search_ReturnsCorrectRecipes()
    {
        // Arrange: Testidata kahdelle reseptille
        var recipes = new List<ReseptiRequest>
    {
        new ReseptiRequest
        {
            Id = 1,
            TekijaId = 1001,
            Nimi = "Pasta Carbonara",
            Valmistuskuvaus = "Herkullinen pasta",
            Avainsanat = new[] { "pasta", "italialainen" },
            Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" } }
        },
        new ReseptiRequest
        {
            Id = 2,
            TekijaId = 1002,
            Nimi = "Marjapiirakka",
            Valmistuskuvaus = "Makea herkku",
            Avainsanat = new[] { "leivonta", "jälkiruoka" },
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
        new Avainsana { Sana = "pasta", IsChecked = false },
        new Avainsana { Sana = "leivonta", IsChecked = false }
    };

        var component = RenderComponent<Home>(parameters => parameters
            .Add(p => p.recipes, recipes)
            .Add(p => p.ingredients, ingredients) // Nyt voi lisätä ingredients!
            .Add(p => p.keywords, keywords)       // Nyt voi lisätä keywords!
        );

        // Simuloidaan hakua avainsanalla "pasta"
        component.Find("input").Input("leivonta");
        component.Find("button.search-btn").Click();

        // Odotetaan, että Blazor päivittää UI:n
        await Task.Delay(200); // Pieni viive UI-päivitykselle


        // Assert: Varmistetaan, että vain "Pasta Carbonara" näkyy ja "Marjapiirakka" ei
        Assert.Contains("Marjapiirakka", component.Markup);
        Assert.DoesNotContain("Pasta Carbonara", component.Markup);
    }


}
