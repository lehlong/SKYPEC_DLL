namespace SMO.AppCode.Status
{
    public class BudgetPeriodAction
    {
        public const string CHUYEN_GIAI_DOAN_DONG = "03";
        public const string MO_GIAI_DOAN = "01";
        public const string DONG_GIAI_DOAN = "02";
        public const string CHUYEN_GIAI_DOAN_MO = "00";
        public static string GetBudgetPeriodActionText(string action, string lang = "vi")
        {
            if (lang == "vi")
            {
                switch (action)
                {
                    case CHUYEN_GIAI_DOAN_MO:
                        return "Chuyển giai đoạn (Mở)";
                    case CHUYEN_GIAI_DOAN_DONG:
                        return "Chuyển giai đoạn (Đóng)";
                    case MO_GIAI_DOAN:
                        return "Mở giai đoạn";
                    case DONG_GIAI_DOAN:
                        return "Đóng giai đoạn";
                    default:
                        return string.Empty;
                }
            }
            else
            {
                switch (action)
                {
                    case CHUYEN_GIAI_DOAN_MO:
                        return "Chuyển giai đoạn (Mở)";
                    case CHUYEN_GIAI_DOAN_DONG:
                        return "Chuyển giai đoạn (Đóng)";
                    case MO_GIAI_DOAN:
                        return "Mở giai đoạn";
                    case DONG_GIAI_DOAN:
                        return "Đóng giai đoạn";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
