using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PythoPlus
{
    public class XmlFileService
    {
        private readonly string _folderName;
        private readonly string _fileName;
        private readonly List<XmlFields> _structure;

        public XmlFileService(List<XmlFields> structure, string folderName = "SuperContext", string fileName = "SuperContext.xml")
        {
            _folderName = folderName;
            _fileName = fileName;
            _structure = structure;
        }
        public XmlFileService()
        {
            throw new Exception();
        }
        public List<XmlFields> ReadXml()
        {
            List<XmlFields> result = new List<XmlFields>();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"PythoPlus.Resources.Raw.{_folderName}.{_fileName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("The specified file was not found in the resources.", resourceName);
                }

                XDocument xdoc = XDocument.Load(stream);

                foreach (var field in _structure)
                {
                    var elements = xdoc.Descendants(field.Name);
                    foreach (var element in elements)
                    {
                        string value = element.Value;
                        List<XmlAttribute> attributes = null;

                        if (field.IsHaveAttributes && element.HasAttributes)
                        {
                            attributes = element.Attributes()
                                                .Select(a => new XmlAttribute(a.Name.LocalName, a.Value))
                                                .ToList();
                        }

                        result.Add(new XmlFields(field.Name, value, field.IsHaveAttributes, attributes));
                    }
                }
            }

            return result;
        }
    }
}
