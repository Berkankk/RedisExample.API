using Microsoft.EntityFrameworkCore;

using RedisExample.API.Models;
using RedisExample.API.Repository;
using RedisExampleApp.Cache.Service;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductRepository>(sp =>
{
    var appDbContext = sp.GetRequiredService<AppDbContext>();

    var productRepository = new ProductRepository(appDbContext);

    var redisService = sp.GetRequiredService<RedisService>();

    var cacheDb = sp.GetRequiredService<IDatabase>(); // IDatabase baðýmlýlýðýný ekliyoruz

    return new ProductRepositoryWithCache(productRepository, redisService, cacheDb); // Üç baðýmlýlýðý da geçiyoruz
});




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("myDatabase");
});

// RedisService'i Build'den önce ekleyin
builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration["CacheOptions:Url"]);
});

builder.Services.AddSingleton<IDatabase>(sp =>
{
    var redisService = sp.GetRequiredService<RedisService>();
    return redisService.GetDb(0);

});


var app = builder.Build();  // Servisleri eklediðiniz yerin sonuna geliyoruz

using var scope = app.Services.CreateScope();
var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbcontext.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
