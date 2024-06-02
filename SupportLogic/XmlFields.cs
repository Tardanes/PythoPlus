using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class XmlFields
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsHaveAttributes { get; set; }
        public List<XmlAttribute>? Attributes { get; set; }

        public XmlFields(string name, string value, bool isHaveAttributes = false, List<XmlAttribute>? attributes = null)
        {
            Name = name;
            Value = value;
            IsHaveAttributes = isHaveAttributes;
            Attributes = attributes;
        }
        public XmlFields()
        {
            Name = "null";
            Value = "null";
            IsHaveAttributes = false;
            Attributes = null;
        }
    }
}
