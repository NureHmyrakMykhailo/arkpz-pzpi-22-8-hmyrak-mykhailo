using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
