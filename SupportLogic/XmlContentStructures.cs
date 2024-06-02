using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class Paragraph
    {
        public int ID { get; set; }
        public string ParagraphType { get; set; }
        public string Content { get; set; }
        public List<TestRadioOption> TestOptions { get; set; }
        public string TestAsk { get; set; }
        public int CorrectAnswer { get; set; }
        public List<int> CorrectAnswers { get; set; }
    }

    public class TestRadioOption
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }

}
