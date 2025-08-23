using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args); //crea el buider de la aplicacion

// registra el servicio de controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// Registrar el servicio de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {

        options.LoginPath = "/Usuario/Login"; // Ruta de login por defecto
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de la cookie 30 miniutos
        options.AccessDeniedPath = "/Usuario/Login"; // Ruta cuando el usuario no tiene permisos
    });



// Registrar autorización con roles y políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("administrador"));
    options.AddPolicy("Empleado", policy => policy.RequireRole("empleado"));
});

var app = builder.Build(); //construye la aplicacion

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Middleware para manejar errores en producción
    // El valor por defecto de HSTS es 30 días. Se recomienda cambiarlo en producción.
    app.UseHsts(); // Middleware para seguridad HTTP Strict Transport Security
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Middleware de autenticación
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
