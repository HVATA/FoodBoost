
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using varjoBlazor;
using varjoBlazor.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");





builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7048/") }); //serverin osoite
builder.Services.AddSingleton<TunnistaKayttaja>();


await builder.Build().RunAsync();
