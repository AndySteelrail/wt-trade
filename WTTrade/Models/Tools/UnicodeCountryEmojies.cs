namespace WTTrade.Models;

public class UnicodeCountryEmojies
{
    private static Dictionary<string, string> dictEmojies = new Dictionary<string, string>()
    {
        { "США", "\U0001F1FA\U0001F1F8" },
        { "СССР", "\U0001F1F7\U0001F1FA" },
        { "Германия", "\U0001F1E9\U0001F1EA" },
        { "Британия", "\U0001F1EC\U0001F1E7" },
        { "Франция", "\U0001F1EB\U0001F1F7" },
        { "Италия", "\U0001F1EE\U0001F1F9" },
        { "Япония", "\U0001F1EF\U0001F1F5" },
        { "Китай", "\U0001F1E8\U0001F1F3" },
        { "Швеция", "\U0001F1F8\U0001F1EA" },
        { "Израиль", "\U0001F1EE\U0001F1F1" },
    };

    public static string GetEmoji(string country)
    {
        if (country == null)
        {
            return "";
        }
        return dictEmojies.GetValueOrDefault(country, "");
    }
}