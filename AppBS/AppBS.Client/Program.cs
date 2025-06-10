using AppBS.Shared.services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://bienes-servicios.curridabat.go.cr/") });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7244/") });

// Servicio para manejar el estado del usuario
builder.Services.AddScoped<UserStateService>();

// Agregar soporte para BlazorBootstrap
builder.Services.AddBlazorBootstrap(); // Esto es opcional si usas BlazorBootstrap en tu cliente


await builder.Build().RunAsync();
