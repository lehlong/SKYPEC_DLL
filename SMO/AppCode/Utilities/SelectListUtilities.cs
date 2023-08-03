using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.BP;
using SMO.Repository.Implement.BP.COST_CF;
using SMO.Repository.Implement.BP.COST_PL;
using SMO.Repository.Implement.BP.REVENUE_CF;
using SMO.Repository.Implement.BP.REVENUE_PL;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.WF;
using SMO.Service;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SMO
{
    public class SelectListUtilities
    {
        public class Data
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string Group { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Data data &&
                       Value == data.Value;
            }

            public override int GetHashCode()
            {
                return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
            }
        }

        /// <summary>
        /// Lấy ra danh sách Domain để chọn dropdownlist 
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllDomain(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstDomain = UnitOfWork.Repository<DomainRepo>().GetAll();
            foreach (var obj in lstDomain)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// Lấy ra danh sách chọn cho dropdownlist
        /// </summary>
        /// <param name="strDomainCode">Mã domain</param>
        /// <param name="isShowValue">Có hiển thị cả mã dictionary trên dropdownlist không. Ví dụ : vn - Việt Nam</param>
        /// <param name="isAddBlank">Có thêm giá trị blank vào đầu tiên trong list chọn của dropdownlist hay không</param>
        /// <param name="selected">Giá trị được chọn mặc định khi hiển thị dropdownlist</param>
        /// <param name="strLang">Ngôn ngữ - để lấy ra giá trị của dictionary theo ngôn ngữ</param>
        /// <param name="isShowDefault"></param>
        /// <returns></returns>
        public static SelectList GetDictionary(string strDomainCode, bool isShowValue = true, bool isAddBlank = true, string selected = "", string strLang = "vi", bool isShowDefault = true)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            var strValue = string.Empty;
            var listDic = UnitOfWork.Repository<DictionaryRepo>().Queryable().Where(x => x.FK_DOMAIN == strDomainCode && x.LANG == strLang).OrderBy(x => x.C_ORDER).ToList();
            if (isAddBlank)
            {
                lstData.Add(new Data { Value = string.Empty, Text = " - " });
            }
            foreach (var dic in listDic)
            {
                if (string.IsNullOrWhiteSpace(selected) && dic.C_DEFAULT == true && isShowDefault == true)
                {
                    selected = dic.CODE;
                }
                strValue = dic.CODE + " - " + dic.C_VALUE;
                if (!isShowValue)
                {
                    strValue = dic.C_VALUE;
                }
                lstData.Add(new Data { Value = dic.CODE, Text = strValue });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetAllDatatableName()
        {
            var service = new DynamicSqlService();
            service.GetAllTableName();
            var lstData = new List<Data>
            {
                new Data() { Value = "", Text = " - " }
            };
            foreach (var item in service.ObjList)
            {
                lstData.Add(new Data() { Value = item.Name, Text = item.Name });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetUserApprove(string right, bool isAddBlank = true)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstUserName = new List<string>();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            //Tìm trong T_AD_USER_RIGHT
            var findUserRight = UnitOfWork.Repository<UserRightRepo>().Queryable().Where(x => x.IS_ADD == true && x.FK_RIGHT == right).ToList();
            lstUserName.AddRange(findUserRight.Select(x => x.USER_NAME));

            //Tìm theo ROLE
            var findRole = UnitOfWork.Repository<RoleDetailRepo>().Queryable().Where(x => x.FK_RIGHT == right).ToList();
            if (findRole.Count > 0)
            {
                var lstRole = findRole.Select(x => x.FK_ROLE).ToList();
                //Tìm user gán với role
                var findUserRole = UnitOfWork.Repository<UserRoleRepo>().Queryable().Where(x => lstRole.Contains(x.ROLE_CODE)).ToList();
                lstUserName.AddRange(findUserRole.Select(x => x.USER_NAME));

                //Tìm user group gán với role
                var findUserGroupRole = UnitOfWork.Repository<UserGroupRoleRepo>().Queryable().Where(x => lstRole.Contains(x.ROLE_CODE)).ToList();
                var lstGroup = findUserGroupRole.Select(x => x.USER_GROUP_CODE).ToList();
                var findUserUserGroup = UnitOfWork.Repository<UserUserGroupRepo>().Queryable().Where(x => lstGroup.Contains(x.USER_GROUP_CODE));

                lstUserName.AddRange(findUserUserGroup.Select(x => x.USER_NAME));
            }

            lstUserName = lstUserName.Distinct().ToList();

            var lstUser = UnitOfWork.Repository<UserRepo>().Queryable().Where(x => lstUserName.Contains(x.USER_NAME) && x.USER_TYPE == UserType.Fecon).OrderBy(x => x.USER_NAME).ToList();
            foreach (var obj in lstUser)
            {
                lstData.Add(new Data { Value = obj.USER_NAME, Text = obj.USER_NAME + " - " + obj.FULL_NAME });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetAllUserFecon(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstUser = UnitOfWork.Repository<UserRepo>().GetAll().OrderBy(x => x.USER_NAME).ToList();
            foreach (var obj in lstUser)
            {
                lstData.Add(new Data { Value = obj.USER_NAME, Text = obj.USER_NAME + " - " + obj.FULL_NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllForm(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstForm = UnitOfWork.Repository<FormRepo>().GetAll();
            foreach (var obj in lstForm)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllRight(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<RightRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllConnectionFile(string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            var lstAll = UnitOfWork.Repository<ConnectionRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.PKID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }


        /// <summary>
        /// Lấy ra danh sách năm ngân sách
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetTimeYear(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstPeriod = UnitOfWork.Repository<PeriodTimeRepo>().GetAll();
            foreach (var obj in lstPeriod)
            {
                lstData.Add(new Data { Value = obj.TIME_YEAR.ToString(), Text = obj.TIME_YEAR.ToString() });
            }
            // selected value phải để dạng số vì trong 1 số màn hình sử dụng giá trị này làm giá trị mặc định
            return new SelectList(lstData, "Value", "Text", selectedValue: lstPeriod.FirstOrDefault(x => x.IS_DEFAULT)?.TIME_YEAR,
                disabledValues: lstPeriod.Where(x => x.IS_CLOSE).Select(x => x.TIME_YEAR));
        }
        /// <summary>
        /// Lấy ra danh sách năm ngân sách
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static IEnumerable<T_MD_PERIOD_TIME> GetPureTimeYear()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();

            return UnitOfWork.Repository<PeriodTimeRepo>().GetAll().OrderByDescending(x => x.TIME_YEAR);

        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllOrganize(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<OrganizeRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.PKID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllCostCenter(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CostCenterRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetMaterialType(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<MaterialTypeRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.CODE))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetBidType(string lang = "vi", bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<BidTypeRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.CODE))
            {
                if (lang == "vi")
                {
                    lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
                }
                else
                {
                    lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT_EN });
                }
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetBidSpecType()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>
            {
                new Data { Value = "NUMBER", Text = "Nhập giá trị số" },
                new Data { Value = "SELECT", Text = "Danh sách chọn" },
                new Data { Value = "TEXT", Text = "Nhập chữ" },
                new Data { Value = "GROUP", Text = "Nhóm tiêu chí" }
            };
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCompany(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<OrganizeRepo>().GetAll();
            foreach (var obj in lstAll.Where(x => x.TYPE == "CP").OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.PKID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCountry(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CountryRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.CODE))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetUnit(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<UnitRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.CODE))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        ///// <summary>
        ///// </summary>
        ///// <param name="selected"></param>
        ///// <returns></returns>
        //public static SelectList GetUnit(bool isAddBlank = true, string selected = "")
        //{
        //    IUnitOfWork UnitOfWork = new NHUnitOfWork();
        //    var lstData = new List<Data>();
        //    if (isAddBlank)
        //    {
        //        lstData.Add(new Data() { Value = "", Text = " - " });
        //    }
        //    var lstAll = UnitOfWork.Repository<UnitRepo>().GetListActive();
        //    foreach (var obj in lstAll)
        //    {
        //        lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
        //    }
        //    return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        //}

        ///// <summary>
        ///// </summary>
        ///// <param name="selected"></param>
        ///// <returns></returns>
        //public static SelectList GetUnit(List<T_MD_UNIT> lstUnit)
        //{
        //    IUnitOfWork UnitOfWork = new NHUnitOfWork();
        //    var lstData = new List<Data>();
        //    foreach (var obj in lstUnit)
        //    {
        //        lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
        //    }
        //    return new SelectList(lstData, "Value", "Text");
        //}

        public static SelectList GetCurrency(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CurrencyRepo>().GetAll();
            foreach (var obj in lstAll.OrderByDescending(x => x.CODE))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// Lấy danh sách mẫu khai báo của một đơn vị
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="isAddBlank"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetTemplateOfOrg(string orgCode, string budgetType, string elementType, string objectType, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            var lstObjectTypes = new List<string> { objectType };
            if (elementType == ElementType.DoanhThu && objectType == TemplateObjectType.Department)
            {
                lstObjectTypes.Add(TemplateObjectType.Project);
            }

            var lstTemplate = UnitOfWork.Repository<TemplateRepo>()
                .GetManyWithFetch(x => x.ORG_CODE == orgCode
                && x.BUDGET_TYPE == budgetType
                && x.ELEMENT_TYPE == elementType
                && lstObjectTypes.Contains(x.OBJECT_TYPE)).ToList();
            foreach (var obj in lstTemplate.Where(x => x.ACTIVE).OrderByDescending(x => x.CODE))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetElementTypes(bool isAddBlank = false, string selected = "")
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            lstData.Add(new Data { Value = ElementType.ChiPhi, Text = "Chi phí" });
            lstData.Add(new Data { Value = ElementType.DoanhThu, Text = "Doanh thu" });
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }
        public static SelectList GetTemplateObjectTypes(bool isAddBlank = false, string selected = "")
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            lstData.Add(new Data { Value = TemplateObjectType.Department, Text = TemplateObjectType.GetText(TemplateObjectType.Department) });
            lstData.Add(new Data { Value = TemplateObjectType.Project, Text = TemplateObjectType.GetText(TemplateObjectType.Project) });
            lstData.Add(new Data { Value = TemplateObjectType.DevelopProject, Text = TemplateObjectType.GetText(TemplateObjectType.DevelopProject) });
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetBudgetTypes(bool isAddBlank = false, string selected = "")
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            lstData.Add(new Data { Value = BudgetType.KinhDoanh, Text = "Kinh doanh" });
            lstData.Add(new Data { Value = BudgetType.DongTien, Text = "Dòng tiền" });
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetChildOrg<T, Repo>(string root = "", bool isAddBlank = false, string selected = "") where Repo : GenericCenterRepository<T> where T : CoreCenter
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var allOrgs = GetDescendants(UnitOfWork.Repository<Repo>().GetAll(), root).ToList();

            return BuildTreeData(allOrgs, isAddBlank, selected);
        }
        public static SelectList GetChildElement<T, Repo>(int year, string root = "", bool isAddBlank = false, string selected = "") where Repo : GenericElementRepository<T> where T : CoreElement
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var allOrgs = GetDescendantsElement(UnitOfWork.Repository<Repo>().GetManyWithFetch(x => x.TIME_YEAR == year), root).ToList();

            return BuildTreeDataElement(allOrgs, isAddBlank, selected);
        }

        private static IEnumerable<T> GetDescendants<T>(IList<T> list, string root) where T : CoreCenter
        {
            var lookup = list.ToLookup(i => i.PARENT_CODE);
            Stack<T> st = new Stack<T>(lookup[root]);
            var current = list.FirstOrDefault(x => x.CODE == root);
            var hasParrent = false;
            if (current != null)
            {
                hasParrent = true;
                yield return current;
            }
            if (st.Count != 0)
            {
                while (st.Count > 0)
                {
                    var item = st.Pop();
                    if (hasParrent)
                    {
                        item.LEVEL++;
                    }
                    yield return item;
                    foreach (var i in lookup[item.CODE])
                    {
                        i.LEVEL += item.LEVEL + 1;
                        st.Push(i);
                    }
                }
            }
        }
        public static IEnumerable<T> GetDescendantsElement<T>(IList<T> list, string root) where T : CoreElement
        {
            var lookup = list.ToLookup(i => i.PARENT_CODE);
            Stack<T> st = new Stack<T>(lookup[root]);
            var current = list.FirstOrDefault(x => x.CODE == root);
            var hasParrent = false;
            if (current != null)
            {
                hasParrent = true;
                yield return current;
            }
            if (st.Count != 0)
            {
                while (st.Count > 0)
                {
                    var item = st.Pop();
                    if (hasParrent)
                    {
                        item.LEVEL++;
                    }
                    yield return item;
                    foreach (var i in lookup[item.CODE])
                    {
                        i.LEVEL += item.LEVEL + 1;
                        st.Push(i);
                    }
                }
            }
        }

        public static SelectList GetChildRevenue(string root, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            // get all cost center
            var lstRevenueCenter = UnitOfWork.Repository<ProfitCenterRepo>().GetAll().OrderBy(x => x.C_ORDER).ToList();

            // set all parent code to leave
            lstRevenueCenter.ForEach(x => x.PARENT_CODE = string.Concat(x.COMPANY_CODE, x.PROJECT_CODE));
            lstRevenueCenter.ForEach(x => x.REAL_CENTER_CODE = x.CODE);

            var lookupCompany = lstRevenueCenter.ToLookup(x => x.COMPANY_CODE);

            lstRevenueCenter.AddRange(from c in lookupCompany.Select(x => x.Key)
                                      where c != string.Empty
                                      select new T_MD_PROFIT_CENTER
                                      {
                                          COMPANY_CODE = c,
                                          PARENT_CODE = null,
                                          PROJECT_CODE = null,
                                          REAL_CENTER_CODE = c,
                                          CODE = c,
                                          NAME = lookupCompany[c].First().Company.NAME,
                                      });
            var lookupProject = lstRevenueCenter.ToLookup(x => x.PROJECT_CODE);

            lstRevenueCenter.AddRange(from p in lookupProject.Select(x => x.Key)
                                      where p != null
                                      let lookupProjectCompany = lookupProject[p].ToLookup(x => x.COMPANY_CODE)
                                      from c in lookupProjectCompany.Select(x => x.Key)
                                      select new T_MD_PROFIT_CENTER
                                      {
                                          COMPANY_CODE = c,
                                          PARENT_CODE = c,
                                          PROJECT_CODE = p,
                                          REAL_CENTER_CODE = p,
                                          CODE = string.Concat(c, p),
                                          NAME = lookupProject[p].First()?.Project?.NAME,
                                      });
            lstRevenueCenter = GetDescendants(lstRevenueCenter, root).ToList();

            return BuildTreeData(lstRevenueCenter, isAddBlank, selected);
        }

        private static SelectList BuildTreeData<T>(List<T> lstCenters, bool isAddBlank, string selected) where T : CoreCenter
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            foreach (var item in lstCenters.OrderBy(x => x.C_ORDER))
            {
                var space = new StringBuilder();
                for (int i = 0; i < item.LEVEL; i++)
                {
                    space.Append("....");
                }
                lstData.Add(new Data
                {
                    Text = $"{space} {item.CODE} - {item.NAME}",
                    Value = item.CODE
                });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }
        private static SelectList BuildTreeDataElement<T>(List<T> lstCenters, bool isAddBlank, string selected) where T : CoreElement
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            //var realName = false;
            //var propertyRealName = typeof(T).GetProperties().FirstOrDefault(x => x.Name.Equals("REAL_CENTER_CODE"));
            //if (propertyRealName != null)
            //{
            //    realName = true;
            //}
            foreach (var item in lstCenters.OrderBy(x => x.C_ORDER))
            {
                var space = new StringBuilder();
                for (int i = 0; i < item.LEVEL; i++)
                {
                    space.Append("....");
                }
                lstData.Add(new Data
                {
                    Text = $"{space} {item.CODE} - {item.NAME}",
                    Value = item.CODE
                });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetYearCompareCostPL(string templateId, string centerCode, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var yearsCompare = new List<int>();
            if (templateId is null)
            {
                yearsCompare = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode)
                    .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            else
            {
                yearsCompare = UnitOfWork.Repository<CostPLVersionRepo>()
             .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode)
             .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            return GetYearCompare(yearsCompare, isAddBlank, selected);
        }
        public static SelectList GetYearCompareCostCF(string templateId, string centerCode, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var yearsCompare = new List<int>();
            if (templateId is null)
            {
                yearsCompare = UnitOfWork.Repository<CostCFSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode)
                    .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            else
            {
                yearsCompare = UnitOfWork.Repository<CostCFVersionRepo>()
             .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode)
             .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            return GetYearCompare(yearsCompare, isAddBlank, selected);
        }

        public static SelectList GetYearCompareRevenuePL(string templateId, string centerCode, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var yearsCompare = new List<int>();
            if (templateId is null)
            {
                yearsCompare = UnitOfWork.Repository<RevenuePLSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode)
                    .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            else
            {
                yearsCompare = UnitOfWork.Repository<RevenuePLVersionRepo>()
             .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode)
             .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            return GetYearCompare(yearsCompare, isAddBlank, selected);
        }
        public static SelectList GetYearCompareRevenueCF(string templateId, string centerCode, bool isAddBlank = false, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var yearsCompare = new List<int>();
            if (templateId is null)
            {
                yearsCompare = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode)
                    .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            else
            {
                yearsCompare = UnitOfWork.Repository<RevenueCFVersionRepo>()
             .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode)
             .Select(x => x.TIME_YEAR).Distinct().ToList();
            }
            return GetYearCompare(yearsCompare, isAddBlank, selected);
        }

        private static SelectList GetYearCompare(IList<int> yearsCompare, bool isAddBlank, string selected)
        {
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            lstData.AddRange(from year in yearsCompare
                             select new Data
                             {
                                 Text = year.ToString(),
                                 Value = year.ToString()
                             });

            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList SelectElementsFilter(string selected = "")
        {
            var lstData = new List<Data>
            {
                new Data
                {
                    Text = "Hiện tất cả chỉ tiêu",
                    Value = string.Empty
                },
                new Data
                {
                    Text = "Chỉ hiện những chỉ tiêu đã đạt",
                    Value = true.ToString()
                },
                new Data
                {
                    Text = "Chỉ hiện những chỉ tiêu chưa đạt",
                    Value = false.ToString()
                },

            };


            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetUserReviewSummary(int year, int version, string orgCode = null, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = UnitOfWork.Repository<CostCenterRepo>().GetFirstByExpression(x => x.PARENT_CODE == string.Empty)?.CODE;
            }
            if (orgCode != null)
            {
                lstData.AddRange(from user in UnitOfWork.Repository<CostPLReviewRepo>()
                                .GetManyByExpression(x => x.ORG_CODE.Equals(orgCode) && x.TIME_YEAR == year && x.DATA_VERSION == version)
                                 select new Data
                                 {
                                     Text = user.UserReview.FULL_NAME,
                                     Value = user.REVIEW_USER
                                 });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetReviewUsers(int year, string selected = "", bool isAddBlank = false)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var reviewUsers = UnitOfWork.Repository<UserReviewRepo>()
                .GetManyWithFetch(x => x.TIME_YEAR == year)
                .Select(x => x.USER_NAME);
            var users = UnitOfWork.Repository<UserRepo>()
                .GetAll()
                .Select(x => x.USER_NAME)
                .Where(x => !reviewUsers.Contains(x))
                .OrderBy(x => x);
            var lstData = new List<Data>();
            lstData.AddRange(from user in users
                             select new Data
                             {
                                 Text = user,
                                 Value = user
                             });
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetWorkflowActvities(bool hasWorkflow = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var wfs = UnitOfWork.Repository<ProcessRepo>()
                .GetManyWithFetch(x => true, x => x.Activities);

            var lstData = new List<Data>();
            lstData.AddRange(from wf in wfs
                             from activity in wf.Activities
                             select new Data
                             {
                                 Text = $"{activity.CODE} - {activity.NAME}",
                                 Value = activity.CODE,
                                 Group = wf.CODE
                             });
            if (hasWorkflow)
            {
                return new SelectList(lstData, "Value", "Text", "Group", new Data { Value = selected });
            }
            else
            {
                return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
            }
        }
    }
}
