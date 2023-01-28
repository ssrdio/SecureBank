using System;
using System.IO;
using System.Threading.Tasks;
using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;

namespace SecureBank.Controllers
{
    [Route("[controller]/[action]")]
    [AuthorizeNormal(AuthorizeAttributeTypes.Mvc)]
    public class UploadController : MvcBaseContoller
    {
        private readonly IUploadFileBL _uploadFileBL;

        public UploadController(IUploadFileBL uploadFileBL)
        {
            _uploadFileBL = uploadFileBL;
        }

        public async Task<IActionResult> UploadTransactions()
        {
            ViewBag.Success = false;

            try
            {
                if (Request.Form.Files.Count != 1)
                {
                    return View();
                }
            }
            catch (Exception)
            {
                return View();
            }

            using MemoryStream memoryStream = new MemoryStream();
            await Request.Form.Files[0].CopyToAsync(memoryStream);

            string parsedXml = _uploadFileBL.UploadFile(memoryStream);
            if (string.IsNullOrEmpty(parsedXml))
            {
                return View();
            }

            ViewBag.Success = true;
            ViewBag.Content = parsedXml;

            return View();
        }
    }
}