using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Domain.Enums
{
    public enum ReviewStatus
    {
        Pending = 1, 
        Approved = 2,  
        Rejected = 3,  
        Amended = 4    
    }
    public enum EvaluationStatus
    {
        Pending = 1,    
        InProgress = 2, 
        Submitted = 3,   
        UnderReview = 4, 
        Completed = 5,   
        Overdue = 6     
    }
    public enum QuestionType
    {
        SingleChoice = 1,
        MultipleChoice = 2, 
        Text = 3,          
        RatingScale = 4,   
        Boolean = 5
    }
}
