using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class UserStats
    {
        public string UserId { get; set; }
        public int LoginsCount { get; set; }
        public int TotalTimeSpent { get; set; }
        public int TotalAnswers { get; set; }
        public int CorrectAnswers { get; set; }
    }
}
