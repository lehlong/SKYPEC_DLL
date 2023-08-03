namespace SMO.Core.Entities.BP
{
    public class T_BP_STEPPER_BUDGET : BaseEntity
    {
        public virtual int ID { get; set; }
        /// <summary>
        /// link with constant Approve Status
        /// </summary>
        public virtual string STATUS_ID { get; set; }
        public virtual string ACTION_ID { get; set; }
        public virtual string BASE_ACTION_ID { get; set; }
        public virtual string DISPLAY_TEXT { get; set; }
        public virtual string ACTION_USER { get; set; }
        public virtual int ORDER { get; set; }
        /// <summary>
        /// Level of center in company. Company is level 0
        /// </summary>
        public virtual int? LEVEL { get; set; }
        public virtual string DESCRIPTION { get; set; }
    }
}
