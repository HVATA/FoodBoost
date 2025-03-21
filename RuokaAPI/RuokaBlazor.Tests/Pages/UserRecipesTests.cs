using Bunit;
using Xunit;
using RuokaBlazor.Pages;
using RuokaBlazor.Properties.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using RuokaBlazor.Services;
using System.Collections.Generic;
using Microsoft.JSInterop;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Components;
using RuokaBlazor.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

public class UserRecipesTests : TestContext
    {
    private readonly ClaimsPrincipal _user;

    public UserRecipesTests ()
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

        // Ensure no claim values are null
        foreach (var claim in claims)
            {
            if (string.IsNullOrEmpty(claim.Value))
                {
                throw new System.ArgumentNullException(nameof(claim.Value), "Claim value cannot be null or empty");
                }
            }

        _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

        var fakeAuthProvider = new FakeAuthenticationStateProvider(_user);
        Services.AddSingleton<AuthenticationStateProvider>(fakeAuthProvider);
        Services.AddSingleton<CustomAuthenticationStateProvider>(fakeAuthProvider);

        // Add other Blazor services that the component might need
        Services.AddAuthorizationCore();

        // Mock JSInterop to respond to localStorage calls
        JSInterop.Setup<string>("localStorage.getItem", "authUser")
                 .SetResult("{ \"id\": 1001, \"role\": \"user\" }"); // Simulate user data
        Services.AddSingleton<NavigationManager, MockNavigationManager>();
        

    // Mock HttpClient for GET requests by default
    var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("/resepti/omat/1001")
                .Respond("application/json", "[{\"Id\":1,\"TekijaId\":1001,\"Nimi\":\"Testi Resepti\",\"Valmistuskuvaus\":\"Tämä on testikuvaus\",\"Avainsanat\":[\"pasta\",\"helppo\"],\"Ainesosat\":[{\"Ainesosa\":\"Spaghetti\",\"Maara\":\"200g\"}]}]");
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new System.Uri("http://localhost");
        Services.AddSingleton<HttpClient>(client);

        
        }

    [Fact]
    public async Task UserRecipesComponent_ShowsUserRecipes_WhenRecipesAreMocked ()
        {
        // Arrange
        var recipes = new List<ReseptiResponse>
        {
            new ReseptiResponse
            {
                Id = 1,
                TekijaId = 1001,
                Nimi = "Testi Resepti",
                Valmistuskuvaus = "Tämä on testikuvaus",
                Avainsanat = new[] { "pasta", "helppo" },
                Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" } }
            }
        };

        // Act
        var component = RenderComponent<UserRecipes>();
        await component.Instance.SetRecipes(recipes);
        component.Render(); // Force UI update

        // Debug: Tulostetaan HTML-merkintä, jos testi epäonnistuu
        System.Console.WriteLine(component.Markup);

        // Assert
        Assert.Contains("Testi Resepti", component.Markup);
        Assert.Contains("Tämä on testikuvaus", component.Markup);
        }

    [Fact]
    public async Task UserRecipesComponent_EditButton_NavigatesToEditPage ()
        {
        // Arrange: Aseta reseptiluettelo
        var recipes = new List<ReseptiResponse>
        {
            new ReseptiResponse
            {
                Id = 1,
                TekijaId = 1001,
                Nimi = "Testi Resepti",
                Valmistuskuvaus = "Tämä on testikuvaus",
                Avainsanat = new[] { "pasta", "helppo" },
                Ainesosat = new[] { new AinesosanMaaraDto { Ainesosa = "Spaghetti", Maara = "200g" } }
            }
        };

        var component = RenderComponent<UserRecipes>();
        // Aseta reseptit manuaalisesti testidatan avulla
        await component.Instance.SetRecipes(recipes);
        component.Render();

        // Odotetaan, että "Ladataan reseptejä..." -viesti poistuu ja painikkeet ilmestyvät
        component.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Ladataan reseptejä...", component.Markup);
            Assert.Contains("Muokkaa", component.Markup);
        }, timeout: TimeSpan.FromSeconds(5));

        var navigationManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        Assert.NotNull(navigationManager);

        // Debug: Tulostetaan kaikki painikkeet
        var buttons = component.FindAll("button");
        foreach (var button in buttons)
            {
            System.Console.WriteLine(button.TextContent);
            }

        // Act: Klikataan "Muokkaa" -painiketta
        var editButton = component.FindAll("button")
            .FirstOrDefault(btn => btn.TextContent.Contains("Muokkaa"));
        Assert.NotNull(editButton);
        editButton.Click();

        // Assert: Varmistetaan, että navigaatio ohjaa muokkaussivulle
        Assert.Contains("/editRecipe/1", navigationManager.Uri);
        }

    

    }

