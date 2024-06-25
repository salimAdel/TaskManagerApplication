using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIS.Data;
using SIS.Helper;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DBContext>(
                options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
               // .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                );
//services.AddDefaultIdentity<IdentityUser, IdentityRole>()
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddScoped<UserManager<User>, UserManager<User>>();
//configute custome helper
builder.Services.AddScoped<RoleHelper>();
builder.Services.AddScoped<FilesHelper>();
builder.Services.Configure<IdentityOptions>(opt =>
{
    // Password settings 
    opt.Password.RequireDigit = true;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    // Lockout settings 
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    opt.Lockout.MaxFailedAccessAttempts = 5;

    //Signin option
    opt.SignIn.RequireConfirmedEmail = false;

    // User settings 
    opt.User.RequireUniqueEmail = true;

    //Token Option
    opt.Tokens.AuthenticatorTokenProvider = "Name of AuthenticatorTokenProvider";

});

// Cookie settings 
builder.Services.ConfigureApplicationCookie(options =>
{
    //options.Cookie.Expiration = TimeSpan.FromDays(150);
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.SlidingExpiration = true;
});
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<DBContext>()
//    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});
builder.Services.AddRazorPages();
//.AddDefaultUI();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
var supportedCultures = new[]
{
 new CultureInfo("en-US"),
 new CultureInfo("fr"),
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-GB"),
    // Formatting numbers, dates, etc.
    SupportedCultures = supportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = supportedCultures
});
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
             name: "default_area",
             pattern: "{area=dashboard}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
