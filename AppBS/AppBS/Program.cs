using AppBS.Client.Pages;
using AppBS.Components;
using ServiceReferenceSICOP; // Espacio de nombres generado
using ServiceReferenceFinancieroContable;
using System;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using AppBS.SOAP;
using AppBS.Shared.services;
using AppBS.DAO;
using Microsoft.EntityFrameworkCore;
using AppBS.Services;
var builder = WebApplication.CreateBuilder(args);


//Configuraci�n de controladores y servicios HTTP
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Configuración de servicios de autenticación

// Inyección del servicio de estado del usuario  //// Registra un servicio personalizado `UserStateService` con ciclo de vida Scoped (instancia única por solicitud).
builder.Services.AddScoped<UserStateService>();

builder.Services.AddScoped<SicopService>();

// Inyección del servicio de estado del Reporteador para inyectalo en los controladores
builder.Services.AddScoped<ReporteService>();




// Configurar cliente SOAP para SICOP
builder.Services.AddScoped<CataWebServiceClient>(provider =>
{
    var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) // Transport para HTTPS
    {
        MaxReceivedMessageSize = 65536, // Permite respuestas grandes
        ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
        {
            MaxDepth = 32,
            MaxStringContentLength = 8192,
            MaxArrayLength = 16384,
            MaxBytesPerRead = 4096,
            MaxNameTableCharCount = 16384
        }
    };

    // Definir el endpoint del servicio SOAP
    var endpoint = new EndpointAddress("https://www.SICOP.go.cr:8080/CataItemInfoWS/CataItemInfoWService");

    // Retornar la instancia del cliente SOAP
    return new CataWebServiceClient(binding, endpoint);
});

// Agregar cliente del servicio SOAP (WCF) servicios de Sistemas Administrativos financieros
builder.Services.AddScoped<ServiceReferenceFinancieroContable.WSSoapClient>(provider =>
{
    var binding = new System.ServiceModel.BasicHttpBinding();
    var endpoint = new System.ServiceModel.EndpointAddress("http://wsconsultasistemasfinancieros.curridabat.go.cr/ws.asmx");
    return new ServiceReferenceFinancieroContable.WSSoapClient(binding, endpoint);
});


// Agregar cliente del servicio SOAP (WCF) servicios de Recursos Humanos
builder.Services.AddScoped<ServiceReferenceRecursosHumano.WS_RecursosHumanosSoapClient>(provider =>
{
    var binding = new System.ServiceModel.BasicHttpBinding();
    var endpoint = new System.ServiceModel.EndpointAddress("http://gestiones-administrativas.curridabat.go.cr/Ws-recursoshumanos.asmx");
    return new ServiceReferenceRecursosHumano.WS_RecursosHumanosSoapClient(binding, endpoint);
});



// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("http://bienes-servicios.curridabat.go.cr/") // Cambia los dominios según sea necesari-https://localhost:7244--http://bienes-servicios.curridabat.go.cr
              .AllowAnyHeader() // Permite cualquier encabezado en las solicitudes (por ejemplo, autenticación).
              .AllowAnyMethod() // Permite cualquier método HTTP (GET, POST, PUT, DELETE, etc.).
              .AllowCredentials(); // Permite el uso de cookies o credenciales en las solicitudes.
    });
});


/*configuración inicial de la primera coneccción a los servicios de la base de datos del propipo Sistema */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);



var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Usar CORS
app.UseCors("AllowAll");


// Mapear los controladores API
app.MapControllers(); // Configura los endpoints de los controladores definidos en el proyecto (por ejemplo, rutas de API).


app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AppBS.Client._Imports).Assembly);

app.Run();
