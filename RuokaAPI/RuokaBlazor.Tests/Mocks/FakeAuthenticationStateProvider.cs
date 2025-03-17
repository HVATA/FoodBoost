using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RuokaBlazor.Services;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

public class FakeAuthenticationStateProvider : CustomAuthenticationStateProvider
    {
    private ClaimsPrincipal _user;

    public FakeAuthenticationStateProvider ( ClaimsPrincipal user )
        : base(new FakeJSRuntime()) // Käytetään fake IJSRuntimea testeissä
        {
        _user = user ?? new ClaimsPrincipal(new ClaimsIdentity()); // Varmistetaan, ettei _user ole null
        }

    public override Task<AuthenticationState> GetAuthenticationStateAsync ()
        {
        return Task.FromResult(new AuthenticationState(_user));
        }

    // Ylikirjoitetaan IsUserLoggedIn niin, että se palauttaa oikean arvon testitilanteessa
    public override Task<bool> IsUserLoggedIn ()
        {
        return Task.FromResult(_user.Identity != null && _user.Identity.IsAuthenticated);
        }

    // (Valinnainen) Voit säilyttää tämän metodin, jos sitä haluat erikseen testien käyttöön
    public Task<bool> FakeIsUserLoggedIn ()
        {
        return Task.FromResult(_user.Identity != null && _user.Identity.IsAuthenticated);
        }

    // Päivitetään käyttäjä testin aikana ja pakotetaan renderöinti
    public void SetUser ( ClaimsPrincipal user )
        {
        _user = user ?? new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

// Fake JSRuntime, jotta testit eivät kaadu
public class FakeJSRuntime : IJSRuntime
    {
    public ValueTask<TValue> InvokeAsync<TValue> ( string identifier, object?[]? args )
        {
        return new ValueTask<TValue>(default(TValue)!);
        }

    public ValueTask<TValue> InvokeAsync<TValue> ( string identifier, CancellationToken cancellationToken, object?[]? args )
        {
        return new ValueTask<TValue>(default(TValue)!);
        }
    }
