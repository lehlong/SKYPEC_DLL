using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Core.Entities.MD;
using SMO.Models;
using SMO.Repository.Common;

using System.Collections.Generic;
using System.Web;

namespace SMO.Service.BP
{
    public interface IBaseBPService<TEntity, TRepo, TElement, TVersion> : IGenericService<TEntity>
        where TEntity : T_BP_BASE
        where TRepo : GenericRepository<TEntity>
        where TElement : CoreElement
        where TVersion : BaseBPVersionEntity
    {
        string CalculateOrgCode(string orgCode, string templateCode);
        void CheckPeriodTimeValid(int year);
        TEntity GetBPHeader(string templateId, int? version, int year, string centerCode);
        T_MD_COST_CENTER GetCenter(string centerCode);
        T_MD_COST_CENTER GetCorp();
        IEnumerable<TElement> GetDetailSumUpTemplate(string elementCode, int year, int version, string templateCode, string centerCode);
        IList<T_MD_COST_CENTER> GetListOfChildrenCenter(string centerCode);
        T_MD_PERIOD_TIME GetPeriodTime(int year);
        T_MD_TEMPLATE GetTemplate(string templateCode);
        IList<int> GetVersion(string orgCode, int year, out IList<string> templates, out string type);
        IList<TVersion> GetVersions(string orgCode, string templateId, int year);
        void ImportExcel(HttpRequestBase request);
        void ImportExcelBase(HttpRequestBase request);
        bool IsChildUpload(string orgCode, string templateId);
        bool IsLeaf();
        bool IsLeaf(string orgCode);
        bool IsSelfUpload(string orgCode, string templateId);
        T_BP_TYPE GetBudgetType();
        List<StepBudgetItem> GetStepperBudget(int year, string centerCode, string templateCode);

    }
}
