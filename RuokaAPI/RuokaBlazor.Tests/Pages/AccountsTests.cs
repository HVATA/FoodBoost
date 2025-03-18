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

public class AccountsUserTests : TestContext
{
    public AccountsUserTests()
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
    public void AccountPage_Should_Render_UserData_Correctly()
    {
        // Renderöidään Accounts-komponentti
        var component = RenderComponent<Accounts>();

        // Odotetaan, että komponentti renderöi käyttäjän tiedot
        component.WaitForState(() => component.Markup.Contains("Etunimi"));

        // Etsitään käyttäjän tiedot lomakkeesta
        var firstNameInput = component.Find("input#firstname");
        var lastNameInput = component.Find("input#lastname");
        var emailInput = component.Find("input#email");
        var usernameInput = component.Find("input#username");
        var passwordInput = component.Find("input#password");

        // Tarkistetaan, että tiedot vastaavat kirjautunutta käyttäjää
        Assert.Equal("TestGivenName", firstNameInput.GetAttribute("value"));
        Assert.Equal("TestSurname", lastNameInput.GetAttribute("value"));
        Assert.Equal("test@example.com", emailInput.GetAttribute("value"));
        Assert.Equal("TestUser", usernameInput.GetAttribute("value"));
        Assert.Equal("testsalasana", passwordInput.GetAttribute("value"));
    }

}
public class AccountsAdminTests : TestContext
{
    public AccountsAdminTests()
    {
        // 🔹 Luo admin-käyttäjä
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1001"),
            new Claim(ClaimTypes.GivenName, "AdminName"),
            new Claim(ClaimTypes.Surname, "AdminSurname"),
            new Claim("Nimimerkki", "AdminUser"),
            new Claim(ClaimTypes.Email, "admin@example.com"),
            new Claim(ClaimTypes.Role, "admin"),  // 🔹 Admin-rooli
            new Claim("Salasana", "adminsalasana")
        };

        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

        var fakeAuthProvider = new FakeAuthenticationStateProvider(adminUser);
        Services.AddSingleton<AuthenticationStateProvider>(fakeAuthProvider);
        Services.AddSingleton<CustomAuthenticationStateProvider>(fakeAuthProvider);

        Services.AddAuthorizationCore();
        Services.AddSingleton<NavigationManager, MockNavigationManager>();

        // Mockaa käyttäjälatauksen HTTP-pyyntö
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "/Kayttaja/Haekaikki/1001/adminsalasana/admin@example.com")
                .Respond("application/json", "[{\"Id\":2001,\"Etunimi\":\"TestiKäyttäjä\",\"Sukunimi\":\"Testinen\",\"Sahkopostiosoite\":\"test@example.com\",\"Nimimerkki\":\"TestUser\",\"Kayttajataso\":\"user\"}]");

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");
        Services.AddSingleton<HttpClient>(client);
    }

    [Fact]
    public void AdminUser_ShouldSee_AdminElements()
    {
        var component = RenderComponent<Accounts>();

        // 🔹 Varmista, että admin näkee roolikentän
        var roleInput = component.Find("input#role");
        Assert.NotNull(roleInput);

        // 🔹 Varmista, että admin näkee käyttäjälistan
        var userList = component.Find("div.user-list");
        Assert.NotNull(userList);
        Assert.Contains("Käyttäjät", userList.TextContent);
    }

    [Fact]
    public void AdminUser_ClicksUser_FromUserList_ShouldPopulateForm()
    {
        var component = RenderComponent<Accounts>();

        // 🔹 Odota, että käyttäjälista näkyy
        component.WaitForState(() => component.Markup.Contains("Käyttäjät"));

        // 🔹 Hae käyttäjälistan ensimmäinen käyttäjä
        var userItem = component.Find("li.user-item");
        Assert.NotNull(userItem);
        Assert.Contains("TestiKäyttäjä", userItem.TextContent);

        // 🔹 Klikkaa käyttäjää listalla
        userItem.Click();

        // 🔹 Varmista, että käyttäjän tiedot ilmestyvät lomakkeeseen
        var firstNameInput = component.Find("input#firstname");
        var lastNameInput = component.Find("input#lastname");
        var emailInput = component.Find("input#email");
        var usernameInput = component.Find("input#username");
        var roleInput = component.Find("input#role");

        Assert.Equal("TestiKäyttäjä", firstNameInput.GetAttribute("value"));
        Assert.Equal("Testinen", lastNameInput.GetAttribute("value"));
        Assert.Equal("test@example.com", emailInput.GetAttribute("value"));
        Assert.Equal("TestUser", usernameInput.GetAttribute("value"));
        Assert.Equal("user", roleInput.GetAttribute("value"));
    }
}

