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

public class AccountsTests : TestContext
{
    public AccountsTests()
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
}
