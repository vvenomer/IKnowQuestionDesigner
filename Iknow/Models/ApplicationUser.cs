using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "User Name")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
