namespace SMO
{
    public static class BudgetType
    {
        public const string KinhDoanh = "P";
        public const string DongTien = "C";

        public static string GetText(string type)
        {
            switch (type)
            {
                case KinhDoanh:
                    return "Kinh doanh";
                case DongTien:
                    return "Dòng tiền";
                default:
                    return type;
            }
        }
    }
}