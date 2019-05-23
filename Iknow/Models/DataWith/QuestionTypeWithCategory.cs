using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models.DataWith
{
    public class QuestionTypeWithCategory : QuestionType
    {
        public QuestionTypeWithCategory()
        { }

        public QuestionTypeWithCategory(QuestionType questionType, string category)
        {
            type = questionType.type;
            Category = category;
        }
        
        public string Category { get; set; }
    }
}
