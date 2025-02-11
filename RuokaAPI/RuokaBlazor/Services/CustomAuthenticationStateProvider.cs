namespace RuokaBlazor.Services
{
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Security.Claims;
    using System.Text.Json;
    using Microsoft.JSInterop;

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authUser");

            if (string.IsNullOrEmpty(userJson))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var userData = JsonSerializer.Deserialize<UserData>(userJson);

            var identity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userData.Id.ToString()),
        new Claim(ClaimTypes.GivenName, userData.Etunimi),
        new Claim(ClaimTypes.Surname, userData.Sukunimi),
        new Claim("Nimimerkki", userData.Nimimerkki),
        new Claim(ClaimTypes.Email, userData.Sahkopostiosoite),
        new Claim(ClaimTypes.Role, userData.Kayttajataso),
    }, "auth");

            _currentUser = new ClaimsPrincipal(identity);
            return new AuthenticationState(_currentUser);
        }


        public async Task Login(UserData user)
        {
            var userJson = JsonSerializer.Serialize(user);

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authUser", userJson);

            var identity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.GivenName, user.Etunimi),
        new Claim(ClaimTypes.Surname, user.Sukunimi),
        new Claim("Nimimerkki", user.Nimimerkki),
        new Claim(ClaimTypes.Email, user.Sahkopostiosoite),
        new Claim(ClaimTypes.Role, user.Kayttajataso)
    }, "auth");

            _currentUser = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }


        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authUser");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }

    public class UserData
    {
        public int Id { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Nimimerkki { get; set; }
        public string Sahkopostiosoite { get; set; }
        public string Salasana { get; set; }
        public string Kayttajataso { get; set; }
    }

}


