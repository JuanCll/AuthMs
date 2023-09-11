using System.ComponentModel.DataAnnotations;

namespace AuthMs.Models
{
    public class Credential
    {
        [Key]
        public int UserId { get; set; }
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
