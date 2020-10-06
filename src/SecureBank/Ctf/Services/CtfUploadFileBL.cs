using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SecureBank.Ctf.Services
{
    public class CtfUploadFileBL : UploadFileBL
    {
        private readonly string[] CTF_XEE_FILES = new string[]
        {
            "C:/Windows/System32/drivers/etc/hosts",
            "C:\\Windows\\System32\\drivers\\etc\\hosts",
            "etc/passwd"
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CtfOptions _ctfOptions;

        public CtfUploadFileBL(IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> options) : base()
        {
            _httpContextAccessor = httpContextAccessor;
            _ctfOptions = options.Value;
        }

        protected override string ParseXml(string xml)
        {
            string parsedXml;

            if(_ctfOptions.CtfChallengeOptions.XxeInjection)
            {
                parsedXml = base.ParseXml(xml);

                if (parsedXml != null)
                {
                    try
                    {
                        string validParse = ValidXmlParse(xml);
                    }
                    catch (Exception)
                    {
                        CtfChallangeModel xxeChallange = _ctfOptions.CtfChallanges
                            .Where(x => x.Type == CtfChallengeTypes.XxeInjection)
                            .Single();

                        if (CTF_XEE_FILES.Any(x => xml.Contains(x)))
                        {
                            _httpContextAccessor.HttpContext.Response.Headers.Add(xxeChallange.FlagKey, xxeChallange.Flag);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    parsedXml = ValidXmlParse(xml);
                }
                catch(Exception)
                {
                    parsedXml = null;
                }
            }

            return parsedXml;
        }

        private string ValidXmlParse(string xml)
        {
            string result = string.Empty;

            XmlReader reader = XmlReader.Create(new StringReader(xml));
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
