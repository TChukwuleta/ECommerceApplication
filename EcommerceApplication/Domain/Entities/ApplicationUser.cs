﻿using EcommerceApplication.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApplication.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }

    }
}
