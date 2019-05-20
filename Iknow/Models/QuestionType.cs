using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models
{
    public class QuestionType
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Type")]
        public string type { get; set; }
        public virtual Category category { get; set; }
        public virtual ICollection<Question> questions { get; set; }
    }
}
