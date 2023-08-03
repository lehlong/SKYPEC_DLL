using SMO.Core.Entities;
using SMO.Models;
using SMO.Repository.Implement.MD;
using SMO.Service.BP;
using SMO.Service.BP.COST_PL;
using SMO.Service.Class;
using SMO.Service.Class.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    public class OtherCostPLReviewController : Controller
    {
        private readonly OtherCostPLReviewService _service;

        public OtherCostPLReviewController()
        {
            _service = new OtherCostPLReviewService();
        }
        // GET: BP/OtherCostPLReview
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R314")]
        public ActionResult Index(int year)
        {
            _service.ObjDetail.TIME_YEAR = year;

            _service.ObjDetail.ORG_CODE = _service.GetCorp().CODE;
            var dataCost = _service.SummaryCenterVersion(out IList<T_BP_OTHER_COST_PL_DATA> detailCostElements);
            _service.ObjDetail.DATA_VERSION = detailCostElements.FirstOrDefault()?.VERSION ?? 0;
            dataCost = dataCost.OrderBy(x => x.C_ORDER)
                                            .GroupBy(x => x.CODE)
                                            .Select(x => x.First()).ToList();
            var orgCode = _service.ObjDetail.ORG_CODE;
            var version = _service.ObjDetail.DATA_VERSION;
            _service.IsReview = true;
            var isReview = true;
            var detail = _service.GetDetail();
            var model = new OtherCostPLReviewViewModel
            {
                Elements = _service.PrepareListReview(dataCost),
                OrgCode = orgCode,
                Version = version,
                Year = year,
                IsEnd = detail != null && detail.IS_END,
                Id = detail?.PKID,
                IsSummary = !isReview,
                IsCompleted = true,
                IsNotCompleted = true,
                ParentFormId = _service.FormId
            };

            var lstPlLastReview = _service.GetLastReview();
            if (lstPlLastReview != null)
            {
                ViewBag.different = SMOUtilities.CompareVersion(dataCost, lstPlLastReview);
            }
            return PartialView(nameof(Details), model);
        }

        public ActionResult ResultReviewsIndex(int? year)
        {
            return PartialView(_service);
        }

        // Tổng kiểm soát
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R310")]
        public ActionResult SummaryIndex(int year)
        {
            _service.ObjDetail.TIME_YEAR = year;

            _service.ObjDetail.ORG_CODE = _service.GetCorp().CODE;
            var dataCost = _service.SummaryCenterVersion(out IList<T_BP_OTHER_COST_PL_DATA> detailCostElements);
            _service.ObjDetail.DATA_VERSION = detailCostElements.FirstOrDefault()?.VERSION ?? 0;
            dataCost = dataCost.OrderBy(x => x.C_ORDER)
                                            .GroupBy(x => x.CODE)
                                            .Select(x => x.First()).ToList();
            var orgCode = _service.ObjDetail.ORG_CODE;
            var version = _service.ObjDetail.DATA_VERSION;
            var isReview = false;
            _service.IsReview = false;
            var detail = _service.GetDetail();

            var model = new OtherCostPLReviewViewModel
            {
                Elements = _service.PrepareListReview(dataCost),
                OrgCode = orgCode,
                Version = version,
                Year = year,
                IsEnd = detail != null && detail.IS_END,
                Id = detail?.PKID,
                IsSummary = !isReview,
                IsCompleted = true,
                IsNotCompleted = true,
                ParentFormId = _service.FormId
            };

            var lstPlLastReview = _service.GetLastReviewSummary();
            if (lstPlLastReview != null)
            {
                ViewBag.different = SMOUtilities.CompareVersion(dataCost, lstPlLastReview);
            }
            return PartialView(nameof(Details), model);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ReviewDataCenter(int year, int? version, string reviewUser)
        {
            var reviewDataCenterModel = new ReviewDataCenterModel
            {
                ORG_CODE = reviewUser ?? string.Empty,
                YEAR = year,
                VERSION = version,
                IS_COMPLETED = true,
                IS_NOT_COMPLETED = true,
                IS_CONTROL = true,
                IS_COUNCIL_BUDGET = true,
            };
            ViewBag.currencies = _service.GetAllMasterData<CurrencyRepo, T_MD_CURRENCY>();
            return PartialView(reviewDataCenterModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewIndex(OtherCostPLReviewService service)
        {
            service.SearchReview();
            ViewBag.header = service.GetHeader();
            ViewBag.showReviewBtn = new OtherCostPLService()
                .ShowReviewBtn(service.ObjDetail.TIME_YEAR);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewHistoryIndex(OtherCostPLReviewService service)
        {
            service.SearchHistory();
            return PartialView(service);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SummaryReviewDataCenter(ReviewDataCenterModel model)
        {
            var data = _service.SummaryReviewDataCenter(model);

            // chuyển đơn vị tiền tệ 
            if (model.EXCHANGE_RATE.HasValue && model.EXCHANGE_RATE != 1)
            {
                foreach (var d in data.Elements)
                {
                    for (int i = 0; i < d.Values.Length; i++)
                    {
                        d.Values[i] = Math.Round(d.Values[i] / model.EXCHANGE_RATE.Value, 2);
                    }
                }
            }

            ViewBag.model = model;
            ReviewCenterViewModelBase<IElementReviewCenterBase> reviewCenterViewModelBase = new ReviewCenterViewModelBase<IElementReviewCenterBase>
            {
                OrgCode = data.OrgCode,
                UserControl = data.UserControl,
                UserCouncil = data.UserCouncil,
                Version = data.Version,
                Year = data.Year,
                Elements = data.Elements.Cast<IElementReviewCenterBase>().ToList()
            };
            return PartialView(new SummaryReviewDataCenterModel(reviewCenterViewModelBase));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // 210003
        public ActionResult Details(OtherCostPLReviewService service)
        {
            service.ObjDetail.ORG_CODE = service.GetCorp().CODE;
            var dataCost = service.SummaryCenterVersion(out IList<T_BP_OTHER_COST_PL_DATA> detailCostElements);
            service.ObjDetail.DATA_VERSION = detailCostElements.FirstOrDefault()?.VERSION ?? 0;
            dataCost = dataCost.OrderBy(x => x.C_ORDER)
                                            .GroupBy(x => x.CODE)
                                            .Select(x => x.First()).ToList();
            var orgCode = service.ObjDetail.ORG_CODE;
            var version = service.ObjDetail.DATA_VERSION;
            var year = service.ObjDetail.TIME_YEAR;
            var isReview = service.IsReview;
            var detail = service.GetDetail();

            var model = new OtherCostPLReviewViewModel
            {
                Elements = service.PrepareListReview(dataCost),
                OrgCode = orgCode,
                Version = version,
                Year = year,
                IsEnd = detail != null && detail.IS_END,
                Id = detail?.PKID,
                IsSummary = !isReview,
                ParentFormId = service.FormId
            };

            var lstPlLastReview = service.GetLastReview();
            if (lstPlLastReview != null)
            {
                ViewBag.different = SMOUtilities.CompareVersion(dataCost, lstPlLastReview);
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateReview(OtherCostPLReviewViewModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateReview(model);
            if (_service.State)
            {
                if (model.IsEnd)
                {
                    if (model.IsSummary)
                    {
                        return RedirectToAction("TrinhDuyetTongKiemSoat", "OtherCostPL",
                            new { orgCode = model.OrgCode, year = model.Year, version = model.Version });
                    }
                    else
                    {
                        return RedirectToAction("KetThucThamDinh", "OtherCostPL",
                            new { orgCode = model.OrgCode, year = model.Year, version = model.Version });
                    }
                }
                else
                {
                    SMOUtilities.GetMessage("1002", _service, result);
                    result.ExtData = "try{UpdateFormData();}catch(e){};";
                }
            }
            else
            {
                result.Type = TransferType.AlertDangerAndJsCommand;
                result.ExtData = "try{UpdateStatusIsEnd();}catch(e){};";
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public JsonResult GetVersions(int year)
        {
            var lstVersions = _service.GetVersions(year);

            return Json(lstVersions, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        public JsonResult GetReviewUsers(int year, int version)
        {
            var lstOrgs = _service.GetReviewUsers(year, version);

            return Json(lstOrgs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lịch sử tổng kiểm soát
        /// </summary>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [MyValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult HistoryControl(int year)
        {
            var lstActionsFilter = new string[] { Approve_Action.KiemSoat, Approve_Action.PheDuyetKiemSoat, Approve_Action.TrinhDuyetKiemSoat, Approve_Action.TuChoiKiemSoat };
            var history = _service.GetHistory(year, lstActionsFilter);
            if (history.Count == 0)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }
            else
            {
                return PartialView(history);
            }
        }

        /// <summary>
        /// Lịch sử hội đồng ngân sách thẩm định
        /// </summary>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [MyValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult HistoryReview(int year)
        {
            var lstActionsFilter = new string[] { Approve_Action.ThamDinh, Approve_Action.KetThucThamDinh };
            var history = _service.GetHistory(year, lstActionsFilter);
            if (history.Count == 0)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }
            else
            {
                return PartialView(history);
            }
        }


    }
}