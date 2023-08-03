using FluentNHibernate.Mapping;

using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_USER_Map : ClassMap<T_AD_USER>
    {
        public T_AD_USER_Map()
        {
            Table("T_AD_USER");
            Id(x => x.USER_NAME);
            Map(x => x.PASSWORD).Not.Nullable();
            Map(x => x.FULL_NAME).Not.Nullable();
            Map(x => x.EMAIL);
            Map(x => x.PHONE);
            Map(x => x.NOTES);
            Map(x => x.USER_TYPE);
            Map(x => x.TITLE_ID);
            Map(x => x.TITLE);
            Map(x => x.ORGANIZE_CODE);
            Map(x => x.ACCOUNT_AD);
            Map(x => x.LAST_CHANGE_PASS_DATE);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_MODIFY_RIGHT).Not.Nullable().CustomType<YesNoType>();
            Map(m => m.CREATE_BY).Not.Update();
            Map(m => m.CREATE_DATE).Generated.Insert().Not.Update();
            Map(m => m.UPDATE_BY).Not.Insert();
            Map(m => m.UPDATE_DATE).Not.Insert();
            HasMany(x => x.ListUserUserGroup).KeyColumn("USER_NAME").LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.ListUserRight).KeyColumn("USER_NAME").LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.ListUserRole).KeyColumn("USER_NAME").LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.ListUserHistory).KeyColumn("USER_NAME").LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.ListUserOrg).KeyColumn("USER_NAME").LazyLoad().Inverse().Cascade.Delete();
            References(x => x.Organize).Column("ORGANIZE_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
