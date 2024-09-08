using Microsoft.EntityFrameworkCore;
using MinimalApiMovies;
using MinimalApiMovies.Endpoints;
using MinimalApiMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddCors(options => {
    options.AddPolicy(name: "free", policy => {
        policy.WithOrigins(builder.Configuration["allowedOrigins"]!).AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("free");
app.UseOutputCache();

app.MapGet("/", () => {
    return "Hello World";
});

 app.MapGroup("/genres").MapGenres();

app.Run();

