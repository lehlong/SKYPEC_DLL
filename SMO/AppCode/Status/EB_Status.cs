namespace SMO
{
    public class EB_Status
    {
        public const string KhoiTao = "01";
        public const string ChoPheDuyet = "02";
        public const string TuChoi = "03";
        public const string ChoCapNhat = "04";
        public const string DaPheDuyet = "05";
        public const string Huy = "06";

        public static string GetStatusText(string status)
        {
            var lang = "vi";
            if (ProfileUtilities.User != null)
            {
                lang = ProfileUtilities.User.LANGUAGE;
            }

            if (lang != "vi" && lang != "en")
            {
                lang = "vi";
            }

            if (lang == "vi")
            {
                switch (status)
                {
                    case KhoiTao:
                        return "Khởi tạo";
                    case ChoPheDuyet:
                        return "Chờ phê duyệt";
                    case TuChoi:
                        return "Từ chối";
                    case ChoCapNhat:
                        return "Chờ cập nhật";
                    case DaPheDuyet:
                        return "Đã phê duyệt";
                    case Huy:
                        return "Đã bị hủy";
                    default:
                        return status;
                }
            }
            else
            {
                switch (status)
                {
                    case KhoiTao:
                        return "Created";
                    case ChoPheDuyet:
                        return "Waiting Approve";
                    case TuChoi:
                        return "Reject";
                    case ChoCapNhat:
                        return "Required Update";
                    case DaPheDuyet:
                        return "Approved";
                    case Huy:
                        return "Cancel";
                    default:
                        return status;
                }
            }

        }

        public static string GetStatusColor(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "bg-indigo";
                case ChoPheDuyet:
                    return "bg-brown";
                case TuChoi:
                    return "bg-red";
                case ChoCapNhat:
                    return "bg-deep-orange";
                case DaPheDuyet:
                    return "bg-blue";
                case Huy:
                    return "bg-red";
                default:
                    return "";
            }
        }

        public static string GetStatusIcon(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "note_add";
                case ChoPheDuyet:
                    return "hourglass_empty";
                case TuChoi:
                    return "cancel";
                case ChoCapNhat:
                    return "hourglass_full";
                case DaPheDuyet:
                    return "done_all";
                case Huy:
                    return "delete";
                default:
                    return "";
            }
        }
    }
}