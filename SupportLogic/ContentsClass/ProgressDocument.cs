using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class ProgressDocument
    {
        public string UserId { get; set; }
        public int MaterialNumber { get; set; }
        public List<int> CorrectAnswers { get; set; }
    }
}
