using azure_openai_quickstart.Services;
using InnovationInc.TextToSql.WebApi.Factories;
using InnovationInc.TextToSql.WebApi.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAIClientFactory, AzureOpenAIClientFactory>();
builder.Services.AddSingleton<IDbService, SQLiteService>();
builder.Services.AddSingleton<IAIService, AzOpenAIService>();

var app = builder.Build();

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
