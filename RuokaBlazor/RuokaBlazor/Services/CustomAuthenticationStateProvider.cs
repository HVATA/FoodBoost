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
                new Claim(ClaimTypes.Name, userData.Email),
                new Claim(ClaimTypes.Role, userData.Role)
            }, "auth");

            _currentUser = new ClaimsPrincipal(identity);
            return new AuthenticationState(_currentUser);
        }

        public async Task Login(string email, string role)
        {
            var userData = new UserData { Email = email, Role = role };
            var userJson = JsonSerializer.Serialize(userData);

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authUser", userJson);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
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
        public string Email { get; set; }
        public string Role { get; set; }
    }
}


