using SMO.Service.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.RP.Controllers
{
    public class ReportTableauController : Controller
    {
        private readonly ConfigTableauService _service;
        public ReportTableauController()
        {
            _service = new ConfigTableauService();
        }
        public ActionResult Index()
        {
            _service.GetAll();
            return View(_service);
        }

        public ActionResult ViewTableau(string id)
        {
            _service.Get(id);
            ViewBag.Ticket = _service.GetTableauTicket();
            return View(_service);
        }
    }
}