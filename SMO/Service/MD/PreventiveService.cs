using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class PreventiveService : GenericService<T_MD_PREVENTIVE, PreventiveRepo>
    {
        public override void Create()
        {
            if (!IsLeaf(ObjDetail.ORG_CODE))
            {
                State = false;
                ErrorMessage = "Không thể thiết lập kế hoạch dự phòng cho đơn vị khác phòng ban";
                return;
            }

            if (ObjDetail.PERCENTAGE < 0)
            {
                State = false;
                ErrorMessage = "Kế hoạch dự phòng không hợp lệ";
                return;
            }
            if (CheckExist(x => x.ORG_CODE == ObjDetail.ORG_CODE && x.TIME_YEAR == ObjDetail.TIME_YEAR))
            {
                State = false;
                ErrorMessage = "Phòng ban đã được thiết lập kế hoạch";
                return;
            }
            ObjDetail.ID = Guid.NewGuid().ToString();
            base.Create();
        }

        public override void Update()
        {
            if (!IsLeaf(ObjDetail.ORG_CODE))
            {
                State = false;
                ErrorMessage = "Không thể thiết lập kế hoạch dự phòng cho đơn vị khác phòng ban";
                return;
            }

            if (ObjDetail.PERCENTAGE < 0)
            {
                State = false;
                ErrorMessage = "Kế hoạch dự phòng không hợp lệ";
                return;
            }

            base.Update();
        }

        internal bool IsLeaf(string centerCode)
        {
            return UnitOfWork.Repository<CostCenterRepo>()
                .GetFirstWithFetch(x => x.PARENT_CODE == centerCode) == null;
        }
    }
}
