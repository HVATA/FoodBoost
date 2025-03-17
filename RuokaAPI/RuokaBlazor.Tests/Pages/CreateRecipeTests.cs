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
            mockHttp.When(HttpMethod.Post, "/api/reseptit")
                    .Respond("application/json", "{\"id\": 1}");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");
            Services.AddSingleton<HttpClient>(client);

            // Use FakeNavigationManager
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();
            }

        [Fact]
        public async Task CreateRecipeComponent_SavesRecipe_WhenFormIsSubmitted ()
            {
            // Arrange
            var navigationManager = Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            var component = RenderComponent<CreateRecipe>();

            // Act
            component.Find("input[name='nimi']").Change("Testi Resepti");
            component.Find("textarea[name='valmistuskuvaus']").Change("Tämä on testikuvaus");
            component.Find("button[type='submit']").Click();

            // Wait for async operations to complete
            await Task.Delay(100);

            // Assert
            Assert.Equal("http://localhost/recipe/1", navigationManager?.Uri);
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


