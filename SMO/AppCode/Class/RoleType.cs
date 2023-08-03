namespace SMO
{
    public static class RoleType
    {
        public const string ThamDinhGia = "TDG";
        public const string ThamDinhKyThuat = "TDKT";
        public const string ThamDinhChatLuong = "TDCL";

        public static string GetText(string type)
        {
            if (type == RoleType.ThamDinhGia)
            {
                return "Thẩm định giá";
            }
            else if (type == RoleType.ThamDinhKyThuat)
            {
                return "Thẩm định kỹ thuật";
            }
            else if (type == RoleType.ThamDinhChatLuong)
            {
                return "Thẩm định chất lượng";
            }
            else
            {
                return type;
            }
        }
    }
}