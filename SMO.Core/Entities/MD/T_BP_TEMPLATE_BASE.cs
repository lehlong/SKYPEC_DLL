namespace SMO.Core.Entities.MD
{
    public class T_BP_TEMPLATE_BASE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string TEMPLATE_CODE { get; set; }
        public virtual int TIME_YEAR { get; set; }
        public virtual string FILE_ID { get; set; }
        public virtual T_CM_FILE_UPLOAD FileUpload { get; set; }
    }
}
