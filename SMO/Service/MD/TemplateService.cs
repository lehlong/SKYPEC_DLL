using NHibernate;

using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Implement.BP;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class TemplateService : GenericService<T_MD_TEMPLATE, TemplateRepo>
    {
        public int TIME_YEAR { get; set; }
        public override void Create()
        {
            if (string.IsNullOrEmpty(ObjDetail.CODE))
            {
                State = false;
                MesseageCode = "1101";
                return;
            }
            try
            {
                if (!CheckExist(x => x.CODE == ObjDetail.CODE))
                {
                    ObjDetail.ACTIVE = true;
                    base.Create();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        public override void Search()
        {
            if (!AuthorizeUtilities.IGNORE_USERS.Contains(ProfileUtilities.User.USER_NAME))
            {
                ObjDetail.ORG_CODE = ProfileUtilities.User.ORGANIZE_CODE;
            }
            base.Search();
        }

        #region Update detail information
        internal void UpdateDetailInformation(string centerCode, string template, int year, IList<string> detailCodes, Budget budget)
        {
            Get(template);
            switch (ObjDetail.OBJECT_TYPE)
            {
                case TemplateObjectType.Department:
                    switch (budget)
                    {
                        case Budget.COST_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_COST_PL, TemplateDetailCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_COST_CENTER, CostPLRepo, T_BP_COST_PL>(centerCode, template, detailCodes, year);
                            }
                            else
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_COST_CF, TemplateDetailCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_COST_CENTER, CostCFRepo, T_BP_COST_CF>(centerCode, template, detailCodes, year);
                            }
                            break;
                        case Budget.PROFIT_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_PL, TemplateDetailRevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER, RevenuePLRepo, T_BP_REVENUE_PL>(centerCode, template, detailCodes, year);
                            }
                            else
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_CF, TemplateDetailRevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER, RevenueCFRepo, T_BP_REVENUE_CF>(centerCode, template, detailCodes, year);
                            }
                            break;
                        case Budget.COST_ELEMENT:
                        case Budget.REVENUE_ELEMENT:
                        default:
                            Exception = new FormatException("Type of center not support");
                            State = false;
                            ErrorMessage = "Type of center not support";
                            break;
                    }
                    break;
                case TemplateObjectType.Project:
                    switch (budget)
                    {
                        case Budget.OTHER_PROFIT_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                if (ObjDetail.ELEMENT_TYPE == ElementType.ChiPhi)
                                {
                                    UpdateDetail<T_MD_TEMPLATE_DETAIL_OTHER_COST_PL, TemplateDetailOtherCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_OTHER_PROFIT_CENTER, OtherCostPLRepo, T_BP_OTHER_COST_PL>(centerCode, template, detailCodes, year);
                                }
                                else
                                {
                                    UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_PL, TemplateDetailRevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER, RevenuePLRepo, T_BP_REVENUE_PL>(centerCode, template, detailCodes, year);
                                }
                            }
                            else
                            {
                                if (ObjDetail.ELEMENT_TYPE == ElementType.DoanhThu)
                                {
                                    UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_CF, TemplateDetailRevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER, RevenueCFRepo, T_BP_REVENUE_CF>(centerCode, template, detailCodes, year);
                                }
                                else
                                {
                                    UpdateDetail<T_MD_TEMPLATE_DETAIL_OTHER_COST_CF, TemplateDetailOtherCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_OTHER_PROFIT_CENTER, OtherCostCFRepo, T_BP_OTHER_COST_CF>(centerCode, template, detailCodes, year);
                                }
                            }
                            break;
                        case Budget.COST_CENTER:
                        case Budget.PROFIT_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_PL, TemplateDetailRevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER, RevenuePLRepo, T_BP_REVENUE_PL>(centerCode, template, detailCodes, year);
                            }
                            else
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_REVENUE_CF, TemplateDetailRevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER, RevenueCFRepo, T_BP_REVENUE_CF>(centerCode, template, detailCodes, year);
                            }
                            break;
                        case Budget.COST_ELEMENT:
                        case Budget.REVENUE_ELEMENT:
                        default:
                            Exception = new FormatException("Type of center not support");
                            State = false;
                            ErrorMessage = "Type of center not support";
                            break;
                    }
                    break;
                case TemplateObjectType.DevelopProject:
                    switch (budget)
                    {
                        case Budget.COST_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL, TemplateDetailContructCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_INTERNAL_ORDER, ContructCostPLRepo, T_BP_CONTRUCT_COST_PL>(centerCode, template, detailCodes, year);
                            }
                            else
                            {
                                UpdateDetail<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF, TemplateDetailContructCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_INTERNAL_ORDER, ContructCostCFRepo, T_BP_CONTRUCT_COST_CF>(centerCode, template, detailCodes, year);
                            }
                            break;
                        case Budget.PROFIT_CENTER:
                        case Budget.COST_ELEMENT:
                        case Budget.REVENUE_ELEMENT:
                        default:
                            Exception = new FormatException("Type of center not support");
                            State = false;
                            ErrorMessage = "Type of center not support";
                            break;
                    }
                    break;
                default:
                    break;
            }

        }

        private void UpdateDetail<TTemplateDetail, TTemplateDetailRepo, TElement, TCenter, TBPRepo, TBPEntity>(string centerCode, string template, IList<string> detailCodes, int year)
            where TTemplateDetail : BaseTemplateDetail<TElement, TCenter>
            where TTemplateDetailRepo : GenericTemplateDetailRepository<TTemplateDetail, TElement, TCenter>
            where TElement : CoreElement
            where TCenter : CoreCenter
            where TBPRepo : GenericBPRepository<TBPEntity>
            where TBPEntity : T_BP_BASE
        {
            if (detailCodes != null)
            {
                var detailsCost = from d in detailCodes
                                  select (TTemplateDetail)Activator.CreateInstance(typeof(TTemplateDetail), new object[]
                                  {
                                  Guid.NewGuid().ToString(), template, d, centerCode, year
                                  });
                if (detailsCost.Count() > 0)
                {
                    try
                    {
                        var repo = UnitOfWork.Repository<TTemplateDetailRepo>();
                        var existLstItems = repo.GetManyByExpression(x => x.TEMPLATE_CODE == template && x.CENTER_CODE == centerCode);
                        // lst delete
                        var deleteLst = existLstItems.Where(x => !detailsCost.Contains(x) && x.TIME_YEAR == year).ToList();
                        // check nếu mẫu đã được xử dụng thì không cho phép xóa
                        if (deleteLst.Count > 0 && UnitOfWork.Repository<TBPRepo>().CheckExist(x => x.TIME_YEAR == year && x.TEMPLATE_CODE == template))
                        {
                            ErrorMessage = "Không thể xóa khoản mục do khoản mục nằm trong mẫu đã được xử dụng";
                            State = false;
                            return;
                        }
                        // lst new
                        var addLst = detailsCost.Where(x => !existLstItems.Contains(x) && x.TIME_YEAR == year).ToList();
                        UnitOfWork.BeginTransaction();
                        repo.Delete(deleteLst);
                        repo.Create(addLst.ToList());
                        UnitOfWork.Commit();
                    }
                    catch (Exception e)
                    {
                        UnitOfWork.Rollback();
                        Exception = e;
                        ErrorMessage = e.Message;
                        State = false;
                    }
                }
            }
            else
            {
                try
                {
                    UnitOfWork.BeginTransaction();
                    var repo = UnitOfWork.Repository<TTemplateDetailRepo>();

                    repo.Delete(x => x.TEMPLATE_CODE == template && x.CENTER_CODE == centerCode);
                    UnitOfWork.Commit();
                }
                catch (Exception e)
                {
                    UnitOfWork.Rollback();
                    Exception = e;
                    ErrorMessage = e.Message;
                    State = false;
                }
            }
        }
        #endregion

        #region Get node details by center code
        internal IEnumerable<string> GetNodeDetailOtherCompany(Budget budget, string projectCode, int year, string templateId)
        {
            if (budget != Budget.OTHER_PROFIT_CENTER)
            {
                return new List<string>();
            }
            Get(templateId);
            if (ObjDetail == null)
            {
                return new List<string>();
            }
            switch (ObjDetail.OBJECT_TYPE)
            {
                case TemplateObjectType.Project:
                    if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                    {
                        if (ObjDetail.ELEMENT_TYPE == ElementType.ChiPhi)
                        {
                            return UnitOfWork.Repository<TemplateDetailOtherCostPLRepo>()
                                .GetManyWithFetch(x => x.TIME_YEAR == year && x.TEMPLATE_CODE == templateId, x => x.Center)
                                .Select(x => x.Center)
                                .Where(x => x.PROJECT_CODE == projectCode)
                                .Select(x => x.COMPANY_CODE)
                                .Distinct();
                        }
                        else
                        {
                            return new List<string>();
                        }
                    }
                    else
                    {
                        if (ObjDetail.ELEMENT_TYPE == ElementType.DoanhThu)
                        {
                            return new List<string>();
                        }
                        else
                        {
                            return UnitOfWork.Repository<TemplateDetailOtherCostCFRepo>()
                                .GetManyWithFetch(x => x.TIME_YEAR == year && x.TEMPLATE_CODE == templateId, x => x.Center)
                                .Select(x => x.Center)
                                .Where(x => x.PROJECT_CODE == projectCode)
                                .Select(x => x.COMPANY_CODE)
                                .Distinct();
                        }
                    }
                case TemplateObjectType.Department:
                case TemplateObjectType.DevelopProject:
                default:
                    return new List<string>();
            }
        }

        internal IList<string> GetNodeDetailOtherElement(Budget budget, string projectCode, string companyCode, int year, string templateId)
        {
            if (budget != Budget.OTHER_PROFIT_CENTER)
            {
                return new List<string>();
            }
            var otherProfitCenterCode = GetOtherProfitCenter(companyCode, projectCode);
            if (otherProfitCenterCode == null)
            {
                return new List<string>();
            }
            else
            {
                return GetNodeDetailElement(budget, otherProfitCenterCode.CODE, year, templateId);
            }
        }

        internal IList<string> GetNodeDetailElement(Budget budget, string centerCode, int year, string templateId)
        {
            Get(templateId);
            switch (ObjDetail.OBJECT_TYPE)
            {
                case TemplateObjectType.Department:
                    switch (budget)
                    {
                        case Budget.COST_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_COST_PL, TemplateDetailCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_COST_CENTER>(centerCode, year, templateId);
                            }
                            else
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_COST_CF, TemplateDetailCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_COST_CENTER>(centerCode, year, templateId);
                            }
                        case Budget.PROFIT_CENTER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_REVENUE_PL, TemplateDetailRevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER>(centerCode, year, templateId);
                            }
                            else
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_REVENUE_CF, TemplateDetailRevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER>(centerCode, year, templateId);
                            }
                        case Budget.COST_ELEMENT:
                        case Budget.REVENUE_ELEMENT:
                        default:
                            return new List<string>();
                    }
                case TemplateObjectType.Project:
                    if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                    {
                        if (ObjDetail.ELEMENT_TYPE == ElementType.ChiPhi)
                        {
                            return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_OTHER_COST_PL, TemplateDetailOtherCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_OTHER_PROFIT_CENTER>(centerCode, year, templateId);
                        }
                        else
                        {
                            return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_REVENUE_PL, TemplateDetailRevenuePLRepo, T_MD_REVENUE_PL_ELEMENT, T_MD_PROFIT_CENTER>(centerCode, year, templateId);
                        }
                    }
                    else
                    {
                        if (ObjDetail.ELEMENT_TYPE == ElementType.DoanhThu)
                        {
                            return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_REVENUE_CF, TemplateDetailRevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_MD_PROFIT_CENTER>(centerCode, year, templateId);
                        }
                        else
                        {
                            return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_OTHER_COST_CF, TemplateDetailOtherCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_OTHER_PROFIT_CENTER>(centerCode, year, templateId);
                        }
                    }
                case TemplateObjectType.DevelopProject:
                    switch (budget)
                    {
                        case Budget.INTERNAL_ORDER:
                            if (ObjDetail.BUDGET_TYPE == BudgetType.KinhDoanh)
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL, TemplateDetailContructCostPLRepo, T_MD_COST_PL_ELEMENT, T_MD_INTERNAL_ORDER>(centerCode, year, templateId);
                            }
                            else
                            {
                                return GetNodeDetailElement<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF, TemplateDetailContructCostCFRepo, T_MD_COST_CF_ELEMENT, T_MD_INTERNAL_ORDER>(centerCode, year, templateId);
                            }
                        case Budget.COST_CENTER:
                        case Budget.PROFIT_CENTER:
                        case Budget.COST_ELEMENT:
                        case Budget.REVENUE_ELEMENT:
                        default:
                            return new List<string>();
                    }
                default:
                    return new List<string>();
            }
        }

        internal string GetBPTypeName(string objectType, string budgetType, string elementType)
        {
            return GetBPType(objectType, budgetType, elementType)?.NAME;
        }

        private T_BP_TYPE GetBPType(string objectType, string budgetType, string elementType)
        {
            return UnitOfWork.Repository<TypeRepo>().GetFirstWithFetch(x => x.OBJECT_TYPE == objectType && x.BUDGET_TYPE == budgetType && x.ELEMENT_TYPE == elementType);
        }

        private string NonUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        internal string GetBPTypeAcronymName(string objectType, string budgetType, string elementType)
        {
            var bpType = GetBPType(objectType, budgetType, elementType);
            return bpType?.ACRONYM_NAME;
        }

        private T_MD_OTHER_PROFIT_CENTER GetOtherProfitCenter(string companyCode, string projectCode)
        {
            return UnitOfWork.Repository<OtherProfitCenterRepo>()
                .GetFirstWithFetch(x => x.PROJECT_CODE == projectCode && x.COMPANY_CODE == companyCode);
        }

        internal void UpdateDetailInformationOther(string projectCode, string companyCode, string template, int year, IList<string> detailCodes, Budget budget)
        {
            Get(template);
            var otherProfitCenter = GetOtherProfitCenter(companyCode, projectCode);
            if (otherProfitCenter != null)
            {
                UpdateDetailInformation(otherProfitCenter.CODE, template, year, detailCodes, budget);
            }
            else
            {
                switch (budget)
                {
                    case Budget.OTHER_PROFIT_CENTER:
                        try
                        {
                            UnitOfWork.BeginTransaction();
                            var centerCode = Guid.NewGuid().ToString();
                            // create other profit center
                            UnitOfWork.Repository<OtherProfitCenterRepo>().Create(new T_MD_OTHER_PROFIT_CENTER
                            {
                                CODE = centerCode,
                                COMPANY_CODE = companyCode,
                                PROJECT_CODE = projectCode,
                            });

                            switch (ObjDetail.BUDGET_TYPE)
                            {
                                case BudgetType.KinhDoanh:
                                    var detailsCostPL = from d in detailCodes
                                                        select new T_MD_TEMPLATE_DETAIL_OTHER_COST_PL
                                                        (Guid.NewGuid().ToString(), template, d, centerCode, year);

                                    UnitOfWork.Repository<TemplateDetailOtherCostPLRepo>().Create(detailsCostPL.ToList());
                                    break;
                                case BudgetType.DongTien:
                                    var detailsCostCF = from d in detailCodes
                                                        select new T_MD_TEMPLATE_DETAIL_OTHER_COST_CF
                                                        (Guid.NewGuid().ToString(), template, d, centerCode, year);

                                    UnitOfWork.Repository<TemplateDetailOtherCostCFRepo>().Create(detailsCostCF.ToList());
                                    break;
                                default:
                                    break;
                            }

                            UnitOfWork.Commit();
                        }
                        catch (Exception e)
                        {
                            UnitOfWork.Rollback();
                            Exception = e;
                            State = false;
                            ErrorMessage = e.Message;
                        }
                        break;
                    case Budget.COST_CENTER:
                    case Budget.COST_ELEMENT:
                    case Budget.PROFIT_CENTER:
                    case Budget.REVENUE_ELEMENT:
                    case Budget.INTERNAL_ORDER:
                    default:
                        Exception = new FormatException("Type of center not support");
                        State = false;
                        ErrorMessage = "Type of center not support";
                        break;
                }
            }
        }

        private IList<string> GetNodeDetailElement<TTemplateDetail, TTemplateDetailRepo, TElement, TCenter>(string centerCode, int year, string templateId)
            where TTemplateDetailRepo : GenericTemplateDetailRepository<TTemplateDetail, TElement, TCenter>
            where TTemplateDetail : BaseTemplateDetail<TElement, TCenter>
            where TElement : CoreElement
            where TCenter : CoreCenter
        {
            return UnitOfWork.Repository<TTemplateDetailRepo>()
                .GetManyByExpression(x => x.CENTER_CODE == centerCode
                && x.TEMPLATE_CODE == templateId
                && x.TIME_YEAR == year)
                .Select(x => x.ELEMENT_CODE).ToList();
        }


        #endregion

        internal void ToggleStatusTemplate(string templateId, bool currentStatus)
        {
            if (!currentStatus)
            {
                // active template
                Update(x => x.CODE == templateId, x => x.ACTIVE = !currentStatus);
            }
            else
            {
                // deactive template
                // check mẫu đã được nộp dữ liệu ở trạng thái khác Chưa trình duyệt không
                Get(templateId);
                if (ObjDetail == null)
                {
                    State = false;
                    ErrorMessage = "Mẫu không khả dụng";
                    return;
                }
                else
                {
                    var canDeactiveTemplate = false;
                    switch (ObjDetail.OBJECT_TYPE)
                    {
                        case TemplateObjectType.Department:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // CostCF
                                            canDeactiveTemplate = CheckStatusData<CostCFRepo, T_BP_COST_CF>(templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // CostPL
                                            canDeactiveTemplate = CheckStatusData<CostPLRepo, T_BP_COST_PL>(templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ElementType.DoanhThu:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // RevenueCF
                                            canDeactiveTemplate = CheckStatusData<RevenueCFRepo, T_BP_REVENUE_CF>(templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // REvenuePL
                                            canDeactiveTemplate = CheckStatusData<RevenuePLRepo, T_BP_REVENUE_PL>(templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TemplateObjectType.Project:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // OtherCostCF
                                            canDeactiveTemplate = CheckStatusData<OtherCostCFRepo, T_BP_OTHER_COST_CF>(templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // OtherCostPL
                                            canDeactiveTemplate = CheckStatusData<OtherCostPLRepo, T_BP_OTHER_COST_PL>(templateId);
                                            break;

                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TemplateObjectType.DevelopProject:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // ContructCostCF
                                            canDeactiveTemplate = CheckStatusData<ContructCostCFRepo, T_BP_CONTRUCT_COST_CF>(templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // ContructCostPL
                                            canDeactiveTemplate = CheckStatusData<ContructCostPLRepo, T_BP_CONTRUCT_COST_PL>(templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            State = false;
                            ErrorMessage = "Mẫu không khả dụng";
                            return;
                    }
                    if (canDeactiveTemplate)
                    {
                        Update(x => x.CODE == templateId, x => x.ACTIVE = !currentStatus);
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "Mẫu không thể Deactive do đã có dữ liệu ở trạng thái khác với Chưa trình duyệt";
                        return;
                    }
                }
            }

        }

        internal void CopyTemplate(int sourceYear, int destinationYear, string templateId)
        {
            if (sourceYear == destinationYear)
            {
                State = false;
                ErrorMessage = "Năm kế hoạch đích phải khác với năm nguồn.";
                return;
            }
            else
            {
                Get(templateId);
                if (ObjDetail == null)
                {
                    State = false;
                    ErrorMessage = "Mẫu không khả dụng.";
                    return;
                }
                else
                {
                    switch (ObjDetail.OBJECT_TYPE)
                    {
                        case TemplateObjectType.Department:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // CostCF
                                            CopyTemplate<TemplateDetailCostCFRepo, T_MD_TEMPLATE_DETAIL_COST_CF, CostCFElementRepo, T_MD_COST_CF_ELEMENT, CostCenterRepo, T_MD_COST_CENTER>(sourceYear, destinationYear, templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // CostPL
                                            CopyTemplate<TemplateDetailCostPLRepo, T_MD_TEMPLATE_DETAIL_COST_PL, CostPLElementRepo, T_MD_COST_PL_ELEMENT, CostCenterRepo, T_MD_COST_CENTER>(sourceYear, destinationYear, templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ElementType.DoanhThu:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // RevenueCF
                                            CopyTemplate<TemplateDetailRevenueCFRepo, T_MD_TEMPLATE_DETAIL_REVENUE_CF, RevenueCFElementRepo, T_MD_REVENUE_CF_ELEMENT, ProfitCenterRepo, T_MD_PROFIT_CENTER>(sourceYear, destinationYear, templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // REvenuePL
                                            CopyTemplate<TemplateDetailRevenuePLRepo, T_MD_TEMPLATE_DETAIL_REVENUE_PL, RevenuePLElementRepo, T_MD_REVENUE_PL_ELEMENT, ProfitCenterRepo, T_MD_PROFIT_CENTER>(sourceYear, destinationYear, templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TemplateObjectType.Project:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // OtherCostCF
                                            CopyTemplate<TemplateDetailOtherCostCFRepo, T_MD_TEMPLATE_DETAIL_OTHER_COST_CF, CostCFElementRepo, T_MD_COST_CF_ELEMENT, OtherProfitCenterRepo, T_MD_OTHER_PROFIT_CENTER>(sourceYear, destinationYear, templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // OtherCostPL
                                            CopyTemplate<TemplateDetailOtherCostPLRepo, T_MD_TEMPLATE_DETAIL_OTHER_COST_PL, CostPLElementRepo, T_MD_COST_PL_ELEMENT, OtherProfitCenterRepo, T_MD_OTHER_PROFIT_CENTER>(sourceYear, destinationYear, templateId);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TemplateObjectType.DevelopProject:
                            switch (ObjDetail.ELEMENT_TYPE)
                            {
                                case ElementType.ChiPhi:
                                    switch (ObjDetail.BUDGET_TYPE)
                                    {
                                        case BudgetType.DongTien:   // ContructCostCF
                                            CopyTemplate<TemplateDetailContructCostCFRepo, T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF, CostCFElementRepo, T_MD_COST_CF_ELEMENT, InternalOrderRepo, T_MD_INTERNAL_ORDER>(sourceYear, destinationYear, templateId);
                                            break;
                                        case BudgetType.KinhDoanh:  // ContructCostPL
                                            CopyTemplate<TemplateDetailContructCostPLRepo, T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_PL, CostPLElementRepo, T_MD_COST_PL_ELEMENT, InternalOrderRepo, T_MD_INTERNAL_ORDER>(sourceYear, destinationYear, templateId);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            State = false;
                            ErrorMessage = "Mẫu không khả dụng";
                            return;
                    }
                }
            }
        }

        private bool CheckStatusData<TRepo, TEntity>(string templateId)
            where TRepo : GenericRepository<TEntity>
            where TEntity : T_BP_BASE
        {
            var lstData = UnitOfWork.Repository<TRepo>().GetManyWithFetch(x => x.TEMPLATE_CODE == templateId);
            return lstData.Count == 0 || !lstData.Any(x => x.STATUS != Approve_Status.ChuaTrinhDuyet && x.STATUS != Approve_Status.TuChoi);
        }

        private void CopyTemplate<TTemplateDetailRepo, TTemplateDetail, TElementRepo, TElement, TCenterRepo, TCenter>(int sourceYear, int destinationYear, string templateId)
            where TTemplateDetailRepo : GenericTemplateDetailRepository<TTemplateDetail, TElement, TCenter>
            where TTemplateDetail : BaseTemplateDetail<TElement, TCenter>
            where TElement : CoreElement
            where TElementRepo : GenericElementRepository<TElement>
            where TCenter : CoreCenter
            where TCenterRepo : GenericCenterRepository<TCenter>

        {
            var lstSource = UnitOfWork.Repository<TTemplateDetailRepo>()
                .GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == sourceYear);
            if (lstSource == null || lstSource.Count == 0)
            {
                State = false;
                ErrorMessage = "Mẫu nguồn chưa được tạo.";
                return;
            }
            else
            {
                var lstDestination = UnitOfWork.Repository<TTemplateDetailRepo>().GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == destinationYear);
                if (lstDestination != null && lstDestination.Count > 0 && lstSource.Count > 0)
                {
                    State = false;
                    ErrorMessage = "Mẫu đích đã được tạo từ trước.";
                    return;
                }
                else
                {
                    var lstSourceElements = lstSource.Select(x => x.ELEMENT_CODE).Distinct();
                    var elementsInDestinationYear = UnitOfWork.Repository<TElementRepo>()
                        .GetManyWithFetch(x => x.TIME_YEAR == destinationYear && lstSourceElements.Contains(x.CODE))
                        .Select(x => x.CODE);
                    if (elementsInDestinationYear == null ||
                        lstSourceElements.Count() > elementsInDestinationYear.Count())
                    {
                        State = false;
                        ErrorMessage = $"Năm {destinationYear} chưa được khai báo những khoản mục sau: " +
                            $"{string.Join(", ", lstSourceElements.Where(x => !elementsInDestinationYear.ToList().Contains(x)).Distinct())}";
                        return;
                    }
                    else
                    {
                        UnitOfWork.OpenStatelessSession();
                        using (IStatelessSession session = UnitOfWork.GetStatelessSession())
                        using (ITransaction transaction = session.Transaction)
                        {
                            try
                            {
                                var currentUser = ProfileUtilities.User?.USER_NAME;
                                lstSource.ForEach(x => x.TIME_YEAR = destinationYear);
                                lstSource.ForEach(x => x.CREATE_BY = currentUser);
                                lstSource.ForEach(x => x.UPDATE_BY = null);
                                lstSource.ForEach(x => x.UPDATE_DATE = null);
                                lstSource.ForEach(x => x.PKID = Guid.NewGuid().ToString());

                                session.SetBatchSize(1000);
                                transaction.Begin();
                                foreach (var obj in lstSource)
                                {
                                    session.Insert(obj);
                                }
                                transaction.Commit();


                                // separate lstSource to each 1k rows
                                //for (int i = 0; i < Math.Ceiling(lstSource.Count / (double)1000); i++)
                                //{
                                //    repo.Create(lstSource.Skip(i * 1000).Take(1000));
                                //}
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                State = false;
                                ErrorMessage = e.Message;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

}
