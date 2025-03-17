using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuokaBlazor.Tests.Mocks
{
    // 🔹 Mock NavigationManager (pakollinen, koska Bunit ei tue oletuksena suoraa navigointia)
    public class MockNavigationManager : NavigationManager
    {
        public string LastUri { get; private set; } = string.Empty;

        public MockNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            LastUri = new Uri(new Uri(BaseUri), uri).ToString(); // Muodostetaan koko URL
            Uri = LastUri; // Päivitetään myös NavigationManager.Uri
        }
    }

}
