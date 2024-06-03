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
        public bool IsBold { get; set; }
        public List<TestRadioOption> TestOptions { get; set; }
        public string TestAsk { get; set; }
        public int CorrectAnswer { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public List<CompliancePair> CompliancePairs { get; set; }
        public string CorrectEntry { get; set; }
    }

    public class TestRadioOption
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }

    public class CompliancePair
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string Element { get; set; }
        public int CorrectID { get; set; }
    }
}
