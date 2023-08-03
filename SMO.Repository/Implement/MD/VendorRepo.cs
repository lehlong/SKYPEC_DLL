using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class VendorRepo : GenericRepository<T_MD_VENDOR>, IVendorRepo
    {
        public VendorRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_VENDOR> Search(T_MD_VENDOR objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE)
                    || x.SHORT_NAME.Contains(objFilter.CODE)
                    || x.LONG_NAME.Contains(objFilter.CODE)
                    || x.MA_SO_THUE.Contains(objFilter.CODE)
                    || x.DIA_CHI.Contains(objFilter.CODE)
                    || x.SO_DIEN_THOAI.Contains(objFilter.CODE)
                    || x.SO_FAX.Contains(objFilter.CODE)
                    || x.EMAIL.Contains(objFilter.CODE)
                    || x.WEBSITE.Contains(objFilter.CODE)
                    || x.LIEN_HE.Contains(objFilter.CODE));
            }

            query = query.OrderBy(x => x.CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
