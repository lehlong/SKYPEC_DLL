using SharpSapRfc;
using SharpSapRfc.Plain;

using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using SMO.Service.AD;
using SMO.Service.Common;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class ProfitCenterService : GenericCenterService<T_MD_PROFIT_CENTER, ProfitCenterRepo>
    {

        internal IList<NodeProfitCenter> GetNodeProfitCenterByTemplate(string templateId, int year)
        {
            var lstNode = new List<NodeProfitCenter>();

            var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
            // get all cost center
            var lstRevenueCenter = UnitOfWork.Repository<ProfitCenterRepo>()
                .GetManyByExpression(x => !string.IsNullOrWhiteSpace(x.COMPANY_CODE) && !string.IsNullOrWhiteSpace(x.PROJECT_CODE))
                .OrderBy(x => x.C_ORDER).ToList();

            // set all parent code to leave
            lstRevenueCenter.ForEach(x => x.PARENT_CODE = string.Concat(x.COMPANY_CODE, x.PROJECT_CODE));
            lstRevenueCenter.ForEach(x => x.REAL_CENTER_CODE = x.CODE);

            var lstDetails = new List<string>();
            if (template != null)
            {
                if (template.BUDGET_TYPE == BudgetType.KinhDoanh)
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailRevenues
                    .Where(x => x.TIME_YEAR == year)
                    .GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First().CENTER_CODE).ToList();
                }
                else
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailRevenuesCF
                        .Where(x => x.TIME_YEAR == year)
                        .GroupBy(x => x.CENTER_CODE)
                        .Select(x => x.First().CENTER_CODE).ToList();
                }
            }

            var lookupCompany = lstRevenueCenter.ToLookup(x => x.COMPANY_CODE);

            lstRevenueCenter.AddRange(from c in lookupCompany.Select(x => x.Key)
                                      where c != null
                                      select new T_MD_PROFIT_CENTER
                                      {
                                          COMPANY_CODE = c,
                                          PARENT_CODE = null,
                                          PROJECT_CODE = null,
                                          REAL_CENTER_CODE = c,
                                          CODE = c,
                                          NAME = lookupCompany[c].First()?.Company?.NAME,
                                      });
            var lookupProject = lstRevenueCenter.ToLookup(x => x.PROJECT_CODE);

            lstRevenueCenter.AddRange(from p in lookupProject.Select(x => x.Key)
                                      where !string.IsNullOrEmpty(p)
                                      let lookupProjectCompany = lookupProject[p].ToLookup(x => x.COMPANY_CODE)
                                      from c in lookupProjectCompany.Select(x => x.Key)
                                      select new T_MD_PROFIT_CENTER
                                      {
                                          COMPANY_CODE = c,
                                          PARENT_CODE = c,
                                          PROJECT_CODE = p,
                                          REAL_CENTER_CODE = c,
                                          CODE = string.Concat(c, p),
                                          NAME = lookupProject[p].FirstOrDefault()?.Project?.NAME,
                                      });

            foreach (var item in lstRevenueCenter)
            {
                var node = new NodeProfitCenter()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    name = $"<span class='pre-whitespace'>{item.CODE} - {item.NAME}</span>",
                    type = Budget.PROFIT_CENTER.ToString()
                };

                // check selected cost center
                if (lstDetails.SingleOrDefault(x => x.Equals(item.CODE)) != null)
                    node.@checked = "true";

                lstNode.Add(node);
            }

            return lstNode;
        }

        public List<NodeProfitCenter> BuildTree()
        {
            var lstNode = new List<NodeProfitCenter>();
            GetAll();
            foreach (var item in ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeProfitCenter()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME,
                    icon = "/Content/zTreeStyle/img/diy/donvi.gif"
                };
                lstNode.Add(node);
            }
            return lstNode;
        }

        public void UpdateTree(List<NodeProfitCenter> lstNode)
        {
            try
            {
                var strSql = "";
                var order = 0;
                UnitOfWork.BeginTransaction();

                foreach (var item in lstNode)
                {
                    var isParent = lstNode.Count(x => x.pId == item.id) > 0;
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql = "UPDATE T_MD_PROFIT_CENTER SET C_ORDER = :C_ORDER, PARENT_CODE = :PARENT_CODE, IS_GROUP = :IS_GROUP WHERE CODE = :CODE";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PARENT_CODE", item.pId)
                            .SetParameter("CODE", item.id)
                            .SetParameter("IS_GROUP", isParent ? "Y" : "N")
                            .ExecuteUpdate();
                    }
                    else
                    {
                        strSql = "UPDATE T_MD_PROFIT_CENTER SET C_ORDER = :C_ORDER, PARENT_CODE = '', IS_GROUP = :IS_GROUP WHERE CODE = :CODE";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("CODE", item.id)
                            .SetParameter("IS_GROUP", isParent ? "Y" : "N")
                            .ExecuteUpdate();
                    }
                    order++;
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                foreach (var item in lstId)
                {
                    UnitOfWork.BeginTransaction();
                    if (!CheckExist(x => x.PARENT_CODE == item))
                    {
                        CurrentRepository.Delete(item);
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "Đơn vị này đang là cha của đơn vị khác.";
                        UnitOfWork.Rollback();
                        return;
                    }
                    UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }

        }

        public void Synchronize()
        {
            // Đồng bộ company , project trước
            var serviceCompany = new CompanyService();
            serviceCompany.Synchronize();

            if (!serviceCompany.State)
            {
                this.State = false;
                this.Exception = serviceCompany.Exception;
                return;
            }

            var serviceProject = new ProjectService();
            serviceProject.Synchronize();

            if (!serviceProject.State)
            {
                this.State = false;
                this.Exception = serviceProject.Exception;
                return;
            }

            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var function = new SynMD_ProfitCenter_Function();

                    var result = conn.ExecuteFunction(function).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_PROFIT_CENTER";
                    SqlDataAdapter daAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, objConn);
                    daAdapter.UpdateBatchSize = 0;
                    DataSet dataSet = new DataSet(tableName);
                    daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                    daAdapter.Fill(dataSet, tableName);
                    DataTable dataTable;
                    dataTable = dataSet.Tables[tableName];

                    var actionBy = ProfileUtilities.User.USER_NAME;
                    var actionDate = DateTime.Now;
                    foreach (var item in result)
                    {
                        if (!string.IsNullOrWhiteSpace(item.COMPANY_CODE) && item.COMPANY_CODE.Split(',').Count() > 1)
                        {
                            this.State = false;
                            this.ErrorMessage = $"Mã profit center [{item.CODE}] đang được gán nhiều hơn một Company code [{item.COMPANY_CODE}]";
                            return;
                        }
                        var row = dataTable.Rows.Find(item.CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["COMPANY_CODE"] = item.COMPANY_CODE;
                            newRow["PROJECT_CODE"] = item.PROJECT_CODE;
                            newRow["CODE"] = item.CODE;
                            newRow["NAME"] = item.NAME;
                            newRow["ACTIVE"] = "Y";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["NAME"].ToString() != item.NAME
                            || row["COMPANY_CODE"].ToString() != item.COMPANY_CODE
                            || row["PROJECT_CODE"].ToString() != item.PROJECT_CODE)
                        {
                            row.BeginEdit();
                            row["NAME"] = item.NAME;
                            row["COMPANY_CODE"] = item.NAME;
                            row["PROJECT_CODE"] = item.NAME;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                    daAdapter.Update(dataSet, tableName);
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

    }
}
