using BlogMVC.Configuraciones;
using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Jobs;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
 
// configuracion variables ConfiguracionesIA
builder.Services.AddOptions<ConfiguracionesIA>()
    .Bind(builder.Configuration.GetSection(ConfiguracionesIA.Seccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Agregar servicios de openAI
builder.Services.AddScoped(sp =>
{
    var configuracionesIA = sp.GetRequiredService<IOptions<ConfiguracionesIA>>();
    return new OpenAIClient(configuracionesIA.Value.LlaveOpenAI);
});

// Agregar blazor del lado del servidor
builder.Services.AddServerSideBlazor();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Subir archivos
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IServicioChat, ServicioChatOpenAI>();

// Configurar tarea de fondo
builder.Services.AddHostedService<AnalisisSentimientosRecurrente>();

// Configurar bd
builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection") 
.UseSeeding(Seeding.Aplicar) // Aplicar Seedings
.UseAsyncSeeding(Seeding.AplicarAsync)
);

// Configurar Identity.EntityFrameworkCore
builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// configurar urls por defecto del login
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/login";
        opciones.AccessDeniedPath = "/usuarios/login";
    } 
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Balzor
app.MapBlazorHub();

app.Run();
