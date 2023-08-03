namespace SMO
{
    public enum WorkFlowType
    {
        Process,
        Organize,
        Activity,
        Sender,
        Receiver,
        Com
    }

    public enum ActivityType
    {
        Standard,
        Approve
    }

    public enum ApproveType
    {
        Approve,
        Reject
    }

    public enum Domain
    {
        LANG,
        OBJECT_TYPE,
        ORGANIZE_TYPE,
        CONFIG_TYPE,
        USER_TYPE,
        LOAI_HINH_DOANH_NGHIEP,
        VAI_TRO
    }

    public enum Budget
    {
        COST_CENTER,
        COST_ELEMENT,
        PROFIT_CENTER,
        OTHER_PROFIT_CENTER,
        REVENUE_ELEMENT,
        INTERNAL_ORDER
    }

    public enum TemplateType
    {
        CENTER,
        ELEMENT,
    }

    public enum DiffType
    {
        MODIFIED,
        ADD,
        DELELTE
    }

    public enum Activity
    {
        AC_HUY_NOP,
        AC_NHAP_DU_LIEU,
        AC_TONG_HOP,
        AC_TRINH_DUYET,
        AC_HUY_TRINH_DUYET,
        AC_PHE_DUYET,
        AC_HUY_PHE_DUYET,
        AC_TU_CHOI,
        AC_CHUYEN_TKS,
        AC_TRINH_DUYET_TKS,
        AC_PHE_DUYET_TKS,
        AC_TU_CHOI_TKS,
        //AC_DIEU_CHINH,
        AC_CHUYEN_HDNS,
        AC_KET_THUC_THAM_DINH,
        AC_KET_THUC_TOAN_BO_THAM_DINH,
        AC_TRINH_TGD,
        AC_TGD_PHE_DUYET,
        AC_TGD_TU_CHOI,
        AC_TGD_HUY_PHE_DUYET,
        AC_HUY_TRINH_TGD,
        AC_YEU_CAU_CAP_DUOI_DIEU_CHINH,

        AC_CHUYEN_GIAI_DOAN
    }

    public enum BudgetPeriod
    {
        /// <summary>
        /// Các đơn vị cơ sở nộp ngân sách
        /// </summary>
        CO_SO_NOP_NS = 1,
        /// <summary>
        /// Cấp Ban/Trung tâm tổng hợp NS
        /// </summary>
        BAN_TT_TONG_HOP_NS = 2,
        /// <summary>
        /// BTC Tổng hợp NS tập đoàn
        /// </summary>
        BTC_TONG_HOP_NS = 3,
        /// <summary>
        /// Tổng kiểm soát
        /// </summary>
        TKS = 4,
        /// <summary>
        /// Thẩm định NS
        /// </summary>
        THAM_DINH = 5,
        /// <summary>
        /// Chủ tịch/TGĐ phê duyệt NS toàn tập đoàn
        /// </summary>
        CHU_TICH_TGD_PHE_DUYET_NS = 6
    }
}