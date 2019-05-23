using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models.DataWith
{
    public class QuestionWithType : Question
    {
        public QuestionWithType()
        {

        }
        public QuestionWithType(Question question, string type)
        {
            answer = question.answer;
            hint1 = question.hint1;
            hint2 = question.hint2;
            hint3 = question.hint3;
            this.type = type;
        }
        [Display(Name = "Type")]
        public string type { get; set; }
    }
}
