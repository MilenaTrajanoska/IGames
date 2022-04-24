using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using IGames.Domain.DomainModels;

namespace IGames.Domain.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
