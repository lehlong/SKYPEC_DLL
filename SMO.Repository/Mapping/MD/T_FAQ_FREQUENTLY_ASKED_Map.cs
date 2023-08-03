using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_FAQ_FREQUENTLY_ASKED_Map : BaseMapping<T_FAQ_FREQUENTLY_ASKED>
    {
        public T_FAQ_FREQUENTLY_ASKED_Map()
        {
            Table("T_FAQ_FREQUENTLY_ASKED");
            Id(x => x.PKID);
            Map(x => x.TIEU_DE);
            Map(x => x.TIEU_DE_EN);
            Map(x => x.NOI_DUNG).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.NOI_DUNG_EN).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.C_ORDER);
        }
    }
}
