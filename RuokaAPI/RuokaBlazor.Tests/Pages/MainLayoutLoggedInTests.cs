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

public class MainLayoutLoggedInTests : TestContext
{

    public MainLayoutLoggedInTests()
    {
        // 🔹 Luo kirjautunut käyttäjä
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

        // 🔹 Rekisteröi FakeAuthenticationStateProvider testipalveluihin
        var authProvider = new FakeAuthenticationStateProvider(user);
        Services.AddSingleton<AuthenticationStateProvider>(authProvider);
        Services.AddSingleton<CustomAuthenticationStateProvider>(sp => (CustomAuthenticationStateProvider)authProvider);

        // 🔹 Lisää mockattu NavigationManager
        Services.AddSingleton<NavigationManager, MockNavigationManager>();

        // 🔹 Lisää AuthorizationCore Blazor-testejä varten
        Services.AddAuthorizationCore();

        // 🔹 Varmistetaan, että käyttäjä ON kirjautunut sisään
        var authStateProvider = Services.GetRequiredService<AuthenticationStateProvider>() as FakeAuthenticationStateProvider;
        Assert.NotNull(authStateProvider);

        var isLoggedInTask = authStateProvider.FakeIsUserLoggedIn();
        isLoggedInTask.Wait(); // Synkroninen odotus testissä
        Assert.True(isLoggedInTask.Result); // ✅ Varmistetaan, että käyttäjä on kirjautunut sisään

    }

    [Fact]
    public void LoggedInUser_ShouldSee_UserRecipeLink()
    {
        var component = RenderComponent<MainLayout>();

        var userRecipesLink = component.Find("a[href='/userRecipes']");
        Assert.NotNull(userRecipesLink);
        Assert.Equal("Omat reseptit", userRecipesLink.TextContent);
    }

    [Fact]
    public void LoggedInUser_ShouldSee_CreateRecipeLink()
    {
        var component = RenderComponent<MainLayout>();

        var createRecipeLink = component.Find("a[href='/createRecipe']");
        Assert.NotNull(createRecipeLink);
        Assert.Equal("Luo uusi resepti", createRecipeLink.TextContent);
    }

    [Fact]
    public void LoggedInUser_ShouldSee_FavoritesLink()
    {
        var component = RenderComponent<MainLayout>();

        var favoritesLink = component.Find("a[href='/favorites']");
        Assert.NotNull(favoritesLink);
        Assert.Equal("Suosikit", favoritesLink.TextContent);
    }

    [Fact]
    public void ProfileDropdown_ShouldBeVisible_WhenUserIsLoggedIn()
    {
        // 🔹 Haetaan testattava komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Odotetaan, että komponentti renderöityy ja sisältää käyttäjän nimen
        component.WaitForAssertion(() =>
        {
            var markup = component.Markup;
            Assert.Contains("TestGivenName", markup);
        }, TimeSpan.FromSeconds(10)); // Lisätään aikaa komponentin päivittymiselle

        // 🔹 Haetaan käyttäjän dropdown (user-info)
        var userInfoElement = component.Find("div.user-info");
        Assert.NotNull(userInfoElement);
        Assert.Contains("TestGivenName", userInfoElement.TextContent);

        // 🔹 Klikataan käyttäjävalikkoa avataksemme sen
        userInfoElement.Click();

        // 🔹 Odotetaan, että dropdown-valikko näkyy DOM:issa
        component.WaitForAssertion(() =>
        {
            var dropdownMarkup = component.Markup;
            Assert.Contains("Asetukset", dropdownMarkup);
            Assert.Contains("Kirjaudu ulos", dropdownMarkup);
        }, TimeSpan.FromSeconds(10));

        // 🔹 Etsitään "Asetukset"-linkki ja varmistetaan sen olemassaolo
        var settingsLink = component.Find("a[href='/settings']");
        Assert.NotNull(settingsLink);
        Assert.Equal("Asetukset", settingsLink.TextContent);

        // 🔹 Etsitään "Kirjaudu ulos" -linkki ja varmistetaan sen olemassaolo
        var logoutLink = component.Find("a[href='/']");
        Assert.NotNull(logoutLink);
        Assert.Equal("Kirjaudu ulos", logoutLink.TextContent);
    }




    [Fact]
    public void Clicking_HomeLink_NavigatesToHomePage()
    {
        // 🔹 Haetaan mockattu NavigationManager
        var mockNav = Services.GetRequiredService<NavigationManager>();

        // 🔹 Renderöidään MainLayout-komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Etsitään "RuokaBoost-Home" -linkki
        var homeLink = component.Find("a.homenappi");

        // 🔹 Tarkistetaan, että linkki löytyy
        Assert.NotNull(homeLink);

        // 🔹 Tarkistetaan, että linkin href on oikein (etusivulle "/")
        Assert.Equal("/", homeLink.GetAttribute("href"));

        // 🔹 Tarkistetaan, että linkissä on oikea teksti
        Assert.Equal("RuokaBoost-Home", homeLink.TextContent);
    }

}

