using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Tts.TMusic.Updates.Web.Controllers
{
    public class ServiceUpdateController : Controller
    {
        public string GetLastVersion()
        {
            var path = Server.MapPath("~ServiceVersion.xml");
            XDocument xdoc = XDocument.Load(path);
            var v = xdoc.Element("File").Element("Version").Value.Trim();
            return v;
        }

        public ActionResult Download()
        {
            var path = Server.MapPath("~/ServiceUpdates/last.zip");
            return new FilePathResult(path, "application/x-zip-compressed");
        }
    }
}