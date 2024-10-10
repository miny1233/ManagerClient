using ManagerClient.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context,options) => {
    options.ListenAnyIP(1233, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        //listenOptions.UseHttdps();
    });
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddAntDesign();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
}
// Logger
//app.UseW3CLogging();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

