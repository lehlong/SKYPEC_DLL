namespace SMO.Service.Class
{
    public class ViewDataCenterModel
    {
        public string ORG_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public int YEAR { get; set; }
        public decimal? EXCHANGE_RATE { get; set; } = 1;
        public string EXCHANGE_TYPE { get; set; } = "VND";  // fix value is VND
        public int? VERSION { get; set; }
        public bool IS_DRILL_DOWN { get; set; }
        public bool IS_HAS_VALUE { get; set; }
        public bool IS_HAS_NOT_VALUE { get; set; }
        public bool IS_LEAF { get; internal set; }
    }
}
