using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class ReportController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        // GET: /<controller>/
        public ReportController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Generate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Expenses()
        {
            string basePath = _hostingEnvironment.ContentRootPath;
            FileStream inputStream = new FileStream(basePath + @"/Reports/Expenses.rdlc", FileMode.Open, FileAccess.Read);
            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream);
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, Syncfusion.ReportWriter.WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = "Invoice.pdf";
            return fileStreamResult;
        }
    }
}
