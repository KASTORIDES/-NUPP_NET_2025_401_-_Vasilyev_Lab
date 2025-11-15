using Microsoft.EntityFrameworkCore;
using StudentSystem.Common;
using StudentSystem.Infrastructure;
using StudentSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// тот же SQLite-файл, что и в консольном приложении
var dbPath = Path.Combine(AppContext.BaseDirectory, "students.db");
builder.Services.AddDbContext<StudentSystemContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// DI для репозитория и CRUD-сервиса
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICrudServiceAsync<>), typeof(CrudServiceAsync<>));

// Контроллеры
builder.Services.AddControllers();

// Встроенный OpenAPI из шаблона
builder.Services.AddOpenApi();   // ← вместо AddSwaggerGen
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();            // ← вместо UseSwagger/UseSwaggerUI
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentSystemContext>();
    db.Database.Migrate();
}

app.Run();