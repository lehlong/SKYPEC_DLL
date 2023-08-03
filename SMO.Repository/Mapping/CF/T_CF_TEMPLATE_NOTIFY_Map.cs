using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_TEMPLATE_NOTIFY_Map : BaseMapping<T_CF_TEMPLATE_NOTIFY>
    {
        public T_CF_TEMPLATE_NOTIFY_Map()
        {
            Table("T_CF_TEMPLATE_NOTIFY");
            Id(x => x.PKID);
            Map(x => x.INVITE_SUBJECT);
            Map(x => x.INVITE_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.INVITE_COUNCIL_SUBJECT);
            Map(x => x.INVITE_COUNCIL_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.SCORE_SUBJECT);
            Map(x => x.SCORE_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.WIN_SUBJECT);
            Map(x => x.WIN_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.FAIL_SUBJECT);
            Map(x => x.FAIL_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.ACCOUNT_SUBJECT);
            Map(x => x.ACCOUNT_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.REQUEST_ACCOUNT_SUBJECT);
            Map(x => x.REQUEST_ACCOUNT_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");

            Map(x => x.CONG_VIEC_HOAN_THANH_SUBJECT);
            Map(x => x.CONG_VIEC_HOAN_THANH_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.CONG_VIEC_XU_LY_SUBJECT);
            Map(x => x.CONG_VIEC_XU_LY_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");

            Map(x => x.FEED_BACK_SUBJECT);
            Map(x => x.FEED_BACK_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");

            Map(x => x.UPDATE_INFO_BID_SUBJECT);
            Map(x => x.UPDATE_INFO_BID_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}
