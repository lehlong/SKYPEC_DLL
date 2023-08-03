namespace SMO
{
    public class Approve_Status
    {
        public const string ChuaTrinhDuyet = "01";
        public const string ChoPheDuyet = "02";
        public const string DaPheDuyet = "03";
        public const string TuChoi = "04";

        public const string TGD_PheDuyet = "05";
        public const string TGD_TuChoi = "06";
        public const string TGD_HuyPheDuyet = "07";
        public const string TGD_ChoPheDuyet = "08";
        public const string TGD_HuyTrinh = "09";

        public const string TKS_DuLieu = "10";
        public const string TKS_PheDuyet = "11";

        public const string ThamDinh_DuLieu = "12";
        public const string ThamDinh_KetThuc = "13";

        public const string TKS_TrinhDuyet = "14";
        public const string TKS_TuChoi = "15";

        public static string GetStatusText(string status, string lang = "vi")
        {
            if (lang == "vi")
            {
                switch (status)
                {
                    case ChuaTrinhDuyet:
                        return "Chưa trình duyệt";
                    case ChoPheDuyet:
                        return "Chờ phê duyệt";
                    case DaPheDuyet:
                        return "Đã phê duyệt";
                    case TuChoi:
                        return "Từ chối";
                    case TGD_PheDuyet:
                        return "TGĐ phê duyệt";
                    case TGD_TuChoi:
                        return "TGĐ từ chối";
                    case TGD_HuyPheDuyet:
                        return "TGĐ hủy phê duyệt";
                    case TGD_ChoPheDuyet:
                        return "Chờ TGĐ phê duyệt";
                    case TGD_HuyTrinh:
                        return "Hủy trình TGĐ";
                    case TKS_DuLieu:
                        return "TKS dữ liệu";
                    case TKS_TrinhDuyet:
                        return "TKS Trình duyệt";
                    case TKS_TuChoi:
                        return "TKS Từ chối";
                    case TKS_PheDuyet:
                        return "TKS Phê duyệt";
                    case ThamDinh_DuLieu:
                        return "Thẩm định dữ liệu";
                    case ThamDinh_KetThuc:
                        return "Kết thúc thẩm định";
                    default:
                        return status;
                }
            }
            else
            {
                switch (status)
                {
                    case ChuaTrinhDuyet:
                        return "Chưa trình duyệt";
                    case ChoPheDuyet:
                        return "Chờ phê duyệt";
                    case DaPheDuyet:
                        return "Đã phê duyệt";
                    case TuChoi:
                        return "Từ chối";
                    case TGD_PheDuyet:
                        return "TGĐ phê duyệt";
                    case TGD_TuChoi:
                        return "TGĐ từ chối";
                    case TGD_HuyPheDuyet:
                        return "TGĐ hủy phê duyệt";
                    case TGD_ChoPheDuyet:
                        return "Chờ TGĐ phê duyệt";
                    case TGD_HuyTrinh:
                        return "Hủy trình TGĐ";
                    case TKS_DuLieu:
                        return "TKS dữ liệu";
                    case TKS_TrinhDuyet:
                        return "TKS Trình duyệt";
                    case TKS_TuChoi:
                        return "TKS Từ chối";
                    case TKS_PheDuyet:
                        return "TKS Phê duyệt";
                    case ThamDinh_DuLieu:
                        return "Thẩm định dữ liệu";
                    case ThamDinh_KetThuc:
                        return "Kết thúc thẩm định";
                    default:
                        return status;
                }
            }
        }

        public static string GetStatusColor(string status)
        {
            switch (status)
            {
                case ChuaTrinhDuyet:
                case TKS_TrinhDuyet:
                    return "bg-blue-grey";
                case ChoPheDuyet:
                case TGD_ChoPheDuyet:
                    return "bg-brown";
                case DaPheDuyet:
                case TGD_PheDuyet:
                case TKS_PheDuyet:
                    return "bg-green";
                case TuChoi:
                case TGD_TuChoi:
                case TKS_TuChoi:
                    return "bg-red";
                default:
                    return status;
            }
        }
    }
}