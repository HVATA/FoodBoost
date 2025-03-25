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
using RuokaBlazor.Layout;
using System.Text.Json;
using Moq.Protected;


public class FavoritesTests : TestContext
{
    public FavoritesTests()
    {
        // Create test user
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

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

        // Käytetään ylikirjoitettua FakeAuthenticationStateProvideria, joka nyt palauttaa oikean kirjautumistilan
        var fakeAuthProvider = new FakeAuthenticationStateProvider(user);

        // Rekisteröidään palvelut

        Services.AddSingleton<CustomAuthenticationStateProvider>(fakeAuthProvider);

        // Add other Blazor services that the component might need
        Services.AddAuthorizationCore();

        // Mock JSInterop to respond to localStorage calls
        JSInterop.Setup<string>("localStorage.getItem", "authUser")
                 .SetResult("{ \"id\": 1001, \"role\": \"user\" }"); // Simulate user data

        // Use MockNavigationManager
        Services.AddSingleton<NavigationManager, MockNavigationManager>();
    }

    [Fact]
    public void FavoritePage_ShowsRecipes_WhenRecipesAreMocked()
    {
        // Arrange
        var recipes = new List<Resepti>
        {
            new Resepti
            {
                Id = 1,
                Tekijäid = 1001,
                Nimi = "Testi Resepti",
                Valmistuskuvaus = "Tämä on testikuvaus",
                AinesosanMaara = new List<ReseptiAinesosa>(),
                Avainsanat = new List<Avainsana>(),
                Katseluoikeus = "Yksityinen",
                Arvostelut = new List<Arvostelu>()
            }
        };

        //Act
        var component = RenderComponent<Favorites>();

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
    public void FavoritePage_NavigatesToRecipe_WhenRecipeClicked()
    {
        // 🔹 Alustetaan Blazorin NavigationManager kunnolla
        var mockNavMan = new Mock<NavigationManager>(MockBehavior.Loose);
        Services.AddScoped<NavigationManager, MockNavigationManager>(); // Käytetään mockia
        var navMan = Services.GetRequiredService<NavigationManager>();
        Assert.NotNull(navMan);

        // 🔹 Luodaan testidata (resepti)
        var recipe = new Resepti
        {
            Id = 123,
            Nimi = "Testi Resepti",
            Valmistuskuvaus = "Testikuvaus",
        };

        // 🔹 Renderöidään komponentti (pakollista, jotta NavigationManager toimii)
        var component = RenderComponent<Favorites>();

        // 🔹 Haetaan `SelectRecipe`-metodi reflektiolla, koska se voi olla private
        var method = component.Instance.GetType().GetMethod("SelectRecipe",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // 🔹 Kutsutaan SelectRecipe-metodia
        method?.Invoke(component.Instance, new object[] { recipe });

        // 🔹 Tarkistetaan, että navigointi on kutsuttu oikealla URL:lla
        Assert.EndsWith($"/{recipe.Id}", navMan.Uri);
    }
}
