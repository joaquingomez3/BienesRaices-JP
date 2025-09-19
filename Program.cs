using Microsoft.AspNetCore.Authentication.Cookies;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Identity;
using bienesraices.Models;

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
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("empleado", policy => policy.RequireRole("empleado"));
});

var app = builder.Build(); //construye la aplicacion

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Middleware para manejar errores en producción
    // El valor por defecto de HSTS es 30 días. Se recomienda cambiarlo en producción.
    app.UseHsts(); // Middleware para seguridad HTTP Strict Transport Security
}


app.UseStaticFiles();// habilita archivos estáticos
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Middleware de autenticación
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}")
    .WithStaticAssets();


//precarga admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();
    var repo = new RepositorioUsuario(config);

    var emailAdmin = "admin@inmobiliaria.com";
    var existe = repo.ObtenerUsuarioPorEmail(emailAdmin);
    if (existe == null)
    {

        var hasher = new PasswordHasher<Usuario>();
        var admin = new Usuario
        {
            Nombre_usuario = "Juan",
            Apellido_usuario = "Pérez",
            Email = emailAdmin,
            Id_tipo_usuario = 1,
            Activo = true,
            Foto = null
        };
        admin.Password = hasher.HashPassword(admin, "Admin1234");

        repo.Alta(admin);
        Console.WriteLine("✅ Usuario administrador creado automáticamente.");
    }
    else
    {
        Console.WriteLine("ℹ️ El administrador ya existe.");
    }
}
app.Run();
