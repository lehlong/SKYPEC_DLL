namespace SMO.Service.Class
{
    public class CompareDataViewModel
    {
        public string TEMPLATE_ID { get; set; }
        public int VERSION_SOURCE { get; set; }
        public int YEAR_SOURCE { get; set; }
        public int VERSION_TARGET { get; set; }
        public int YEAR_TARGET { get; set; }
        public string CENTER_CODE { get; set; }
    }
}