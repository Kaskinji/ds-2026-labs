using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        IDatabase db = _redis.GetDatabase();

        Rank = GetDoubleValueFromRedis(db, "RANK-" + id);
        Similarity = GetDoubleValueFromRedis(db, "SIMILARITY-" + id);
    }

    private double GetDoubleValueFromRedis(IDatabase db, string key)
    {
        string value = db.StringGet(key);
        return !string.IsNullOrEmpty(value) && double.TryParse(value, out double result)
            ? result
            : 0.0;
    }
    //избавиться от дублирования
}