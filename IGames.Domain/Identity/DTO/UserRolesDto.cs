using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Domain.Identity.DTO
{
    public class UserRolesDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
