using SMO.Core.Common;

namespace SMO.Repository.Common
{
    interface IGenericTemplateDetailRepository<TTemplateDetail, TElement, TCenter> : IGenericRepository<TTemplateDetail>
        where TTemplateDetail : BaseTemplateDetail<TElement, TCenter>
        where TElement : CoreElement
        where TCenter : CoreCenter
    {
    }
}
