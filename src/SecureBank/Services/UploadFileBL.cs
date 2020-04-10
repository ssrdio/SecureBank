using Microsoft.AspNetCore.Http;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SecureBank.Services
{
    public class UploadFileBL : IUploadFileBL
    {
        public string UploadFile(MemoryStream stream)
        {
            string xmlStringContent = Encoding.UTF8.GetString(stream.ToArray());
            string xml = ParseXml(xmlStringContent);
            if(string.IsNullOrEmpty(xml))
            {
                return null;
            }

            xml = xml.Replace("\n", "<br />");

            return xml;
        }

        protected virtual string ParseXml(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver
                {
                    Credentials = CredentialCache.DefaultCredentials,
                }
            };

            string result = string.Empty;

            XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    result = reader.ReadElementContentAsString();
                }
            }

            return result;
        }
    }
}
