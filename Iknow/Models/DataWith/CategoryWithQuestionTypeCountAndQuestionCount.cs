using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iknow.Models.DataWith
{
    public class CategoryWithQuestionTypeCountAndQuestionCount : Category
    {
        public CategoryWithQuestionTypeCountAndQuestionCount()
        {

        }
        public CategoryWithQuestionTypeCountAndQuestionCount(Category category, int qtC, int qC)
        {
            this.description = category.description;
            this.locked = category.locked;
            this.name = category.name;
            this.User = category.User;
            questionttypeCount = qtC;
            questionCount = qC;
        }
        public int questionttypeCount { get; set; }
        public int questionCount { get; set; }
    }
}
