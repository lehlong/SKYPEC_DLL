using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using SMO.Service.Common;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class CostCenterService : GenericCenterService<T_MD_COST_CENTER, CostCenterRepo>
    {

        internal IList<NodeCostCenter> GetNodeCostCenterByTemplate(string templateId, int year)
        {
            var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
            var lstNode = new List<NodeCostCenter>();

            // get all cost center
            var lstCostCenter = UnitOfWork.Repository<CostCenterRepo>().GetAll().OrderBy(x => x.C_ORDER).ToList();

            var lstDetails = new List<string>();
            if (template != null)
            {
                if (template.BUDGET_TYPE == BudgetType.KinhDoanh)
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailCosts
                        .Where(x => x.TIME_YEAR == year)
                        .GroupBy(x => x.CENTER_CODE)
                        .Select(x => x.First().CENTER_CODE).ToList();
                }
                else
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailCostsCF
                        .Where(x => x.TIME_YEAR == year)
                        .GroupBy(x => x.CENTER_CODE)
                        .Select(x => x.First().CENTER_CODE).ToList();
                }
            }
            if (lstDetails == null || lstDetails.Count == 0)
            {
                lstDetails = new List<string> { ProfileUtilities.User.ORGANIZE_CODE };
            }
            foreach (var item in lstCostCenter)
            {
                var node = new NodeCostCenter()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    name = $"<span class='pre-whitespace'>{item.CODE} - {item.NAME}</span>",
                    type = Budget.COST_CENTER.ToString()
                };

                // check selected cost center
                if (lstDetails.SingleOrDefault(x => x.Equals(item.CODE)) != null)
                {
                    node.name = $"<span class='col-red'>{item.CODE} - {item.NAME}</span>";
                    node.@checked = true.ToString();
                }

                lstNode.Add(node);
            }

            return lstNode;
        }

        public List<NodeCostCenter> BuildTree()
        {
            var lstNode = new List<NodeCostCenter>();
            GetAll();
            foreach (var item in ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeCostCenter()
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

        public void UpdateTree(List<NodeCostCenter> lstNode)
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
                        strSql = "UPDATE T_MD_COST_CENTER SET C_ORDER = :C_ORDER, PARENT_CODE = :PARENT_CODE, IS_GROUP = :IS_GROUP WHERE CODE = :CODE";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PARENT_CODE", item.pId)
                            .SetParameter("CODE", item.id)
                            .SetParameter("IS_GROUP", isParent ? "Y" : "N")
                            .ExecuteUpdate();
                    }
                    else
                    {
                        strSql = "UPDATE T_MD_COST_CENTER SET C_ORDER = :C_ORDER, PARENT_CODE = '', IS_GROUP = :IS_GROUP WHERE CODE = :CODE";
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
    }
}
