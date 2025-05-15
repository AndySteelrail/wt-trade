using Newtonsoft.Json;


namespace WTTrade.Initialization
{
    public class Authorization
    {
        public string TelegramToken { get; private set; }
        public string WarThunderToken { get; private set; }
        public long UserId { get; private set; }
        public string dbHost { get; private set; }
        public int dbPort { get; private set; }
        public string dbName { get; private set; }
        public string dbUsername { get; private set; }
        public string dbPassword { get; private set; }

        public Authorization()
        {
            InitializeAsync("Initialization/Tokens.json").Wait();
        }

        private async Task InitializeAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл конфигурации не найден: {filePath}");
                }

                string json = await File.ReadAllTextAsync(filePath);
                var dictTokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                    ?? throw new InvalidOperationException("Не удалось десериализовать JSON.");
                
                TelegramToken = dictTokens["TelegramToken"];
                WarThunderToken = dictTokens["WarThunderToken"];
                UserId = Convert.ToInt64(dictTokens["UserId"]);
                dbHost = dictTokens["dbHost"];
                dbPort = Convert.ToInt32(dictTokens["dbPort"]);
                dbName = dictTokens["dbName"];
                dbUsername = dictTokens["dbUsername"];
                dbPassword = dictTokens["dbPassword"];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка инициализации конфигурации.", ex);
            }
        }

        private string GetValueOrDefault(Dictionary<string, string> dict, string key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Ключ '{key}' не найден в конфигурации.");
        }

        private int GetIntValueOrDefault(Dictionary<string, string> dict, string key)
        {
            string value = GetValueOrDefault(dict, key);
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            throw new FormatException($"Не удалось преобразовать значение для ключа '{key}' в int.");
        }

        private long GetLongValueOrDefault(Dictionary<string, string> dict, string key)
        {
            string value = GetValueOrDefault(dict, key);
            if (long.TryParse(value, out long result))
            {
                return result;
            }
            throw new FormatException($"Не удалось преобразовать значение для ключа '{key}' в long.");
        }
    }
}