using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANPaX.IO.DTO
{
    [Table("User")]
    public class UserDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string User { get; set; }
        public string EMail { get; set; }
    }
}
