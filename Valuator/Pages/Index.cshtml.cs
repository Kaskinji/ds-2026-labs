using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;
    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    [TempData]
    public string ErrorMessage { get; set; }
    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        if (!_redis.IsConnected)
        {
            _logger.LogError("Redis не подключен!");
            throw new InvalidOperationException("Redis не доступен");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            _logger.LogWarning("Пользователь отправил пустой текст");
            ErrorMessage = "Ошибка: Текст не может быть пустым. Пожалуйста, введите текст.";
            return RedirectToPage(); 
        }

        string id = Guid.NewGuid().ToString();
        IDatabase db = _redis.GetDatabase();

        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey
        db.StringSet(textKey, text);

        string rankKey = "RANK-" + id;
        // TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey
        // Вычисляем rank (доля неалфавитных символов)
        double rank = CalculateRank(text);
        db.StringSet(rankKey, rank.ToString());

        string similarityKey = "SIMILARITY-" + id;
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        // Проверяем на дубликаты
        double similarity = CheckForDuplicate(db, text, id);
        db.StringSet(similarityKey, similarity.ToString());
        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0.0;

        int totalChars = text.Length;
        int nonAlphabetChars = 0;

        foreach (char c in text)
        {
            if (!IsAlphabetic(c))
            {
                nonAlphabetChars++;
            }
        }

        return (double)nonAlphabetChars / totalChars;
    }

    private bool IsAlphabetic(char c)
    {
        return char.IsLetter(c);
    }

    private double CheckForDuplicate(IDatabase db, string text, string currentId)
    {
        // Получаем все ключи, начинающиеся с TEXT-
        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());

        var keys = server.Keys(pattern: "TEXT-*");

        foreach (var key in keys)
        {
            // Пропускаем текущий текст
            if (key.ToString().EndsWith(currentId))
            {
                continue;
            }

            string storedText = db.StringGet(key);
            if (storedText == text)
            {
                _logger.LogDebug($"Найден дубликат с ключом: {key}");
                return 1.0;
            }
        }

        return 0.0;
    }
}
