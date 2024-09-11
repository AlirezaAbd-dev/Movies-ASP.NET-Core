using Microsoft.EntityFrameworkCore;
using MinimalApiMovies;
using MinimalApiMovies.Endpoints;
using MinimalApiMovies.Repositories;
using MinimalApiMovies.Services;

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
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();

builder.Services.AddTransient<IFileStorage,LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors("free");
app.UseOutputCache();

app.MapGet("/", () =>"Hello World");

 app.MapGroup("/genres").MapGenres();
 app.MapGroup("/actors").MapActors();

app.Run();

