using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_MATERIAL_TYPE_Map : BaseMapping<T_MD_MATERIAL_TYPE>
    {
        public T_MD_MATERIAL_TYPE_Map()
        {
            Table("T_MD_MATERIAL_TYPE");
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.TEXT_EN);
        }
    }
}
