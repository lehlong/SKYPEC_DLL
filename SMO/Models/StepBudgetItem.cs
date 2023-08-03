using System;

namespace SMO.Models
{
    public class StepBudgetItem
    {
        public int Order { get; set; }
        public bool Status { get; set; }
        public string DisplayText { get; set; }
        public string ActionUser { get; set; }
        public string ActionUserFullname { get; set; }
        public string CenterName { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
