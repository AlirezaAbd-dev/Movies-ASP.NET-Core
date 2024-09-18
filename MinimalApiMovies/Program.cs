using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MinimalApiMovies;
using MinimalApiMovies.Endpoints;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Repositories;
using MinimalApiMovies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("name=DefaultConnection")
    );

builder.Services.AddCors(options => {
    options.AddPolicy(name: "free", policy => {
        policy.WithOrigins(builder.Configuration["allowedOrigins"]!)
            .AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseCors("free");
app.UseOutputCache();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => {
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature!.Error;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();

    await repository.Create(error);

    await Results.BadRequest(new {
        type = "error",
        message = "an unexpected exception has occured",
        status = 500
    }).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.MapGet("/error", () => {
    throw new InvalidOperationException("exampleError");
});

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/commets").MapComments();

app.Run();

