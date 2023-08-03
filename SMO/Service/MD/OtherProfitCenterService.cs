using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using SMO.Service.Common;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SMO.Service.MD
{
    public class OtherProfitCenterService : GenericCenterService<T_MD_OTHER_PROFIT_CENTER, OtherProfitCenterRepo>
    {
        internal IList<NodeCompany> GetNodeCompanyByTemplate(string templateId, int year)
        {
            var lstNode = new List<NodeCompany>();

            var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
            var allCompanies = UnitOfWork.Repository<CompanyRepo>().GetAll();
            var lstDetails = new List<string>();
            if (template != null)
            {
                if (template.BUDGET_TYPE == BudgetType.KinhDoanh)
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailOtherCostPL
                    .Where(x => x.TIME_YEAR == year)
                    .GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First().CENTER_CODE).ToList();
                }
                else
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailOtherCostCF
                        .Where(x => x.TIME_YEAR == year)
                        .GroupBy(x => x.CENTER_CODE)
                        .Select(x => x.First().CENTER_CODE).ToList();
                }
            }

            var selectedCompanies = CurrentRepository.GetManyWithFetch(x => lstDetails.Contains(x.CODE))
                .GroupBy(x => x.COMPANY_CODE)
                .Select(x => x.First().COMPANY_CODE);
            foreach (var item in allCompanies)
            {
                var node = new NodeCompany()
                {
                    id = item.CODE,
                    pId = null,
                    name = $"<span>{item.CODE} - {item.NAME}</span>",
                    type = Budget.OTHER_PROFIT_CENTER.ToString()
                };

                // check selected cost center
                if (selectedCompanies.SingleOrDefault(x => x.Equals(item.CODE)) != null)
                    node.@checked = "true";

                lstNode.Add(node);
            }

            return lstNode;
        }

        internal IList<NodeProject> GetNodeProjectByTemplate(string templateId, int year)
        {
            var lstNode = new List<NodeProject>();

            var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
            var allProjects = UnitOfWork.Repository<ProjectRepo>().GetAll();
            var lstDetails = new List<string>();
            if (template != null)
            {
                if (template.BUDGET_TYPE == BudgetType.KinhDoanh)
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailOtherCostPL
                    .Where(x => x.TIME_YEAR == year)
                    .GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First().CENTER_CODE).ToList();
                }
                else
                {
                    // get all cost center selected in template
                    lstDetails = template.DetailOtherCostCF
                        .Where(x => x.TIME_YEAR == year)
                        .GroupBy(x => x.CENTER_CODE)
                        .Select(x => x.First().CENTER_CODE).ToList();
                }
            }

            var selectedProjects = CurrentRepository.GetManyWithFetch(x => lstDetails.Contains(x.CODE))
                .GroupBy(x => x.PROJECT_CODE)
                .Select(x => x.First().PROJECT_CODE);
            foreach (var item in allProjects)
            {
                var node = new NodeProject()
                {
                    id = item.CODE,
                    pId = null,
                    name = $"<span>{item.CODE} - {item.NAME}</span>",
                    type = Budget.OTHER_PROFIT_CENTER.ToString()
                };

                // check selected cost center
                if (selectedProjects.SingleOrDefault(x => x.Equals(item.CODE)) != null)
                    node.@checked = "true";

                lstNode.Add(node);
            }

            return lstNode;
        }
    }
}
