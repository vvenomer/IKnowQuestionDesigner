﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models
{
    public class ApplicationUser : IdentityUser
    {

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
