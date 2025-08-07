using Application.MappingProfiles;
using Infrastructure.MappingProfiles;
using Mapster;
using MapsterMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new TypeAdapterConfig();
config.Scan(typeof(ApplicationMappingProfile).Assembly);
config.Scan(typeof(InfrastructureMappingProfile).Assembly);
builder.Services.AddSingleton(config);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();