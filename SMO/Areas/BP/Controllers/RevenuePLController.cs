
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.BP;
using SMO.Service.BP;
using SMO.Service.Class;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class RevenuePLController : 
        BPControllerBase<RevenuePLService, T_BP_REVENUE_PL, RevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_BP_REVENUE_PL_VERSION, T_BP_REVENUE_PL_HISTORY, RevenuePLHistoryRepo>, 
        IBPController<RevenuePLService, T_BP_REVENUE_PL, RevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_BP_REVENUE_PL_VERSION, T_BP_REVENUE_PL_HISTORY, RevenuePLHistoryRepo>
    {
        public override FileContentResult DownloadTemplate(string templateId, int year)
        {
            var template = _service.GetTemplate(templateId);
            string path;
            MemoryStream outFileStream = new MemoryStream();
            if (template.IS_BASE)
            {
                path = Server.MapPath("~/TemplateExcel/" + "Template_Base_RevenuePL.xlsx");
                _service.GenerateTemplateBase(ref outFileStream, path, templateId, year);
            }
            else
            {
                path = Server.MapPath("~/TemplateExcel/" + "Template_RevenuePL.xlsx");
                _service.GenerateTemplate(ref outFileStream, path, templateId, year);
            }
            var fileName = template.NAME;

            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        public override ActionResult ViewTemplate(string templateId, int? version, int year, string centerCode = "")
        {
            if (!string.IsNullOrEmpty(templateId))
            {
                var dataRevenue = _service.PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements, templateId, year);
                ViewBag.detailRevenueElements = detailRevenueElements;
                var isBaseTemplate = _service.GetTemplate(templateId).IS_BASE;
                if (isBaseTemplate)
                {
                    return PartialView("ViewTemplateBaseRevenuePL", dataRevenue);
                }
                else
                {
                    return PartialView("ViewTemplateRevenuePL", dataRevenue);
                }
            }
            else
            {
                return RedirectToAction(nameof(SummaryCenter), new { version, year, centerCode });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult SummaryDataCenter(ViewDataCenterModel model)
        {
            var dataRevenue = _service.GetDataRevenue(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_PL> detailRevenueElements,
                out IList<T_BP_REVENUE_PL_DATA> detailRevenueData, out bool isDrillDownApply, model);
            if (dataRevenue == null)
            {
                ViewBag.dataCenterModel = model;
                return PartialView(dataRevenue);
            }
            dataRevenue = dataRevenue.Distinct().ToList();
            // chuyển đơn vị tiền tệ 
            if (model.EXCHANGE_RATE.HasValue && model.EXCHANGE_RATE != 1)
            {
                foreach (var data in dataRevenue)
                {
                    for (int i = 0; i < data.Values.Length; i++)
                    {
                        data.Values[i] = Math.Round(data.Values[i] / model.EXCHANGE_RATE.Value, 2);
                    }
                }
                if (isDrillDownApply && detailRevenueData != null)
                {
                    foreach (var data in detailRevenueData)
                    {
                        data.VALUE_JAN = Math.Round((data.VALUE_JAN ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_FEB = Math.Round((data.VALUE_FEB ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_MAR = Math.Round((data.VALUE_MAR ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_APR = Math.Round((data.VALUE_APR ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_MAY = Math.Round((data.VALUE_MAY ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_JUN = Math.Round((data.VALUE_JUN ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_JUL = Math.Round((data.VALUE_JUL ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_AUG = Math.Round((data.VALUE_AUG ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_SEP = Math.Round((data.VALUE_SEP ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_NOV = Math.Round((data.VALUE_NOV ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_OCT = Math.Round((data.VALUE_OCT ?? 0) / model.EXCHANGE_RATE.Value, 2);
                        data.VALUE_DEC = Math.Round((data.VALUE_DEC ?? 0) / model.EXCHANGE_RATE.Value, 2);
                    }
                }
            }

            if (detailRevenueData != null)
            {
                ViewBag.detailRevenueElements = detailRevenueData;
            }
            if (detailRevenueElements != null)
            {
                ViewBag.detailRevenueElements = detailRevenueElements;
            }
            ViewBag.revenuePLHeader = _service.GetHeader(model);
            model.IS_DRILL_DOWN = isDrillDownApply;
            ViewBag.dataCenterModel = model;
            return PartialView(dataRevenue);
        }

        public override ActionResult SummaryCenter(string centerCode, int? year, int? version, bool isRenderPartial = false)
        {
            // revenue
            var dataRevenue = _service.SummaryCenterOut(out IList<T_BP_REVENUE_PL_DATA> detailRevenueElements, centerCode ?? ProfileUtilities.User.ORGANIZE_CODE, year.Value, version);
            ViewBag.detailRevenueElements = detailRevenueElements;
            ViewBag.Header = _service.GetHeader(string.Empty, centerCode ?? ProfileUtilities.User.ORGANIZE_CODE, year.Value, version);
            if (!isRenderPartial)
            {
                return PartialView("ViewSummaryCenterRevenuePL", dataRevenue);
            }
            else
            {
                return PartialView("_PartialViewSummaryCenterRevenuePL", dataRevenue);
            }
        }
    }
}
