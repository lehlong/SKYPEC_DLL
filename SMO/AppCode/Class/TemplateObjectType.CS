namespace SMO
{
    public static class TemplateObjectType
    {
        /// <summary>
        /// Phòng ban
        /// </summary>
        public const string Department = "01";
        /// <summary>
        /// Dự án (Other)
        /// </summary>
        public const string Project = "02";
        /// <summary>
        /// Xây dựng dự án (Contruct)
        /// </summary>
        public const string DevelopProject = "03";

        public static string GetText(string type)
        {
            switch (type)
            {
                case Department:
                    return "Phòng ban";
                case Project:
                    return "Dự án";
                case DevelopProject:
                    return "Xây dựng dự án";
                default:
                    return type;
            }
        }
    }
}
