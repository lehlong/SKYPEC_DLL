using System.Collections.Generic;

namespace SMO.Service.Class.WF
{
    public class ConfigSendViewModel
    {
        public ConfigSendViewModel()
        {
            Receivers = new List<string>();
        }
        public string OrgCode { get; set; }
        public string ActivityCode { get; set; }
        public string Sender { get; set; }
        public IList<string> Receivers { get; set; }
    }
}
