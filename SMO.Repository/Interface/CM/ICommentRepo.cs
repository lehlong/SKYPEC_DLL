using SMO.Core.Entities;
using SMO.Core.Entities.CM;
using SMO.Repository.Common;

using System.Collections.Generic;

namespace SMO.Repository.Interface.CM
{
    public interface ICommentRepo : IGenericRepository<T_CM_COMMENT>
    {
        IList<T_CM_COMMENT> GetCommentsBP(T_CM_HEADER_BP_COMMENT header);
        List<T_CM_COMMENT> GetCommentsOfDocument(string referenceId);
    }
}
