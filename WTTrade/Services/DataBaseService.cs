using Microsoft.EntityFrameworkCore;
using WTTrade.Data;
using WTTrade.Models;
using WTTrade.Models.DB;

namespace WTTrade.Services;

public class DataBaseService
{
    private readonly TradeDbContext _context;

    public DataBaseService(TradeDbContext context)
    {
        _context = context;
    }
    
    public async Task<Dictionary<string, int>> GetWhitelistAsync()
    {
        try
        {
            return await _context.Whitelists
                .AsNoTracking()
                .ToDictionaryAsync(w => w.Name, w => w.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении белого списка: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }

    public Dictionary<string, int> GetWhitelist()
    {
        try
        {
            return _context.Whitelists
                .AsNoTracking()
                .ToDictionary(w => w.Name, w => w.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении белого списка: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }
    
    public async Task<HashSet<string>> GetBlacklistAsync()
    {
        return await _context.Blacklists
            .AsNoTracking()
            .Select(b => b.Name)
            .ToHashSetAsync();
    }

    public HashSet<string> GetBlacklist()
    {
        return _context.Blacklists
            .AsNoTracking()
            .Select(b => b.Name)
            .ToHashSet();
    }
    
    public async Task<Dictionary<string, int>> GetHashNameValuesAsync()
    {
        return await _context.HashnameValues
            .AsNoTracking()
            .ToDictionaryAsync(h => h.Hashname, h => h.Value);
    }

    public Dictionary<string, int> GetHashNameValues()
    {
        return _context.HashnameValues
            .AsNoTracking()
            .ToDictionary(h => h.Hashname, h => h.Value);
    }
    
    public async Task<Dictionary<string, int>> GetNameValuesAsync()
    {
        return await _context.NameValues
            .AsNoTracking()
            .ToDictionaryAsync(n => n.Name, n => n.Value);
    }

    public Dictionary<string, int> GetNameValues()
    {
        return _context.NameValues
            .AsNoTracking()
            .ToDictionary(n => n.Name, n => n.Value);
    }
    
    public async Task InsertHashNameValueAsync(string hashName, int value)
    {
        var entity = new HashnameValue
        {
            Hashname = hashName,
            Value = value
        };

        await _context.HashnameValues.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void InsertHashNameValue(string hashName, int value)
    {
        var entity = new HashnameValue
        {
            Hashname = hashName,
            Value = value
        };

        _context.HashnameValues.Add(entity);
        _context.SaveChanges();
    }
    
    public async Task InsertNameValueAsync(string name, int value)
    {
        var entity = new NameValue
        {
            Name = name,
            Value = value
        };

        await _context.NameValues.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void InsertNameValue(string name, int value)
    {
        var entity = new NameValue
        {
            Name = name,
            Value = value
        };

        _context.NameValues.Add(entity);
        _context.SaveChanges();
    }
    
    public async Task InsertItemAsync(AppItem appItem)
    {
        var dbItem = new Item
        {
            Value = appItem.Value,
            MarketHashName = appItem.MarketHashName,
            Name = appItem.Name,
            Marketable = appItem.Marketable,
            Timestamp = appItem.Timestamp,
            Type = appItem.Type,
            Quality = appItem.Quality,
            Country = appItem.Country,
            EventName = appItem.EventName,
            SortIndex = appItem.SortIndex
        };
        
        var existing = await _context.Items
            .FirstOrDefaultAsync(i => i.Value == appItem.Value);

        if (existing != null)
        {
            existing.MarketHashName = dbItem.MarketHashName;
            existing.Name = dbItem.Name;
            existing.Marketable = dbItem.Marketable;
            existing.Timestamp = dbItem.Timestamp;
            existing.Type = dbItem.Type;
            existing.Quality = dbItem.Quality;
            existing.Country = dbItem.Country;
            existing.EventName = dbItem.EventName;
            existing.SortIndex = dbItem.SortIndex;
        }
        else
        {
            await _context.Items.AddAsync(dbItem);
        }

        await _context.SaveChangesAsync();
    }

    public void InsertItem(AppItem appItem)
    {
        var dbItem = new Item
        {
            Value = appItem.Value,
            MarketHashName = appItem.MarketHashName,
            Name = appItem.Name,
            Marketable = appItem.Marketable,
            Timestamp = appItem.Timestamp,
            Type = appItem.Type,
            Quality = appItem.Quality,
            Country = appItem.Country,
            EventName = appItem.EventName,
            SortIndex = appItem.SortIndex
        };
        
        var existing = _context.Items
            .FirstOrDefault(i => i.Value == appItem.Value);

        if (existing != null)
        {
            existing.MarketHashName = dbItem.MarketHashName;
            existing.Name = dbItem.Name;
            existing.Marketable = dbItem.Marketable;
            existing.Timestamp = dbItem.Timestamp;
            existing.Type = dbItem.Type;
            existing.Quality = dbItem.Quality;
            existing.Country = dbItem.Country;
            existing.EventName = dbItem.EventName;
            existing.SortIndex = dbItem.SortIndex;
        }
        else
        {
            _context.Items.Add(dbItem);
        }

        _context.SaveChanges();
    }
    
    public async Task<Dictionary<int, Item>> GetValueItemsAsync()
    {
        var items = await _context.Items
            .AsNoTracking()
            .ToListAsync();

        return items.ToDictionary(
            i => i.Value,
            i => new Item
            {
                Value = i.Value,
                MarketHashName = i.MarketHashName,
                Name = i.Name,
                Marketable = i.Marketable,
                Timestamp = i.Timestamp,
                Type = i.Type,
                Quality = i.Quality,
                Country = i.Country,
                EventName = i.EventName,
                SortIndex = i.SortIndex
            });
    }

    public Dictionary<int, AppItem> GetValueItems()
    {
        var items = _context.Items
            .AsNoTracking()
            .ToList();

        return items.ToDictionary(
            i => i.Value,
            i => new AppItem
            {
                Value = i.Value,
                MarketHashName = i.MarketHashName,
                Name = i.Name,
                Marketable = i.Marketable,
                Timestamp = i.Timestamp,
                Type = i.Type,
                Quality = i.Quality,
                Country = i.Country,
                EventName = i.EventName,
                SortIndex = i.SortIndex
            });
    }
}