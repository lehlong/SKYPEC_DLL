using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_FAQ_QUESTION_Map : BaseMapping<T_FAQ_QUESTION>
    {
        public T_FAQ_QUESTION_Map()
        {
            Table("T_FAQ_QUESTION");
            Id(x => x.PKID);
            Map(x => x.NAME);
            Map(x => x.EMAIL);
            Map(x => x.SUBJECT);
            Map(x => x.CONTENTS);
            Map(x => x.ANSWER);
            Map(x => x.STATUS).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
