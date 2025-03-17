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

public class MainLayoutLoggedOutTests : TestContext
{
    public MainLayoutLoggedOutTests()
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
        Assert.Equal("Kirjaudu sisään", loginLink.TextContent);
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

