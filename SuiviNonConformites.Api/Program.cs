using Microsoft.EntityFrameworkCore;
using SuiviNonConformites.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Déclaration des services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    }); //Ce que ça fait concrètement : dès que le sérialiseur JSON détecte qu'il est en train de resérialiser un objet déjà rencontré dans la chaîne actuelle, il omet simplement cette référence (elle n'apparaît pas dans le JSON) plutôt que de planter. Concrètement, dans ta réponse GET /api/Audits/1, chaque NonConformite listée n'aura pas son audit réaffiché en boucle (puisque tu es déjà en train de le lire depuis cet audit) — ce qui est logiquement correct, juste un peu moins explicite visuellement.
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configuration du pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SuiviNonConformites API");
    });   
}

app.UseHttpsRedirection();

app.UseCors("AngularDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
