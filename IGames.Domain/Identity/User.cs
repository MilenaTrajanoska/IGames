using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IGames.Domain.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Cart UserCart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
