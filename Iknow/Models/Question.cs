using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models
{
    public class Question
    {
        public int ID { get; set; }

        [Display(Name = "Hint 1")]
        public string hint1 { get; set; }

        [Display(Name = "Hint 2")]
        public string hint2 { get; set; }

        [Display(Name = "Hint 3")]
        public string hint3 { get; set; }

        [Display(Name = "Answer")]
        public string answer { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual QuestionType questionType { get; set; }
    }
}
