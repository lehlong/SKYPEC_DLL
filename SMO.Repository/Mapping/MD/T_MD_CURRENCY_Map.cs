using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CURRENCY_Map : BaseMapping<T_MD_CURRENCY>
    {
        public T_MD_CURRENCY_Map()
        {
            Table("T_MD_CURRENCY");
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.EXCHANGE_RATE);
        }
    }
}
