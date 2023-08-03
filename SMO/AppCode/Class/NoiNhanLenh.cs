namespace SMO
{
    public class NoiNhanLenh
    {
        public const string VanPhong = "VP";
        public const string NhaBe = "NB";

        public static string GetText(string type)
        {
            if (type == VanPhong)
            {
                return "Văn phòng";
            }
            else if (type == NhaBe)
            {
                return "Kho Nhà Bè";
            }
            return "";
        }
    }
}