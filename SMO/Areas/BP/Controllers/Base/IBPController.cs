using SMO.Core.Common;
using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Service.BP;
using SMO.Service.Class;

using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    public interface IBPController<TService, T, TRepo, TElement, TVersion, THistory, THistoryRepo>
        where TService : BaseBPService<T, TRepo, TElement, TVersion, THistory, THistoryRepo>
        where T : T_BP_BASE
        where TElement : CoreElement
        where TRepo : GenericRepository<T>
        where TVersion: BaseBPVersionEntity
        where THistoryRepo : GenericRepository<THistory>
        where THistory : BaseBPHistoryEntity
    {
        ActionResult ChuyenHDNS(string code);
        ActionResult ChuyenTKS(string code);
        ActionResult DataFlowIndex(string centerCode, int year, int version);
        ActionResult DataFlowTree(TService service);
        FileContentResult DownloadTemplate(string templateId, int year);
        ActionResult ExportData(string templateCode, int year, int version, string orgCode);
        ActionResult ExportDataButtonsFunction(string orgCode, string templateId, int year, int? version, string viewId);
        ActionResult ExportDataFlowIndex(string centerCode, int year, int version);
        ActionResult ExportDataFlowTree(TService service);
        ActionResult ExportDataHistorySumUp(string orgCode, int year, string templateId, string viewId, string formId);
        ActionResult ExportDataInformation(string orgCode, string templateId, int year, int? version);
        ActionResult ExportDataVersion(string orgCode, string templateId, int year, string viewId, string formId);
        ActionResult ExportDataViewHistory(string orgCode, string templateId, int year, string viewId, string formId);
        FileContentResult ExportExcel(string html, int exportExcelYear, int? exportExcelVersion, string exportExcelCenterCode, string exportExcelTemplate, string exportExcelUnit, decimal exportExcelExchangeRate);
        JsonResult GetDetailPreviewSumUp(string centerCode, int year, string elementCode);
        JsonResult GetDetailSumUp(string centerCode, int year, string elementCode, int version, int? sumUpVersion);
        ActionResult GetDetailSumUpReview(string centerCode, int year, string elementCode, int version, int? sumUpVersion, string templateCode, bool? isShowFile, bool? fileBase);
        JsonResult GetDetailSumUpTemplate(string templateCode, int year, string elementCode, int version, string centerCode);
        ActionResult GetFileBase(int year, string templateCode, int version, string centerCode);
        JsonResult GetRealOrgCode(string templateCode, string orgCode);
        JsonResult GetTemplate(string orgCode);
        JsonResult GetTemplateVersion(string templateId, string centerCode, int year);
        JsonResult GetVersions(string orgCode, string templateId, int year);
        JsonResult GetYear(string orgCode, string templateId);
        ActionResult HuyNop(string code);
        ActionResult HuyPheDuyet(string code);
        ActionResult HuyTrinhDuyet(string code);
        ActionResult ImportExcel(TService service);
        ActionResult ImportExcel(int year);
        ActionResult Index(int? year);
        ActionResult IndexOfChild();
        ActionResult TrinhDuyetTongKiemSoat(string orgCode, int year, int version);
        ActionResult KetThucThamDinh(string orgCode, int year, int version);
        ActionResult PheDuyet(string code);
        ActionResult SearchIndex(TService service);
        ActionResult SearchIndexHistory(TService service);
        ActionResult SearchIndexOfChild(TService service);
        ActionResult SummaryCenter(string centerCode, int? year, int? version, bool isRenderPartial = false);
        ActionResult SummaryDataCenter(ViewDataCenterModel model);
        ActionResult SumUpData(int year);
        ActionResult TGDHuyPheDuyet(string code);
        ActionResult TGDPheDuyet(string code);
        ActionResult TGDTuChoi(string code);
        JsonResult TreeData(int year, string orgCode, int version, int? sumUpVersion);
        ActionResult TrinhDuyet(string code);
        ActionResult TrinhTGD(string code);
        ActionResult TuChoi(string code);
        ActionResult ViewHistory(string id);
        ActionResult ViewTemplate(string templateId, int? version, int year, string centerCode = "");
        ActionResult YeuCauCapDuoiDieuChinh(string childOrgCode, string templateCode, int timeYear, string comment, int? templateVersion, int? parentVersion, bool isSummaryReview = false);
        ActionResult StepperBudget(int year, string centerCode, string templateCode);
    }
}