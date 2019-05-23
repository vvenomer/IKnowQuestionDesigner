using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Iknow.Models.DataWith
{
    public class UserWithQuestionCount
    {
        public string user;
        public int questionCount;
    }
    public class QuestionTypeWithQuestionCount : QuestionType
    {
        public QuestionTypeWithQuestionCount()
        {

        }
        public QuestionTypeWithQuestionCount(QuestionType questionType, List<UserWithQuestionCount> usersWithQuestions)
        {
            this.ID = questionType.ID;
            this.type = questionType.type;
            this.usersWithQuestions = usersWithQuestions;
        }
        [Display(Name = "Number of Questions")]
        public List<UserWithQuestionCount> usersWithQuestions { get; set; }
    }
}
