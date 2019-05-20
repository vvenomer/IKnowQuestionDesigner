using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models
{
    public class Category
    {
        public int ID { get; set; }


        [Display(Name = "Category Name")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Choosen")]
        public bool locked { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<QuestionType> questionTypes { get; set; }
    }
}
