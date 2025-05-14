using ApiGradeProject.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<PostgresContext>();

// Добавляем Swagger и API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var serviceProvider = app.Services;

// Запускаем фоновую задачу для удаления просроченных токенов
var backgroundTask = new Task(async () =>
{
    while (true)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
            var expiredTokens = await context.Tokensses
                .Where(t => t.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Count > 0)
            {
                context.Tokensses.RemoveRange(expiredTokens);
                await context.SaveChangesAsync();
                Console.WriteLine($"Удалено токенов: {expiredTokens.Count}");
            }
        }

        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}, TaskCreationOptions.LongRunning);

backgroundTask.Start();

if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader());
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();