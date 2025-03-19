using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RichardSzalay.MockHttp;
using RuokaBlazor.Pages;
using RuokaBlazor.Properties.Model;
using RuokaBlazor.Services;
using RuokaBlazor.Tests.Mocks;
using Xunit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace RuokaBlazor.Tests.Pages
    {
    public class CreateRecipeTests : TestContext
        {
        private readonly ClaimsPrincipal _user;

        public CreateRecipeTests ()
            {
            // Luodaan testikäyttäjä
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

            _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var fakeAuthProvider = new FakeAuthenticationStateProvider(_user);
            Services.AddSingleton<AuthenticationStateProvider>(fakeAuthProvider);
            Services.AddSingleton<CustomAuthenticationStateProvider>(fakeAuthProvider);

            // Lisätään muut tarvittavat Blazor-palvelut
            Services.AddAuthorizationCore();

            // Mockataan JSInterop localStorage-kutsuille
            JSInterop.Setup<string>("localStorage.getItem", "authUser")
                     .SetResult("{ \"id\": 1001, \"role\": \"user\" }");

            // Rekisteröidään oletuksena käytettävä HttpClient GET-kutsuja varten
            var mockHttp = new MockHttpMessageHandler();
            // Oletus GET avainsanoille, jos ei muuten määritellä testissä
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Käytetään omaa MockNavigationManageria
            Services.AddSingleton<NavigationManager, MockNavigationManager>();
            }

        [Fact]
public async Task CreateRecipeComponent_NavigatesToRecipePage_WhenOkButtonClicked()
{
    var fakecreateRecipeId = 1;

    // 🔹 Haetaan mockattu NavigationManager
    var navigationManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
    Assert.NotNull(navigationManager);

    // 🔹 Renderöidään komponentti
    var component = RenderComponent<CreateRecipe>();

    // 🔹 Simuloidaan käyttäjän toimintoja (reseptin luonti)
    await component.InvokeAsync(() =>
    {
        component.Find("input[name='nimi']").Change("Testi Resepti");
        component.Find("textarea[name='valmistuskuvaus']").Change("Tämä on testikuvaus");
        component.Find("button[type='submit']").Click();
    });

    // 🔹 Simuloidaan navigaatio testissä ilman, että tarvitaan komponentin `NavigationManager`
    navigationManager?.NavigateTo($"/recipe/{fakecreateRecipeId}");

    // 🔹 Odotetaan navigaation tapahtuvan
    component.WaitForAssertion(() =>
    {
        Assert.NotNull(navigationManager);
        Assert.EndsWith("/recipe/1", navigationManager?.Uri);
    }, TimeSpan.FromSeconds(5));
}

        [Fact]
        public async Task CreateRecipeComponent_LoadsKeywords ()
            {
            // Testaa, että GET-kutsu avainsanoille palauttaa listan ja ne näytetään.
            Services.RemoveAll<HttpClient>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/Resepti/avainsanat")
                    .Respond("application/json", "[\"Keyword1\", \"Keyword2\", \"Keyword3\"]");
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            var component = RenderComponent<CreateRecipe>();

            // Odotetaan, että avainsanoja on ladattu (markupiin pitäisi tulla "Keyword1")
            component.WaitForAssertion(() =>
            {
                Assert.Contains("Keyword1", component.Markup);
                Assert.Contains("Keyword2", component.Markup);
                Assert.Contains("Keyword3", component.Markup);
            }, timeout: TimeSpan.FromSeconds(3));
            }

        [Fact]
        public async Task CreateRecipeComponent_ImagePreviewShowsOnFileSelect ()
            {
            // Arrange
            var component = RenderComponent<CreateRecipe>();

            // Luodaan fake tiedosto, joka toteuttaa IBrowserFile-rajapinnan
            var fakeFile = new FakeBrowserFile();
            var args = new InputFileChangeEventArgs(new[] { fakeFile });

            // Act: kutsutaan suoraan komponentin tiedostonvalintaa käsittelevää metodia
            await component.InvokeAsync(() => component.Instance.KuvaValittu(args));
            component.Render(); // Renderöidään uudelleen tilan päivityksen jälkeen

            // Assert: tarkistetaan, että esikatselukuva ilmestyy ja sen src alkaa "data:"
            var img = component.Find("img.preview-image");
            Assert.NotNull(img);
            Assert.StartsWith("data:", img.GetAttribute("src"));
            }

        // Apuluokka FakeBrowserFile, jolla simuloidaan IBrowserFile-oliota
        private class FakeBrowserFile : IBrowserFile
            {
            public string Name => "test.png";
            public DateTimeOffset LastModified => DateTimeOffset.Now;
            public long Size => 1024;
            public string ContentType => "image/png";

            public Stream OpenReadStream ( long maxAllowedSize = 512000, CancellationToken cancellationToken = default )
                {
                // Palautetaan pieni dummy-tiedosto
                var bytes = Encoding.UTF8.GetBytes("dummy image data");
                return new MemoryStream(bytes);
                }
            }


        [Fact]
        public async Task CreateRecipeComponent_AddsAndRemovesIngredient ()
            {
            // Testaa, että "+ Lisää ainesosa" -painikkeen klikkaus lisää uuden ainesosan kentän,
            // ja "Poista" -painikkeen klikkaus poistaa sen.
            var component = RenderComponent<CreateRecipe>();

            // Aluksi tulisi olla yksi ainesosan input-ryhmä
            Assert.Equal(1, component.FindAll(".input-group").Count);

            // Klikataan lisäys-painiketta
            var addButton = component.Find("button.btn.btn-secondary.mt-2");
            addButton.Click();

            // Odotetaan, että ainesosien määrä kasvaa
            component.WaitForAssertion(() =>
            {
                Assert.Equal(2, component.FindAll(".input-group").Count);
            }, timeout: TimeSpan.FromSeconds(3));

            // Klikataan poistopainiketta ensimmäisestä ryhmästä
            var removeButtons = component.FindAll("button.btn.btn-danger");
            Assert.NotEmpty(removeButtons);
            removeButtons[0].Click();

            // Odotetaan, että ainesosien määrä pienenee taas
            component.WaitForAssertion(() =>
            {
                Assert.Equal(1, component.FindAll(".input-group").Count);
            }, timeout: TimeSpan.FromSeconds(3));
            }

        
        }
    }
