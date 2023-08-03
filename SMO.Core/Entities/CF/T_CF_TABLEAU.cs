namespace SMO.Core.Entities
{
    public partial class T_CF_TABLEAU : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string PARENT_ID { get; set; }
        public virtual string NAME { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool IS_GROUP { get; set; }
        public virtual string RIGHT_CODE { get; set; }
        public virtual string SITE_NAME { get; set; }
        public virtual string SITE_ID { get; set; }
        public virtual string WORKBOOK_NAME { get; set; }
        public virtual string WORKBOOK_CONTENT_URL { get; set; }
        public virtual string WORKBOOK_ID { get; set; }
        public virtual string VIEW_NAME { get; set; }
        public virtual string VIEW_CONTENT_URL { get; set; }
        public virtual string VIEW_ID { get; set; }
        public virtual string PATH_IMAGE_PREVIEW { get; set; }
        public virtual bool IS_TAB { get; set; }
        public virtual bool IS_SHOW_APP_BANNER { get; set; }
        public virtual bool IS_TOOLBAR { get; set; }
    }
}
