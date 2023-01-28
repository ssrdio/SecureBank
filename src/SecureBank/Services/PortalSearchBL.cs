using SecureBank.Interfaces;
using SecureBank.Models.PortalSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class PortalSearchBL : IPortalSearchBL
    {
        protected string loremIpsumText = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

        public virtual PortalSearchModel Search(string search)
        {
            PortalSearchModel portalSearchModel = new PortalSearchModel
            {
                SearchString = search
            };

            if(string.IsNullOrEmpty(search))
            {
                return portalSearchModel;
            }

            loremIpsumText = loremIpsumText.ToLower();

            string[] split = loremIpsumText.Split(" ");

            portalSearchModel.Results = split
                .Where(t => t.StartsWith(search.ToLower()))
                .ToList();

            return portalSearchModel;
        }
    }
}
