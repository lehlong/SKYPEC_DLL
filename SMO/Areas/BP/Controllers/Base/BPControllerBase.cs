using Newtonsoft.Json;

using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Models;
using SMO.Repository.Common;
using SMO.Repository.Implement.MD;
using SMO.Service.BP;
using SMO.Service.Class;

using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace SMO.Areas.BP.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public abstract class BPControllerBase<TService, T, TRepo, TElement, TVersion, THistory, THistoryRepo> : Controller, IBPController<TService, T, TRepo, TElement, TVersion, THistory, THistoryRepo>
        where TService : BaseBPService<T, TRepo, TElement, TVersion, THistory, THistoryRepo>, new()
        where T : T_BP_BASE
        where TElement : CoreElement
        where TRepo : GenericRepository<T>
        where TVersion : BaseBPVersionEntity
        where THistoryRepo : GenericRepository<THistory>
        where THistory : BaseBPHistoryEntity
    {
        public TService _service;
        public BPControllerBase()
        {
            _service = new TService();
        }
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R313")]
        public ActionResult ChuyenHDNS(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ChuyenHDNS(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R309")]
        [MyValidateAntiForgeryToken]
        public ActionResult ChuyenTKS(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ChuyenTKS(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult DataFlowIndex(string centerCode, int year, int version)
        {
            _service.ObjDetail.ORG_CODE = centerCode;
            _service.ObjDetail.Organize = _service.GetCenter(centerCode);
            _service.ObjDetail.TIME_YEAR = year;
            _service.ObjDetail.VERSION = version;
            return PartialView(_service);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult DataFlowTree(TService service)
        {
            string orgCode = service.ObjDetail.ORG_CODE;
            int year = service.ObjDetail.TIME_YEAR;
            var treeNodes = service.BuildDataFlowTree(orgCode, year, null, null);
            if (treeNodes.Count == 0)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }
            else
            {
                JavaScriptSerializer oSerializer = new JavaScriptSerializer
                {
                    MaxJsonLength = int.MaxValue
                };

                ViewBag.lstDataTree = oSerializer.Serialize(treeNodes).Replace(@"\""", @"""");

                return PartialView();
            }
        }

        //[HttpPost]
        public abstract FileContentResult DownloadTemplate(string templateId, int year);

        public ActionResult ExportData(string templateCode, int year, int version, string orgCode)
        {
            orgCode = orgCode ?? ProfileUtilities.User.ORGANIZE_CODE;
            var viewDataCenterModel = new ViewDataCenterModel
            {
                ORG_CODE = _service.CalculateOrgCode(orgCode, templateCode),
                IS_LEAF = _service.IsLeaf(),
                TEMPLATE_CODE = templateCode ?? string.Empty,
                YEAR = year,
                VERSION = version,
                IS_HAS_NOT_VALUE = false,
                IS_HAS_VALUE = true,
            };
            ViewBag.currencies = _service.GetAllMasterData<CurrencyRepo, T_MD_CURRENCY>();
            return PartialView("ViewData", viewDataCenterModel);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ExportDataButtonsFunction(string orgCode, string templateId, int year, int? version, string viewId)
        {
            ViewBag.periodTime = _service.GetPeriodTime(year);
            ViewBag.ViewID = viewId;
            ViewBag.showReviewBtn = _service.ShowReviewBtn(year);
            ViewBag.header = _service.GetBPHeader(templateId, version, year, orgCode);
            return PartialView();
        }
        [MyValidateAntiForgeryToken]
        public ActionResult ExportDataFlowIndex(string centerCode, int year, int version)
        {
            _service.ObjDetail.ORG_CODE = centerCode;
            _service.ObjDetail.Organize = _service.GetCenter(centerCode);
            _service.ObjDetail.TIME_YEAR = year;
            _service.ObjDetail.VERSION = version;
            return PartialView(_service);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult ExportDataFlowTree(TService service)
        {
            string orgCode = service.ObjDetail.ORG_CODE;
            int year = service.ObjDetail.TIME_YEAR;
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.lstDataTree = oSerializer.Serialize(service.BuildDataFlowTree(orgCode, year, null, null)).Replace(@"\""", @"""");

            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ExportDataHistorySumUp(string orgCode, int year, string templateId, string viewId, string formId)
        {
            if (!_service.IsSelfUpload(orgCode, templateId) && !_service.IsChildUpload(orgCode, templateId))
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có quyền xem dữ liệu</h5>"
                };
                return contentResult;
            }

            ViewBag.OrgCode = orgCode;
            ViewBag.ViewId = viewId;
            ViewBag.FormId = formId;
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            }
            if (_service.IsLeaf(orgCode))
            {
                // lịch sử nhập dữ liệu
                _service.GetHistoryVersion(orgCode, templateId, year);
                return PartialView("ExportDataHistory", _service);
            }
            else
            {
                // lịch sử tổng hợp
                _service.GetSumUpHistory(orgCode, year);
                return PartialView(_service);
            }

        }

        [MyValidateAntiForgeryToken]
        public ActionResult ExportDataInformation(string orgCode, string templateId, int year, int? version)
        {
            if (year == -1)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }

            var header = _service.GetHeader(templateId, orgCode, year, version);
            if (header == null)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }
            ViewBag.Header = header;
            ViewBag.HasVersion = version.HasValue;

            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ExportDataVersion(string orgCode, string templateId, int year, string viewId, string formId)
        {
            if (year == -1)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }

            var lstPlVersions = _service.GetVersions(orgCode, templateId, year);
            ViewBag.OrgCode = orgCode;
            ViewBag.ViewId = viewId;
            ViewBag.FormId = formId;
            return PartialView(lstPlVersions);
        }

        [MyValidateAntiForgeryToken]
        // quá trình phê duyệt
        public ActionResult ExportDataViewHistory(string orgCode, string templateId, int year, string viewId, string formId)
        {
            var header = _service.GetBPHeader(templateId, null, year, orgCode);
            var currentUserOrgCode = ProfileUtilities.User.ORGANIZE_CODE;
            var lstChildren = _service.GetListOfChildrenCenter(currentUserOrgCode).Select(x => x.CODE);
            if (header == null)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }
            if (lstChildren.Contains(header.ORG_CODE) || header.ORG_CODE == currentUserOrgCode)
            {
                _service.ObjDetail.PKID = header.PKID;
                _service.GetHistory();

                ViewBag.ViewId = viewId;
                ViewBag.FormId = formId;
                return PartialView(_service);
            }
            else
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có quyền xem dữ liệu</h5>"
                };
                return contentResult;
            }
        }

        [ValidateInput(false)]
        public FileContentResult ExportExcel(string html, int exportExcelYear, int? exportExcelVersion, string exportExcelCenterCode, string exportExcelTemplate, string exportExcelUnit, decimal exportExcelExchangeRate)
        {
            if (html is null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (exportExcelCenterCode is null)
            {
                throw new ArgumentNullException(nameof(exportExcelCenterCode));
            }

            if (exportExcelTemplate is null)
            {
                throw new ArgumentNullException(nameof(exportExcelTemplate));
            }

            var path = Server.MapPath("~/TemplateExcel/" + "DuLieuNganSachPhongBan.xlsx");
            MemoryStream outFileStream = new MemoryStream();
            _service.GenerateExportExcel(ref outFileStream, html, path, exportExcelYear, exportExcelCenterCode, exportExcelVersion, exportExcelTemplate, exportExcelUnit, exportExcelExchangeRate);

            var fileName = $"ExportExcel_{DateTime.Now.ToLongTimeString()}";
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        [HttpGet]
        public JsonResult GetDetailPreviewSumUp(string centerCode, int year, string elementCode)
        {
            var lstDetailSumUp = _service.GetDetailPreviewSumUp(centerCode, elementCode, year);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };
            return Json(JsonConvert.SerializeObject(lstDetailSumUp, settings), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[MyValidateAntiForgeryToken]
        public JsonResult GetDetailSumUp(string centerCode, int year, string elementCode, int version, int? sumUpVersion)
        {
            var lstDetailsSumUp = _service.GetDetailSumUp(centerCode, elementCode, year, version, sumUpVersion, false);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };
            return Json(JsonConvert.SerializeObject(lstDetailsSumUp, settings), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDetailSumUpReview(string centerCode, int year, string elementCode, int version, int? sumUpVersion, string templateCode, bool? isShowFile, bool? fileBase)
        {
            if (fileBase.HasValue)
            {
                return RedirectToAction(nameof(GetFileBase), new { year, templateCode, version, centerCode });
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };
            var lstDetailsSumUp = _service.GetDetailSumUp(centerCode, elementCode, year, version, sumUpVersion, true, isShowFile);
            return Json(JsonConvert.SerializeObject(lstDetailsSumUp, settings), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Drilldown khi xem dữ liệu theo mẫu cơ sở
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="year"></param>
        /// <param name="elementCode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
        //[MyValidateAntiForgeryToken]
        public JsonResult GetDetailSumUpTemplate(string templateCode, int year, string elementCode, int version, string centerCode)
        {
            var lstDetailsSumUp = _service.GetDetailSumUpTemplate(elementCode, year, version, templateCode, centerCode);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };
            return Json(JsonConvert.SerializeObject(lstDetailsSumUp, settings), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Hiển thị danh sách file data base của template
        /// </summary>
        /// <param name="year">Năm</param>
        /// <param name="templateCode">Mã template muốn download</param>
        /// <param name="version">Version tổng hợp</param>
        /// <param name="centerCode">Mã khoản mục cha tổng hợp</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetFileBase(int year, string templateCode, int version, string centerCode)
        {
            var files = _service.GetFilesBase(year, templateCode, version, centerCode);
            return PartialView("ListFile", files);
        }

        public JsonResult GetRealOrgCode(string templateCode, string orgCode)
        {
            return Json(_service.CalculateOrgCode(orgCode, templateCode), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy tất cả các template bao gồm cả những mẫu khai báo hộ nếu là đơn vị cấp cha
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        [MyValidateAntiForgeryToken]
        public JsonResult GetTemplate(string orgCode)
        {
            orgCode = orgCode ?? ProfileUtilities.User.ORGANIZE_CODE;

            var lstTemplates = _service.GetTemplates(orgCode);

            return Json(new { templates = lstTemplates, isParent = !_service.IsLeaf(orgCode) }, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        public JsonResult GetTemplateVersion(string templateId, string centerCode, int year)
        {
            var lstVersions = _service.GetTemplateVersion(templateId, centerCode, year);
            return Json(lstVersions, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        public JsonResult GetVersions(string orgCode, string templateId, int year)
        {
            var lstVersions = _service.GetVersionsNumber(orgCode, templateId, year);

            return Json(lstVersions, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        public JsonResult GetYear(string orgCode, string templateId)
        {
            var lstYears = _service.GetYears(orgCode, templateId);

            return Json(lstYears, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeCustom(Right = "R303")]
        [MyValidateAntiForgeryToken]
        public ActionResult HuyNop(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.HuyNop(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R306")]
        [MyValidateAntiForgeryToken]
        public ActionResult HuyPheDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.HuyPheDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R304")]
        [MyValidateAntiForgeryToken]
        public ActionResult HuyTrinhDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.HuyTrinhDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R302")]
        public ActionResult ImportExcel(int year)
        {
            _service.ObjDetail.ORG_CODE = ProfileUtilities.User.ORGANIZE_CODE;
            _service.ObjDetail.TIME_YEAR = year;
            return PartialView(_service);
        }

        [HttpPost]
        [AuthorizeCustom(Right = "R302")]
        public ActionResult ImportExcel(TService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            if (Request.Files.Count == 0)
            {
                service.State = false;
                service.ErrorMessage = "Hãy chọn file excel!";
            }
            else if (Request.Files.Count > 1)
            {
                service.State = false;
                service.ErrorMessage = "Chỉ được phép chọn 1 file excel!";
            }
            else
            {
                var template = service.GetTemplate(service.ObjDetail.TEMPLATE_CODE);
                if (template.IS_BASE)
                {
                    service.ImportExcelBase(Request);
                }
                else
                {

                    service.ImportExcel(Request);
                }
            }

            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }


        /// <summary>
        /// Danh sách dữ liệu tại đơn vị
        /// </summary>
        /// <returns></returns>
        [AuthorizeCustom(Right = "R301")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index(int? year)
        {
            _service.ObjDetail.TIME_YEAR = year ?? 0;
            return PartialView(_service);
        }

        /// <summary>
        /// Danh sách dữ liệu tại đơn vị cấp dưới
        /// </summary>
        /// <returns></returns>
        [AuthorizeCustom(Right = "R301")]
        [MyValidateAntiForgeryToken]
        public ActionResult IndexOfChild()
        {
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R324")]
        public ActionResult TrinhDuyetTongKiemSoat(string orgCode, int year, int version)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TrinhDuyetTongKiemSoat(orgCode, year, version);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();CloseView();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R311")]
        public ActionResult PheDuyetTongKiemSoat(string orgCode, int year, int version)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.PheDuyetTongKiemSoat(orgCode, year, version);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();CloseView();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        [AuthorizeCustom(Right = "R311")]
        public ActionResult TuChoiTongKiemSoat(string orgCode, int year, int version, string comment)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TuChoiTongKiemSoat(orgCode, year, version, comment);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();CloseView();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R315")]
        public ActionResult KetThucThamDinh(string orgCode, int year, int version)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.KetThucThamDinh(orgCode, year, version);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();CloseView();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R305")]
        [MyValidateAntiForgeryToken]
        public ActionResult PheDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.PheDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R301")]
        public ActionResult SearchIndex(TService service)
        {
            service.Search();
            ViewBag.showReviewBtn = service.ShowReviewBtn();
            ViewBag.canDeleteDictionary = service.GetCanDeleteTemplateDictionary();
            return PartialView(service);
        }

        /// <summary>
        /// Lấy toàn bộ thông tin lịch sử dữ liệu trong năm của một đơn vị
        /// Phục vụ cho màn hình chủ Index
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult SearchIndexHistory(TService service)
        {
            service.GetHistory(ProfileUtilities.User.ORGANIZE_CODE, service.ObjDetail.TIME_YEAR);
            return PartialView(service);
        }

        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R301")]
        public ActionResult SearchIndexOfChild(TService service)
        {
            service.GetListOfChild();
            var headerOfParent = service.GetBPHeader("", null, service.ObjDetail.TIME_YEAR, ProfileUtilities.User.ORGANIZE_CODE);
            if (headerOfParent != null)
            {
                service.GetSumUpHistory(ProfileUtilities.User.ORGANIZE_CODE, service.ObjDetail.TIME_YEAR, headerOfParent.VERSION);
            }
            ViewBag.HeaderOfParent = headerOfParent;
            return PartialView(service);
        }

        public abstract ActionResult SummaryCenter(string centerCode, int? year, int? version, bool isRenderPartial = false);


        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract ActionResult SummaryDataCenter(ViewDataCenterModel model);

        /// <summary>
        /// Tổng hợp dữ liệu tại các đơn vị cha
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [MyValidateAntiForgeryToken]
        [HttpPost]
        [AuthorizeCustom(Right = "R308")]
        public ActionResult SumUpData(int year)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };

            _service.SumUpDataCenter(out _, ProfileUtilities.User.ORGANIZE_CODE, year);

            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R318")]
        public ActionResult TGDHuyPheDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TGDHuyPheDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshReviewData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R317")]
        public ActionResult TGDPheDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TGDPheDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshReviewData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R319")]
        public ActionResult TGDTuChoi(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TGDTuChoi(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshReviewData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public JsonResult TreeData(int year, string orgCode, int version, int? sumUpVersion)
        {
            var lstDataTree = _service.BuildDataFlowTree(orgCode, year, version, sumUpVersion);
            return Json(lstDataTree, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeCustom(Right = "R303")]
        [MyValidateAntiForgeryToken]
        public ActionResult TrinhDuyet(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TrinhDuyet(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R316")]
        public ActionResult TrinhTGD(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TrinhTGD(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R307")]
        [MyValidateAntiForgeryToken]
        public ActionResult TuChoi(string code)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.TuChoi(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ViewHistory(string id)
        {
            _service.ObjDetail.PKID = id;
            _service.GetHistory();
            return PartialView(_service);
        }

        public abstract ActionResult ViewTemplate(string templateId, int? version, int year, string centerCode = "");

        [AuthorizeCustom(Right = "R320")]
        [MyValidateAntiForgeryToken]
        public ActionResult YeuCauCapDuoiDieuChinh(string childOrgCode, string templateCode, int timeYear, string comment, int? templateVersion, int? parentVersion, bool isSummaryReview = false)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.YeuCauCapDuoiDieuChinh(childOrgCode, templateCode, timeYear, comment, templateVersion, parentVersion, isSummaryReview);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StepperBudget(int year, string centerCode, string templateCode)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            var steps = _service.GetStepperBudget(year, centerCode, templateCode)
                .OrderBy(x => x.Order).ToList();
            var center = _service.GetCenter(centerCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{RefreshData();}catch(e){};";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return PartialView(new StepBudgetModel
            {
                Steps = steps,
                Year = year,
                CenterName = center.NAME,
                BudgetName = _service.GetBudgetType()?.NAME
            });
        }
    }
}
