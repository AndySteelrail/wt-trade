using Npgsql;
using WTTrade.Models;


namespace WTTrade.Services;

public class DataBaseService
{
    private readonly string _connectionString;

    public DataBaseService(string host, int port, string name, string userName, string password)
    {
        _connectionString = $"Host={host};Port={port};Database={name};Username={userName};Password={password};";
    }

    private T MakeQuery<T>(string query, Func<NpgsqlDataReader, T> mapper, params NpgsqlParameter[] parameters)
    {
        using (var _connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(query, _connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        return mapper(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при выполнении запроса к базе данных.", ex);
            }
        }
    }

    private void ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)                                                                                                                                                                                                                                                        
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при выполнении запроса к базе данных.", ex);
            }
        }
    }

    public Dictionary<string, int> GetWhitelist()
    {
        return MakeQuery<Dictionary<string, int>>(
            "SELECT * FROM whitelist;",
            reader =>
            {
                var dictionary = new Dictionary<string, int>();
                while (reader.Read())
                {
                    string name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    int count = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dictionary[name] = count;
                }
                return dictionary;
            });
    }

    public HashSet<string> GetBlacklist()
    {
        return MakeQuery<HashSet<string>>(
            "SELECT * FROM blacklist;",
            reader =>
            {
                var set = new HashSet<string>();
                while (reader.Read())
                {
                    string name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    set.Add(name);
                }
                return set;
            });
    }

    public Dictionary<string, int> GetHashNameValues()
    {
        return MakeQuery<Dictionary<string, int>>(
            "SELECT * FROM hashname_values;",
            reader =>
            {
                var dictionary = new Dictionary<string, int>();
                while (reader.Read())
                {
                    string hashName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    int value = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dictionary[hashName] = value;
                }
                return dictionary;
            });
    }

    public Dictionary<string, int> GetNameValues()
    {
        return MakeQuery<Dictionary<string, int>>(
            "SELECT * FROM name_values;",
            reader =>
            {
                var dictionary = new Dictionary<string, int>();
                while (reader.Read())
                {
                    string name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    int value = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dictionary[name] = value;
                }
                return dictionary;
            });
    }

    public void InsertHashNameValue(string hashName, int value)
    {
        ExecuteNonQuery(
            "INSERT INTO hashname_values (hashname, value) VALUES (@hashname, @value);",
            new NpgsqlParameter("@hashname", hashName),
            new NpgsqlParameter("@value", value));
    }

    public void InsertNameValue(string name, int value)
    {
        ExecuteNonQuery(
            "INSERT INTO name_values (name, value) VALUES (@name, @value);",
            new NpgsqlParameter("@name", name),
            new NpgsqlParameter("@value", value));
    }
    
    public void InsertItem(Item item)
    {
        const string query = @"
            INSERT INTO items (
                value, 
                market_hash_name, 
                name, 
                marketable, 
                timestamp,
                type,
                quality,
                country,
                event_name,
                sort_index
            )
            VALUES (
                @value, 
                @marketHashName, 
                @name, 
                @marketable, 
                @timestamp,
                @type,
                @quality,
                @country,
                @eventName,
                @sortIndex
            )
            ON CONFLICT (value) DO UPDATE SET
                market_hash_name = EXCLUDED.market_hash_name,
                name = EXCLUDED.name,
                marketable = EXCLUDED.marketable,
                timestamp = EXCLUDED.timestamp,
                type = EXCLUDED.type,
                quality = EXCLUDED.quality,
                country = EXCLUDED.country,
                event_name = EXCLUDED.event_name,
                sort_index = EXCLUDED.sort_index;";

        ExecuteNonQuery(query,
            new NpgsqlParameter("@value", item.Value),
            new NpgsqlParameter("@marketHashName", item.MarketHashName),
            new NpgsqlParameter("@name", item.Name),
            new NpgsqlParameter("@marketable", item.Marketable),
            new NpgsqlParameter("@timestamp", item.Timestamp),
            new NpgsqlParameter("@type", item.Type ?? (object)DBNull.Value),
            new NpgsqlParameter("@quality", item.Quality ?? (object)DBNull.Value),
            new NpgsqlParameter("@country", item.Country ?? (object)DBNull.Value),
            new NpgsqlParameter("@eventName", item.EventName ?? (object)DBNull.Value),
            new NpgsqlParameter("@sortIndex", item.SortIndex));
    }

    public Dictionary<int, Item> GetValueItems()
    {
        const string query = "SELECT * FROM items;";
        
        return MakeQuery(query, reader =>
        {
            var result = new Dictionary<int, Item>();
            while (reader.Read())
            {
                var item = new Item
                {
                    Value = reader.GetInt32(0),
                    MarketHashName = reader.GetString(1),
                    Name = reader.GetString(2),
                    Marketable = reader.GetBoolean(3),
                    Timestamp = reader.GetInt64(4),
                    Type = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Quality = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Country = reader.IsDBNull(7) ? null : reader.GetString(7),
                    EventName = reader.IsDBNull(8) ? null : reader.GetString(8),
                    SortIndex = reader.IsDBNull(9) ? 1 : reader.GetInt32(9)
                };
                result.Add(item.Value, item);
            }
            return result;
        });
    }
}