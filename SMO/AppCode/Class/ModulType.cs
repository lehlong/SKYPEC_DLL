namespace SMO
{
    public static class ModulType
    {
        public const string ContructCostCF = "ContructCostCF";
        public const string ContructCostPL = "ContructCostPL";
        public const string CostCF = "CostCF";
        public const string CostPL = "CostPL";
        public const string OtherCostCF = "OtherCostCF";
        public const string OtherCostPL = "OtherCostPL";
        public const string RevenueCF = "RevenueCF";
        public const string RevenuePL = "RevenuePL";

        public static string GetTextSheetName(string type)
        {
            // sheet name trong excel bị giới hạn độ dài
            return GetText(type).Replace("Kế hoạch", "NS");
        }

        public static string GetText(string type)
        {
            switch (type)
            {
                case ContructCostCF:
                    return "Kế hoạch chi dự án";
                case ContructCostPL:
                    return "Kế hoạch chi phí dự án";
                case CostCF:
                    return "Kế hoạch chi thường xuyên";
                case CostPL:
                    return "Kế hoạch chi phí";
                case OtherCostCF:
                    return "Kế hoạch chi khác của dự án";
                case OtherCostPL:
                    return "Kế hoạch chi phí khác của dự án";
                case RevenueCF:
                    return "Kế hoạch thu thường xuyên";
                case RevenuePL:
                    return "Kế hoạch doanh thu";
                default:
                    return type;
            }
        }
    }
}