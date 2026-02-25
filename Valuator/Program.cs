using StackExchange.Redis;

public class Program
{
    public static void Main(string[] args)
    {
var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    Console.WriteLine("Попытка подключения к Redis на localhost:6379...");

    try
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");

        // Проверка подключения
        var db = redis.GetDatabase();
        return redis;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка подключения к Redis: {ex.Message}");
        Console.WriteLine("\nПроверьте:");
        Console.WriteLine("1. Docker запущен: docker --version");
        Console.WriteLine("2. Redis контейнер запущен: docker ps");
        Console.WriteLine("3. Порт проброшен (должно быть 0.0.0.0:6379->6379/tcp)");
        Console.WriteLine("\nЗапустите Redis:");
        Console.WriteLine("docker run -d --name valuator-redis -p 6379:6379 redis");

        throw; 
    }
});

var app = builder.Build();

// Конфигурация middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
        app.UseStaticFiles();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
    }
}
