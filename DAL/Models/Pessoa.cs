using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("Pessoas")]
    public class Pessoa : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Contato { get; set; } = string.Empty;

        // Navigation properties can be added here if needed
        // For example, if Pessoa has relationships with other entities
    }
} 