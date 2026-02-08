using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WTTrade.Models.DB;

[Table("items")]
[Index(nameof(MarketHashName), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class Item
{
    [Key]
    [Column("value")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Value { get; set; }

    [Column("market_hash_name")]
    [Required]
    [MaxLength(500)]
    public string MarketHashName { get; set; } = string.Empty;

    [Column("name")]
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;

    [Column("marketable")]
    [Required]
    public bool Marketable { get; set; }

    [Column("timestamp")]
    [Required]
    public long Timestamp { get; set; }

    [Column("type")]
    [MaxLength(100)]
    public string? Type { get; set; }

    [Column("quality")]
    [MaxLength(100)]
    public string? Quality { get; set; }

    [Column("country")]
    [MaxLength(100)]
    public string? Country { get; set; }

    [Column("event_name")]
    [MaxLength(200)]
    public string? EventName { get; set; }

    [Column("sort_index")]
    [DefaultValue(1)]
    public int SortIndex { get; set; } = 1;
}