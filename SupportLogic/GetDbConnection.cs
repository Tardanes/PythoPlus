using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class GetDbConnection
    {
        public GetDbConnection()
        {
        }

        private async Task<string> GetConnectionString()
        {
            var structure = new List<XmlFields>
            {
                new XmlFields("ConnectionString", string.Empty)
            };
            var document = new XmlFileService(structure, "Connection", "conn.xml");
            var result = await document.ReadXmlAsync();
            return result[result.Count - 1].Value;
        }
    }
}
