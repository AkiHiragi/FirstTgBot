using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FirstTgBot.Models;

namespace FirstTgBot.Services;

public class YandereService(HttpClient httpClient, TagNormalizationService tagService)
{
    private const string BaseUrl = "https://yande.re/post.json";

    public async Task<YanderePost?> GetRandomImageAsync(string tags = "", string rating ="s")
    {
        var normalizedTags = tagService.NormalizeTags(tags);
        var url = $"{BaseUrl}?tags={normalizedTags}&rating={rating}&limit=20";
        var response = await httpClient.GetStringAsync(url);
        var posts = JsonSerializer.Deserialize<YanderePost[]>(response);

        if (posts?.Length > 0)
            return posts[Random.Shared.Next(posts.Length)];

        return null;
    }
}