using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductApi.Data;
using ProductApi.Models;
using ProductApi.Services;
using Scalar.AspNetCore;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

    });




builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Categories.Any())
    {
        var home = new Category { Name = "Home", CreatedAt = DateTime.UtcNow };
        var electronics = new Category { Name = "Electronics", CreatedAt = DateTime.UtcNow };
        var sports = new Category { Name = "Sports", CreatedAt = DateTime.UtcNow };

        db.Categories.AddRange(home, electronics, sports);
        await db.SaveChangesAsync(); 

        db.Products.AddRange(
            new Product
            {
                Name = "Laptop Pro",
                CategoryId = electronics.Id,         
                Price = 1299m,
                StockQuantity = 15,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Wireless Mouse",
                CategoryId = electronics.Id,               
                Price = 49.99m,
                StockQuantity = 0,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Coffee Maker",
                CategoryId = home.Id,
                Price = 89.99m,
                StockQuantity = 30,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Yoga Mat",
                CategoryId = sports.Id,
                Price = 29.99m,
                StockQuantity = 100,
                CreatedAt = DateTime.UtcNow
            }
        );

        await db.SaveChangesAsync();
    }
}



if (app.Environment.IsDevelopment())
{

    app.MapOpenApi(); 

    
        app.MapScalarApiReference(options =>
        {
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecuritySchemes = new List<string> { "Bearer" }  
            };
        });
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ResourceNotFoundMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();
app.Run();