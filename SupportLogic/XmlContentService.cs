using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PythoPlus
{
    public class XmlContentService
    {
        public async Task<List<Paragraph>> LoadContentAsync(string filePath)
        {
            var paragraphs = new List<Paragraph>();
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = await FileSystem.OpenAppPackageFileAsync(filePath))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("The specified file was not found in the resources.", filePath);
                }

                var doc = XDocument.Load(stream);

                foreach (var element in doc.Descendants("Paragraph"))
                {
                    var paragraph = new Paragraph
                    {
                        ID = int.Parse(element.Attribute("ID").Value),
                        ParagraphType = element.Element("ParagraphType").Value,
                        Content = element.Element("Content").Value,
                        IsBold = element.Element("Content").Attribute("Bold")?.Value == "true"
                    };

                    if (paragraph.ParagraphType == "TestRadio" || paragraph.ParagraphType == "TestCheck")
                    {
                        paragraph.TestAsk = element.Element("Content").Element("TestAsk").Value;
                        paragraph.TestOptions = element.Element("Content")
                                                        .Elements("TestRadio")
                                                        .Select(e => new TestRadioOption
                                                        {
                                                            ID = int.Parse(e.Attribute("ID").Value),
                                                            Text = e.Value
                                                        }).ToList();

                        if (paragraph.ParagraphType == "TestRadio")
                        {
                            paragraph.CorrectAnswer = int.Parse(element.Element("Content").Attribute("Correct").Value);
                        }
                        else if (paragraph.ParagraphType == "TestCheck")
                        {
                            paragraph.CorrectAnswers = element.Element("Content")
                                                              .Attribute("Correct")
                                                              .Value
                                                              .Split(' ')
                                                              .Select(int.Parse)
                                                              .ToList();
                        }
                    }
                    else if (paragraph.ParagraphType == "Compliance")
                    {
                        paragraph.TestAsk = element.Element("Content").Element("TestAsk").Value;
                        paragraph.CompliancePairs = element.Element("Content")
                                                           .Elements("ComplLabel")
                                                           .Select(e => new CompliancePair
                                                           {
                                                               ID = int.Parse(e.Attribute("ID").Value),
                                                               Label = e.Value
                                                           }).ToList();

                        var elements = element.Element("Content").Elements("ComplElem").ToList();
                        for (int i = 0; i < elements.Count; i++)
                        {
                            var elem = elements[i];
                            var pair = paragraph.CompliancePairs.First(p => p.ID == int.Parse(elem.Attribute("Correct_ID").Value));
                            pair.Element = elem.Value;
                            pair.CorrectID = int.Parse(elem.Attribute("Correct_ID").Value);
                        }
                    }
                    else if (paragraph.ParagraphType == "Entry")
                    {
                        paragraph.TestAsk = element.Element("Content").Element("TestAsk").Value;
                        paragraph.CorrectEntry = element.Element("Content").Element("CorrectEntry").Value;
                    }

                    paragraphs.Add(paragraph);
                }
            }
            return paragraphs;
        }
    }
}
