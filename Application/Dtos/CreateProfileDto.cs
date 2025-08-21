using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class CreateProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public List<int> Roles { get; set; } = [];

    }
}
