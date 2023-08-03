using SMO.Core.Interface;

namespace SMO.Core.Common
{
    public interface ICoreTree : IEntity
    {
        int C_ORDER { get; set; }
        string CODE { get; set; }
        string NAME { get; set; }
        string PARENT_CODE { get; set; }
    }
}