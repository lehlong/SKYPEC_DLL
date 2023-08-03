using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Core.Entities.MD;
using SMO.Models;
using SMO.Repository.Common;
using SMO.Repository.Implement.BP;
using SMO.Repository.Implement.MD;
using SMO.Service.Class;

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

using static SMO.SelectListUtilities;

namespace SMO.Service.BP
{
    public abstract class BaseBPService<TEntity, TRepo, TElement, TVersion, THistory, THistoryRepo> : GenericService<TEntity, TRepo>, IBaseBPService<TEntity, TRepo, TElement, TVersion>
        where TEntity : T_BP_BASE
        where TRepo : GenericRepository<TEntity>
        where THistoryRepo : GenericRepository<THistory>
        where THistory : BaseBPHistoryEntity
        where TElement : CoreElement
        where TVersion : BaseBPVersionEntity

    {
        protected readonly string ELEMENT_TYPE;
        protected readonly string BUDGET_TYPE;
        protected readonly string OBJECT_TYPE;

        public override void Search()
        {
            var orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            this.ObjDetail.ORG_CODE = orgCode;
            // this.ObjDetail.IS_DELETED = false;
            base.Search();

            // Tìm các mẫu nộp hộ của đơn vị cấp lá
            if (!ProfileUtilities.User.Organize.IS_GROUP && AuthorizeUtilities.CheckUserRight("R323"))
            {
                var listTemplateCode = GetTemplates(orgCode, ObjDetail.TIME_YEAR)?.Select(x => x.Value);
                if (listTemplateCode != null)
                {
                    var findOtherCostPL = this.CurrentRepository.Queryable().Where(x => listTemplateCode.Contains(x.TEMPLATE_CODE) && x.ORG_CODE != orgCode && x.TIME_YEAR == this.ObjDetail.TIME_YEAR && !x.IS_DELETED);
                    this.ObjList.AddRange(findOtherCostPL);
                }
                this.ObjList = this.ObjList.OrderBy(x => x.TEMPLATE_CODE).ToList();
            }
        }

        /// <summary>
        /// Check org code of ObjDetail is leaf or is group
        /// </summary>
        /// <returns>Returns true if org code is leaf and false if org code is group</returns>
        public bool IsLeaf()
        {
            string orgCode;
            if (string.IsNullOrEmpty(ObjDetail.ORG_CODE))
            {
                orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            }
            else
            {
                orgCode = ObjDetail.ORG_CODE;
            }
            return IsLeaf(orgCode);
        }

        public bool IsLeaf(string orgCode)
        {
            var item = UnitOfWork.Repository<CostCenterRepo>().Get(orgCode);
            if (item != null && UnitOfWork.Repository<CostCenterRepo>().GetFirstByExpression(x => x.PARENT_CODE == item.CODE) == null)
            {
                return true;
            }
            return false;
        }

        public abstract void ChuyenHDNS(string code);

        public IList<T_MD_COST_CENTER> GetListOfChildrenCenter(string centerCode)
        {
            return UnitOfWork.Repository<CostCenterRepo>().GetManyByExpression(x => x.PARENT_CODE == centerCode);
        }

        public T_MD_TEMPLATE GetTemplate(string templateCode)
        {
            if (string.IsNullOrEmpty(templateCode))
            {
                return null;
            }
            else
            {
                return UnitOfWork.Repository<TemplateRepo>().Get(templateCode);
            }
        }

        public abstract void ChuyenTKS(string code);
        //public abstract void TrinhDuyetTKS(string code);

        public T_MD_COST_CENTER GetCenter(string centerCode)
        {
            return UnitOfWork.Repository<CostCenterRepo>().Get(centerCode);
        }

        /// <summary>
        /// Get list version data by org code and year
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="year"></param>
        /// <param name="templates"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<int> GetVersion(string orgCode, int year, out IList<string> templates, out string type)
        {
            var item = UnitOfWork.Repository<CostCenterRepo>().Get(orgCode);

            if (UnitOfWork.Repository<CostCenterRepo>().GetFirstByExpression(x => x.PARENT_CODE == orgCode) == null)
            {
                // get all template of orgCode
                templates = UnitOfWork.Repository<TemplateRepo>()
                    .GetManyByExpression(x => x.ORG_CODE.Equals(orgCode) && x.ELEMENT_TYPE.Equals(ElementType.ChiPhi) && x.BUDGET_TYPE.Equals(BudgetType.KinhDoanh))
                    .Select(x => x.CODE).Distinct().OrderByDescending(x => x).ToList();
                type = "leaf";
            }
            else
            {
                // is group
                templates = new List<string>();
                type = "group";
            }
            var tempTemplates = templates;

            return UnitOfWork.Repository<TRepo>()
                .GetManyByExpression(x => x.ORG_CODE.Equals(orgCode) && x.TIME_YEAR == year && tempTemplates.Contains(x.TEMPLATE_CODE))
                .Select(x => x.VERSION).Distinct().OrderByDescending(x => x).ToList();
        }

        public abstract IList<NodeDataFlow> BuildDataFlowTree(string orgCode, int year, int? version, int? sumUpVersion);

        public bool IsSelfUpload(string orgCode, string templateId)
        {
            var template = GetTemplate(templateId);
            return template == null || template.ORG_CODE.Equals(orgCode);
        }

        public bool IsChildUpload(string orgCode, string templateId)
        {
            var children = GetListOfChildrenCenter(orgCode).Select(x => x.CODE);
            var template = GetTemplate(templateId);
            return template == null || children.Contains(template.ORG_CODE);
        }

        public abstract void GenerateTemplateBase(ref MemoryStream outFileStream, string path, string templateId, int year);
        public abstract void GenerateTemplate(ref MemoryStream outFileStream, string path, string templateId, int year);

        public TEntity GetBPHeader(string templateId, int? version, int year, string centerCode)
        {
            templateId = templateId ?? string.Empty;
            if (!string.IsNullOrEmpty(templateId))
            {
                var template = GetTemplate(templateId);
                if (template?.ORG_CODE != centerCode)
                {
                    // nộp hộ
                    centerCode = template.ORG_CODE;
                }
            }
            if (version.HasValue)
            {
                return GetFirstByExpression(x => x.TEMPLATE_CODE == templateId
                    && x.ORG_CODE == centerCode
                    && x.TIME_YEAR == year
                    && x.VERSION == version.Value);
            }
            else
            {
                return GetNewestByExpression(x => x.TEMPLATE_CODE == templateId
                    && x.ORG_CODE == centerCode
                    && x.TIME_YEAR == year, x => x.VERSION, true);
            }
        }

        public abstract bool ShowReviewBtn(int year);


        /// <summary>
        /// Lấy dữ liệu năm ngân sách
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public T_MD_PERIOD_TIME GetPeriodTime(int year)
        {
            return UnitOfWork.Repository<PeriodTimeRepo>().Get(year);
        }


        /// <summary>
        /// Chuyển đổi orgcode nếu là đơn vị nộp hộ thì trả về nộp hộ cho đơn vị nào
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public string CalculateOrgCode(string orgCode, string templateCode)
        {
            if (string.IsNullOrEmpty(templateCode))
            {
                return orgCode;
            }
            var currentUser = ProfileUtilities.User.ORGANIZE_CODE;
            if (IsLeaf(currentUser) || string.IsNullOrEmpty(orgCode))
            {
                return orgCode ?? currentUser;
            }
            else
            {
                var template = GetTemplate(templateCode);
                var children = GetListOfChildrenCenter(currentUser).Select(x => x.CODE);
                // mẫu đơn vị con nộp
                if (template.ORG_CODE == orgCode && children.Contains(orgCode))
                {
                    return orgCode;
                }
                else
                {
                    // mẫu được nộp hộ
                    return currentUser;
                }
            }
        }

        public T_MD_COST_CENTER GetCorp()
        {
            return UnitOfWork.Repository<CostCenterRepo>().GetFirstWithFetch(x => x.PARENT_CODE == "");
        }

        public virtual void ImportExcel(HttpRequestBase request)
        {
            var template = GetTemplate(ObjDetail.TEMPLATE_CODE);
            if (template == null || !template.ACTIVE)
            {
                ErrorMessage = "Mẫu không khả dụng hoặc đang ở trạng thái Deactive";
                State = false;
                return;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
            {
                return;
            }
        }

        public virtual void ImportExcelBase(HttpRequestBase request)
        {
            var template = GetTemplate(ObjDetail.TEMPLATE_CODE);
            if (template == null || !template.ACTIVE)
            {
                ErrorMessage = "Mẫu không khả dụng hoặc đang ở trạng thái Deactive";
                State = false;
                return;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
            {
                return;
            }
        }

        public abstract void GetSumUpHistory(string orgCode, int year = 0, int version = 0);
        public abstract void GetHistoryVersion(string orgCode, string templateId, int year);
        abstract public IEnumerable<TElement> GetDetailSumUpTemplate(string elementCode, int year, int version, string templateCode, string centerCode);
        public abstract TVersion GetHeader(string templateId, string orgCode, int year, int? version);
        public abstract IList<int> GetVersionsNumber(string orgCode, string templateId, int year);
        public abstract void GenerateExportExcel(ref MemoryStream outFileStream, string html, string path, int exportExcelYear, string exportExcelCenterCode, int? exportExcelVersion, string exportExcelTemplate, string exportExcelUnit, decimal exportExcelExchangeRate);
        public abstract IList<TElement> GetDetailPreviewSumUp(string centerCode, string elementCode, int year);
        public abstract IEnumerable<TElement> GetDetailSumUp(string centerCode, string elementCode, int year, int version, int? sumUpVersion, bool isCountComments, bool? isShowFile = null);
        public abstract IList<T_CM_FILE_UPLOAD> GetFilesBase(int year, string templateCode, int version, string centerCode);
        public abstract IList<Data> GetTemplates(string orgCode, int? year = null);
        public abstract IList<int> GetTemplateVersion(string templateId, string centerCode, int year);
        public abstract IList<int> GetYears(string orgCode, string templateId);
        public abstract void HuyNop(string code);
        public abstract void HuyPheDuyet(string code);
        public abstract void HuyTrinhDuyet(string code);
        public abstract void TrinhDuyetTongKiemSoat(string orgCode, int year, int version);
        public abstract void PheDuyetTongKiemSoat(string orgCode, int year, int version);
        internal abstract void TuChoiTongKiemSoat(string orgCode, int year, int version, string comment);
        public abstract void KetThucThamDinh(string orgCode, int year, int version);
        public abstract void PheDuyet(string code);
        public abstract bool ShowReviewBtn();
        public abstract void GetListOfChild();
        public abstract void GetHistory(string orgCode, int year);
        public abstract void GetHistory(string orgCode, string templateId, int? year);
        public abstract void GetHistory();

        public abstract void SumUpDataCenter(out TVersion _, string oRGANIZE_CODE, int year);
        public abstract void TGDHuyPheDuyet(string code);
        public abstract void TGDPheDuyet(string code);
        public abstract void TGDTuChoi(string code);
        public abstract void TrinhDuyet(string code);
        public abstract void TrinhTGD(string code);
        public abstract void TuChoi(string code);
        public abstract void ValidateData(DataTable dataTable, bool isDataBase);
        public abstract void ConvertData(DataTable dataTable, List<TElement> lstElement, int startColumn, int endColumn, bool isDataBase);
        public abstract void YeuCauCapDuoiDieuChinh(string childOrgCode, string templateCode, int timeYear, string comment, int? templateVersion, int? parentVersion, bool isSummaryReview);
        /// <summary>
        /// Kiểm tra xem năm ngân sách đã bị đóng chưa
        /// Chỉ kiểm tra đối với các đơn vị cấp dưới tập đoàn
        /// </summary>
        /// <param name="year"></param>
        public void CheckPeriodTimeValid(int year)
        {
            var periodTime = this.UnitOfWork.Repository<PeriodTimeRepo>().Get(year);
            if ((periodTime == null || periodTime.IS_CLOSE) && string.IsNullOrWhiteSpace(ProfileUtilities.User.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Năm kế hoạch đã bị đóng!";
            }
        }
        public TVersion GetHeader(ViewDataCenterModel model)
        {
            return GetHeader(model.TEMPLATE_CODE, model.ORG_CODE, model.YEAR, model.VERSION);
        }

        public abstract IList<TVersion> GetVersions(string orgCode, string templateId, int year);

        /// <summary>
        /// Nếu có nhiều history thì không thể delete. Ngoài ra nếu chỉ có 1 bản ghi history thì có thể delete
        /// </summary>
        /// <returns></returns>
        internal IDictionary<string, bool> GetCanDeleteTemplateDictionary()
        {
            if (ObjList.Count == 0)
            {
                return new Dictionary<string, bool>();
            }

            return ObjList.ToDictionary(x => x.PKID, x => !string.IsNullOrEmpty(x.TEMPLATE_CODE) && x.STATUS == Approve_Status.ChuaTrinhDuyet && !x.IS_DELETED);
        }

        #region Validate Workflow
        internal virtual bool ValidateTrinhDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (ObjDetail.IS_DELETED)
            {
                this.State = false;
                this.ErrorMessage = "Không thể trình duyệt mẫu đã hủy";
                return false;
            }

            if (!(this.ObjDetail.STATUS == Approve_Status.ChuaTrinhDuyet || this.ObjDetail.STATUS == Approve_Status.TuChoi))
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chưa trình duyệt";
                return false;
            }

            if (!ObjDetail.IS_SUMUP)
            {
                // trình duyệt mức cơ sở
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (ObjDetail.ORG_CODE == GetCorp().CODE)
            {
                // trình duyệt mức btc tổng hợp ns tập đoàn
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
                {
                    return false;
                }
            }
            else
            {
                // trình duyệt mức ban, trung tâm
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateHuyTrinhDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.ChoPheDuyet && this.ObjDetail.STATUS != Approve_Status.TGD_ChoPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chờ phê duyệt";
                return false;
            }

            if (!ObjDetail.IS_SUMUP)
            {
                // hủy trình duyệt mức cơ sở
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (ObjDetail.ORG_CODE == GetCorp().CODE)
            {
                // hủy trình duyệt mức btc tổng hợp ns tập đoàn
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
                {
                    return false;
                }
            }
            else
            {
                // hủy trình duyệt mức ban, trung tâm
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateTGDTuChoi(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (ObjDetail.STATUS != Approve_Status.TGD_ChoPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chờ tổng giám đốc phê duyệt";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CHU_TICH_TGD_PHE_DUYET_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateTGDHuyPheDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.TGD_PheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái đã được tổng giám đốc phê duyệt";
                return false;
            }

            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CHU_TICH_TGD_PHE_DUYET_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateTGDPheDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (ObjDetail.STATUS != Approve_Status.TGD_ChoPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chờ tổng giám đốc phê duyệt";
                return false;
            }

            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CHU_TICH_TGD_PHE_DUYET_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidatePheDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.ChoPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chờ phê duyệt";
                return false;
            }

            if (!ObjDetail.IS_SUMUP)
            {
                // phê duyệt duyệt mức cơ sở
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (ObjDetail.ORG_CODE == GetCorp().CODE)
            {
                // phê duyệt duyệt mức btc tổng hợp ns tập đoàn
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
                {
                    return false;
                }
            }
            else
            {
                // phê duyệt duyệt mức ban, trung tâm
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateHuyNop(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            // check header is in ChuaTrinhDuyet status or not
            if (ObjDetail == null || this.ObjDetail.STATUS != Approve_Status.ChuaTrinhDuyet)
            {
                State = false;
                ErrorMessage = "Không thể hủy nộp mẫu ở trạng thái khác chưa trình duyệt.";
                return false;
            }

            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
            {
                return false;
            }

            return true;
        }

        internal virtual bool ValidateHuyPheDuyet(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.DaPheDuyet &&
                this.ObjDetail.STATUS != Approve_Status.TGD_HuyTrinh &&
                this.ObjDetail.STATUS != Approve_Status.TGD_TuChoi &&
                this.ObjDetail.STATUS != Approve_Status.TKS_PheDuyet &&
                this.ObjDetail.STATUS != Approve_Status.ThamDinh_KetThuc)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái đã phê duyệt";
                return false;
            }
            if (!ObjDetail.IS_SUMUP)
            {
                // hủy phê duyệt mức cơ sở
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (ObjDetail.ORG_CODE == GetCorp().CODE)
            {
                // hủy phê duyệt mức btc tổng hợp ns tập đoàn
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
                {
                    return false;
                }
            }
            else
            {
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateTuChoi(string code)
        {
            Get(code);
            this.CheckPeriodTimeValid(this.ObjDetail.TIME_YEAR);
            if (!this.State)
            {
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.ChoPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái chờ phê duyệt";
                return false;
            }
            if (!ObjDetail.IS_SUMUP)
            {
                // hủy phê duyệt mức cơ sở
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (ObjDetail.ORG_CODE == GetCorp().CODE)
            {
                // hủy phê duyệt mức btc tổng hợp ns tập đoàn
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
                {
                    return false;
                }
            }
            else
            {
                if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateChuyenTKS(string code)
        {
            Get(code);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (this.ObjDetail.STATUS != Approve_Status.DaPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateChuyenHDNS(string code)
        {
            Get(code);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }

            if (this.ObjDetail.STATUS != Approve_Status.TKS_PheDuyet && !ObjDetail.IS_REVIEWED)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt, và giai đoạn Kết thúc tổng kiểm soát";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateTrinhTGD(string code)
        {
            Get(code);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (this.ObjDetail.STATUS != Approve_Status.ThamDinh_KetThuc &&
                this.ObjDetail.STATUS != Approve_Status.TKS_PheDuyet &&
                this.ObjDetail.STATUS != Approve_Status.TGD_TuChoi &&
                this.ObjDetail.STATUS != Approve_Status.TGD_HuyTrinh &&
                this.ObjDetail.STATUS != Approve_Status.DaPheDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.BTC_TONG_HOP_NS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateKetThucThamDinh(string orgCode, int year, int version)
        {
            ObjDetail = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.VERSION == version
                                                && x.IS_SUMUP);
            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }

            if (this.ObjDetail == null || this.ObjDetail.STATUS != Approve_Status.ThamDinh_DuLieu)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt, và giai đoạn Thẩm định";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.THAM_DINH))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateTrinhDuyetTKS(string orgCode, int year, int version)
        {
            ObjDetail = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.VERSION == version
                                                && x.IS_SUMUP);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (this.ObjDetail == null || (this.ObjDetail.STATUS != Approve_Status.TKS_DuLieu && this.ObjDetail.STATUS != Approve_Status.TKS_TuChoi))
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt, và giai đoạn Tổng kiểm soát";
                return false;
            }
            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.TKS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidatePheDuyetTKS(string orgCode, int year, int version)
        {
            ObjDetail = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.VERSION == version
                                                && x.IS_SUMUP);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (this.ObjDetail == null || this.ObjDetail.STATUS != Approve_Status.TKS_TrinhDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt, và giai đoạn Trình duyệt Tổng kiểm soát";
                return false;
            }

            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.TKS))
            {
                return false;
            }
            return true;
        }

        internal virtual bool ValidateTuChoiTKS(string orgCode, int year, int version)
        {
            ObjDetail = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.VERSION == version
                                                && x.IS_SUMUP);

            if (!string.IsNullOrEmpty(ProfileUtilities.User?.Organize.PARENT_CODE))
            {
                this.State = false;
                this.ErrorMessage = "Không thể xử lý yêu cầu này";
                return false;
            }
            if (this.ObjDetail == null || this.ObjDetail.STATUS != Approve_Status.TKS_TrinhDuyet)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu không ở trạng thái Đã phê duyệt, và giai đoạn Trình duyệt Tổng kiểm soát";
                return false;
            }

            if (!ValidateBudgetPeriod(ObjDetail.TIME_YEAR, BudgetPeriod.TKS))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lấy danh sách trạng thái các bước ngân sách của đơn vị theo năm
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<StepBudgetItem> GetStepperBudget(int year, string centerCode, string templateCode)
        {
            templateCode = templateCode ?? string.Empty;
            // lấy danh sách tất cả đơn vị cha (1)
            var query = UnitOfWork.GetSession().CreateSQLQuery("exec GetRecusiveCostCenter ?, ?")
                .AddEntity(typeof(T_MD_COST_CENTER))
                .SetString(0, centerCode)
                .SetInt16(1, 0);    // get all parent
            var lstParents = query.List<T_MD_COST_CENTER>().ToList();
            // -> cấp độ của đơn vị hiện tại
            var levelCurrentCenterCode = lstParents.Count - 1;

            // lấy danh sách stepper từ level hiện tại
            var isCenterIsLeaf = IsLeaf(centerCode);    // only leaf center code should be display Nộp dữ liệu
            var lstSteppersBudget = UnitOfWork.Repository<StepperBudgetRepo>()
                .GetManyWithFetch(x => (isCenterIsLeaf && !x.LEVEL.HasValue) || x.LEVEL.Value <= levelCurrentCenterCode);

            if (isCenterIsLeaf && UnitOfWork.Repository<StepperBudgetRepo>().GetFirstWithFetch(x => x.LEVEL > levelCurrentCenterCode) != null)
            {
                // đơn vị lá nhưng k phải là đơn vị có cấp thấp nhất
                // bỏ step tổng hợp ngân sách ở cấp lá
                lstSteppersBudget = lstSteppersBudget.Where(x => x.LEVEL != levelCurrentCenterCode || x.LEVEL == levelCurrentCenterCode && x.STATUS_ID != Approve_Status.ChuaTrinhDuyet).ToList();
            }
            // với mỗi đơn vị thuộc danh sách (1) và không phải là tập đoàn, lấy ra mẫu mới nhất theo năm
            // nếu trạng thái không phải đã phê duyệt thì thêm tất cả stepper ở level cao hơn với tên mặc định và trả về
            // nếu trạng thái là đã phê duyệt thì nhảy lên đơn vị cha tiếp theo
            TEntity lastActiveStepperBudget = null;
            int lastActiveLevel = -1;
            for (int i = 0; i < lstParents.Count; i++)
            {
                TEntity headerBudget = null;
                var parent = lstParents[i];
                if (!parent.IS_GROUP)
                {
                    // lấy dữ liệu header và history mới nhất
                    headerBudget = GetNewestByExpression(x => x.TIME_YEAR == year && x.ORG_CODE == parent.CODE && x.TEMPLATE_CODE == templateCode, x => x.VERSION, isDescending: true);
                }
                else
                {
                    // lấy dữ liệu header và history mới nhất
                    headerBudget = GetNewestByExpression(x => x.TIME_YEAR == year && x.ORG_CODE == parent.CODE, x => x.VERSION, isDescending: true);
                }
                if (headerBudget != null)
                {
                    lastActiveStepperBudget = headerBudget;
                    lastActiveLevel = levelCurrentCenterCode - i;
                }

                if (headerBudget == null || (headerBudget.STATUS != Approve_Status.DaPheDuyet && !string.IsNullOrEmpty(parent.PARENT_CODE)))
                {
                    // không phải đơn vị cấp tập đoàn va trạng thái không phải đã phê duyệt
                    break;
                }
            }

            var lstStepperBudgetModels = CalculateBudgetStep(lastActiveStepperBudget, lstSteppersBudget, levelCurrentCenterCode, year, lstParents);

            return VerifyStepBudgetReturnData(lstStepperBudgetModels, isCenterIsLeaf, lstParents, lstSteppersBudget, year, centerCode);
        }

        private List<StepBudgetItem> CalculateBudgetStep(TEntity lastActiveStepperBudget, IList<T_BP_STEPPER_BUDGET> lstSteppersBudget, int levelCurrentCenterCode, int year, List<T_MD_COST_CENTER> lstParents)
        {
            var historyRepo = UnitOfWork.Repository<THistoryRepo>();
            var lstStepperBudgetModels = new List<StepBudgetItem>();
            // chưa xảy ra hành động ở bất kỳ cấp nào
            if (lastActiveStepperBudget == null)
            {
                lstStepperBudgetModels = (from stepper in lstSteppersBudget
                                          where stepper.LEVEL.HasValue
                                          let parentCenter = lstParents[levelCurrentCenterCode - stepper.LEVEL.Value]
                                          select new StepBudgetItem
                                          {
                                              DisplayText = stepper.DISPLAY_TEXT.Replace("{CENTER_NAME}", parentCenter.NAME),
                                              Status = false,
                                              Order = stepper.ORDER,
                                              ActionUser = stepper.ACTION_USER
                                          }).ToList();
            }
            else
            {
                // get last history
                // lấy history của version mới nhất theo năm
                var lastHistory = historyRepo.GetNewestByExpression(x => x.TIME_YEAR == year && x.ORG_CODE == lastActiveStepperBudget.ORG_CODE && x.TEMPLATE_CODE == lastActiveStepperBudget.TEMPLATE_CODE, x => x.CREATE_DATE, isDescending: true);

                int lastDoneStepOrder = lstSteppersBudget.FirstOrDefault(x => x.STATUS_ID.Contains(lastActiveStepperBudget.STATUS) && x.ACTION_ID.Contains(lastHistory.ACTION) && (x.LEVEL == levelCurrentCenterCode - lstParents.FindIndex(y => y.CODE == lastHistory.ORG_CODE) || !x.LEVEL.HasValue)).ORDER;

                // so sánh action của history mới nhất vào list. Lấy tất cả action trong list từ vị trí của history action đến phía trước thì sẽ là các bước đã hoàn thành. Còn các bước phía sau thì chưa hoàn thành
                lstStepperBudgetModels = (from step in lstSteppersBudget
                                          where step.LEVEL.HasValue && step.ORDER <= lastDoneStepOrder
                                          let parentCenter = lstParents[levelCurrentCenterCode - step.LEVEL.Value]
                                          let history = historyRepo.GetNewestByExpression(x => x.TIME_YEAR == year && x.ORG_CODE == parentCenter.CODE && step.BASE_ACTION_ID == x.ACTION, x => x.CREATE_DATE, isDescending: true)
                                          select new StepBudgetItem
                                          {
                                              DisplayText = step.DISPLAY_TEXT.Replace("{CENTER_NAME}", parentCenter.NAME),
                                              Status = true,
                                              ActionDate = history.ACTION_DATE,
                                              ActionUser = history.ACTION_USER,
                                              ActionUserFullname = history.USER_CREATE.FULL_NAME,
                                              CenterName = parentCenter.NAME,
                                              Order = step.ORDER
                                          }).ToList();
                var lstNotDoneSteps = (from step in lstSteppersBudget
                                       where step.LEVEL.HasValue && step.ORDER > lastDoneStepOrder
                                       let parentCenter = lstParents[levelCurrentCenterCode - step.LEVEL.Value]
                                       select new StepBudgetItem
                                       {
                                           DisplayText = step.DISPLAY_TEXT.Replace("{CENTER_NAME}", parentCenter.NAME),
                                           Status = false,
                                           ActionUser = step.ACTION_USER,
                                           Order = step.ORDER
                                       });
                lstStepperBudgetModels.AddRange(lstNotDoneSteps);
            }

            return lstStepperBudgetModels;
        }

        private List<StepBudgetItem> VerifyStepBudgetReturnData(List<StepBudgetItem> lstStepperBudgetModels, bool isCenterIsLeaf, List<T_MD_COST_CENTER> lstParents, IList<T_BP_STEPPER_BUDGET> lstSteppersBudget, int year, string centerCode)
        {
            var historyRepo = UnitOfWork.Repository<THistoryRepo>();
            // nếu là đơn vị lá thì có thêm step nộp dữ liệu
            if (isCenterIsLeaf)
            {
                var importDataStep = lstSteppersBudget.Where(x => !x.LEVEL.HasValue).FirstOrDefault();
                var currentCenterName = lstParents[0].NAME;
                var history = historyRepo.GetNewestByExpression(x => x.TIME_YEAR == year && x.ORG_CODE == centerCode && importDataStep.BASE_ACTION_ID == x.ACTION, x => x.CREATE_DATE, isDescending: true);

                var result = new List<StepBudgetItem>
                    {
                        new StepBudgetItem
                        {
                            DisplayText = importDataStep.DISPLAY_TEXT.Replace("{CENTER_NAME}", currentCenterName),
                            Status = history != null,
                            ActionDate = history?.CREATE_DATE,
                            ActionUser = history?.ACTION_USER,
                            ActionUserFullname = history?.USER_CREATE.FULL_NAME,
                            CenterName = history == null ? string.Empty : currentCenterName,
                            Order = importDataStep.ORDER
                        }
                    };
                result.AddRange(lstStepperBudgetModels);
                return result;
            }
            else
            {
                return lstStepperBudgetModels.ToList();
            }
        }

        public abstract T_BP_TYPE GetBudgetType();

        internal virtual bool ValidateYeuCauDieuChinh(int year, string currentOrgCode, bool isSummaryReview)
        {
            if (IsLeaf(currentOrgCode))
            {
                // yêu cầu điều chỉnh mức cơ sở
                if (!ValidateBudgetPeriod(year, BudgetPeriod.CO_SO_NOP_NS))
                {
                    return false;
                }
            }
            else if (currentOrgCode == GetCorp().CODE)
            {
                if (isSummaryReview)
                {
                    // yêu cầu điều chỉnh mức tgd
                    if (!ValidateBudgetPeriod(year, BudgetPeriod.CHU_TICH_TGD_PHE_DUYET_NS))
                    {
                        return false;
                    }
                } else
                {
                    // yêu cầu điều chỉnh mức btc tổng hợp ns tập đoàn
                    if (!ValidateBudgetPeriod(year, BudgetPeriod.BTC_TONG_HOP_NS))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // yêu cầu điều chỉnh mức trung tâm
                if (!ValidateBudgetPeriod(year, BudgetPeriod.BAN_TT_TONG_HOP_NS))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual bool ValidateBudgetPeriod(int year, BudgetPeriod budgetPeriod)
        {
            var bp = UnitOfWork.Repository<BudgetPeriodRepo>().GetFirstWithFetch(x => x.TIME_YEAR == year && x.PERIOD_ID == (int)budgetPeriod, x => x.Period);
            if (bp == null)
            {
                ErrorMessage = $"Thông tin cấu hình giai đoạn cho năm {year} chưa được cấu hình.";
                State = false;
                return false;
            }
            else if (!bp.STATUS)
            {
                ErrorMessage = $"Giai đoạn: {bp.Period.NAME} của năm {year} đã đóng và không thể chỉnh sửa.";
                State = false;
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
