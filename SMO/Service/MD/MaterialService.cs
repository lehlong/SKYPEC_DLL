using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

namespace SMO.Service.MD
{
    public class MaterialService : GenericService<T_MD_MATERIAL, MaterialRepo>
    {
        public MaterialService() : base()
        {

        }

        public override void Create()
        {
            ObjDetail.CODE = "MT" + GetSequence("MATERIAL");
            base.Create();
        }
    }
}
