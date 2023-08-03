using SMO.Core.Entities.MD;

namespace SMO.Core.Common
{
    public interface ICoreElement: ICoreTree
    {
        string CENTER_CODE { get; set; }
        string DESCRIPTION { get; set; }
        bool HasAssignValue { get; set; }
        bool IS_GROUP { get; set; }
        bool IsBase { get; set; }
        bool IsChildren { get; set; }
        bool IsSumUp { get; set; }
        int LEVEL { get; set; }
        string NUMBER_COMMENTS { get; set; }
        string ORG_CODE { get; set; }
        string ORG_NAME { get; set; }
        string PKID { get; set; }
        T_MD_TEMPLATE Template { get; set; }
        string TEMPLATE_CODE { get; set; }
        string TEMPLATE_CODE_PURE { get; }
        int TIME_YEAR { get; set; }
        decimal[] Values { get; set; }
        string[] ValuesBaseString { get; set; }
        int VERSION { get; set; }

        bool Equals(object obj);
        int GetHashCode();
    }
}