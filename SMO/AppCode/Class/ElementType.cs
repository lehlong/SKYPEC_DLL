namespace SMO
{
    public static class ElementType
    {
        public const string ChiPhi = "C";
        public const string DoanhThu = "R";

        public static string GetText(string type)
        {
            switch (type)
            {
                case ChiPhi:
                    return "Chi phí";
                case DoanhThu:
                    return "Doanh thu";
                default:
                    return type;
            }
        }
    }
}