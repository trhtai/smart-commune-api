using SmartCommune.Api;

// 1. CREATE BUILDER
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Environment);

// 2. BUILD APP
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();