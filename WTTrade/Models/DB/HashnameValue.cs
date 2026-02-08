using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTTrade.Models.DB;

[Table("hashname_values")]
public class HashnameValue
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("hashname")]
    [Required]
    [MaxLength(255)]
    public string Hashname { get; set; } = string.Empty;

    [Column("value")]
    [Required]
    public int Value { get; set; }
}