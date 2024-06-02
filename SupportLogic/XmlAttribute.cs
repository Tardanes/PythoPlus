using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class XmlAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public XmlAttribute(string name, string value) 
        { 
            Name = name;
            Value = value;
        }
        public XmlAttribute() 
        {
            Name = "null";
            Value = "null";
        }
    }
}
