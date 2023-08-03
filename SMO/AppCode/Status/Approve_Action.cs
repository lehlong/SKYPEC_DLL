namespace SMO
{
    public class Approve_Action
    {
        public const string HuyNop = "00";

        public const string NhapDuLieu = "01";
        public const string TongHop = "02";
        public const string TrinhDuyet = "03";
        public const string HuyTrinhDuyet = "04";
        public const string PheDuyetDuLieu = "05";
        public const string HuyPheDuyet = "06";
        public const string TuChoi = "07";

        public const string ChuyenTKS = "08";
        public const string KiemSoat = "09";
        public const string PheDuyetKiemSoat = "10";
        public const string DieuChinh = "11";
        public const string ChuyenHDNS = "12";
        public const string ThamDinh = "13";
        public const string KetThucThamDinh = "14";
        public const string TrinhTGD = "15";

        public const string TGD_PheDuyet = "16";
        public const string TGD_TuChoi = "17";
        public const string TGD_HuyPheDuyet = "18";

        public const string YeuCauCapDuoiDieuChinh = "19";
        public const string CapTrenYeuCauDieuChinh = "20";

        public const string TrinhDuyetKiemSoat = "21";
        public const string TuChoiKiemSoat = "22";

        public static string GetStatusText(string status, string lang = "vi")
        {
            if (lang == "vi")
            {
                switch (status)
                {
                    case HuyNop:
                        return "Hủy nộp dữ liệu";
                    case NhapDuLieu:
                        return "Nhập dữ liệu";
                    case TongHop:
                        return "Tổng hợp dữ liệu";
                    case TrinhDuyet:
                        return "Trình duyệt";
                    case HuyTrinhDuyet:
                        return "Hủy trình duyệt";
                    case PheDuyetDuLieu:
                        return "Phê duyệt";
                    case HuyPheDuyet:
                        return "Hủy phê duyệt";
                    case TuChoi:
                        return "Từ chối";
                    case ChuyenTKS:
                        return "Chuyển tổng kiểm soát";
                    case KiemSoat:
                        return "Kiểm soát dữ liệu";
                    case PheDuyetKiemSoat:
                        return "Phê duyệt kiểm soát dữ liệu";
                    case TuChoiKiemSoat:
                        return "Yêu cầu điều chỉnh kiểm soát dữ liệu";
                    case TrinhDuyetKiemSoat:
                        return "Trình duyệt kiểm soát dữ liệu";
                    case DieuChinh:
                        return "Yêu cầu điều chỉnh";
                    case ChuyenHDNS:
                        return "Chuyển HĐNS";
                    case ThamDinh:
                        return "Thẩm định dữ liệu";
                    case KetThucThamDinh:
                        return "Kết thúc thẩm định";
                    case TrinhTGD:
                        return "Trình tổng giám đốc";
                    case TGD_PheDuyet:
                        return "Tổng giám đốc phê duyệt";
                    case TGD_TuChoi:
                        return "Tổng giám đốc từ chối";
                    case TGD_HuyPheDuyet:
                        return "Tổng giám đốc hủy phê duyệt";
                    case YeuCauCapDuoiDieuChinh:
                        return "Yêu cầu cấp dưới điều chỉnh";
                    case CapTrenYeuCauDieuChinh:
                        return "Cấp trên yêu cầu điều chỉnh";
                    default:
                        return status;
                }
            }
            else
            {
                switch (status)
                {
                    case HuyNop:
                        return "Hủy nộp dữ liệu";
                    case NhapDuLieu:
                        return "Nhập dữ liệu";
                    case TongHop:
                        return "Tổng hợp dữ liệu";
                    case TrinhDuyet:
                        return "Trình duyệt";
                    case HuyTrinhDuyet:
                        return "Hủy trình duyệt";
                    case PheDuyetDuLieu:
                        return "Phê duyệt";
                    case HuyPheDuyet:
                        return "Hủy phê duyệt";
                    case TuChoi:
                        return "Từ chối";
                    case ChuyenTKS:
                        return "Chuyển tổng kiểm soát";
                    case KiemSoat:
                        return "Kiểm soát dữ liệu";
                    case PheDuyetKiemSoat:
                        return "Phê duyệt kiểm soát dữ liệu";
                    case TuChoiKiemSoat:
                        return "Yêu cầu điều chỉnh kiểm soát dữ liệu";
                    case TrinhDuyetKiemSoat:
                        return "Trình duyệt kiểm soát dữ liệu";
                    case DieuChinh:
                        return "Yêu cầu điều chỉnh";
                    case ChuyenHDNS:
                        return "Chuyển HĐNS";
                    case ThamDinh:
                        return "Thẩm định dữ liệu";
                    case KetThucThamDinh:
                        return "Kết thúc thẩm định";
                    case TrinhTGD:
                        return "Trình tổng giám đốc";
                    case TGD_PheDuyet:
                        return "Tổng giám đốc phê duyệt";
                    case TGD_TuChoi:
                        return "Tổng giám đốc từ chối";
                    case TGD_HuyPheDuyet:
                        return "Tổng giám đốc hủy phê duyệt";
                    case YeuCauCapDuoiDieuChinh:
                        return "Yêu cầu cấp dưới điều chỉnh";
                    case CapTrenYeuCauDieuChinh:
                        return "Cấp trên yêu cầu điều chỉnh";
                    default:
                        return status;
                }
            }
        }

        public static string GetStatusColor(string status)
        {
            return "";
        }
    }
}