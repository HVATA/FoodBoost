using Bunit;
using Xunit;
using RuokaBlazor.Pages;
using RuokaBlazor.Properties.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using RuokaBlazor.Services;

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
            new ReseptiRequest { Nimi = "Testi Resepti", Valmistuskuvaus = "Tämä on testikuvaus", Kuva1 = "testikuva.jpg" }
        };

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
}
