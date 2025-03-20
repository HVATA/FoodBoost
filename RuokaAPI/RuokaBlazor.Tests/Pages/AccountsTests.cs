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
    public void AdminUser_ShouldSeeUserList()
    {
        // 🔹 Luo testikäyttäjälista
        var testUsers = new List<Kayttaja>
        {
            new Kayttaja
            {
                Id = 2001,
                Etunimi = "TestiKäyttäjä",
                Sukunimi = "Testinen",
                Sahkopostiosoite = "test@example.com",
                Nimimerkki = "TestUser",
                Salasana = "testpassword",
                Kayttajataso = "user"
            },
            new Kayttaja
            {
                Id = 2002,
                Etunimi = "ToinenKäyttäjä",
                Sukunimi = "Toinen",
                Sahkopostiosoite = "toinen@example.com",
                Nimimerkki = "ToinenUser",
                Salasana = "password",
                Kayttajataso = "admin"
            }
        };

        // 🔹 Muuta käyttäjälista JSON-muotoon
        var usersJson = JsonSerializer.Serialize(testUsers);

        // 🔹 Luo MockHttpMessageHandler ja määritä vastaus
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "/Kayttaja/Haekaikki/*")
                .Respond("application/json", usersJson);  // Mockaa vastauksen JSONina

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");
        Services.AddSingleton<HttpClient>(client);

        // 🔹 Renderöi Accounts-komponentti
        var component = RenderComponent<Accounts>();

        // 🔹 Odota, että käyttäjälista näkyy
        component.WaitForState(() => component.Markup.Contains("Käyttäjät"));

        // 🔹 Varmista, että listassa näkyy ainakin yksi käyttäjä
        var userItems = component.FindAll("li.user-item");
        Assert.NotEmpty(userItems);
        Assert.Contains(userItems, item => item.TextContent.Contains("TestiKäyttäjä"));
    }

    [Fact]
    public void AdminUser_ClicksUser_FromUserList_ShouldPopulateForm()
    {
        // 🔹 Luo testikäyttäjälista
        var testUsers = new List<Kayttaja>
        {
            new Kayttaja
            {
                Id = 2001,
                Etunimi = "TestiKäyttäjä",
                Sukunimi = "Testinen",
                Sahkopostiosoite = "test@example.com",
                Nimimerkki = "TestUser",
                Salasana = "testpassword",
                Kayttajataso = "user"
            },
            new Kayttaja
            {
                Id = 2002,
                Etunimi = "ToinenKäyttäjä",
                Sukunimi = "Toinen",
                Sahkopostiosoite = "toinen@example.com",
                Nimimerkki = "ToinenUser",
                Salasana = "password",
                Kayttajataso = "admin"
            }
        };

        // 🔹 Muuta käyttäjälista JSON-muotoon
        var usersJson = JsonSerializer.Serialize(testUsers);

        // 🔹 Luo MockHttpMessageHandler ja määritä vastaus
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "/Kayttaja/Haekaikki/*")
                .Respond("application/json", usersJson);  // Mockaa vastauksen JSONina

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");
        Services.AddSingleton<HttpClient>(client);

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
        var passwordInput = component.Find("input#password");
        var roleInput = component.Find("input#role");

        Assert.Equal("TestiKäyttäjä", firstNameInput.GetAttribute("value"));
        Assert.Equal("Testinen", lastNameInput.GetAttribute("value"));
        Assert.Equal("test@example.com", emailInput.GetAttribute("value"));
        Assert.Equal("TestUser", usernameInput.GetAttribute("value"));
        Assert.Equal("testpassword", passwordInput.GetAttribute("value"));
        Assert.Equal("user", roleInput.GetAttribute("value"));
    }

    [Fact]
    public async Task AdminUser_UpdatesUser_ShouldShowSuccessMessage()
    {
        // 🔹 Luo testikäyttäjälista
        var testUsers = new List<Kayttaja>
        {
            new Kayttaja
            {
                Id = 2001,
                Etunimi = "TestiKäyttäjä",
                Sukunimi = "Testinen",
                Sahkopostiosoite = "test@example.com",
                Nimimerkki = "TestUser",
                Salasana = "testpassword",
                Kayttajataso = "user"
            },
            new Kayttaja
            {
                Id = 2002,
                Etunimi = "ToinenKäyttäjä",
                Sukunimi = "Toinen",
                Sahkopostiosoite = "toinen@example.com",
                Nimimerkki = "ToinenUser",
                Salasana = "password",
                Kayttajataso = "admin"
            }
        };

        // 🔹 Muuta käyttäjälista JSON-muotoon
        var usersJson = JsonSerializer.Serialize(testUsers);

        // 🔹 Luo MockHttpMessageHandler ja määritä vastaus
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, "/Kayttaja/Haekaikki/*")
                .Respond("application/json", usersJson);  // Mockaa vastauksen JSONina

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");
        Services.AddSingleton<HttpClient>(client);

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
        var passwordInput = component.Find("input#password");
        var roleInput = component.Find("input#role");

        // 🔹 Syötetään käyttäjän tiedot lomakkeelle ennen päivitystä
        component.Find("input#firstname").Change("TestiEtunimi");
        component.Find("input#lastname").Change("TestiSukunimi");
        component.Find("input#email").Change("test@example.com");
        component.Find("input#username").Change("TestUser");
        component.Find("input#role").Change("admin");

        // 🔹 Varmistetaan, että syötteet ovat oikein ennen päivitystä
        Assert.Equal("TestiEtunimi", component.Find("input#firstname").GetAttribute("value"));
        Assert.Equal("TestiSukunimi", component.Find("input#lastname").GetAttribute("value"));
        Assert.Equal("test@example.com", component.Find("input#email").GetAttribute("value"));
        Assert.Equal("TestUser", component.Find("input#username").GetAttribute("value"));
        Assert.Equal("admin", component.Find("input#role").GetAttribute("value"));

        mockHttp.When(HttpMethod.Put, "/Kayttaja/PaivitaTietoja")
                .Respond("application/json", "{\"status\": \"success\"}");

        // 🔹 Klikkaa "Päivitä" -nappia
        component.Find("button.btn-primary").Click();

        // 🔹 Odota, että onnistumisviesti näkyy
        component.WaitForState(() => component.Markup.Contains("Käyttäjätiedot päivitetty!"));

        // 🔹 Tarkista, että onnistumisviesti näkyy
        var messageElement = component.Find("p");
        Assert.NotNull(messageElement);
        Assert.Contains("Käyttäjätiedot päivitetty!", messageElement.TextContent);
    }

    [Fact]
    public async Task AdminUser_DeletesUser_ThroughModal_ShouldShowSuccessMessage()
    {
        // 🔹 Luo testikäyttäjälista
        var testUsers = new List<Kayttaja>
    {
        new Kayttaja
        {
            Id = 2001,
            Etunimi = "TestiKäyttäjä",
            Sukunimi = "Testinen",
            Sahkopostiosoite = "test@example.com",
            Nimimerkki = "TestUser",
            Salasana = "testpassword",
            Kayttajataso = "user"
        },
        new Kayttaja
        {
            Id = 2002,
            Etunimi = "ToinenKäyttäjä",
            Sukunimi = "Toinen",
            Sahkopostiosoite = "toinen@example.com",
            Nimimerkki = "ToinenUser",
            Salasana = "password",
            Kayttajataso = "admin"
        }
    };

        // 🔹 Muuta käyttäjälista JSON-muotoon
        var usersJson = JsonSerializer.Serialize(testUsers);

        // 🔹 Luo MockHttpMessageHandler ja määritä vastaukset
        var mockHttp = new MockHttpMessageHandler();

        // 🔹 Mockataan käyttäjien haku API
        mockHttp.When(HttpMethod.Get, "/Kayttaja/Haekaikki/*")
                .Respond("application/json", usersJson);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        // 🔹 Rekisteröidään kaikki palvelut ENNEN komponentin renderöintiä!
        Services.AddSingleton<HttpClient>(client);
        Services.AddSingleton<NavigationManager, MockNavigationManager>();

        // 🔹 Hae mockattu navigointipalvelu ennen komponentin renderöintiä
        var navigationManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        Assert.NotNull(navigationManager);

        // 🔹 Renderöidään komponentti
        var component = RenderComponent<Accounts>();

        // 🔹 Odota, että käyttäjälista näkyy
        component.WaitForState(() => component.Markup.Contains("Käyttäjät"));

        // 🔹 Hae käyttäjälistan ensimmäinen käyttäjä
        var userItem = component.Find("li.user-item");
        Assert.NotNull(userItem);
        Assert.Contains("TestiKäyttäjä", userItem.TextContent);

        // 🔹 Klikkaa käyttäjää listalla
        userItem.Click();

        // 🔹 Klikkaa "Poista"-painiketta (avaa modalin)
        component.Find("button.btn-danger").Click();

        // 🔹 Odota, että modal avautuu
        component.WaitForState(() => component.Markup.Contains("Oletko varma että haluat poistaa käyttäjän?"), TimeSpan.FromSeconds(5));

        // 🔹 Klikkaa "Kyllä"-painiketta modalista (poistaa käyttäjän)
        var confirmButton = component.WaitForElement("button.btn-primary", TimeSpan.FromSeconds(5));
        Assert.NotNull(confirmButton);
        confirmButton.Click();

        // 🔹 Odota hetki, jotta UI ehtii päivittyä
        await Task.Delay(1000);

        // 🔹 Tarkista, että navigointi tapahtui oikeaan osoitteeseen "/"
        component.WaitForAssertion(() =>
        {
            Assert.NotNull(navigationManager);
            Assert.EndsWith("/", navigationManager?.Uri);
        }, TimeSpan.FromSeconds(5)); // ✅ Odota max 5 sekuntia navigoinnin tapahtumista
    }


}

