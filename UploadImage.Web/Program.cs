using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UploadImage.Web;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

    // Configurar o CORS
    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");

    return httpClient;
});
builder.Services.AddMudServices();

await builder.Build().RunAsync();
