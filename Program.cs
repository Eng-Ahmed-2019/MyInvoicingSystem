using Microsoft.AspNetCore.Authentication.JwtBearer;
using InvoicingSystem.Services.Implementations;
using InvoicingSystem.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using InvoicingSystem.Middleware;
using Microsoft.OpenApi.Models;
using InvoicingSystem.Services;
using InvoicingSystem.Filter;
using InvoicingSystem.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddSingleton<FileLoggerService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var lang = new[] { "en", "ar" };
    options.SetDefaultCulture("en")
    .AddSupportedCultures(lang)
    .AddSupportedUICultures(lang);
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateInvoice", policy =>
        policy.RequireRole("Admin", "Accountant"));

    options.AddPolicy("ViewInvoice", policy =>
        policy.RequireRole("Admin", "Accountant", "User"));

    options.AddPolicy("CreateInvoiceItems", policy =>
        policy.RequireRole("Admin", "Manager"));
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoicing System API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token (e.g. 'Bearer eyJhbGciOi...')"
    });
    c.AddSecurityDefinition("Accept-Language", new OpenApiSecurityScheme
    {
        Name = "Accept-Language",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Language for localization (e.g. 'en' or 'ar')"
    });
    c.AddSecurityDefinition("X-Company-Id", new OpenApiSecurityScheme
    {
        Name = "X-Company-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Company ID to identify the tenant"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Company-Id"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Accept-Language"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoicing System API v1");
    });
}

app.UseHttpsRedirection();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthentication();
app.UseTenantMiddleware();
app.UseAuthorization();
app.UseRequestLocalization();
app.MapControllers();

//Execute seed date one time in this project
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.SeedDatabase();
}

app.Run();