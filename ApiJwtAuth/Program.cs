var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => {
        opt.SwaggerEndpoint("/openapi/v1.json", "Auth");
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.Run();

