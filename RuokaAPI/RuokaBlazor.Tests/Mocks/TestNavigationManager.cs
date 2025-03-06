using Microsoft.AspNetCore.Components;

public class TestNavigationManager : NavigationManager
    {
    public TestNavigationManager ()
        {
        Initialize("http://localhost/", "http://localhost/");
        }

    protected override void NavigateToCore ( string uri, bool forceLoad )
        {
        Uri = uri;
        }

    public new string Uri { get; private set; }
    }
