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

public class MainLayoutLoggedTests : TestContext
{
    public MainLayoutLoggedTests()
    {
        // 🔹 Luo CustomAuthenticationStateProvider testikäyttöön
        var customAuthProvider = new CustomAuthenticationStateProvider(new FakeJSRuntime());
        Services.AddSingleton(customAuthProvider);

        // 🔹 Lisää mockattu NavigationManager
        Services.AddSingleton<NavigationManager, MockNavigationManager>();

        // 🔹 Lisää AuthorizationCore Blazor-testejä varten
        Services.AddAuthorizationCore();

        // 🔹 Simuloidaan localStorage.getItem("authUser") palauttamaan NULL (käyttäjä ei kirjautunut)
        JSInterop.Setup<string>("localStorage.getItem", "authUser").SetResult(null);

        // 🔹 Varmistetaan, että käyttäjä EI ole kirjautunut sisään
        var isLoggedIn = customAuthProvider.IsUserLoggedIn();
    }

    [Fact]
    public void Clicking_LoginLink_NavigatesToLoginPage()
    {
        // 🔹 Haetaan mockattu NavigationManager
        var mockNav = Services.GetRequiredService<NavigationManager>();

        // 🔹 Renderöidään MainLayout-komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Etsitään "Kirjaudu sisään" -linkki
        var loginLink = component.Find("a[href='/login']");


        // 🔹 Tarkistetaan, että linkki löytyy
        Assert.NotNull(loginLink);

        // 🔹 Simuloidaan navigaatio käyttäen mockNav.NavigateTo()
        mockNav.NavigateTo(loginLink.GetAttribute("href"));

        // 🔹 Pakotetaan komponentti renderöimään uudelleen
        component.Render();

        // 🔹 Tarkistetaan, että linkin href on oikein
        Assert.Equal("/login", loginLink.GetAttribute("href"));
    }

    [Fact]
    public void LoginLink_ShouldBeVisible_WhenUserIsNotLoggedIn()
    {
        var component = RenderComponent<MainLayout>();
        var loginLink = component.Find("a[href='/login']");
        Assert.NotNull(loginLink);
        Assert.Equal("🔑 Kirjaudu sisään", loginLink.TextContent);
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
        Assert.Equal("🏠 RuokaBoost-Home", homeLink.TextContent.Trim());

    }
}
public class MainLayoutLoggedInTests : TestContext
{

    public MainLayoutLoggedInTests()
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
    public void LoggedInUser_ShouldSee_CreateRecipeLink()
    {
        var component = RenderComponent<MainLayout>();

        // Debug: Print the rendered HTML to verify the content
        Console.WriteLine(component.Markup);

        var createRecipeLink = component.Find("a[href='/createRecipe']");
        Assert.NotNull(createRecipeLink);
        Assert.Equal("📝 Luo uusi resepti", createRecipeLink.TextContent);
    }



    [Fact]
    public void LoggedInUser_ShouldSee_UserRecipeLink()
    {
        var component = RenderComponent<MainLayout>();

        var userRecipesLink = component.Find("a[href='/userRecipes']");
        Assert.NotNull(userRecipesLink);
        Assert.Equal("📚 Omat reseptit", userRecipesLink.TextContent);
    }




    [Fact]
    public void LoggedInUser_ShouldSee_FavoritesLink()
    {
        var component = RenderComponent<MainLayout>();

        var favoritesLink = component.Find("a[href='/favorites']");
        Assert.NotNull(favoritesLink);
        Assert.Equal("❤️ Suosikit", favoritesLink.TextContent);
    }

    [Fact]
    public void ProfileDropdown_ShouldBeVisible_WhenUserIsLoggedIn()
    {
        // 🔹 Renderöidään MainLayout-komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Varmistetaan, että käyttäjän nimi näkyy (dropdown käynnistyy tästä)
        var userInfo = component.Find("div.user-info");
        Assert.NotNull(userInfo);
        Assert.Contains("TestGivenName", userInfo.TextContent);

        // 🔹 Simuloidaan dropdownin avaaminen
        userInfo.Click();

        // 🔹 Odotetaan, että dropdown-valikko näkyy
        component.WaitForState(() => component.Markup.Contains("Asetukset"), TimeSpan.FromSeconds(5));

        // 🔹 Varmistetaan, että dropdown-valikko on renderöity
        var dropdownMenu = component.Find("div.dropdown-menu");
        Assert.NotNull(dropdownMenu);

        // 🔹 Varmistetaan, että "Asetukset" -linkki löytyy
        var settingsLink = component.Find("a[href='/settings']");
        Assert.NotNull(settingsLink);
        Assert.Equal("⚙️ Asetukset", settingsLink.TextContent);

        // 🔹 Varmistetaan, että "Kirjaudu ulos" -linkki löytyy
        var logoutLink = component.Find(".dropdown-menu a[href='/']");
        Assert.NotNull(logoutLink);
        Assert.Equal("🚪 Kirjaudu ulos", logoutLink.TextContent);
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
        Assert.Equal("🏠 RuokaBoost-Home", homeLink.TextContent.Trim());
    }

}

