using System;
using System.ComponentModel.DataAnnotations;
namespace Proyecto.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime Expira { get; set; }

        public bool Usado { get; set; } = false;
    }
}
