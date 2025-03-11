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

public class MainLayoutTests : TestContext
{
    public MainLayoutTests()
    {
        // 🔹 Luo testikäyttäjä ilman kirjautumista (tyhjä ClaimsIdentity)
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        // 🔹 Rekisteröi ClaimsPrincipal palveluihin
        Services.AddSingleton(user);

        // 🔹 Rekisteröi FakeAuthenticationStateProvider käyttäen ClaimsPrincipalia
        Services.AddSingleton<CustomAuthenticationStateProvider>(new FakeAuthenticationStateProvider(user));
        Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

        // 🔹 Lisää mockattu NavigationManager
        Services.AddSingleton<NavigationManager, MockNavigationManager>();

        // 🔹 Lisää AuthorizationCore Blazor-testejä varten
        Services.AddAuthorizationCore();
    }

    [Fact]
    public async Task When_UserIsNotLoggedIn_LoginLinkIsVisible()
    {
        // 🔹 Renderöi MainLayout-komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Haetaan palvelusta FakeAuthenticationStateProvider ja varmistetaan, että käyttäjä ei ole kirjautunut sisään
        var authStateProvider = Services.GetRequiredService<AuthenticationStateProvider>() as FakeAuthenticationStateProvider;
        Assert.NotNull(authStateProvider);
        var isLoggedIn = await authStateProvider.IsUserLoggedIn();
        Assert.False(isLoggedIn); // Varmistetaan, että käyttäjä ei ole kirjautunut sisään

        // 🔹 Tarkista, että "Kirjaudu sisään" -linkki löytyy
        var loginLink = component.Find("a[href='/login']");
        Assert.NotNull(loginLink);
        Assert.Equal("Kirjaudu sisään", loginLink.TextContent);
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

        // 🔹 Tarkistetaan, että elementti löytyi
        Assert.NotNull(loginLink);

        // 🔹 Simuloidaan klikkaus Bunitin tapaan
        component.InvokeAsync(() => loginLink.Click());

        // 🔹 Varmistetaan, että navigointi tapahtui /login-sivulle
        Assert.Equal("/login", mockNav.Uri.Replace(mockNav.BaseUri, ""));
    }

    [Fact]
    public void LoginLink_ShouldExist_And_HaveCorrectHref()
    {
        // 🔹 Renderöidään MainLayout-komponentti
        var component = RenderComponent<MainLayout>();

        // 🔹 Etsitään "Kirjaudu sisään" -linkki
        var loginLink = component.Find("a[href='/login']");

        // 🔹 Varmistetaan, että linkki löytyy ja siinä on oikea teksti
        Assert.NotNull(loginLink);
        Assert.Equal("Kirjaudu sisään", loginLink.TextContent);

        // 🔹 Tarkistetaan, että linkin href on oikein
        Assert.Equal("/login", loginLink.GetAttribute("href"));
    }
}


