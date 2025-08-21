using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class UserProfile
    {
        public DateTime Birthday { get; set; }
        public string Email { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
