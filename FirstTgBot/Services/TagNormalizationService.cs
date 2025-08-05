using System.Collections.Generic;

namespace FirstTgBot.Services;

public class TagNormalizationService
{
    private readonly Dictionary<string, string> _tagMappings = new()
    {
        // Персонажи Touhou
        ["reimu"] = "hakurei_reimu",
        ["reimu hakurei"] = "hakurei_reimu",
        ["hakurei reimu"] = "hakurei_reimu",
        ["reimu_hakurei"] = "hakurei_reimu",
        ["hakurei_reimu"] = "hakurei_reimu",

        ["marisa"] = "kirisame_marisa",
        ["marisa kirisame"] = "kirisame_marisa",
        ["kirisame marisa"] = "kirisame_marisa",
        ["marisa_kirisame"] = "kirisame_marisa",
        ["kirisame_marisa"] = "kirisame_marisa",

        // Общие теги
        ["school uniform"] = "school_uniform",
        ["long hair"] = "long_hair",
        ["short hair"] = "short_hair",
        ["blue eyes"] = "blue_eyes",
        ["brown hair"] = "brown_hair"
    };

    public string NormalizeTags(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        var normalizedInput = input.ToLowerInvariant().Trim();

        if (_tagMappings.TryGetValue(normalizedInput, out var exactMatch))
            return exactMatch;

        return normalizedInput.Replace(' ', '_');
    }
}