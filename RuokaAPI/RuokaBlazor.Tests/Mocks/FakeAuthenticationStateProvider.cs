using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RuokaBlazor.Services;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

public class FakeAuthenticationStateProvider : CustomAuthenticationStateProvider
    {
    private readonly ClaimsPrincipal _user;

    public FakeAuthenticationStateProvider ( ClaimsPrincipal user )
        : base(new FakeJSRuntime()) // Käytetään fake IJSRuntimeä testeissä
        {
        _user = user;
        }

    public override Task<AuthenticationState> GetAuthenticationStateAsync ()
        {
        return Task.FromResult(new AuthenticationState(_user));
        }
    }

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
