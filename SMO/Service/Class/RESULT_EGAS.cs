using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class RESULT_EGAS
    {
        public bool STATUS { get; set; }
        public List<EGAS_HEADER> DATA { get; set; }
        public string MESSAGE { get; set; }
    }
}