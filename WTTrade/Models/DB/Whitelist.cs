using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WTTrade.Models.DB;

[Table("whitelist")]
[Index(nameof(Name), IsUnique = true, Name = "whitelist_name_unique")]
public class Whitelist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column("count")]
    [Range(0, int.MaxValue, ErrorMessage = "Count must be non-negative")]
    public int Count { get; set; }
}