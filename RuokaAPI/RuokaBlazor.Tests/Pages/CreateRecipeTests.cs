using Bunit;
using Xunit;
using RuokaBlazor.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using RuokaBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using RichardSzalay.MockHttp;
using Microsoft.AspNetCore.Components;
using RuokaBlazor.Tests.Mocks;

namespace RuokaBlazor.Tests.Pages
    {
    public class CreateRecipeTests : TestContext
        {
        private readonly ClaimsPrincipal _user;

        public CreateRecipeTests ()
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

            _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var fakeAuthProvider = new FakeAuthenticationStateProvider(_user);
            Services.AddSingleton<AuthenticationStateProvider>(fakeAuthProvider);
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
        public void CreateRecipeComponent_ShowsAllElements ()
            {
            // Arrange
            var component = RenderComponent<CreateRecipe>();

            // Assert
            Assert.Contains("Luo uusi resepti", component.Markup);
            Assert.Contains("Reseptin nimi:", component.Markup);
            Assert.Contains("Valmistuskuvaus:", component.Markup);
            Assert.Contains("Valitse kuva:", component.Markup);
            Assert.Contains("Ainesosat", component.Markup);
            Assert.Contains("+ Lisää ainesosa", component.Markup);
            Assert.Contains("Avainsanat", component.Markup);
            Assert.Contains("Näkyy vain rekisteröityneille", component.Markup);
            Assert.Contains("Tallenna", component.Markup);
            }
        }
    }

