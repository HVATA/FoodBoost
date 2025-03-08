﻿using Bunit;
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
                throw new ArgumentNullException(nameof(claim.Value), "Claim value cannot be null or empty");
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

        // Mock HttpClient
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("/resepti/omat/1001")
                .Respond("application/json", "[{\"Id\":1,\"TekijaId\":1001,\"Nimi\":\"Testi Resepti\",\"Valmistuskuvaus\":\"Tämä on testikuvaus\",\"Avainsanat\":[\"pasta\",\"helppo\"],\"Ainesosat\":[{\"Ainesosa\":\"Spaghetti\",\"Maara\":\"200g\"}]}]");
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");
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

        // Debug: Print HTML if it fails
        Console.WriteLine(component.Markup);

        // Assert
        Assert.Contains("Testi Resepti", component.Markup);
        Assert.Contains("Tämä on testikuvaus", component.Markup);
        }
    }
