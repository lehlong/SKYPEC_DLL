namespace SMO.Service.Class
{
    public abstract class BaseReviewViewModel
    {
        public string OrgCode { get; set; }
        public string ParentFormId { get; set; }
        public string Id { get; set; }
        public int Year { get; set; }
        public int Version { get; set; }
        public bool? Status { get; set; }
        public bool IsEnd { get; set; }
        /// <summary>
        /// Có phải TKS hay không
        /// </summary>
        public bool IsSummary { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsNotCompleted { get; set; }

    }
}