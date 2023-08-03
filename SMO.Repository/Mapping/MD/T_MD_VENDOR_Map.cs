using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_VENDOR_Map : BaseMapping<T_MD_VENDOR>
    {
        public T_MD_VENDOR_Map()
        {
            Table("T_MD_VENDOR");
            Id(x => x.CODE);
            Map(x => x.SHORT_NAME).Nullable();
            Map(x => x.LONG_NAME).Nullable();
            Map(x => x.MA_SO_THUE).Nullable();
            Map(x => x.DIA_CHI).Nullable();
            Map(x => x.SO_DIEN_THOAI).Nullable();
            Map(x => x.SO_FAX).Nullable();
            Map(x => x.EMAIL).Nullable();
            Map(x => x.WEBSITE).Nullable();
            Map(x => x.LIEN_HE).Nullable();
            Map(x => x.LINH_VUC_CHINH).Nullable();
            Map(x => x.KHU_VUC).Nullable();
            Map(x => x.TINH_THANH).Nullable();
            Map(x => x.DON_VI_TUNG_GIAO_DICH).Nullable();
            Map(x => x.USER_NAME);
            Map(x => x.STATUS).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
