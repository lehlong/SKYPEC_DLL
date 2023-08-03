using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using SMO.AppCode.Utilities;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Core.Entities.BP.COST_PL;
using SMO.Core.Entities.BP.COST_PL.COST_PL_DATA_BASE;
using SMO.Core.Entities.MD;
using SMO.Helper;
using SMO.Models;
using SMO.Repository.Implement.BP;
using SMO.Repository.Implement.BP.COST_PL;
using SMO.Repository.Implement.BP.COST_PL.COST_PL_DATA_BASE;
using SMO.Repository.Implement.MD;
using SMO.Service.BP.COST_PL;
using SMO.Service.Class;
using SMO.Service.Common;
using SMO.ServiceInterface.BP.CostPL;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using static SMO.SelectListUtilities;

namespace SMO.Service.BP
{
    public class CostPLService : BaseBPService<T_BP_COST_PL, CostPLRepo, T_MD_COST_PL_ELEMENT, T_BP_COST_PL_VERSION, T_BP_COST_PL_HISTORY, CostPLHistoryRepo>, ICostPLService
    {
        private readonly List<Point> InvalidCellsList;
        private readonly List<string> ListColumnName;
        private readonly List<string> ListColumnNameDataBase;
        private int StartRowData;
        public List<T_BP_COST_PL_HISTORY> ObjListHistory { get; set; }
        public List<T_BP_COST_PL_VERSION> ObjListVersion { get; set; }
        public List<T_BP_COST_PL_SUM_UP_DETAIL> ObjListSumUpHistory { get; set; }

        public CostPLService()
        {
            this.StartRowData = 6;
            this.ListColumnName = new List<string>
            {
                "MÃ ĐƠN VỊ",
                "TÊN ĐƠN VỊ",
                "MÃ KHOẢN MỤC",
                "TÊN KHOẢN MỤC",
                "THÁNG 1",
                "THÁNG 2",
                "THÁNG 3",
                "THÁNG 4",
                "THÁNG 5",
                "THÁNG 6",
                "THÁNG 7",
                "THÁNG 8",
                "THÁNG 9",
                "THÁNG 10",
                "THÁNG 11",
                "THÁNG 12",
                "TỔNG NĂM",
                "TRUNG BÌNH NĂM",
                "GHI CHÚ"
            };

            this.ListColumnNameDataBase = new List<string> 
            { 
                "MÃ ĐƠN VỊ", 
                "TÊN ĐƠN VỊ", 
                "MÃ KHOẢN MỤC", 
                "TÊN KHOẢN MỤC", 
                "HÀNG HÓA, DỊCH VỤ", 
                "ĐƠN VỊ TÍNH", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "ĐƠN GIÁ", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "THÀNH TIỀN", 
                "SỐ LƯỢNG", "THÀNH TIỀN", 
                "GHI CHÚ" 
            };
            this.InvalidCellsList = new List<Point>();
            this.ObjListHistory = new List<T_BP_COST_PL_HISTORY>();
            this.ObjListVersion = new List<T_BP_COST_PL_VERSION>();
            this.ObjListSumUpHistory = new List<T_BP_COST_PL_SUM_UP_DETAIL>();
        }

        /// <summary>
        /// check xem người dùng hiện tại có hiển thị nút thẩm định hay không
        /// </summary>
        /// <returns></returns>
        public override bool ShowReviewBtn()
        {
            return ShowReviewBtn(ObjDetail.TIME_YEAR);
        }

        public override bool ShowReviewBtn(int year)
        {
            var lstReviewUsers = UnitOfWork.Repository<UserReviewRepo>()
                .GetManyWithFetch(x => x.TIME_YEAR == year)
                .Select(x => x.USER_NAME);
            var currentUser = ProfileUtilities.User?.USER_NAME;

            // check người dùng có nằm trong hội đồng thẩm định 
            if (!lstReviewUsers.Contains(currentUser))
            {
                return false;
            }
            else
            {
                var corp = UnitOfWork.Repository<CostCenterRepo>()
                    .GetFirstWithFetch(x => x.PARENT_CODE == "").CODE;
                var version = GetFirstWithFetch(x => x.ORG_CODE == corp &&
                    x.TIME_YEAR == year)?.VERSION;
                if (version == null)
                {
                    return false;
                }
                // check người dùng đã kết thúc thẩm định chưa
                var historyReview = UnitOfWork.Repository<CostPLReviewRepo>()
                    .GetFirstWithFetch(x => x.ORG_CODE == corp &&
                    x.DATA_VERSION == version &&
                    x.TIME_YEAR == year &&
                    x.REVIEW_USER == currentUser &&
                    x.IS_END &&
                    !x.IS_SUMMARY);

                return historyReview == null;
            }
        }

        #region Workflow
        private void SendNotify(Activity activity)
        {
            NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = activity,
                        OrgCode = ProfileUtilities.User?.ORGANIZE_CODE,
                        TimeYear = this.ObjDetail.TIME_YEAR,
                        ModulType = ModulType.CostPL,
                        TemplateCode = this.ObjDetail.TEMPLATE_CODE,
                        UserSent = ProfileUtilities.User?.USER_NAME
                    });
        }
        public override void TrinhDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTrinhDuyet(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.ChoPheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TrinhDuyet,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.ChoPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();

                SendNotify(Activity.AC_TRINH_DUYET);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void HuyTrinhDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateHuyTrinhDuyet(code))
                {
                    return;
                }

                if (this.ObjDetail.STATUS == Approve_Status.ChoPheDuyet)
                {
                    ObjDetail.STATUS = Approve_Status.ChuaTrinhDuyet;
                }
                else
                {
                    ObjDetail.STATUS = Approve_Status.TGD_HuyTrinh;
                }
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.HuyTrinhDuyet,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.ChuaTrinhDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();

                if (ObjDetail.STATUS == Approve_Status.TGD_HuyTrinh)
                {
                    SendNotify(Activity.AC_HUY_TRINH_TGD);
                }
                else
                {
                    SendNotify(Activity.AC_HUY_TRINH_DUYET);
                }

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TGDTuChoi(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTGDTuChoi(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TGD_TuChoi;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TGD_TuChoi,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                UnitOfWork.Commit();
                SendNotify(Activity.AC_TGD_TU_CHOI);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TGDHuyPheDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTGDHuyPheDuyet(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TGD_ChoPheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.TGD_HuyPheDuyet,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                UnitOfWork.Commit();

                SendNotify(Activity.AC_TGD_HUY_PHE_DUYET);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TGDPheDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTGDPheDuyet(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TGD_PheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.TGD_PheDuyet,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                UnitOfWork.Commit();
                SendNotify(Activity.AC_TGD_PHE_DUYET);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void PheDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidatePheDuyet(code))
                {
                    return;
                }
                ObjDetail.STATUS = Approve_Status.DaPheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.PheDuyetDuLieu,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.DaPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();
                SendNotify(Activity.AC_PHE_DUYET);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void HuyNop(string code)
        {
            // get header
            var currentUser = ProfileUtilities.User?.USER_NAME;
            if (!ValidateHuyNop(code))
            {
                return;
            }
            // get history
            var lstHistory = UnitOfWork.Repository<CostPLHistoryRepo>()
                .GetManyWithFetch(x => x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.VERSION == ObjDetail.VERSION);
            // check header is in ChuaTrinhDuyet status or not
            if (lstHistory.Count == 1)
            {
                var history = lstHistory.First();
                // update version property
                try
                {
                    UnitOfWork.BeginTransaction();
                    // update version table
                    UnitOfWork.Repository<CostPLVersionRepo>().Update(x => x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.VERSION == ObjDetail.VERSION, x => x.IS_DELETED = 1, x => x.UPDATE_BY = currentUser);

                    // update main table
                    ObjDetail.IS_DELETED = true; CurrentRepository.Update(ObjDetail);

                    // create history log
                    UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.HuyNop,
                        ACTION_USER = currentUser,
                        CREATE_BY = currentUser,
                        ACTION_DATE = DateTime.Now
                    });
                    UnitOfWork.Commit();
                    SendNotify(Activity.AC_HUY_NOP);
                }
                catch (Exception e)
                {
                    UnitOfWork.Rollback();
                    State = false;
                    Exception = e;
                }

            }
            else
            {
                State = false;
                ErrorMessage = "Không thể hủy nộp version đã từng được trình duyệt.";
            }
        }

        public override void HuyPheDuyet(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateHuyPheDuyet(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.ChoPheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.HuyPheDuyet,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.ChoPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();

                SendNotify(Activity.AC_HUY_PHE_DUYET);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TuChoi(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTuChoi(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TuChoi;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = ObjDetail.ORG_CODE,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = ObjDetail.VERSION,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.TuChoi,
                    ACTION_USER = currentUser,
                    ACTION_DATE = DateTime.Now,
                    CREATE_BY = currentUser
                });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.TuChoi}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();
                SendNotify(Activity.AC_TU_CHOI);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void ChuyenTKS(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateChuyenTKS(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TKS_DuLieu;
                ObjDetail.UPDATE_BY = currentUser;
                ObjDetail.IS_REVIEWED = true;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.ChuyenTKS,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                // TODO: cập nhật trạng thái cho những người thẩm định và tổng kiểm soát về trạng thái chưa kết thúc
                UnitOfWork.Repository<CostPLReviewRepo>()
                    .Update(x => x.ORG_CODE == ObjDetail.ORG_CODE
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR
                    && x.DATA_VERSION == ObjDetail.VERSION,
                    x => x.IS_END = false);

                UnitOfWork.Commit();
                SendNotify(Activity.AC_CHUYEN_TKS);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void KetThucThamDinh(string orgCode, int year, int version)
        {
            try
            {
                if (!ValidateKetThucThamDinh(orgCode, year, version))
                {
                    return;
                }
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var reviewHeader = UnitOfWork.Repository<CostPLReviewRepo>()
                    .GetFirstWithFetch(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year && x.DATA_VERSION == version && !x.IS_SUMMARY && x.REVIEW_USER == currentUser);
                if (reviewHeader != null)
                {
                    reviewHeader.IS_END = true;
                    UnitOfWork.Repository<CostPLReviewRepo>().Update(reviewHeader);
                }
                

                var totalReviewUsers = UnitOfWork.Repository<UserReviewRepo>()
                    .GetManyWithFetch(x => x.TIME_YEAR == ObjDetail.TIME_YEAR).Count;

                var totalUsersEndReview = UnitOfWork.Repository<CostPLReviewRepo>()
                    .GetManyWithFetch(x => x.TIME_YEAR == ObjDetail.TIME_YEAR &&
                    x.ORG_CODE == ObjDetail.ORG_CODE &&
                    x.DATA_VERSION == ObjDetail.VERSION &&
                    x.IS_END &&
                    !x.IS_SUMMARY).Count;

                if (totalReviewUsers <= totalUsersEndReview)
                {
                    ObjDetail.STATUS = Approve_Status.ThamDinh_KetThuc;
                    ObjDetail.UPDATE_BY = currentUser;

                    CurrentRepository.Update(ObjDetail);
                }

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.KetThucThamDinh,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                var review = UnitOfWork.Repository<CostPLReviewRepo>()
                    .GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.DATA_VERSION == version
                                                && !x.IS_SUMMARY
                                                && x.REVIEW_USER == currentUser);
                if (review == null)
                {
                    var reviewId = Guid.NewGuid().ToString();
                    UnitOfWork.Repository<CostPLReviewRepo>().Create(
                        new T_BP_COST_PL_REVIEW
                        {
                            PKID = reviewId,
                            DATA_VERSION = version,
                            IS_END = true,
                            IS_SUMMARY = false,
                            ORG_CODE = orgCode,
                            REVIEW_USER = currentUser,
                            TIME_YEAR = year,
                            CREATE_BY = currentUser,
                        });

                    // create all result not complete
                    var plReviewService = new CostPLReviewService();
                    plReviewService.ObjDetail.TIME_YEAR = year;
                    plReviewService.ObjDetail.ORG_CODE = orgCode;
                    var dataCost = plReviewService.SummaryCenterVersion(out IList<T_BP_COST_PL_DATA> detailCostElements);
                    dataCost = dataCost.OrderBy(x => x.C_ORDER)
                                                    .GroupBy(x => x.CODE)
                                                    .Select(x => x.First()).ToList();
                    var elements = plReviewService.PrepareListReview(dataCost);
                    UnitOfWork.Repository<CostPLReviewResultRepo>()
                        .Create((from e in elements.Where(x => !x.IS_GROUP)
                                 select new T_BP_COST_PL_REVIEW_RESULT
                                 {
                                     PKID = Guid.NewGuid().ToString(),
                                     COST_PL_ELEMENT_CODE = e.CODE,
                                     HEADER_ID = reviewId,
                                     RESULT = false,
                                     TIME_YEAR = year,
                                     CREATE_BY = currentUser
                                 }).ToList());
                }
                else
                {
                    review.IS_END = true;
                    UnitOfWork.Repository<CostPLReviewRepo>().Update(review);
                }

                UnitOfWork.Commit();
                SendNotify(Activity.AC_KET_THUC_THAM_DINH);
                if (totalReviewUsers <= totalUsersEndReview)
                {
                    SendNotify(Activity.AC_KET_THUC_TOAN_BO_THAM_DINH);
                }

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void PheDuyetTongKiemSoat(string orgCode, int year, int version)
        {
            try
            {
                if (!ValidatePheDuyetTKS(orgCode, year, version))
                {
                    return;
                }

                var currentUser = ProfileUtilities.User?.USER_NAME;

                ObjDetail.STATUS = Approve_Status.TKS_PheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.PheDuyetKiemSoat,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.TKS_PheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

                UnitOfWork.Commit();

                SendNotify(Activity.AC_PHE_DUYET_TKS);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TrinhDuyetTongKiemSoat(string orgCode, int year, int version)
        {
            try
            {
                if (!ValidateTrinhDuyetTKS(orgCode, year, version))
                {
                    return;
                }
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();

                ObjDetail.STATUS = Approve_Status.TKS_TrinhDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TrinhDuyetKiemSoat,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });
                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.TKS_TrinhDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();
                UnitOfWork.Repository<CostPLReviewRepo>()
                    .Update(x => x.ORG_CODE == orgCode
                                && x.TIME_YEAR == year
                                && x.DATA_VERSION == version
                                && x.IS_SUMMARY, x => x.IS_END = true);

                UnitOfWork.Commit();

                SendNotify(Activity.AC_TRINH_DUYET_TKS);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }
        internal override void TuChoiTongKiemSoat(string orgCode, int year, int version, string comment)
        {
            try
            {
                if (!ValidateTuChoiTKS(orgCode, year, version))
                {
                    return;
                }

                var currentUser = ProfileUtilities.User?.USER_NAME;

                ObjDetail.STATUS = Approve_Status.TKS_TuChoi;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                var note = $"Yêu cầu điều chỉnh lại dữ liệu TKS. Lý do: [{comment}]";
                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        NOTES = note,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TuChoiKiemSoat,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_COST_PL_DATA SET STATUS = '{Approve_Status.TKS_TuChoi}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();
                UnitOfWork.Repository<CostPLReviewRepo>()
                    .Update(x => x.ORG_CODE == orgCode
                                && x.TIME_YEAR == year
                                && x.DATA_VERSION == version
                                && x.IS_SUMMARY, x => x.IS_END = false);
                UnitOfWork.Commit();

                SendNotify(Activity.AC_TU_CHOI_TKS);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }


        public override void YeuCauCapDuoiDieuChinh(string childOrgCode, string templateCode, int timeYear, string comment, int? templateVersion, int? parentVersion, bool isSummaryReview)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                // Tìm version hiện tại của đơn vị cha
                var parentOrgCode = ProfileUtilities.User.ORGANIZE_CODE;
                var findParent = GetFirstByExpression(x => x.ORG_CODE == parentOrgCode && x.TIME_YEAR == timeYear);
                var findChild = GetFirstByExpression(x => x.ORG_CODE == childOrgCode && x.TIME_YEAR == timeYear);
                if (!ValidateYeuCauDieuChinh(timeYear, parentOrgCode, isSummaryReview))
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(templateCode))
                {
                    findChild = GetFirstByExpression(x => x.ORG_CODE == childOrgCode && x.TEMPLATE_CODE == templateCode && x.TIME_YEAR == timeYear);
                }
                var childOrg = UnitOfWork.Repository<CostCenterRepo>().Get(childOrgCode);
                var parentOrg = UnitOfWork.Repository<CostCenterRepo>().Get(parentOrgCode);
                var template = UnitOfWork.Repository<TemplateRepo>().Get(templateCode);
                var noteParent = $"Yêu cầu đơn vị [{childOrg.NAME}] cập nhật lại kế hoạch của mẫu [{(string.IsNullOrWhiteSpace(templateCode) ? "Dữ liệu tổng hợp" : $"{template.CODE}-{template.NAME}")}]. Lý do: [{comment}]";

                UnitOfWork.BeginTransaction();
                // Tạo history của đơn vị cha
                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = parentOrgCode,
                        TEMPLATE_CODE = templateCode,
                        VERSION = parentVersion ?? (findParent == null ? 0 : findParent.VERSION),
                        TIME_YEAR = timeYear,
                        ACTION = Approve_Action.YeuCauCapDuoiDieuChinh,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser,
                        NOTES = noteParent
                    });

                // Tạo history của đơn vị con
                var noteChild = $"Đơn vị [{parentOrg.NAME}] yêu cầu cập nhật lại kế hoạch mẫu [{(string.IsNullOrWhiteSpace(templateCode) ? "Dữ liệu tổng hợp" : $"{template.CODE}-{template.NAME}")}]. Lý do: [{comment}]";
                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = childOrgCode,
                        TEMPLATE_CODE = templateCode,
                        VERSION = templateVersion ?? (findChild == null ? 0 : findChild.VERSION),
                        TIME_YEAR = timeYear,
                        ACTION = Approve_Action.CapTrenYeuCauDieuChinh,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser,
                        NOTES = noteChild
                    });
                UnitOfWork.Commit();

                NotifyUtilities.CreateNotifyYeuCauCapDuoiDieuChinh(
                   new NotifyPara()
                   {
                       Activity = Activity.AC_YEU_CAU_CAP_DUOI_DIEU_CHINH,
                       OrgCode = ProfileUtilities.User.ORGANIZE_CODE,
                       ChildOrgCode = childOrgCode,
                       TemplateCode = templateCode,
                       TimeYear = timeYear,
                       ModulType = ModulType.CostPL,
                       Note = noteChild,
                       UserSent = currentUser
                   });
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void ChuyenHDNS(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateChuyenHDNS(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.ThamDinh_DuLieu;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.ChuyenHDNS,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });
                // cập nhật dữ liệu kết thúc thẩm định sang chưa kết thúc nếu là chuyển thẳng lên HDNS
                if (ObjDetail.IS_REVIEWED)
                {
                    UnitOfWork.Repository<CostPLReviewRepo>().Update(x => x.DATA_VERSION == ObjDetail.VERSION
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && !x.IS_SUMMARY && x.ORG_CODE == ObjDetail.ORG_CODE, x => x.IS_END = false);
                }
                UnitOfWork.Commit();
                SendNotify(Activity.AC_CHUYEN_HDNS);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }

        public override void TrinhTGD(string code)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (!ValidateTrinhTGD(code))
                {
                    return;
                }

                ObjDetail.STATUS = Approve_Status.TGD_ChoPheDuyet;
                ObjDetail.UPDATE_BY = currentUser;

                UnitOfWork.BeginTransaction();

                CurrentRepository.Update(ObjDetail);

                UnitOfWork.Repository<CostPLHistoryRepo>()
                    .Create(new T_BP_COST_PL_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TrinhTGD,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                UnitOfWork.Commit();

                SendNotify(Activity.AC_TRINH_TGD);

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                Exception = ex;
                State = false;
            }
        }


        #endregion


        #region Xử lý dữ liệu tại mỗi đơn vị

        /// <summary>
        /// Danh sách dữ liệu của cấp dưới
        /// </summary>
        public override void GetListOfChild()
        {
            var orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            // Tìm tất cả đơn vị con
            var lstChildOrg = this.UnitOfWork.Repository<CostCenterRepo>().Queryable().Where(
                    x => x.PARENT_CODE == orgCode
                ).Select(x => x.CODE).ToList();
            // Tìm ra tất cả các mẫu đã nộp của đơn vị con

            this.ObjList = this.CurrentRepository.Queryable().Where(
                    x => lstChildOrg.Contains(x.ORG_CODE) && x.TIME_YEAR == this.ObjDetail.TIME_YEAR
                ).ToList();

            // Tìm mẫu nộp hộ
            var listTemplateCode = this.UnitOfWork.Repository<TemplateDetailCostPLRepo>().Queryable().Where(
                    x => lstChildOrg.Contains(x.CENTER_CODE) && x.TIME_YEAR == this.ObjDetail.TIME_YEAR
                    && x.TIME_YEAR == (ObjDetail.TIME_YEAR == 0 ? DateTime.Now.Year : ObjDetail.TIME_YEAR)
                ).Select(x => x.TEMPLATE_CODE).Distinct().ToList();
            var findCostPL = this.CurrentRepository.Queryable().Where(
                    x => listTemplateCode.Contains(x.TEMPLATE_CODE) && !lstChildOrg.Contains(x.ORG_CODE) && x.TIME_YEAR == this.ObjDetail.TIME_YEAR
                );

            this.ObjList.AddRange(findCostPL);
            this.ObjList = this.ObjList.OrderBy(x => x.ORG_CODE).ThenBy(x => x.TEMPLATE_CODE).ToList();
        }

        /// <summary>
        /// Lấy lịch sử tổng hợp của một đơn vị
        /// </summary>
        /// <param name="orgCode"></param>
        public override void GetSumUpHistory(string orgCode, int year = 0, int version = 0)
        {
            var query = UnitOfWork.GetSession().QueryOver<T_BP_COST_PL_SUM_UP_DETAIL>();
            query = query.Where(x => x.ORG_CODE == orgCode);

            if (year != 0)
            {
                query = query.Where(x => x.TIME_YEAR == year);
            }

            if (version != 0)
            {
                query = query.Where(x => x.SUM_UP_VERSION == version);
            }

            query = query.Fetch(x => x.CostCenter).Eager
                .Fetch(x => x.FromCostCenter).Eager
                .Fetch(x => x.Template).Eager;
            this.ObjListSumUpHistory = query.List().ToList();
        }

        public IList<T_MD_COST_PL_ELEMENT> GetDataCost(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements,
            out IList<T_BP_COST_PL_DATA> detailCostData,
            out bool isDrillDownApply,
            ViewDataCenterModel model)
        {
            isDrillDownApply = model.IS_DRILL_DOWN;
            if (!model.IS_HAS_NOT_VALUE && !model.IS_HAS_VALUE &&
                (!string.IsNullOrEmpty(model.TEMPLATE_CODE) || model.VERSION == null || model.VERSION.Value == -1))
            {
                detailCostData = null;
                detailCostElements = null;
                return null;
            }
            var isHasValue = model.IS_HAS_VALUE ? model.IS_HAS_NOT_VALUE ? (bool?)null : true : false;
            var isParent = !IsLeaf(model.ORG_CODE);
            if (!string.IsNullOrEmpty(model.TEMPLATE_CODE))
            {
                // view template Data
                detailCostData = null;
                var lstOrgs = new List<string>();
                if (isParent)
                {
                    lstOrgs.AddRange(GetListOfChildrenCenter(model.ORG_CODE).Select(x => x.CODE));
                }
                else
                {
                    lstOrgs.Add(model.ORG_CODE);
                }
                var elements = GetDataCostPreview(out detailCostElements, model.TEMPLATE_CODE, lstOrgs, model.YEAR, model.VERSION, isHasValue);
                var sumElements = new T_MD_COST_PL_ELEMENT
                {
                    // set tổng năm
                    // tạo element tổng
                    NAME = "TỔNG CỘNG",
                    LEVEL = 0,
                    PARENT_CODE = null,
                    IS_GROUP = true,
                    IsChildren = false,
                    C_ORDER = 0,
                    CODE = string.Empty,
                    Values = new decimal[14]
                };
                var isTemplateBase = GetTemplate(model.TEMPLATE_CODE)?.IS_BASE;
                isTemplateBase = isTemplateBase.HasValue && isTemplateBase.Value;
                foreach (var item in elements.Distinct().Where(x => !x.IS_GROUP))
                {
                    if (isTemplateBase.Value && item.Values.Sum() > 0)
                    {
                        item.IsChildren = true;
                    }
                    for (int i = 0; i < sumElements.Values.Length; i++)
                    {
                        sumElements.Values[i] += item.Values[i];
                    }
                }
                elements.Add(sumElements);
                return elements;
            }
            else if (model.VERSION == null || model.VERSION.Value == -1)
            {
                // xem dữ liệu trước khi tổng hợp
                detailCostElements = null;
                // disabled drill down
                isDrillDownApply = false;
                return SummarySumUpCenter(out detailCostData, model.YEAR, model.ORG_CODE, null, isHasValue, templateId: null);
            }
            else
            {
                // xem dữ liệu được tổng hợp cho đơn vị
                detailCostElements = null;
                return SummaryCenterVersion(out detailCostData, model.ORG_CODE, model.YEAR, model.VERSION, model.IS_DRILL_DOWN);
            }
        }

        /// <summary>
        /// Lấy danh sách file cơ sở
        /// </summary>
        /// <param name="year">Năm ngân sách</param>
        /// <param name="templateCode">Mã template</param>
        /// <param name="version">Version tổng hợp dữ liệu của đơn vị cha (trung tâm)</param>
        /// <param name="centerCode">Mã đơn vị con (ban)</param>
        /// <returns></returns>
        public override IList<T_CM_FILE_UPLOAD> GetFilesBase(int year, string templateCode, int version, string centerCode)
        {
            var parentCenterCode = GetCenter(centerCode).PARENT_CODE;
            var lstChildren = GetListOfChildrenCenter(parentCenterCode).Select(x => x.CODE);

            var lstDetails = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
            .GetManyByExpression(x => x.ORG_CODE == parentCenterCode &&
                x.TIME_YEAR == year &&
                x.SUM_UP_VERSION == version);
            var templateVersion = lstDetails.FirstOrDefault(y => y.TEMPLATE_CODE == templateCode)?.DATA_VERSION;
            if (templateVersion.HasValue)
            {
                // get file upload base
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyWithFetch(x => x.TIME_YEAR == year && x.TEMPLATE_CODE == templateCode && x.VERSION == templateVersion.Value, x => x.FileUpload)
                    .Select(x => x.FileUpload)
                    .ToList();
            }
            else
            {
                return new List<T_CM_FILE_UPLOAD>();
            }

        }

        public override IList<T_MD_COST_PL_ELEMENT> GetDetailPreviewSumUp(string centerCode, string elementCode, int year)
        {
            var plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>().GetPLDataByCenterCode(null, new List<string> { centerCode }, year, null, null);

            return plDataCostElements.Where(x => x.COST_PL_ELEMENT_CODE == elementCode)
                .Select(x => (T_MD_COST_PL_ELEMENT)x)
                .OrderBy(x => x.C_ORDER).ToList();
        }

        private IList<T_MD_COST_PL_ELEMENT> GetDataCostPreview(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements, string templateId, List<string> lstOrgs, int year, int? version, bool? isHasValue)
        {
            detailCostElements = new List<T_MD_TEMPLATE_DETAIL_COST_PL>();
            var lstElements = new List<T_MD_COST_PL_ELEMENT>();
            foreach (var orgCode in lstOrgs)
            {
                var elements = GetDataCostPreview(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detail, templateId, orgCode, year, version, isHasValue);
                detailCostElements = SummaryElement(detailCostElements, detail);
                lstElements = SummaryUpElement(lstElements, elements).ToList();
            }
            return lstElements;
        }

        /// <summary>
        /// Tổng hợp 2 list elements. Nếu có rồi thì cộng tổng values
        /// </summary>
        /// <param name="lst1"></param>
        /// <param name="lst2"></param>
        /// <returns></returns>
        private IList<T_MD_COST_PL_ELEMENT> SummaryUpElement(IList<T_MD_COST_PL_ELEMENT> lst1, IList<T_MD_COST_PL_ELEMENT> lst2)
        {
            if (lst1.Count == 0)
            {
                return lst2;
            }
            if (lst2.Count == 0)
            {
                return lst1;
            }

            var result = lst2.ToList();
            foreach (var item in lst1)
            {
                var index = result.FindIndex(x => x.CODE == item.CODE && x.CENTER_CODE == item.CENTER_CODE && x.TIME_YEAR == item.TIME_YEAR);
                if (index > 0)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        result[index].Values[i] += item.Values[i];
                    }
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public override T_BP_COST_PL_VERSION GetHeader(string templateCode, string centerCode, int year, int? version)
        {
            templateCode = templateCode ?? string.Empty;
            if (version == null)
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                .GetManyByExpression(x =>
                x.TIME_YEAR == year &&
                x.ORG_CODE == centerCode &&
                x.TEMPLATE_CODE == templateCode).OrderByDescending(x => x.VERSION).FirstOrDefault();
            }
            else
            {
                if (!string.IsNullOrEmpty(templateCode))
                {
                    return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetFirstByExpression(x =>
                    x.TIME_YEAR == year &&
                    x.VERSION == version &&
                    x.TEMPLATE_CODE == templateCode);
                }
                else
                {
                    return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetFirstByExpression(x =>
                    x.TIME_YEAR == year &&
                    x.VERSION == version &&
                    x.ORG_CODE == centerCode &&
                    x.TEMPLATE_CODE == templateCode);
                }
            }
        }


        public override void GetHistoryVersion(string orgCode, string templateId, int year)
        {
            ObjListVersion = UnitOfWork.Repository<CostPLVersionRepo>()
                .GetManyWithFetch(x => x.ORG_CODE == orgCode
                && x.TEMPLATE_CODE == templateId
                && x.TIME_YEAR == year, x => x.FileUpload).ToList();
        }

        #endregion

        /// <summary>
        /// Lấy tất cả lịch sử dữ liệu trong năm của một đơn vị
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="year"></param>
        public override void GetHistory(string orgCode, int year)
        {
            var query = this.UnitOfWork.GetSession().QueryOver<T_BP_COST_PL_HISTORY>();
            query = query.Where(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year)
                .Fetch(x => x.Template).Eager
                .Fetch(x => x.USER_CREATE).Eager;
            this.ObjListHistory = query.List().ToList();
        }

        public override void GetHistory()
        {
            this.Get(this.ObjDetail.PKID);
            GetHistory(this.ObjDetail.ORG_CODE, this.ObjDetail.TEMPLATE_CODE, this.ObjDetail.TIME_YEAR);
        }

        public override void GetHistory(string orgCode, string templateId, int? year)
        {
            if (year.HasValue)
            {
                this.ObjListHistory = UnitOfWork.Repository<CostPLHistoryRepo>().GetManyByExpression(
                    x => x.ORG_CODE == orgCode &&
                    x.TEMPLATE_CODE == templateId &&
                    x.TIME_YEAR == year.Value
                ).ToList();
            }
            else
            {
                this.ObjListHistory = UnitOfWork.Repository<CostPLHistoryRepo>().GetManyByExpression(
                   x => x.ORG_CODE == orgCode &&
                   x.TEMPLATE_CODE == templateId
               ).ToList();
            }
        }

        #region Import data
        /// <summary>
        /// Kiểm tra có đúng mẫu hay không
        /// Dữ liệu nộp có đúng là số hay không
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="isDataBase">Có phải dữ liệu cơ sở hay không</param>
        public override void ValidateData(DataTable dataTable, bool isDataBase)
        {
            var periodTime = this.UnitOfWork.Repository<PeriodTimeRepo>().Get(this.ObjDetail.TIME_YEAR);
            if (periodTime == null || periodTime.IS_CLOSE)
            {
                this.State = false;
                this.ErrorMessage = $"Năm kế hoạch {this.ObjDetail.TIME_YEAR} đã bị đóng!";
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ObjDetail.TEMPLATE_CODE))
            {
                this.State = false;
                this.ErrorMessage = $"Bạn chưa chọn mẫu khai báo!";
                return;
            }

            var template = this.UnitOfWork.Repository<TemplateRepo>().Get(this.ObjDetail.TEMPLATE_CODE);

            // Kiểm tra template có phải của đơn vị không
            if (template == null || template.ORG_CODE != ProfileUtilities.User.ORGANIZE_CODE)
            {
                this.State = false;
                this.ErrorMessage = "Mẫu khai báo này không phải của đơn vị bạn!";
                return;
            }

            var lstDetailTemplate = this.UnitOfWork.Repository<TemplateDetailCostPLRepo>().GetManyByExpression(x => x.TIME_YEAR == this.ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == this.ObjDetail.TEMPLATE_CODE);

            var lstElement = this.UnitOfWork.Repository<CostPLElementRepo>().GetManyByExpression(x => x.TIME_YEAR == this.ObjDetail.TIME_YEAR);

            // Kiểm tra mẫu này đã được thiết kế cho năm ngân sách đang chọn chưa
            if (lstDetailTemplate.Count == 0)
            {
                this.State = false;
                this.ErrorMessage = $"Mẫu khai báo [{this.ObjDetail.TEMPLATE_CODE}] năm [{this.ObjDetail.TIME_YEAR}] chưa định nghĩa các khoản mục!";
                return;
            }

            // Kiểm tra cost_center có thuộc mẫu thiết kế không
            // Có để trống dữ liệu tại 2 cột Mã đơn vị, và mã khoản mục không
            var lstOrgInTemplate = lstDetailTemplate.Select(x => x.CENTER_CODE).Distinct().ToList();
            for (int i = this.StartRowData; i < dataTable.Rows.Count; i++)
            {
                var orgCodeInExcel = dataTable.Rows[i][0].ToString().Trim();
                var elementCodeInExcel = dataTable.Rows[i][2].ToString().Trim();
                if (string.IsNullOrWhiteSpace(orgCodeInExcel))
                {
                    this.State = false;
                    this.ErrorMessage = $"Không được phép để trống dữ liệu tại cột [Mã đơn vị] tại dòng [{i + 1}] !";
                    return;
                }

                if (string.IsNullOrWhiteSpace(elementCodeInExcel))
                {
                    this.State = false;
                    this.ErrorMessage = $"Không được phép để trống dữ liệu tại cột [Mã khoản mục] tại dòng [{i + 1}] !";
                    return;
                }

                if (!lstOrgInTemplate.Contains(orgCodeInExcel))
                {
                    this.State = false;
                    this.ErrorMessage = $"Mẫu khai báo [{template.CODE}] không chứa đơn vị [{orgCodeInExcel}] tại dòng [{i + 1}] !";
                    return;
                }

                // Kiểm tra các khoản mục lá không nằm trong mẫu thiết kế
                var findElement = lstElement.FirstOrDefault(x => x.CODE == elementCodeInExcel);
                if (findElement == null)
                {
                    this.State = false;
                    this.ErrorMessage = $"Mã khoản mục [{elementCodeInExcel}] không tồn tại !";
                    return;
                }

                if (!findElement.IS_GROUP && lstDetailTemplate.Count(x => x.ELEMENT_CODE == elementCodeInExcel && x.CENTER_CODE == orgCodeInExcel) == 0)
                {
                    this.State = false;
                    this.ErrorMessage = $"Mã khoản mục [{elementCodeInExcel}] tại dòng {i + 1} không nằm trong mẫu thiết kế !";
                    return;
                }
            }

            // Kiểm tra file excel có dữ liệu từ dòng thứ StartRowData hay không
            if (dataTable == null || dataTable.Rows.Count < this.StartRowData)
            {
                this.State = false;
                this.ErrorMessage = "File excel này không có dữ liệu!";
                return;
            }

            //Kiếm tra có đúng mẫu hay không
            if (isDataBase)
            {
                for (int i = 0; i < ListColumnNameDataBase.Count; i++)
                {
                    if (dataTable.Rows[this.StartRowData - 1][i].ToString().ToUpper() != this.ListColumnNameDataBase[i])
                    {
                        this.State = false;
                        this.ErrorMessage = "File excel không đúng theo mẫu thiết kế!";
                        return;
                    }
                }
                this.ConvertData(dataTable, lstElement.ToList(), startColumn: 6, endColumn: ListColumnNameDataBase.Count - 5, isDataBase);

            }
            else
            {
                for (int i = 0; i < ListColumnName.Count; i++)
                {
                    if (dataTable.Rows[this.StartRowData - 1][i].ToString().ToUpper() != this.ListColumnName[i])
                    {
                        this.State = false;
                        this.ErrorMessage = "File excel không đúng theo mẫu thiết kế!";
                        return;
                    }
                }

                // kiểm tra xem có 2 hàng dữ liệu của cùng 1 khoản mục hay không
                var dictionary = lstDetailTemplate.ToDictionary(x => x.PKID, x => true);
                for (int i = StartRowData; i < dataTable.Rows.Count; i++)
                {
                    var foundItem = lstDetailTemplate
                        .FirstOrDefault(x => x.CENTER_CODE == dataTable.Rows[i][0].ToString().Trim() &&
                        x.ELEMENT_CODE == dataTable.Rows[i][2].ToString().Trim());
                    if (foundItem != null && dictionary[foundItem.PKID])
                    {
                        dictionary[foundItem.PKID] = false;
                    }
                    else if (foundItem != null)
                    {
                        this.State = false;
                        this.ErrorMessage = $"Mã khoản mục [{foundItem.ELEMENT_CODE}] tại dòng {i + 1} bị lặp lại!";
                        return;
                    }
                }

                // Kiểm tra dữ liệu có phải là số hay không
                this.ConvertData(dataTable, lstElement.ToList(), startColumn: 4, endColumn: ListColumnName.Count - 3, isDataBase);
            }


            if (this.InvalidCellsList.Count > 0)
            {
                this.State = false;
                this.ErrorMessage = "Dữ liệu tại các cell sau không phải là số!<br>";
                foreach (var item in this.InvalidCellsList)
                {
                    this.ErrorMessage += $"Dòng [{item.X + 1}] - Cột [{this.ListColumnName[item.Y]}] <br>";
                }
                return;
            }

            if (dataTable.Rows.Count == this.StartRowData)
            {
                this.State = false;
                this.ErrorMessage = "File excel này không có dữ liệu!";
                return;
            }
        }

        /// <summary>
        /// Convert data về số
        /// </summary>
        /// <param name="dataTable"></param>
        public override void ConvertData(DataTable dataTable, List<T_MD_COST_PL_ELEMENT> lstElement, int startColumn, int endColumn, bool isDataBase)
        {
            var iActualRow = dataTable.Rows.Count;
            for (int i = this.StartRowData; i < iActualRow; i++)
            {
                for (int j = startColumn; j < endColumn; j++)
                {
                    var strValue = (dataTable.Rows[i][j] ?? "0").ToString() ?? "0";
                    strValue = string.IsNullOrWhiteSpace(strValue) ? "0" : strValue.Trim();
                    var canParse = decimal.TryParse(strValue, out decimal result);
                    if (canParse)
                    {
                        dataTable.Rows[i][j] = result;
                    }
                    else
                    {
                        this.InvalidCellsList.Add(new Point(i, j));
                    }
                }
            }

            if (this.InvalidCellsList.Count > 0)
            {
                return;
            }

            // Xóa những dữ liệu trắng, và dữ liệu mã cha
            var lstDataRowInValid = new List<DataRow>();
            for (int i = this.StartRowData; i < iActualRow; i++)
            {
                var yearAmount = 0m;
                bool isValid = true;
                bool isEmptyValues = true;
                var elementCodeInExcel = dataTable.Rows[i][2].ToString().Trim();
                var findElement = lstElement.FirstOrDefault(x => x.CODE == elementCodeInExcel);
                if (findElement.IS_GROUP)
                {
                    isValid = false;
                }
                else
                {
                    for (int j = startColumn; j < endColumn; j++)
                    {
                        var cellValue = dataTable.Rows[i][j].ToString().Trim();
                        if (!string.IsNullOrEmpty(cellValue) && cellValue != "0")
                        {
                            isEmptyValues = false;
                        }

                        if (!string.IsNullOrEmpty(cellValue) &&
                            decimal.TryParse(cellValue, out decimal val))
                        {
                            if ((isDataBase && (j - startColumn) % 3 == 2 && j <= endColumn)
                                || !isDataBase)
                            {
                                yearAmount += val;
                            }
                            if (val != 0 && !isValid)
                            {
                                isValid = true;
                            }
                        }
                    }
                }
                if (!isValid || isEmptyValues || yearAmount == 0)
                {
                    lstDataRowInValid.Add(dataTable.Rows[i]);
                }
            }

            foreach (var item in lstDataRowInValid)
            {
                dataTable.Rows.Remove(item);
            }
        }

        /// <summary>
        /// Nhập dữ liệu từ excel vào database
        /// </summary>
        /// <param name="request"></param>
        public override void ImportExcel(HttpRequestBase request)
        {
            base.ImportExcel(request);
            if (!State)
            {
                return;
            }
            this.StartRowData = 6;

            var orgCode = ProfileUtilities.User.ORGANIZE_CODE;

            // Lưu file vào database
            var fileStream = new FILE_STREAM()
            {
                PKID = Guid.NewGuid().ToString(),
                FILESTREAM = request.Files[0]
            };
            FileStreamService.InsertFile(fileStream);

            // Xác định version dữ liệu
            var costPLCurrent = CurrentRepository.Queryable().FirstOrDefault(x => x.ORG_CODE == orgCode
                && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE);

            if (costPLCurrent != null && !(costPLCurrent.STATUS == Approve_Status.TuChoi || costPLCurrent.STATUS == Approve_Status.ChuaTrinhDuyet))
            {
                this.State = false;
                this.ErrorMessage = "Mẫu khai báo này không ở trạng thái Từ chối hoặc Chưa trình duyệt!";
                return;
            }

            var versionNext = 1;
            if (costPLCurrent != null)
            {
                versionNext = costPLCurrent.VERSION + 1;
            }

            // Lấy dữ liệu của version hiện tại
            var dataCurrent = new List<T_BP_COST_PL_DATA>();
            if (costPLCurrent != null)
            {
                dataCurrent = UnitOfWork.Repository<CostPLDataRepo>().Queryable().Where(x => x.ORG_CODE == orgCode
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE).ToList();
            }

            //Insert dữ liệu
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                DataTable tableData = new DataTable();
                tableData = ExcelDataExchange.ReadData(fileStream.FULL_PATH);

                this.ValidateData(tableData, isDataBase: false);
                if (!this.State)
                {
                    return;
                }

                int actualRows = tableData.Rows.Count;
                UnitOfWork.BeginTransaction();

                // Cập nhật version
                if (costPLCurrent != null)
                {
                    // Cập nhật next version vào bảng chính
                    costPLCurrent.VERSION = versionNext;
                    costPLCurrent.IS_DELETED = false;
                    CurrentRepository.Update(costPLCurrent);
                }
                else
                {
                    // Tạo mới bản ghi cost pl
                    CurrentRepository.Create(new T_BP_COST_PL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        IS_DELETED = false,
                        VERSION = versionNext,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        FILE_ID = fileStream.PKID,
                        IS_SUMUP = false,
                        CREATE_BY = currentUser
                    });
                }

                // Đưa next version vào bảng log
                UnitOfWork.Repository<CostPLVersionRepo>().Create(new T_BP_COST_PL_VERSION()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    FILE_ID = fileStream.PKID,
                    CREATE_BY = currentUser
                });

                // Tạo mới bản ghi log trạng thái
                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.NhapDuLieu,
                    ACTION_DATE = DateTime.Now,
                    ACTION_USER = currentUser,
                    CREATE_BY = currentUser
                });


                // Insert data vào history
                foreach (var item in dataCurrent)
                {
                    var costDataHis = (T_BP_COST_PL_DATA_HISTORY)item;
                    UnitOfWork.Repository<CostPLDataHistoryRepo>().Create(costDataHis);
                    UnitOfWork.Repository<CostPLDataRepo>().Delete(item);
                }

                // Insert dữ liệu vào bảng data
                for (int i = this.StartRowData; i < actualRows; i++)
                {
                    var centerCode = tableData.Rows[i][0].ToString().Trim();
                    var percentagePreventive = GetPreventive(centerCode, year: ObjDetail.TIME_YEAR)?.PERCENTAGE;
                    if (percentagePreventive == null)
                    {
                        percentagePreventive = 1;
                    }
                    else
                    {
                        percentagePreventive = 1 + percentagePreventive / 100;
                    }
                    int j = 4;
                    var costData = new T_BP_COST_PL_DATA()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        COST_CENTER_CODE = centerCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        VERSION = versionNext,
                        COST_PL_ELEMENT_CODE = tableData.Rows[i][2].ToString().Trim(),
                        VALUE_JAN = tableData.Rows[i][j++] as decimal?,
                        VALUE_FEB = tableData.Rows[i][j++] as decimal?,
                        VALUE_MAR = tableData.Rows[i][j++] as decimal?,
                        VALUE_APR = tableData.Rows[i][j++] as decimal?,
                        VALUE_MAY = tableData.Rows[i][j++] as decimal?,
                        VALUE_JUN = tableData.Rows[i][j++] as decimal?,
                        VALUE_JUL = tableData.Rows[i][j++] as decimal?,
                        VALUE_AUG = tableData.Rows[i][j++] as decimal?,
                        VALUE_SEP = tableData.Rows[i][j++] as decimal?,
                        VALUE_OCT = tableData.Rows[i][j++] as decimal?,
                        VALUE_NOV = tableData.Rows[i][j++] as decimal?,
                        VALUE_DEC = tableData.Rows[i][j++] as decimal?,

                        DESCRIPTION = tableData.Rows[i][18].ToString(),
                        CREATE_BY = currentUser
                    };
                    costData.VALUE_SUM_YEAR = costData.VALUE_JAN + costData.VALUE_FEB + costData.VALUE_MAR + costData.VALUE_APR + costData.VALUE_MAY + costData.VALUE_JUN + costData.VALUE_JUL + costData.VALUE_AUG + costData.VALUE_SEP + costData.VALUE_OCT + costData.VALUE_NOV + costData.VALUE_DEC;

                    costData.VALUE_SUM_YEAR_PREVENTIVE = costData.VALUE_SUM_YEAR * percentagePreventive;

                    UnitOfWork.Repository<CostPLDataRepo>().Create(costData);
                }
                UnitOfWork.Commit();

                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_NHAP_DU_LIEU,
                        OrgCode = orgCode,
                        TemplateCode = ObjDetail.TEMPLATE_CODE,
                        TimeYear = ObjDetail.TIME_YEAR,
                        ModulType = ModulType.CostPL,
                        UserSent = currentUser
                    });
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        /// <summary>
        /// Lấy entity khai báo ngân sách dự phòng
        /// </summary>
        /// <param name="centerCode">Mã phòng ban</param>
        /// <param name="year">Năm ngân sách</param>
        /// <returns></returns>
        private T_MD_PREVENTIVE GetPreventive(string centerCode, int year)
        {
            return UnitOfWork.Repository<PreventiveRepo>()
                .GetFirstWithFetch(x => x.ORG_CODE == centerCode && x.TIME_YEAR == year);
        }

        /// <summary>
        /// Nhập dữ liệu từ CƠ SỞ excel vào database
        /// </summary>
        /// <param name="request"></param>
        public override void ImportExcelBase(HttpRequestBase request)
        {
            base.ImportExcelBase(request);
            if (!State)
            {
                return;
            }
            this.StartRowData = 6;
            var orgCode = ProfileUtilities.User?.ORGANIZE_CODE;
            var costPLDataRepo = UnitOfWork.Repository<CostPLDataRepo>();
            var costPLDataBaseRepo = UnitOfWork.Repository<CostPLDataBaseRepo>();
            var currentUser = ProfileUtilities.User?.USER_NAME;

            // Lưu file vào database
            var fileStream = new FILE_STREAM()
            {
                PKID = Guid.NewGuid().ToString(),
                FILESTREAM = request.Files[0]
            };
            FileStreamService.InsertFile(fileStream);

            // Xác định version dữ liệu
            var costPLCurrent = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE);

            if (costPLCurrent != null && !(costPLCurrent.STATUS == Approve_Status.TuChoi || costPLCurrent.STATUS == Approve_Status.ChuaTrinhDuyet))
            {
                this.State = false;
                this.ErrorMessage = "Mẫu khai báo này không ở trạng thái Từ chối hoặc Chưa trình duyệt!";
                return;
            }

            var versionNext = 1;
            if (costPLCurrent != null)
            {
                versionNext = costPLCurrent.VERSION + 1;
            }

            // Lấy dữ liệu của version hiện tại
            var dataCurrent = new List<T_BP_COST_PL_DATA>();
            var dataCurrentBase = new List<T_BP_COST_PL_DATA_BASE>();
            if (costPLCurrent != null)
            {
                dataCurrent = costPLDataRepo.GetManyWithFetch(x => x.ORG_CODE == orgCode
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE).ToList();

                dataCurrentBase = costPLDataBaseRepo.GetManyWithFetch(x => x.ORG_CODE == orgCode
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE).ToList();
            }

            //Insert dữ liệu
            try
            {
                DataTable tableData = new DataTable();
                tableData = ExcelDataExchange.ReadData(fileStream.FULL_PATH);

                this.ValidateData(tableData, isDataBase: true);
                if (!this.State)
                {
                    return;
                }

                int actualRows = tableData.Rows.Count;
                UnitOfWork.BeginTransaction();
                // Cập nhật version
                if (costPLCurrent != null)
                {
                    // Cập nhật next version vào bảng chính
                    costPLCurrent.VERSION = versionNext;
                    costPLCurrent.IS_DELETED = false;
                    CurrentRepository.Update(costPLCurrent);
                }
                else
                {
                    // Tạo mới bản ghi cost pl
                    CurrentRepository.Create(new T_BP_COST_PL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        IS_DELETED = false,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        VERSION = versionNext,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        FILE_ID = fileStream.PKID,
                        IS_SUMUP = false,
                        CREATE_BY = currentUser
                    });
                }

                // Đưa next version vào bảng log
                UnitOfWork.Repository<CostPLVersionRepo>().Create(new T_BP_COST_PL_VERSION()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    FILE_ID = fileStream.PKID,
                    CREATE_BY = currentUser
                });

                // Tạo mới bản ghi log trạng thái
                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.NhapDuLieu,
                    ACTION_DATE = DateTime.Now,
                    ACTION_USER = currentUser,
                    CREATE_BY = currentUser
                });


                // Insert data vào history
                foreach (var item in dataCurrent)
                {
                    var costDataHis = (T_BP_COST_PL_DATA_HISTORY)item;
                    UnitOfWork.Repository<CostPLDataHistoryRepo>().Create(costDataHis);
                    costPLDataRepo.Delete(item);
                }

                // Insert dữ liệu vào bảng data
                var lstRowValues = new List<DataRow>();
                for (int i = StartRowData; i < actualRows; i++)
                {
                    lstRowValues.Add(tableData.Rows[i]);
                }
                var lookUp = lstRowValues.OfType<DataRow>().ToLookup(x =>
                new
                {
                    CenterCode = x[0].ToString(),
                    ElementCode = x[2].ToString()
                });

                var lstCenterCodeInExcelFile = lookUp.Select(y => y.Key.CenterCode).Distinct();
                var lstPreventives = UnitOfWork.Repository<PreventiveRepo>()
                    .GetManyWithFetch(x => lstCenterCodeInExcelFile.Contains(x.ORG_CODE) && x.TIME_YEAR == ObjDetail.TIME_YEAR);

                foreach (var key in lookUp.Select(x => x.Key))
                {
                    var percentagePreventive = lstPreventives.FirstOrDefault(x => x.ORG_CODE == key.CenterCode)?.PERCENTAGE;
                    if (percentagePreventive == null)
                    {
                        percentagePreventive = 1;
                    }
                    else
                    {
                        percentagePreventive = 1 + percentagePreventive / 100;
                    }
                    var costData = new T_BP_COST_PL_DATA()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        COST_CENTER_CODE = key.CenterCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        VERSION = versionNext,
                        COST_PL_ELEMENT_CODE = key.ElementCode,
                        VALUE_JAN = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[8].ToString())),
                        VALUE_FEB = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[11].ToString())),
                        VALUE_MAR = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[14].ToString())),
                        VALUE_APR = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[17].ToString())),
                        VALUE_MAY = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[20].ToString())),
                        VALUE_JUN = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[23].ToString())),
                        VALUE_JUL = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[26].ToString())),
                        VALUE_AUG = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[29].ToString())),
                        VALUE_SEP = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[32].ToString())),
                        VALUE_OCT = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[35].ToString())),
                        VALUE_NOV = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[38].ToString())),
                        VALUE_DEC = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[41].ToString())),
                        CREATE_BY = currentUser
                    };

                    costData.VALUE_SUM_YEAR = costData.VALUE_JAN + costData.VALUE_FEB + costData.VALUE_MAR + costData.VALUE_APR + costData.VALUE_MAY + costData.VALUE_JUN + costData.VALUE_JUL + costData.VALUE_AUG + costData.VALUE_SEP + costData.VALUE_OCT + costData.VALUE_NOV + costData.VALUE_DEC;

                    costData.VALUE_SUM_YEAR_PREVENTIVE = costData.VALUE_SUM_YEAR * percentagePreventive;
                    costPLDataRepo.Create(costData);
                }

                // Insert data vào base data history
                var lstBaseDataHistory = (from x in dataCurrentBase
                                          select (T_BP_COST_PL_DATA_BASE_HISTORY)x).ToList();
                UnitOfWork.Repository<CostPLDataBaseHistoryRepo>().Create(lstObj: lstBaseDataHistory);
                costPLDataBaseRepo.Delete(dataCurrentBase);

                // Insert dữ liệu vào bảng data
                for (int i = this.StartRowData; i < actualRows; i++)
                {
                    var centerCode = tableData.Rows[i][0].ToString().Trim();
                    var percentagePreventive = lstPreventives.FirstOrDefault(x => x.ORG_CODE == centerCode)?.PERCENTAGE;
                    if (percentagePreventive == null)
                    {
                        percentagePreventive = 1;
                    }
                    else
                    {
                        percentagePreventive = 1 + percentagePreventive / 100;
                    }
                    var costData = new T_BP_COST_PL_DATA_BASE()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        COST_CENTER_CODE = centerCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        VERSION = versionNext,
                        COST_PL_ELEMENT_CODE = tableData.Rows[i][2].ToString().Trim(),
                        MATERIAL = tableData.Rows[i][4].ToString().Trim(),
                        UNIT = tableData.Rows[i][5].ToString().Trim(),

                        QUANTITY_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][6].ToString().Trim()),
                        PRICE_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][7].ToString().Trim()),
                        AMOUNT_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][8].ToString().Trim()),

                        QUANTITY_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][9].ToString().Trim()),
                        PRICE_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][10].ToString().Trim()),
                        AMOUNT_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][11].ToString().Trim()),

                        QUANTITY_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][12].ToString().Trim()),
                        PRICE_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][13].ToString().Trim()),
                        AMOUNT_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][14].ToString().Trim()),

                        QUANTITY_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][15].ToString().Trim()),
                        PRICE_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][16].ToString().Trim()),
                        AMOUNT_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][17].ToString().Trim()),

                        QUANTITY_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][18].ToString().Trim()),
                        PRICE_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][19].ToString().Trim()),
                        AMOUNT_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][20].ToString().Trim()),

                        QUANTITY_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][21].ToString().Trim()),
                        PRICE_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][22].ToString().Trim()),
                        AMOUNT_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][23].ToString().Trim()),

                        QUANTITY_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][24].ToString().Trim()),
                        PRICE_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][25].ToString().Trim()),
                        AMOUNT_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][26].ToString().Trim()),

                        QUANTITY_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][27].ToString().Trim()),
                        PRICE_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][28].ToString().Trim()),
                        AMOUNT_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][29].ToString().Trim()),

                        QUANTITY_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][30].ToString().Trim()),
                        PRICE_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][31].ToString().Trim()),
                        AMOUNT_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][32].ToString().Trim()),

                        QUANTITY_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][33].ToString().Trim()),
                        PRICE_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][34].ToString().Trim()),
                        AMOUNT_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][35].ToString().Trim()),

                        QUANTITY_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][36].ToString().Trim()),
                        PRICE_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][37].ToString().Trim()),
                        AMOUNT_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][38].ToString().Trim()),

                        QUANTITY_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][39].ToString().Trim()),
                        PRICE_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][40].ToString().Trim()),
                        AMOUNT_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][41].ToString().Trim()),

                        DESCRIPTION = tableData.Rows[i][ListColumnNameDataBase.Count - 1].ToString().Trim(),

                        CREATE_BY = currentUser
                    };

                    costData.AMOUNT_YEAR = costData.AMOUNT_M1 + costData.AMOUNT_M2 + costData.AMOUNT_M3 + costData.AMOUNT_M4 + costData.AMOUNT_M5 + costData.AMOUNT_M6 + costData.AMOUNT_M7 + costData.AMOUNT_M8 + costData.AMOUNT_M9 + costData.AMOUNT_M10 + costData.AMOUNT_M11 + costData.AMOUNT_M12;
                    costData.AMOUNT_YEAR_PREVENTIVE = costData.AMOUNT_YEAR * percentagePreventive.Value;

                    costPLDataBaseRepo.Create(costData);
                }
                UnitOfWork.Commit();

                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_NHAP_DU_LIEU,
                        OrgCode = orgCode,
                        TemplateCode = ObjDetail.TEMPLATE_CODE,
                        TimeYear = ObjDetail.TIME_YEAR,
                        ModulType = ModulType.CostPL,
                        UserSent = currentUser
                    });
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }
        #endregion

        public override IList<T_BP_COST_PL_VERSION> GetVersions(string orgCode, string templateId, int year)
        {
            templateId = templateId ?? string.Empty;
            var lstVersions = GetVersionsNumber(orgCode, templateId, year);
            return UnitOfWork.Repository<CostPLVersionRepo>()
                .GetManyByExpression(x => x.TEMPLATE_CODE == templateId
                && x.TIME_YEAR == year
                && lstVersions.Contains(x.VERSION));
        }

        #region Get template code
        public override IList<Data> GetTemplates(string orgCode, int? year = null)
        {
            ObjDetail.ORG_CODE = orgCode;
            var lstOrgCodes = new List<string>() { orgCode };
            var lstTemplateCurrentUserSelfUpload = new List<string>();

            var isLeaf = IsLeaf();
            if (isLeaf)
            {
                // is leaf
                lstOrgCodes.Add(orgCode);
                lstTemplateCurrentUserSelfUpload.AddRange(GetTemplateCurrentUserSelfUpload(orgCode));
            }
            else
            {
                // is group
                // get all child
                lstOrgCodes.AddRange(GetListOfChildrenCenter(orgCode).Select(x => x.CODE));
            }

            var templates = lstTemplateCurrentUserSelfUpload;
            if (AuthorizeUtilities.CheckUserRight("R323") || !isLeaf)
            {
                templates = GetTemplateData(lstOrgCodes, year)
                    .Union(GetTemplateDataHistory(lstOrgCodes, year))
                    .Union(lstTemplateCurrentUserSelfUpload)
                    .ToList();
            }
            if (templates == null || templates.Count == 0)
            {
                return null;
            }
            return UnitOfWork.Repository<TemplateRepo>()
                .GetManyWithFetch(x => templates.Contains(x.CODE) && x.CODE != "")
                .OrderByDescending(x => x.CODE)
                .Select(x => new Data
                {
                    Value = x.CODE,
                    Text = $"{x.CODE} - {x.NAME}" + (x.ACTIVE ? "" : $" - {Global.DeactiveTemplate}")
                })
                .ToList();
        }

        private IList<string> GetTemplateCurrentUserSelfUpload(string orgCode)
        {
            return UnitOfWork.Repository<CostPLVersionRepo>()
                .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TEMPLATE_CODE != string.Empty)
                .Select(x => x.TEMPLATE_CODE)
                .Distinct()
                .ToList();
        }

        private IList<string> GetTemplateData(List<string> lstOrgCodes, int? year = null)
        {
            return UnitOfWork.Repository<CostPLDataRepo>()
                .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE) && !lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE != string.Empty && (!year.HasValue || x.TIME_YEAR == year.Value))
                .Select(x => x.TEMPLATE_CODE)
                .Distinct()
                .ToList();
        }

        private IList<string> GetTemplateDataHistory(List<string> lstOrgCodes, int? year = null)
        {
            return UnitOfWork.Repository<CostPLDataHistoryRepo>()
                .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE) && !lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE != string.Empty && (!year.HasValue || x.TIME_YEAR == year.Value))
                .Select(x => x.TEMPLATE_CODE)
                .Distinct()
                .ToList();
        }
        #endregion

        #region Get template year
        /// <summary>
        /// lấy tất cả các năm trong các mẫu đã từng được nộp (nộp hộ) của đơn vị hoặc con
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public override IList<int> GetYears(string orgCode, string templateId)
        {
            ObjDetail.ORG_CODE = orgCode;
            var lstOrgCodes = new List<string>() { orgCode };
            var templateCurrentUserUpload = new List<int>();
            templateId = templateId ?? string.Empty;
            if (IsLeaf())
            {
                // is leaf
                lstOrgCodes.Add(orgCode);
                // lấy năm của những mẫu tự nộp của đơn vị
            }
            else
            {
                // is group
                // get all child
                lstOrgCodes.AddRange(GetListOfChildrenCenter(orgCode).Select(x => x.CODE));
            }
            templateCurrentUserUpload.AddRange(GetYearCurrentUserUpload(lstOrgCodes, templateId));

            return GetDataYears(lstOrgCodes, templateId)
                .Union(GetDataHistoryYear(lstOrgCodes, templateId))
                .Union(templateCurrentUserUpload)
                .OrderByDescending(x => x)
                .ToList();
        }

        private IList<int> GetYearCurrentUserUpload(IList<string> lstOrgCodes, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE == templateId)
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
        }

        private IList<int> GetDataHistoryYear(List<string> lstOrgCodes, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<CostPLDataHistoryRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<CostPLDataHistoryRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE) && x.TEMPLATE_CODE == templateId)
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
        }

        private IList<int> GetDataYears(List<string> lstOrgCodes, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<CostPLDataRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<CostPLDataRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.COST_CENTER_CODE) && x.TEMPLATE_CODE == templateId)
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
        }
        #endregion

        #region Get template versions
        public override IList<int> GetVersionsNumber(string orgCode, string templateId, int year)
        {
            if (!string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == year)
                    .Select(x => x.VERSION)
                    .OrderByDescending(x => x)
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == year && x.ORG_CODE == orgCode)
                    .Select(x => x.VERSION)
                    .OrderByDescending(x => x)
                    .ToList();
            }
        }

        public override IList<int> GetTemplateVersion(string templateId, string centerCode, int year)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode && x.TIME_YEAR == year)
                    .Select(x => x.DATA_VERSION).Distinct().OrderByDescending(x => x).ToList();
            }
            else
            {
                return UnitOfWork.Repository<CostPLVersionRepo>()
                    .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode && x.TIME_YEAR == year)
                    .Select(x => x.VERSION).Distinct().OrderByDescending(x => x).ToList();
            }
        }

        #endregion

        #region Export excel from data center view
        public override void GenerateExportExcel(ref MemoryStream outFileStream, string html, string path, int year, string centerCode, int? version, string templateId, string unit, decimal exchangeRate)
        {
            // Create a new workbook and a sheet named "User Accounts"
            //Mở file Template
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            IWorkbook workbook;
            workbook = new XSSFWorkbook(fs);
            workbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.CostPL));
            fs.Close();
            ISheet sheet = workbook.GetSheetAt(0);
            var metaData = ExcelHelper.GetExcelMeta(html);
            var NUM_CELL = string.IsNullOrEmpty(templateId) ? 18 : 20;

            InitHeaderFile(ref sheet, year, centerCode, version, NUM_CELL, templateId, unit, exchangeRate);
            ExcelHelperBP.InsertHeaderTable(ref workbook, ref sheet, metaData.MetaTHead, NUM_CELL, ignoreFirstColumn: string.IsNullOrEmpty(templateId) || (!string.IsNullOrEmpty(templateId) && GetTemplate(templateId).IS_BASE));
            ExcelHelperBP.InsertBodyTable(ref workbook,
                                          ref sheet,
                                          metaData.MetaTBody,
                                          NUM_CELL,
                                          ignoreFirstColumn: string.IsNullOrEmpty(templateId) || (!string.IsNullOrEmpty(templateId) && GetTemplate(templateId).IS_BASE));

            // Save the Excel spreadsheet to a MemoryStream and return it to the client
            workbook.Write(outFileStream);
        }

        private void InitHeaderFile(ref ISheet sheet, int year, string centerCode, int? version, int NUM_CELL, string templateId, string unit, decimal exchangeRate)
        {
            var name = "DỮ LIỆU KẾ HOẠCH CHI PHÍ KINH DOANH";
            var centerName = GetCenter(centerCode).NAME.ToUpper();
            var template = GetTemplate(templateId);
            var templateName = template != null ? $"Mẫu khai báo: {template.CODE} - {template.NAME}" : "Tổng hợp dữ liệu";

            ExcelHelperBP.InitHeaderFile(ref sheet, year, centerName, version, NUM_CELL, templateName, unit, name, exchangeRate);
        }

        #endregion

        #region generate excel template
        /// <summary>
        /// generate excel file and store in path
        /// </summary>
        /// <param name="outFileStream"></param>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="year"></param>
        public override void GenerateTemplate(ref MemoryStream outFileStream, string path, string templateId, int year)
        {
            var dataCost = PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements, templateId, year, ignoreAuth: true);

            if (dataCost.Count == 0 || detailCostElements.Count == 0)
            {
                State = false;
                ErrorMessage = "Không tìm thấy dữ liệu";
                return;
            }

            try
            {
                //Mở file Template
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                templateWorkbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.CostPL));
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);

                //Số hàng và số cột hiện tại
                int numRowCur = 0;
                int NUM_CELL = 19;

                //Style cần dùng
                ICellStyle styleCellHeader = templateWorkbook.CreateCellStyle();
                styleCellHeader.CloneStyleFrom(sheet.GetRow(5).Cells[0].CellStyle);
                styleCellHeader.WrapText = true;

                ICellStyle styleCellDetail = templateWorkbook.CreateCellStyle();
                styleCellDetail.CloneStyleFrom(sheet.GetRow(6).Cells[0].CellStyle);
                styleCellDetail.WrapText = true;

                ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
                styleCellNumber.CloneStyleFrom(styleCellDetail);
                styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0");

                ICellStyle styleCellNumberColor = templateWorkbook.CreateCellStyle();
                styleCellNumberColor.CloneStyleFrom(styleCellNumber);
                styleCellNumberColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                styleCellNumberColor.FillPattern = FillPattern.SolidForeground;

                ICellStyle styleCellBold = templateWorkbook.CreateCellStyle();
                styleCellBold.CloneStyleFrom(sheet.GetRow(6).Cells[0].CellStyle);
                styleCellBold.WrapText = true;
                var fontBold = templateWorkbook.CreateFont();
                fontBold.Boldweight = (short)FontBoldWeight.Bold;
                fontBold.FontHeightInPoints = 11;
                fontBold.FontName = "Times New Roman";

                #region Header
                var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
                var rowHeader1 = ReportUtilities.CreateRow(ref sheet, 0, NUM_CELL);
                ReportUtilities.CreateCell(ref rowHeader1, NUM_CELL);
                rowHeader1.Cells[0].SetCellValue(template.Organize?.Parent?.NAME.ToUpper());

                var rowHeader2 = ReportUtilities.CreateRow(ref sheet, 1, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader2, NUM_CELL);
                rowHeader2.Cells[0].SetCellValue(template.Organize.NAME.ToUpper());
                rowHeader2.Cells[2].SetCellValue(template.TITLE.ToUpper());
                rowHeader2.Cells[14].SetCellValue(rowHeader2.Cells[14].StringCellValue + $" {year}");

                var rowHeader3 = ReportUtilities.CreateRow(ref sheet, 2, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader3, NUM_CELL);
                rowHeader3.Cells[1].SetCellValue(template.CREATE_BY);
                //rowHeader2.Cells[14].SetCellValue(string.Concat(rowHeader2.Cells[14].StringCellValue, " ", template.));
                #endregion

                #region Details

                numRowCur = 6;
                var number = 1;
                var rowHeightDetail = sheet.GetRow(6).Height;
                foreach (var detail in detailCostElements.GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First())
                    .OrderByDescending(x => x.CENTER_CODE))
                {
                    foreach (var item in dataCost
                            .Where(x => x.CENTER_CODE == detail.CENTER_CODE)
                            .OrderBy(x => x.C_ORDER)
                            .GroupBy(x => x.CODE)
                            .Select(x => x.First()))
                    {
                        var space = new StringBuilder();
                        for (int i = 0; i < item.LEVEL; i++)
                        {
                            space.Append("\t");
                        }

                        //ReportUtilities.CreateRow(ref sheet, numRowCur, 50);
                        ReportUtilities.CopyRow(ref sheet, 7, numRowCur);
                        IRow rowCur = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                        rowCur.Height = -1;

                        rowCur.Cells[0].SetCellValue(detail.CENTER_CODE);
                        rowCur.Cells[0].CellStyle = styleCellDetail;

                        rowCur.Cells[1].SetCellValue(detail.Center.NAME);
                        rowCur.Cells[1].CellStyle = styleCellDetail;

                        rowCur.Cells[2].SetCellValue(item.CODE);
                        rowCur.Cells[2].CellStyle = styleCellDetail;

                        rowCur.Cells[3].SetCellValue($"{space}{item.NAME}");
                        if (item.IS_GROUP)
                        {
                            rowCur.Cells[3].CellStyle = styleCellBold;
                            rowCur.Cells[3].CellStyle.SetFont(fontBold);
                        }
                        else
                        {
                            rowCur.Cells[3].CellStyle = styleCellDetail;
                        }

                        for (int i = 4; i < NUM_CELL - 3; i++)
                        {
                            rowCur.Cells[i].SetCellValue(string.Empty);
                            rowCur.Cells[i].CellStyle = styleCellNumber;
                            rowCur.Cells[i].SetCellType(CellType.Numeric);
                        }

                        rowCur.Cells[16].SetCellFormula($"SUM(E{numRowCur + 1}:P{numRowCur + 1})");
                        rowCur.Cells[16].CellStyle = styleCellNumberColor;
                        rowCur.Cells[16].SetCellType(CellType.Formula);

                        rowCur.Cells[17].SetCellFormula($"Q{numRowCur + 1}/12");
                        rowCur.Cells[17].CellStyle = styleCellNumberColor;
                        rowCur.Cells[17].SetCellType(CellType.Formula);

                        rowCur.Cells[18].SetCellValue(string.Empty);
                        rowCur.Cells[18].CellStyle = styleCellDetail;

                        numRowCur++;
                        number++;
                    }
                }

                //Xóa dòng thừa cuối cùng khi tạo các dòng cho detail
                IRow rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                ReportUtilities.DeleteRow(ref sheet, rowLastDetail);

                rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                ReportUtilities.DeleteRow(ref sheet, rowLastDetail);
                #endregion

                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        /// <summary>
        /// generate excel file template base and store in path
        /// </summary>
        /// <param name="outFileStream"></param>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="year"></param>
        public override void GenerateTemplateBase(ref MemoryStream outFileStream, string path, string templateId, int year)
        {
            var dataCost = PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements, templateId, year, ignoreAuth: true);

            if (dataCost.Count == 0 || detailCostElements.Count == 0)
            {
                State = false;
                ErrorMessage = "Không tìm thấy dữ liệu";
                return;
            }

            try
            {
                //Mở file Template
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                templateWorkbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.CostPL));
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);

                //Số hàng và số cột hiện tại
                int numRowCur = 0;
                int NUM_CELL = ListColumnNameDataBase.Count;

                //Style cần dùng
                ICellStyle styleCellHeader = templateWorkbook.CreateCellStyle();
                styleCellHeader.CloneStyleFrom(sheet.GetRow(5).Cells[0].CellStyle);
                styleCellHeader.WrapText = true;

                ICellStyle styleCellDetail = templateWorkbook.CreateCellStyle();
                styleCellDetail.CloneStyleFrom(sheet.GetRow(6).Cells[0].CellStyle);
                styleCellDetail.WrapText = true;

                ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
                styleCellNumber.CloneStyleFrom(styleCellDetail);
                styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0.00");

                ICellStyle styleCellNumberColor = templateWorkbook.CreateCellStyle();
                styleCellNumberColor.CloneStyleFrom(styleCellNumber);
                styleCellNumberColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                styleCellNumberColor.FillPattern = FillPattern.SolidForeground;

                ICellStyle styleCellBold = templateWorkbook.CreateCellStyle();
                styleCellBold.CloneStyleFrom(sheet.GetRow(6).Cells[0].CellStyle);
                styleCellBold.WrapText = true;
                var fontBold = templateWorkbook.CreateFont();
                fontBold.Boldweight = (short)FontBoldWeight.Bold;
                fontBold.FontHeightInPoints = 11;
                fontBold.FontName = "Times New Roman";

                #region Header
                var template = UnitOfWork.Repository<TemplateRepo>().Get(templateId);
                var rowHeader1 = ReportUtilities.CreateRow(ref sheet, 0, NUM_CELL);
                ReportUtilities.CreateCell(ref rowHeader1, NUM_CELL);
                rowHeader1.Cells[0].SetCellValue(template.Organize?.Parent?.NAME.ToUpper());

                var rowHeader2 = ReportUtilities.CreateRow(ref sheet, 1, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader2, NUM_CELL);
                rowHeader2.Cells[0].SetCellValue(template.Organize.NAME.ToUpper());
                rowHeader2.Cells[2].SetCellValue(template.TITLE.ToUpper());
                rowHeader2.Cells[6].SetCellValue(rowHeader2.Cells[6].StringCellValue + $" {year}");

                var rowHeader3 = ReportUtilities.CreateRow(ref sheet, 2, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader3, NUM_CELL);
                rowHeader3.Cells[1].SetCellValue(template.CREATE_BY);
                //rowHeader2.Cells[14].SetCellValue(string.Concat(rowHeader2.Cells[14].StringCellValue, " ", template.));
                #endregion

                #region Details

                numRowCur = 6;
                var number = 1;
                var rowHeightDetail = sheet.GetRow(6).Height;
                foreach (var detail in detailCostElements.GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First())
                    .OrderByDescending(x => x.CENTER_CODE))
                {
                    foreach (var item in dataCost
                            .Where(x => x.CENTER_CODE == detail.CENTER_CODE)
                            .OrderBy(x => x.C_ORDER)
                            .GroupBy(x => x.CODE)
                            .Select(x => x.First()))
                    {
                        var space = new StringBuilder();
                        for (int i = 0; i < item.LEVEL; i++)
                        {
                            space.Append("\t");
                        }

                        //ReportUtilities.CreateRow(ref sheet, numRowCur, 50);
                        ReportUtilities.CopyRow(ref sheet, 7, numRowCur);
                        IRow rowCur = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                        rowCur.Height = -1;

                        rowCur.Cells[0].SetCellValue(detail.CENTER_CODE);
                        rowCur.Cells[0].CellStyle = styleCellDetail;

                        rowCur.Cells[1].SetCellValue(detail.Center.NAME);
                        rowCur.Cells[1].CellStyle = styleCellDetail;

                        rowCur.Cells[2].SetCellValue(item.CODE);
                        rowCur.Cells[2].CellStyle = styleCellDetail;

                        rowCur.Cells[3].SetCellValue($"{space}{item.NAME}");
                        if (item.IS_GROUP)
                        {
                            rowCur.Cells[3].CellStyle = styleCellBold;
                            rowCur.Cells[3].CellStyle.SetFont(fontBold);
                        }
                        else
                        {
                            rowCur.Cells[3].CellStyle = styleCellDetail;
                        }

                        rowCur.Cells[4].SetCellValue(string.Empty);
                        rowCur.Cells[4].CellStyle = styleCellDetail;

                        rowCur.Cells[5].SetCellValue(string.Empty);
                        rowCur.Cells[5].CellStyle = styleCellDetail;

                        for (int j = 6; j < NUM_CELL - 1; j++)
                        {
                            rowCur.Cells[j].SetCellValue(string.Empty);
                            if (j < NUM_CELL - 5 && (j - 5) % 4 == 2)
                            {
                                rowCur.Cells[j].CellStyle = styleCellDetail;
                            }
                            else
                            {
                                if (j >= NUM_CELL - 5)
                                {
                                    rowCur.Cells[j].CellStyle = styleCellNumberColor;
                                }
                                else
                                {
                                    rowCur.Cells[j].CellStyle = styleCellNumber;
                                }
                                rowCur.Cells[j].SetCellType(CellType.Numeric);
                            }
                        }

                        rowCur.Cells[NUM_CELL - 1].SetCellValue(string.Empty);
                        rowCur.Cells[NUM_CELL - 1].CellStyle = styleCellDetail;

                        numRowCur++;
                        number++;
                    }
                }

                //Xóa dòng thừa cuối cùng khi tạo các dòng cho detail
                IRow rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                ReportUtilities.DeleteRow(ref sheet, rowLastDetail);

                rowLastDetail = ReportUtilities.CreateRow(ref sheet, numRowCur, NUM_CELL);
                ReportUtilities.DeleteRow(ref sheet, rowLastDetail);
                #endregion

                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xảy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        #endregion

        public override IList<NodeDataFlow> BuildDataFlowTree(string orgCode, int year, int? version, int? sumUpVersion)
        {
            var costCenterRepo = UnitOfWork.Repository<CostCenterRepo>();

            if (costCenterRepo.GetManyByExpression(x => x.PARENT_CODE == orgCode).Count > 0 && version == null)
            {
                // nếu muốn xem tất cả các bản tổng hợp thì để version và sumUpVersion = null ở lần gọi đầu tiên
                if (!sumUpVersion.HasValue)
                {
                    var costPLData = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                        .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year);
                    return (from data in costPLData.GroupBy(x => x.SUM_UP_VERSION).Select(x => x.First())
                            orderby data.SUM_UP_VERSION descending
                            select new NodeDataFlow
                            {
                                sumUpVersion = data.SUM_UP_VERSION,
                                version = data.SUM_UP_VERSION,
                                year = year,
                                realId = data.ORG_CODE,
                                id = $"{data.ORG_CODE}_{data.SUM_UP_VERSION}",
                                pId = null,
                                isParent = true.ToString(),
                                name = string.Concat(data.CostCenter.NAME, " - ", $"Lần tổng hợp thứ {data.SUM_UP_VERSION}")
                            }).ToList();
                }
                else
                {
                    // level 1: Center
                    // get centers
                    var costPLData = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                        .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year
                         && x.SUM_UP_VERSION == sumUpVersion.Value);
                    return (from data in costPLData.GroupBy(x => x.FROM_ORG_CODE).Select(x => x.First())
                            orderby data.FROM_ORG_CODE descending
                            select new NodeDataFlow
                            {
                                sumUpVersion = data.SUM_UP_VERSION,
                                version = data.SUM_UP_VERSION,
                                year = year,
                                realId = data.FROM_ORG_CODE,
                                id = $"{data.FROM_ORG_CODE}_{data.SUM_UP_VERSION}",
                                pId = $"{orgCode}_{version.Value}",
                                isParent = true.ToString(),
                                name = string.Concat(data.FromCostCenter.NAME, " - ", $"Lần tổng hợp thứ {data.SUM_UP_VERSION}")
                            }).ToList();
                }
            }
            else
            {
                // level 2: department
                // get templates
                var sumupDetails = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE.Equals(orgCode) && x.SUM_UP_VERSION == version && x.TIME_YEAR == year)
                    .OrderByDescending(x => x.TEMPLATE_CODE);
                return (from data in sumupDetails
                        select new NodeDataFlow
                        {
                            id = data.TEMPLATE_CODE,
                            pId = orgCode,
                            realId = data.FROM_ORG_CODE,
                            year = year,
                            isParent = (!IsLeaf(data.FROM_ORG_CODE)).ToString(),
                            name = string.Concat(data.TEMPLATE_CODE, " - ", data.FromCostCenter.NAME, " - Version: ", data.DATA_VERSION),
                            version = data.DATA_VERSION
                        }).ToList();
            }
        }

        #region Data Preview
        /// <summary>
        /// Get list of cost elements have summed up
        /// </summary>
        /// <param name="centerCode">Cost center code has summed up</param>
        /// <param name="elementCode">Element code want to get detail</param>
        /// <param name="year">Year summed up</param>
        /// <param name="version">Version summed up</param>
        /// <returns>Returns list of cost elements have summed up</returns>
        public override IEnumerable<T_MD_COST_PL_ELEMENT> GetDetailSumUp(string centerCode, string elementCode, int year, int version, int? sumUpVersion, bool isCountComments, bool? isShowFile = null)
        {
            var plDataRepo = UnitOfWork.Repository<CostPLDataRepo>();
            var costPlReviewCommentRepo = UnitOfWork.Repository<CostPLReviewCommentRepo>();
            var plVersionRepo = UnitOfWork.Repository<CostPLRepo>();
            var plDataHistoryRepo = UnitOfWork.Repository<CostPLDataHistoryRepo>();

            if (IsLeaf(centerCode))
            {

                var parentCenterCode = GetCenter(centerCode).PARENT_CODE;
                var lstChildren = GetListOfChildrenCenter(parentCenterCode).Select(x => x.CODE);

                var (data, isCorp) = GetDataSumUp(year, parentCenterCode, elementCode, sumUpVersion.Value);
                #region Tìm dựa vào đơn vị tổng hợp
                var lookupData = data.Where(x => x.COST_CENTER_CODE == centerCode).ToLookup(x => x.TEMPLATE_CODE);
                foreach (var key in lookupData.Select(x => x.Key))
                {
                    if (lookupData[key].First().Template.IS_BASE)
                    {
                        var isNewestVersion = UnitOfWork.Repository<CostPLVersionRepo>()
                            .GetNewestByExpression(x => x.TEMPLATE_CODE == key && x.TIME_YEAR == year,
                            order: x => x.VERSION, isDescending: true)
                            .VERSION == lookupData[key].First().VERSION;
                        foreach (var item in lookupData[key])
                        {
                            var element = (T_MD_COST_PL_ELEMENT)item;
                            element.IsBase = true;
                            element.Values = new decimal[14]
                            {
                                lookupData[key].Sum(x => x.VALUE_JAN) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_FEB) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_MAR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_APR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_MAY) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_JUN) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_JUL) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_AUG) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SEP) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_OCT) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_NOV) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_DEC) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                            };

                            element.NAME = item.Template.NAME;
                            yield return element;
                        }
                        if (isNewestVersion)
                        {
                            // get data from base
                            var baseData = UnitOfWork.Repository<CostPLDataBaseRepo>()
                                .GetManyWithFetch(x => x.COST_PL_ELEMENT_CODE == elementCode && x.COST_CENTER_CODE == centerCode && x.TEMPLATE_CODE == key && x.VERSION == lookupData[key].First().VERSION && x.TIME_YEAR == year);
                            foreach (var item in baseData)
                            {
                                yield return (T_MD_COST_PL_ELEMENT)item;
                            }
                        }
                        else
                        {
                            // get data from base history
                            var baseDataHistory = UnitOfWork.Repository<CostPLDataBaseHistoryRepo>()
                                .GetManyWithFetch(x => x.COST_PL_ELEMENT_CODE == elementCode && x.COST_CENTER_CODE == centerCode && x.TEMPLATE_CODE == key && x.VERSION == lookupData[key].First().VERSION && x.TIME_YEAR == year);

                            foreach (var item in baseDataHistory)
                            {
                                yield return (T_MD_COST_PL_ELEMENT)item;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in lookupData[key])
                        {
                            var element = (T_MD_COST_PL_ELEMENT)item;
                            element.IS_GROUP = isShowFile.HasValue; // count comment is in the review, drill down will show the file base so it still will be group
                            yield return element;
                        }
                    }
                }

                #endregion
            }
            else
            {
                var comments = new List<T_BP_COST_PL_REVIEW_COMMENT>();
                if (isCountComments)
                {

                    comments = costPlReviewCommentRepo.GetManyWithFetch(
                            x => x.TIME_YEAR == year &&
                            x.ORG_CODE == GetCorp().CODE &&
                            x.COST_PL_ELEMENT_CODE == elementCode).ToList();
                }
                var (data, isCorp) = GetDataSumUp(year, centerCode, elementCode, version);
                var lookupData = data.ToLookup(x => x.COST_CENTER_CODE);
                foreach (var key in lookupData.Select(x => x.Key))
                {
                    IEnumerable<string> lstChildren = null;
                    var childComments = 0;
                    var parentComments = 0;
                    if (isCountComments)
                    {
                        lstChildren = GetListOfChildrenCenter(key).Select(x => x.CODE);
                        childComments = comments.Where(x => lstChildren.Contains(x.ON_ORG_CODE)).Sum(x => x.NUMBER_COMMENTS);
                        parentComments = comments.Where(x => x.ON_ORG_CODE == key).Sum(x => x.NUMBER_COMMENTS);
                    }
                    var isChild = IsLeaf(key);
                    yield return new T_MD_COST_PL_ELEMENT
                    {
                        ORG_CODE = key,
                        CENTER_CODE = key,
                        CODE = elementCode,
                        IsChildren = isChild,
                        //IS_GROUP = IsLeaf(key) ? lookupData[key].First().Template.IS_BASE ? true : false : true,
                        IS_GROUP = true,
                        TEMPLATE_CODE = lookupData[key].First().TEMPLATE_CODE,
                        TIME_YEAR = year,
                        VERSION = isCorp ? lookupData[key].First().VERSION : isChild ? version : lookupData[key].First().VERSION, // cấp tập đoàn thì sẽ là data_version còn nếu cấp dưới thì sẽ là sumup version
                        ORG_NAME = lookupData[key].First().CostCenter.NAME,
                        Values = new decimal[14]
                            {
                                lookupData[key].Sum(x => x.VALUE_JAN) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_FEB) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_MAR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_APR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_MAY) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_JUN) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_JUL) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_AUG) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SEP) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_OCT) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_NOV) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_DEC) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                lookupData[key].Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                            },

                        NUMBER_COMMENTS = isCountComments ? isChild ?
                        $"{parentComments}" :
                        $"{parentComments + childComments}|{parentComments}" : $"{childComments}|0"
                    };
                }
            }
        }

        public override IEnumerable<T_MD_COST_PL_ELEMENT> GetDetailSumUpTemplate(string elementCode, int year, int version, string templateCode, string centerCode)
        {
            var newestVersionInDb = GetFirstWithFetch(x => x.TEMPLATE_CODE == templateCode && x.TIME_YEAR == year)?.VERSION;
            if (!newestVersionInDb.HasValue)
            {
                return null;
            }
            else
            {
                if (newestVersionInDb.Value == version)
                {
                    // newest data
                    // get data in table base data
                    var baseData = UnitOfWork.Repository<CostPLDataBaseRepo>()
                        .GetManyWithFetch(x => x.TEMPLATE_CODE == templateCode 
                        && x.TIME_YEAR == year 
                        && x.COST_PL_ELEMENT_CODE == elementCode 
                        && x.VERSION == version 
                        && x.COST_CENTER_CODE == centerCode);
                    return from item in baseData
                           select (T_MD_COST_PL_ELEMENT)item;
                }
                else
                {
                    // old data
                    // get data in table base data history
                    var baseData = UnitOfWork.Repository<CostPLDataBaseHistoryRepo>()
                        .GetManyWithFetch(x => x.TEMPLATE_CODE == templateCode 
                        && x.TIME_YEAR == year 
                        && x.COST_PL_ELEMENT_CODE == elementCode 
                        && x.VERSION == version 
                        && x.COST_CENTER_CODE == centerCode);
                    return from item in baseData
                           select (T_MD_COST_PL_ELEMENT)item;
                }
            }
        }

        /// <summary>
        /// Lấy dữ liệu được sum up lên đơn vị theo version và khoản mục
        /// </summary>
        /// <param name="year"></param>
        /// <param name="centerCode"></param>
        /// <param name="elementCode"></param>
        /// <param name="sumUpVersion"></param>
        /// <returns></returns>
        private (IList<T_BP_COST_PL_DATA>, bool) GetDataSumUp(int year, string centerCode, string elementCode, int sumUpVersion)
        {
            var plDataRepo = UnitOfWork.Repository<CostPLDataRepo>();
            var costPlReviewCommentRepo = UnitOfWork.Repository<CostPLReviewCommentRepo>();
            var plVersionRepo = UnitOfWork.Repository<CostPLRepo>();
            var plDataHistoryRepo = UnitOfWork.Repository<CostPLDataHistoryRepo>();

            var lstDetails = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
                .GetManyByExpression(x => x.ORG_CODE == centerCode &&
                    x.TIME_YEAR == year &&
                    x.SUM_UP_VERSION == sumUpVersion);
            var lookup = lstDetails.ToLookup(x => x.FROM_ORG_CODE);
            var lstChildren = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);

            var isCorp = string.IsNullOrEmpty(GetCenter(centerCode).PARENT_CODE);
            var isLeafCenter = IsLeaf(centerCode);

            var data = new List<T_BP_COST_PL_DATA>();
            foreach (var key in lookup.Select(x => x.Key))
            {
                var costPL = new List<T_BP_COST_PL_DATA>();
                var isLeaf = IsLeaf(key);
                if (isLeaf)
                {
                    foreach (var item in lookup[key])
                    {
                        if (lstChildren.Contains(GetTemplate(item.TEMPLATE_CODE).ORG_CODE))
                        {
                            // đơn vị con tự nộp
                            costPL.AddRange(plDataRepo.GetPLDataByCenterCode(item.FROM_ORG_CODE, lstChildren.ToList(), year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList());
                        }
                        else
                        {
                            // được nộp hộ
                            var dataFromOrg = plDataRepo.GetPLDataByOrgCode(item.FROM_ORG_CODE, year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList();
                            costPL.AddRange(dataFromOrg.Where(x => lstChildren.Contains(x.COST_CENTER_CODE)));
                        }
                    }
                }
                else
                {
                    if (isCorp)
                    {
                        // tập đoàn
                        if (string.IsNullOrEmpty(GetCenter(key).PARENT_CODE))
                        {
                            // tập đoàn tổng hợp
                            foreach (var item in lookup[key])
                            {
                                costPL.AddRange(plDataRepo.GetPLDataByOrgCode(key, year, string.Empty, item.DATA_VERSION).ToList());
                            }
                        }
                        else
                        {
                            // được phòng ban nộp hộ
                            foreach (var item in lookup[key])
                            {
                                // được nộp hộ
                                var dataFromOrg = plDataRepo.GetPLDataByOrgCode(item.FROM_ORG_CODE, year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList();
                                costPL.AddRange(dataFromOrg.Where(x => lstChildren.Contains(x.COST_CENTER_CODE)));
                            }
                        }
                    }
                    else
                    {
                        // trung tâm
                        if (key == centerCode)
                        {
                            // trung tâm tổng hợp
                            costPL = plDataRepo.GetPLDataByCenterCode(key, lstChildren.ToList(), year, null, sumUpVersion).ToList();
                        }
                        else
                        {
                            // được phòng ban nộp hộ
                            foreach (var item in lookup[key])
                            {
                                // được nộp hộ
                                var dataFromOrg = plDataRepo.GetPLDataByOrgCode(item.FROM_ORG_CODE, year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList();
                                costPL.AddRange(dataFromOrg.Where(x => lstChildren.Contains(x.COST_CENTER_CODE)));
                            }
                        }
                    }
                }
                data.AddRange(costPL.Where(x => x.COST_PL_ELEMENT_CODE == elementCode &&
                //x.STATUS == Approve_Status.DaPheDuyet &&
                lookup[key].Select(y => y.TEMPLATE_CODE).Contains(x.TEMPLATE_CODE)));
                if (data == null || data.Count() == 0)
                {
                    continue;
                }
            }
            return (data, isCorp);
        }

        /// <summary>
        /// Get template to ObjDetail first before call this method
        /// Get data cost elements include detail cost elements.
        /// </summary>
        /// <param name="detailCostElements">out detail cost elemts</param>
        /// <param name="year">which year of template</param>
        /// <param name="version">which version of template</param>
        /// <returns>Returns list cost elemts with their parents and their value</returns>
        public IList<T_MD_COST_PL_ELEMENT> GetDataCostPreview(
            out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements,
            string templateId,
            string centerCode = "",
            int? year = null,
            int? version = null,
            bool? isHasValue = null)
        {
            var pureLstItems = PreparePureList(out detailCostElements, templateId, year.Value, centerCode);

            var sum = GetSumDescendants(detailCostElements, pureLstItems, parent_id: string.Empty, templateId, year, version).Distinct().ToList();
            if (isHasValue.HasValue)
            {
                if (isHasValue.Value)
                {
                    return sum.Where(x => x.Values.Sum() > 0).ToList();
                }
                else
                {
                    return sum.Where(x => x.Values.Sum() == 0 && !x.IS_GROUP).ToList();
                }
            }
            else
            {
                return sum;
            }
        }

        public IList<T_MD_COST_PL_ELEMENT> PreparePureList(IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements, int year)
        {
            // get all cost elements
            var allCostElements = UnitOfWork.Repository<CostPLElementRepo>().GetManyByExpression(x => x.TIME_YEAR == year);
            // get cost elements in details cost elements
            var costElements = from d in detailCostElements
                               select d.Element;
            // lookup cost elements by center code
            var lookupElementsCenter = detailCostElements.ToLookup(x => x.CENTER_CODE);

            var pureLstItems = new List<T_MD_COST_PL_ELEMENT>();
            // loop through all center
            foreach (var ctCode in lookupElementsCenter.Select(l => l.Key))
            {
                // lookup cost elements
                var lookupElements = lookupElementsCenter[ctCode].ToLookup(x => x.Element.PARENT_CODE);
                foreach (var code in lookupElements.Select(l => l.Key))
                {
                    var level = 0;
                    // temp list
                    var lst = new List<T_MD_COST_PL_ELEMENT>();
                    // add all leaf to temp list item
                    lst.AddRange(from item in lookupElements[code]
                                 select new T_MD_COST_PL_ELEMENT
                                 {
                                     CENTER_CODE = ctCode,
                                     C_ORDER = item.Element.C_ORDER,
                                     NAME = item.Element.NAME,
                                     PARENT_CODE = item.Element.PARENT_CODE,
                                     CODE = item.Element.CODE,
                                     LEVEL = 0,
                                     TIME_YEAR = item.TIME_YEAR
                                 });
                    var parentCode = code;
                    while (!string.IsNullOrEmpty(parentCode))
                    {
                        level++;
                        // find parents to add into list
                        var element = allCostElements.FirstOrDefault(x => x.CODE == parentCode);
                        if (element != null)
                        {
                            parentCode = element.PARENT_CODE;
                            element.CENTER_CODE = ctCode;
                            element.LEVEL = level;
                            element.IS_GROUP = true;
                            lst.Add((T_MD_COST_PL_ELEMENT)element.CloneObject());     // must to clone to other object because it reference to other center
                        }
                        else
                        {
                            break;
                        }
                    }

                    lst.ForEach(x => x.LEVEL = level - x.LEVEL);
                    pureLstItems.AddRange(lst);
                }
            }

            return pureLstItems.OrderBy(x => x.C_ORDER).ToList();
        }

        /// <summary>
        /// Xem theo template
        /// </summary>
        /// <param name="detailCostElements"></param>
        /// <param name="templateId"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public IList<T_MD_COST_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements,
            string templateId,
            int year,
            string centerCode = "",
            bool ignoreAuth = false)
        {
            templateId = templateId ?? string.Empty;
            var template = GetTemplate(templateId);
            var currentUserCenterCode = ProfileUtilities.User.ORGANIZE_CODE;
            var childOrgCosts = GetListOfChildrenCenter(currentUserCenterCode).Select(x => x.CODE);

            if (ignoreAuth || childOrgCosts.Contains(template.ORG_CODE) || currentUserCenterCode.Equals(template.ORG_CODE))
            {
                detailCostElements = UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                    .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && x.TIME_YEAR == year);
            }
            else
            {
                // get details cost elements
                if (string.IsNullOrEmpty(centerCode))
                {
                    var lstChildCenterCodes = UnitOfWork.Repository<CostCenterRepo>()
                        .GetManyByExpression(x => x.PARENT_CODE == currentUserCenterCode || x.CODE.Equals(currentUserCenterCode))
                        .Select(x => x.CODE);
                    detailCostElements = UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                        .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && lstChildCenterCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year);
                }
                else
                {
                    detailCostElements = UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                        .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && x.CENTER_CODE == centerCode && x.TIME_YEAR == year);
                }
            }
            return PreparePureList(detailCostElements, year);
        }

        /// <summary>
        /// Xem theo template
        /// </summary>
        /// <param name="detailCostElements"></param>
        /// <param name="templateId"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public IList<T_MD_COST_PL_ELEMENT> PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements,
            string templateId,
            int year)
        {

            detailCostElements = UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && x.TIME_YEAR == year);

            return PreparePureList(detailCostElements, year);
        }



        /// <summary>
        /// Xem theo center code
        /// </summary>
        /// <param name="detailCostElements"></param>
        /// <param name="centerCodes"></param>
        /// <returns></returns>
        public IList<T_MD_COST_PL_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_COST_PL> detailCostElements,
            IList<string> centerCodes,
            int year)
        {
            // Tìm mẫu nộp hộ
            var listTemplateCodes = this.UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                .GetManyByExpression(x => centerCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year)
                .Select(x => x.TEMPLATE_CODE).Distinct().ToList();
            var findCostPL = this.CurrentRepository.GetManyByExpression(
                    x => listTemplateCodes.Contains(x.TEMPLATE_CODE) && !centerCodes.Contains(x.ORG_CODE));
            var lst = new List<T_MD_TEMPLATE_DETAIL_COST_PL>();
            detailCostElements = new List<T_MD_TEMPLATE_DETAIL_COST_PL>();
            foreach (var template in listTemplateCodes)
            {
                lst.AddRange(UnitOfWork.Repository<TemplateDetailCostPLRepo>()
                    .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(template) && centerCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year));
            }

            detailCostElements = lst;

            return PreparePureList(detailCostElements, year);
        }

        /// <summary>
        /// Sum up data cost center by center code and year (Tổng hợp dữ liệu tại phòng ban)
        /// </summary>
        /// <param name="costPL">Output header cost pl</param>
        /// <param name="centerCode">Cost center code want to sum up</param>
        /// <param name="year">Year want to sum up</param>
        public override void SumUpDataCenter(out T_BP_COST_PL_VERSION costPL, string centerCode, int year)
        {
            if (string.IsNullOrEmpty(GetCenter(centerCode).PARENT_CODE))
            {
                // BTC tổng hợp ngân sách
                ValidateBudgetPeriod(year, BudgetPeriod.BTC_TONG_HOP_NS);
            }
            else
            {
                // TT tổng hợp ngân sách
                ValidateBudgetPeriod(year, BudgetPeriod.BAN_TT_TONG_HOP_NS);
            }
            this.CheckPeriodTimeValid(year);
            if (!this.State)
            {
                costPL = null;
                return;
            }

            var lstData = new List<T_BP_COST_PL_DATA>();
            costPL = new T_BP_COST_PL_VERSION();

            try
            {
                UnitOfWork.BeginTransaction();
                var sumUpDetailRepo = UnitOfWork.Repository<CostPLSumUpDetailRepo>();
                var costPLDataRepo = UnitOfWork.Repository<CostPLDataRepo>();

                // get list all children centers in cost center tree
                var lstCostCenters = GetListOfChildrenCenter(centerCode);

                // get all data have approved
                var costPlDataApproved = costPLDataRepo.GetManyByExpression(x => (x.COST_CENTER_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE)) ||
                (x.STATUS == Approve_Status.DaPheDuyet &&
                x.TIME_YEAR == year &&
                lstCostCenters.Any(c => c.CODE.Equals(x.COST_CENTER_CODE))));

                if (costPlDataApproved.Count == 0)
                {
                    // do not have data to sum up
                    State = false;
                    ErrorMessage = $"Do not have data to sum up in {year}";
                    return;
                }

                // add new record to pl sum up detail
                // get current version in sumup detail
                var newestDetail = sumUpDetailRepo
                    .GetNewestByExpression(x => x.ORG_CODE.Equals(centerCode), x => x.CREATE_DATE, true);
                var version = 1;
                if (newestDetail != null)
                {
                    version = newestDetail.SUM_UP_VERSION + 1;
                }

                // sumup element code with the same element code
                var lookup = costPlDataApproved.ToLookup(x => x.COST_PL_ELEMENT_CODE);
                foreach (var code in lookup.Select(x => x.Key))
                {
                    if (lookup[code].Count() == 1)
                    {
                        lstData.Add((T_BP_COST_PL_DATA)lookup[code].First().CloneObject());
                    }
                    else
                    {
                        lstData.Add(new T_BP_COST_PL_DATA
                        {
                            VALUE_APR = lookup[code].Sum(x => x.VALUE_APR),
                            VALUE_AUG = lookup[code].Sum(x => x.VALUE_AUG),
                            VALUE_DEC = lookup[code].Sum(x => x.VALUE_DEC),
                            VALUE_FEB = lookup[code].Sum(x => x.VALUE_FEB),
                            VALUE_JAN = lookup[code].Sum(x => x.VALUE_JAN),
                            VALUE_JUL = lookup[code].Sum(x => x.VALUE_JUL),
                            VALUE_JUN = lookup[code].Sum(x => x.VALUE_JUN),
                            VALUE_MAR = lookup[code].Sum(x => x.VALUE_MAR),
                            VALUE_MAY = lookup[code].Sum(x => x.VALUE_MAY),
                            VALUE_NOV = lookup[code].Sum(x => x.VALUE_NOV),
                            VALUE_OCT = lookup[code].Sum(x => x.VALUE_OCT),
                            VALUE_SEP = lookup[code].Sum(x => x.VALUE_SEP),
                            VALUE_SUM_YEAR = lookup[code].Sum(x => x.VALUE_SUM_YEAR),
                            VALUE_SUM_YEAR_PREVENTIVE = lookup[code].Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE),

                            COST_PL_ELEMENT_CODE = lookup[code].First().COST_PL_ELEMENT_CODE
                        });
                    }
                }
                var currentUser = ProfileUtilities.User?.USER_NAME;

                // get current version in cost pl data
                var newestPL = UnitOfWork.Repository<CostPLRepo>()
                    .GetNewestByExpression(x => x.ORG_CODE.Equals(centerCode) && x.TIME_YEAR == year, x => x.CREATE_DATE, true);

                var versionPl = 1;
                if (newestPL != null)
                {
                    newestPL.VERSION++;
                    versionPl = newestPL.VERSION;
                    newestPL.UPDATE_BY = currentUser;
                    newestPL.STATUS = Approve_Status.ChuaTrinhDuyet;
                    CurrentRepository.Update(newestPL);
                }
                else
                {
                    // create header cost pl
                    CurrentRepository.Create(new T_BP_COST_PL
                    {
                        PKID = Guid.NewGuid().ToString(),
                        IS_SUMUP = true,
                        ORG_CODE = centerCode,
                        TIME_YEAR = year,
                        VERSION = versionPl,
                        FILE_ID = string.Empty,
                        TEMPLATE_CODE = string.Empty,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        CREATE_BY = currentUser
                    });
                }

                // insert to cost pl version
                costPL = new T_BP_COST_PL_VERSION
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = centerCode,
                    TEMPLATE_CODE = string.Empty,
                    VERSION = versionPl,
                    TIME_YEAR = year,
                    CREATE_BY = currentUser
                };
                UnitOfWork.Repository<CostPLVersionRepo>().Create(costPL);

                foreach (var item in lstData)
                {
                    item.ORG_CODE = centerCode;
                    item.COST_CENTER_CODE = centerCode;
                    item.TEMPLATE_CODE = string.Empty;
                    item.PKID = Guid.NewGuid().ToString();
                    item.VERSION = versionPl;
                    item.TIME_YEAR = year;
                    item.STATUS = Approve_Status.ChuaTrinhDuyet;
                    item.DESCRIPTION = string.Empty;
                    item.CREATE_BY = currentUser;
                }

                // get all cost pl data current with centercode and year
                var lstCostPlDataOldVersion = costPLDataRepo
                    .GetManyByExpression(x => x.COST_CENTER_CODE.Equals(centerCode) && x.TIME_YEAR == year);

                // delete them from table pl data
                // TODO: chỉnh sửa câu lệnh sql
                _ = UnitOfWork.GetSession().CreateSQLQuery($"DELETE FROM T_BP_COST_PL_DATA WHERE COST_CENTER_CODE = '{centerCode}' AND TIME_YEAR = {year}")
                    .ExecuteUpdate();

                // insert to pl data history
                UnitOfWork.Repository<CostPLDataHistoryRepo>().Create((from pl in lstCostPlDataOldVersion
                                                                       select (T_BP_COST_PL_DATA_HISTORY)pl).ToList());

                // insert to pl history
                UnitOfWork.Repository<CostPLHistoryRepo>().Create(new T_BP_COST_PL_HISTORY
                {
                    ACTION = Approve_Action.TongHop,
                    ACTION_USER = currentUser,
                    CREATE_BY = currentUser,
                    ORG_CODE = ProfileUtilities.User.ORGANIZE_CODE,
                    ACTION_DATE = DateTime.Now,
                    PKID = Guid.NewGuid().ToString(),
                    TIME_YEAR = year,
                    TEMPLATE_CODE = string.Empty,
                    VERSION = costPL.VERSION,
                });

                // create new lstData
                costPLDataRepo.Create(lstData.ToList());

                // get list cost pl which have org cost in cost center summary
                var lstCostPL = costPlDataApproved.ToLookup(x => new { OrgCode = x.ORG_CODE, TemplateCode = x.TEMPLATE_CODE, TemplateVersion = x.VERSION });

                // create list sum up detail
                sumUpDetailRepo.Create((from c in lstCostPL.Select(x => x.Key)
                                        select new T_BP_COST_PL_SUM_UP_DETAIL
                                        {
                                            PKID = Guid.NewGuid().ToString(),
                                            FROM_ORG_CODE = c.OrgCode,
                                            ORG_CODE = centerCode,
                                            TEMPLATE_CODE = c.TemplateCode,
                                            TIME_YEAR = year,
                                            DATA_VERSION = c.TemplateVersion,
                                            SUM_UP_VERSION = versionPl,
                                            CREATE_BY = currentUser
                                        }).ToList());
                UnitOfWork.Commit();

                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_TONG_HOP,
                        OrgCode = ProfileUtilities.User.ORGANIZE_CODE,
                        TimeYear = year,
                        ModulType = ModulType.CostPL,
                        UserSent = currentUser
                    });

                // remove session
                foreach (var item in lstData)
                {
                    UnitOfWork.GetSession().Evict(item);
                }
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
                Exception = e;
                costPL = null;
            }
        }

        /// <summary>
        /// Summary sum up cost center
        /// </summary>
        /// <param name="plDataCostElements">List of data cost element output</param>
        /// <param name="year">Year want to summary</param>
        /// <param name="centerCode">Cost center code want to summary</param>
        /// <param name="version">Version want to summary</param>
        /// <returns>Returns list of cost element with their value</returns>
        public IList<T_MD_COST_PL_ELEMENT> SummarySumUpCenter(
            out IList<T_BP_COST_PL_DATA> plDataCostElements,
            int year,
            string centerCode,
            int? version,
            bool? isHasValue = null,
            string templateId = "")
        {
            // get newest cost pl data by center code
            plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>()
                .GetPLDataByCenterCode(null, new List<string> { centerCode }, year, templateId, version);
            plDataCostElements = plDataCostElements.Where(x => x.STATUS == Approve_Status.DaPheDuyet).ToList();
            return SummaryCenter(plDataCostElements, centerCode, year, isHasValue);
        }

        /// <summary>
        /// Calculate data of each cost element with their parents.
        /// Data of parents calculated by sum of all children
        /// View by template
        /// </summary>
        /// <param name="details">List detail cost</param>
        /// <param name="pureItems">List elements want to calculate data</param>
        /// <param name="parent_id">Start calculate from which parent. string Empty to the root</param>
        /// <param name="year">Which year of data</param>
        /// <param name="version">Which version of data</param>
        /// <returns>Returns list of cost element with their data</returns>
        private IEnumerable<T_MD_COST_PL_ELEMENT> GetSumDescendants(
            IEnumerable<T_MD_TEMPLATE_DETAIL_COST_PL> details,
            IEnumerable<T_MD_COST_PL_ELEMENT> pureItems,
            string parent_id,
            string templateId,
            int? year = null,
            int? version = null)
        {
            var lookupCenter = pureItems.ToLookup(x => x.CENTER_CODE);
            foreach (var centerCode in lookupCenter.Select(x => x.Key))
            {
                var costPlData = GetVersionData(templateId, centerCode, year, version);
                var items = lookupCenter[centerCode];

                var lookup = items.ToLookup(i => i.PARENT_CODE);
                Queue<T_MD_COST_PL_ELEMENT> st = new Queue<T_MD_COST_PL_ELEMENT>(lookup[parent_id]);

                while (st.Count > 0)
                {
                    // get first item in queue
                    var item = st.Dequeue();
                    // variable to check should return item or not
                    bool shouldReturn = true;
                    // lst to store children of item which have children
                    var lstHasChild = new List<T_MD_COST_PL_ELEMENT>();
                    // loop through items which have parent id = item id
                    foreach (var i in lookup[item.CODE])
                    {
                        if (lookup[i.CODE].Count() > 0)
                        {
                            shouldReturn = false;
                            lstHasChild.Add(i);
                            st.Enqueue(i);
                        }
                        else
                        {
                            if (i.HasAssignValue)
                            {
                                // if i is total of its child
                                // add total to parent
                                for (int j = 0; j < item.Values.Length; j++)
                                {
                                    item.Values[j] += i.Values[j];
                                }
                            }
                            else
                            {
                                // if i does not count total yet
                                // get it datatree then add to total
                                var detail = details.FirstOrDefault(x => x.ELEMENT_CODE == i.CODE && x.CENTER_CODE == i.CENTER_CODE);
                                if (detail != null)
                                {
                                    detail.PlData = costPlData.FirstOrDefault(x => x.COST_PL_ELEMENT_CODE == detail.ELEMENT_CODE && x.COST_CENTER_CODE == detail.CENTER_CODE);
                                }
                                var treeData = detail?.PlData;
                                if (treeData != null)
                                {
                                    item.Values[0] += treeData.VALUE_JAN ?? 0;
                                    item.Values[1] += treeData.VALUE_FEB ?? 0;
                                    item.Values[2] += treeData.VALUE_MAR ?? 0;
                                    item.Values[3] += treeData.VALUE_APR ?? 0;
                                    item.Values[4] += treeData.VALUE_MAY ?? 0;
                                    item.Values[5] += treeData.VALUE_JUN ?? 0;
                                    item.Values[6] += treeData.VALUE_JUL ?? 0;
                                    item.Values[7] += treeData.VALUE_AUG ?? 0;
                                    item.Values[8] += treeData.VALUE_SEP ?? 0;
                                    item.Values[9] += treeData.VALUE_OCT ?? 0;
                                    item.Values[10] += treeData.VALUE_NOV ?? 0;
                                    item.Values[11] += treeData.VALUE_DEC ?? 0;
                                    item.Values[12] += treeData.VALUE_SUM_YEAR ?? 0;
                                    item.Values[13] += treeData.VALUE_SUM_YEAR_PREVENTIVE ?? 0;

                                    item.HasAssignValue = true;

                                    i.Values[0] = treeData.VALUE_JAN ?? 0;
                                    i.Values[1] = treeData.VALUE_FEB ?? 0;
                                    i.Values[2] = treeData.VALUE_MAR ?? 0;
                                    i.Values[3] = treeData.VALUE_APR ?? 0;
                                    i.Values[4] = treeData.VALUE_MAY ?? 0;
                                    i.Values[5] = treeData.VALUE_JUN ?? 0;
                                    i.Values[6] = treeData.VALUE_JUL ?? 0;
                                    i.Values[7] = treeData.VALUE_AUG ?? 0;
                                    i.Values[8] = treeData.VALUE_SEP ?? 0;
                                    i.Values[9] = treeData.VALUE_OCT ?? 0;
                                    i.Values[10] = treeData.VALUE_NOV ?? 0;
                                    i.Values[11] = treeData.VALUE_DEC ?? 0;
                                    i.Values[12] = treeData.VALUE_SUM_YEAR ?? 0;
                                    i.Values[13] = treeData.VALUE_SUM_YEAR_PREVENTIVE ?? 0;

                                    i.DESCRIPTION = treeData.DESCRIPTION;
                                }
                            }
                            yield return i;
                        }
                    }

                    // remove all child of item
                    // include children have child
                    lookup = lookup
                        .Where(x => x.Key != item.CODE)
                        .SelectMany(x => x)
                        .ToLookup(l => l.PARENT_CODE);
                    if (shouldReturn)
                    {
                        if (item.PARENT_CODE == parent_id && !item.IS_GROUP)
                        {
                            var data = costPlData.Where(x => x.COST_PL_ELEMENT_CODE == item.CODE && x.COST_CENTER_CODE == item.CENTER_CODE);
                            if (data != null)
                            {
                                item.Values = new decimal[14]
                                {
                                    data.Sum(x => x.VALUE_JAN) ?? 0,
                                    data.Sum(x => x.VALUE_FEB) ?? 0,
                                    data.Sum(x => x.VALUE_MAR) ?? 0,
                                    data.Sum(x => x.VALUE_APR) ?? 0,
                                    data.Sum(x => x.VALUE_MAY) ?? 0,
                                    data.Sum(x => x.VALUE_JUN) ?? 0,
                                    data.Sum(x => x.VALUE_JUL) ?? 0,
                                    data.Sum(x => x.VALUE_AUG) ?? 0,
                                    data.Sum(x => x.VALUE_SEP) ?? 0,
                                    data.Sum(x => x.VALUE_OCT) ?? 0,
                                    data.Sum(x => x.VALUE_NOV) ?? 0,
                                    data.Sum(x => x.VALUE_DEC) ?? 0,
                                    data.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                    data.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                                };


                            }
                            yield return item;
                        }
                        else
                        {
                            // if item does not have child
                            // return it
                            yield return item;
                        }
                    }
                    else
                    {
                        // add children of item which have chilren to lookup 
                        if (lstHasChild.Count > 0)
                        {
                            lookup = lookup
                                .SelectMany(l => l)
                                .Concat(lstHasChild)
                                .ToLookup(x => x.PARENT_CODE);
                        }
                        // re-enqueue item to queue
                        st.Enqueue(item);
                    }
                }
            }
        }

        /// <summary>
        /// Get cost PL data by version
        /// </summary>
        /// <param name="templateCode">Template code of PL data</param>
        /// <param name="orgCode">Center code of PL data</param>
        /// <param name="year">Which year of data</param>
        /// <param name="version">Which version of data</param>
        /// <returns>Returns Cost PL Data</returns>
        private IList<T_BP_COST_PL_DATA> GetVersionData(
            string templateCode,
            string centerCode,
            int? year = null,
            int? version = null)
        {
            string orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            var template = GetTemplate(templateCode);
            var lstChildren = GetListOfChildrenCenter(orgCode).Select(x => x.CODE);
            // check if orgCode is org code of template or not
            if (template.ORG_CODE.Equals(orgCode))
            {
                return UnitOfWork.Repository<CostPLDataRepo>().GetPLDataByOrgCode(orgCode, year.Value, templateCode, version);
            }
            else if (lstChildren.Contains(template.ORG_CODE))
            {
                return UnitOfWork.Repository<CostPLDataRepo>().GetPLDataByOrgCode(template.ORG_CODE, year.Value, templateCode, version);
            }
            else
            {
                return UnitOfWork.Repository<CostPLDataRepo>().GetPLDataByCenterCode(template.ORG_CODE, new List<string> { centerCode }, year.Value, templateCode, version);
                // get 
            }
        }

        /// <summary>
        /// Summary data of a center with newest data
        /// </summary>
        /// <param name="plDataCostElements">List cost pl data want to out</param>
        /// <param name="centerCode">Center code want to summary data</param>
        /// <returns>Returns list cost elements with their data</returns>
        public IList<T_MD_COST_PL_ELEMENT> SummaryCenterOut(out IList<T_BP_COST_PL_DATA> plDataCostElements,
                                                              string centerCode,
                                                              int year,
                                                              int? version,
                                                              bool? isHasValue = null)
        {
            if (!version.HasValue)
            {
                // get newest cost pl data by center code
                plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>()
                    .GetManyByExpression(x => x.COST_CENTER_CODE.Equals(centerCode) && x.TIME_YEAR == year);
            }
            else
            {
                ObjDetail.ORG_CODE = centerCode;
                if (IsLeaf())
                {
                    // get all data have approved
                    plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>()
                        .GetManyByExpression(x => x.COST_CENTER_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE));
                }
                else
                {
                    // get list all children centers in cost center tree
                    var lstCostCenters = UnitOfWork.Repository<CostCenterRepo>().GetManyByExpression(x => x.PARENT_CODE == centerCode);

                    // get all data have approved
                    plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>()
                        .GetManyByExpression(x => x.STATUS == Approve_Status.DaPheDuyet && (x.ORG_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE) ||
                    x.TIME_YEAR == year &&
                    lstCostCenters.Any(c => c.CODE.Equals(x.COST_CENTER_CODE))));
                }
            }

            return SummaryCenter(plDataCostElements, centerCode, year, isHasValue);
        }

        /// <summary>
        /// Get data has summed up (history)
        /// Lấy dữ liệu đã được tổng hợp lên đơn vị cha theo version
        /// </summary>
        /// <param name="plDataCostElements">List out data</param>
        /// <param name="centerCode">Org code của đơn vị được tổng hợp</param>
        /// <param name="year">Năm dữ liệu muốn xem</param>
        /// <param name="version">Version của dữ liệu muốn xem. Null thì sẽ lấy mới nhất</param>
        /// <returns></returns>
        public IList<T_MD_COST_PL_ELEMENT> SummaryCenterVersion(out IList<T_BP_COST_PL_DATA> plDataCostElements,
                                                              string centerCode,
                                                              int year,
                                                              int? version,
                                                              bool isDrillDown = false)
        {
            if (isDrillDown)
            {
                plDataCostElements = GetAllSumUpDetails(centerCode, year, version.Value);
            }
            else
            {
                plDataCostElements = UnitOfWork.Repository<CostPLDataRepo>()
                    .GetPLDataByOrgCode(centerCode, year, string.Empty, version);
            }
            return SummaryCenter(plDataCostElements, centerCode, year);
        }

        /// <summary>
        /// Lấy danh sách tất cả các data đã được tổng hợp lên cho centerCode theo version và năm
        /// </summary>
        /// <param name="centerCode"></param>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private IList<T_BP_COST_PL_DATA> GetAllSumUpDetails(string centerCode, int year, int version)
        {
            var detailsSumUp = UnitOfWork.Repository<CostPLSumUpDetailRepo>()
               .GetManyWithFetch(x => x.ORG_CODE == centerCode && x.TIME_YEAR == year && x.SUM_UP_VERSION == version);

            var lstChildren = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);
            var lstDetails = new List<T_BP_COST_PL_DATA>();
            var lstResult = new List<T_BP_COST_PL_DATA>();
            var plDataRepo = UnitOfWork.Repository<CostPLDataRepo>();
            foreach (var detail in detailsSumUp)
            {
                var details = plDataRepo
                    .GetPLDataByCenterCode(
                    detail.FROM_ORG_CODE,
                    lstChildren.ToList(),
                    year, detail.TEMPLATE_CODE,
                    detail.DATA_VERSION);
                if (IsLeaf(detail.FROM_ORG_CODE))
                {
                    details.ForEach(x => x.VERSION = detail.SUM_UP_VERSION);
                }
                else
                {
                    details.ForEach(x => x.VERSION = detail.DATA_VERSION);
                }
                lstDetails.AddRange(details);
            }

            var lookupElement = lstDetails.ToLookup(x => new { elementCode = x.COST_PL_ELEMENT_CODE, centerCode = x.COST_CENTER_CODE });
            foreach (var key in lookupElement.Select(x => x.Key))
            {
                if (lookupElement[key].Count() == 1)
                {
                    lstResult.AddRange(lookupElement[key]);
                }
                else
                {
                    var data = lookupElement[key];
                    var lookupCenter = data.ToLookup(x => x.COST_CENTER_CODE);
                    foreach (var orgCode in lookupCenter.Select(x => x.Key))
                    {
                        if (lookupCenter[orgCode].Count() == 1)
                        {
                            lstResult.AddRange(lookupCenter[orgCode]);
                        }
                        else
                        {
                            var dataOrgCode = lookupCenter[orgCode];
                            lstResult.Add(new T_BP_COST_PL_DATA
                            {
                                ORG_CODE = orgCode,
                                COST_PL_ELEMENT_CODE = key.elementCode,
                                COST_CENTER_CODE = key.centerCode,
                                CostElement = lookupCenter[orgCode].First().CostElement,
                                CostCenter = lookupCenter[orgCode].First().CostCenter,
                                Organize = lookupCenter[orgCode].First().Organize,
                                VALUE_JAN = dataOrgCode.Sum(x => x.VALUE_JAN) ?? 0,
                                VALUE_FEB = dataOrgCode.Sum(x => x.VALUE_FEB) ?? 0,
                                VALUE_MAR = dataOrgCode.Sum(x => x.VALUE_MAR) ?? 0,
                                VALUE_APR = dataOrgCode.Sum(x => x.VALUE_APR) ?? 0,
                                VALUE_MAY = dataOrgCode.Sum(x => x.VALUE_MAY) ?? 0,
                                VALUE_JUN = dataOrgCode.Sum(x => x.VALUE_JUN) ?? 0,
                                VALUE_JUL = dataOrgCode.Sum(x => x.VALUE_JUL) ?? 0,
                                VALUE_AUG = dataOrgCode.Sum(x => x.VALUE_AUG) ?? 0,
                                VALUE_SEP = dataOrgCode.Sum(x => x.VALUE_SEP) ?? 0,
                                VALUE_OCT = dataOrgCode.Sum(x => x.VALUE_OCT) ?? 0,
                                VALUE_NOV = dataOrgCode.Sum(x => x.VALUE_NOV) ?? 0,
                                VALUE_DEC = dataOrgCode.Sum(x => x.VALUE_DEC) ?? 0,
                                VALUE_SUM_YEAR = dataOrgCode.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                VALUE_SUM_YEAR_PREVENTIVE = dataOrgCode.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,


                            });
                        }
                    }

                }
            }
            return lstResult;
        }

        private IList<T_MD_COST_PL_ELEMENT> SummaryCenter(IList<T_BP_COST_PL_DATA> plDataCostElements, string centerCode, int year,
                                                          bool? isHasValue = null)
        {

            // get all cost elements
            var allCostElement = UnitOfWork.Repository<CostPLElementRepo>().GetManyByExpression(x => x.TIME_YEAR == year);
            // get all child
            var childrenCodes = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);
            // list store pure items to send to view
            var pureLstItems = new List<T_MD_COST_PL_ELEMENT>();

            // lookup cost elements by parent code
            var lookupElements = plDataCostElements.GroupBy(x => x.COST_PL_ELEMENT_CODE)
                .Select(x => x.First())
                .ToLookup(x => x.CostElement.PARENT_CODE);

            var childrenCode = GetListOfChildrenCenter(centerCode)
                .Select(x => x.CODE)
                .ToList();
            childrenCode.Add(centerCode);

            foreach (var code in lookupElements.Select(l => l.Key))
            {
                // set level start by 0
                var level = 0;
                // temp list
                var lst = new List<T_MD_COST_PL_ELEMENT>();
                // add all leaf to temp list item with child = true
                lst.AddRange(from item in lookupElements[code]
                             let orgCode = childrenCodes.Contains(item.ORG_CODE) || item.ORG_CODE == centerCode ? item.ORG_CODE : item.COST_CENTER_CODE
                             select new T_MD_COST_PL_ELEMENT
                             {
                                 VERSION = item.VERSION,
                                 CENTER_CODE = item.COST_CENTER_CODE,
                                 C_ORDER = item.CostElement.C_ORDER,
                                 NAME = item.CostElement.NAME,
                                 PARENT_CODE = item.CostElement.PARENT_CODE,
                                 CODE = item.CostElement.CODE,
                                 LEVEL = 0,
                                 IS_GROUP = false,
                                 ORG_NAME = childrenCode.Contains(item.ORG_CODE) ? item.Organize.NAME : item.CostCenter.NAME,
                                 TEMPLATE_CODE = item.TEMPLATE_CODE,
                                 ORG_CODE = orgCode,
                                 IsChildren = true
                             });
                // init parent code = code
                var parentCode = code;

                // find all hierachical parents 
                while (!string.IsNullOrEmpty(parentCode))
                {
                    // increasing level by 1
                    level++;
                    // find parents to add into list
                    var element = allCostElement.FirstOrDefault(x => x.CODE == parentCode);
                    if (element != null)
                    {
                        // if find parent in all cost element
                        parentCode = element.PARENT_CODE;
                        element.CENTER_CODE = centerCode;
                        element.LEVEL = level;
                        element.IS_GROUP = true;
                        element.TEMPLATE_CODE = lookupElements[code].FirstOrDefault().TEMPLATE_CODE;
                        element.ORG_CODE = lookupElements[code].FirstOrDefault().ORG_CODE;
                        lst.Add((T_MD_COST_PL_ELEMENT)element.CloneObject());     // must to clone to other object because it reference to other center
                    }
                    else
                    {
                        // it mean this element is the root so break of loop
                        break;
                    }
                }

                // subtract level so 0 level is root and higher level is child
                lst.ForEach(x => x.LEVEL = level - x.LEVEL);
                pureLstItems.AddRange(lst);
            }

            // calculate data for all pure list item
            var sum = GetSumDescendants(plDataCostElements, pureLstItems, parentId: string.Empty).Distinct().ToList();
            if (isHasValue.HasValue)
            {
                if (isHasValue.Value)
                {
                    return sum.Where(x => x.Values.Sum() > 0).ToList();
                }
                else
                {
                    return sum.Where(x => x.Values.Sum() == 0 && !x.IS_GROUP).ToList();
                }
            }
            else
            {
                return sum;
            }

        }

        /// <summary>
        /// Calculate data for list cost element
        /// </summary>
        /// <param name="costPlData">List cost pl data containt value</param>
        /// <param name="pureItems">List of cost element want to calculate</param>
        /// <param name="parentId">Parent id want to start. Empty for root</param>
        /// <returns>Returns list of cost element has calculated data</returns>
        private IEnumerable<T_MD_COST_PL_ELEMENT> GetSumDescendants(
            IList<T_BP_COST_PL_DATA> costPlData,
            IList<T_MD_COST_PL_ELEMENT> pureItems,
            string parentId)
        {
            var lstResult = new List<T_MD_COST_PL_ELEMENT>
            {
                // set tổng năm
                // tạo element tổng
                new T_MD_COST_PL_ELEMENT
                {
                    NAME = "TỔNG CỘNG",
                    LEVEL = 0,
                    PARENT_CODE = null,
                    IS_GROUP = true,
                    IsChildren = false,
                    C_ORDER = 0,
                    CODE = string.Empty,
                    Values = new decimal[14]
                    {
                        costPlData.Sum(x => x.VALUE_JAN) ?? 0,
                        costPlData.Sum(x => x.VALUE_FEB) ?? 0,
                        costPlData.Sum(x => x.VALUE_MAR) ?? 0,
                        costPlData.Sum(x => x.VALUE_APR) ?? 0,
                        costPlData.Sum(x => x.VALUE_MAY) ?? 0,
                        costPlData.Sum(x => x.VALUE_JUN) ?? 0,
                        costPlData.Sum(x => x.VALUE_JUL) ?? 0,
                        costPlData.Sum(x => x.VALUE_AUG) ?? 0,
                        costPlData.Sum(x => x.VALUE_SEP) ?? 0,
                        costPlData.Sum(x => x.VALUE_OCT) ?? 0,
                        costPlData.Sum(x => x.VALUE_NOV) ?? 0,
                        costPlData.Sum(x => x.VALUE_DEC) ?? 0,
                        costPlData.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                        costPlData.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                    }
                }
            };
            var lookup = pureItems.ToLookup(i => i.PARENT_CODE);
            Queue<T_MD_COST_PL_ELEMENT> st = new Queue<T_MD_COST_PL_ELEMENT>(lookup[parentId]);
            while (st.Count > 0)
            {
                // get first item in queue
                var item = st.Dequeue();
                // variable to check should return item or not
                bool shouldReturn = true;
                // lst to store children of item which have children
                var lstCostElements = new List<T_MD_COST_PL_ELEMENT>();
                // loop through items which have parent id = item id
                foreach (var i in lookup[item.CODE])
                {
                    if (lookup[i.CODE].Count() > 0)
                    {
                        shouldReturn = false;
                        lstCostElements.Add(i);
                        st.Enqueue(i);
                    }
                    else
                    {
                        if (i.HasAssignValue)
                        {
                            // if i is total of its child
                            // add total to parent
                            for (int j = 0; j < item.Values.Length; j++)
                            {
                                item.Values[j] += i.Values[j];
                            }
                        }
                        else
                        {
                            // if i does not count total yet
                            // get its datatree then add to total

                            var treeData = costPlData.Where(x => x.COST_PL_ELEMENT_CODE.Equals(i.CODE));
                            if (treeData != null && treeData.Count() > 0)
                            {
                                item.Values[0] += treeData.Sum(x => x.VALUE_JAN) ?? 0;
                                item.Values[1] += treeData.Sum(x => x.VALUE_FEB) ?? 0;
                                item.Values[2] += treeData.Sum(x => x.VALUE_MAR) ?? 0;
                                item.Values[3] += treeData.Sum(x => x.VALUE_APR) ?? 0;
                                item.Values[4] += treeData.Sum(x => x.VALUE_MAY) ?? 0;
                                item.Values[5] += treeData.Sum(x => x.VALUE_JUN) ?? 0;
                                item.Values[6] += treeData.Sum(x => x.VALUE_JUL) ?? 0;
                                item.Values[7] += treeData.Sum(x => x.VALUE_AUG) ?? 0;
                                item.Values[8] += treeData.Sum(x => x.VALUE_SEP) ?? 0;
                                item.Values[9] += treeData.Sum(x => x.VALUE_OCT) ?? 0;
                                item.Values[10] += treeData.Sum(x => x.VALUE_NOV) ?? 0;
                                item.Values[11] += treeData.Sum(x => x.VALUE_DEC) ?? 0;
                                item.Values[12] += treeData.Sum(x => x.VALUE_SUM_YEAR) ?? 0;
                                item.Values[13] += treeData.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0;

                                item.HasAssignValue = true;

                                foreach (var d in treeData)
                                {
                                    var values = new decimal[14];
                                    values[0] = d.VALUE_JAN ?? 0;
                                    values[1] = d.VALUE_FEB ?? 0;
                                    values[2] = d.VALUE_MAR ?? 0;
                                    values[3] = d.VALUE_APR ?? 0;
                                    values[4] = d.VALUE_MAY ?? 0;
                                    values[5] = d.VALUE_JUN ?? 0;
                                    values[6] = d.VALUE_JUL ?? 0;
                                    values[7] = d.VALUE_AUG ?? 0;
                                    values[8] = d.VALUE_SEP ?? 0;
                                    values[9] = d.VALUE_OCT ?? 0;
                                    values[10] = d.VALUE_NOV ?? 0;
                                    values[11] = d.VALUE_DEC ?? 0;
                                    values[12] = d.VALUE_SUM_YEAR ?? 0;
                                    values[13] = d.VALUE_SUM_YEAR_PREVENTIVE ?? 0;
                                    i.Values = values;

                                    var clone = (T_MD_COST_PL_ELEMENT)i.Clone();
                                    lstResult.Add(clone);
                                    //yield return clone;
                                }
                            }
                        }
                    }
                }

                // remove all child of item
                // include children have child
                lookup = lookup
                    .Where(x => x.Key != item.CODE)
                    .SelectMany(x => x)
                    .ToLookup(l => l.PARENT_CODE);
                if (shouldReturn)
                {
                    if (item.PARENT_CODE == parentId && !item.IS_GROUP)
                    {
                        var data = costPlData.Where(x => x.COST_PL_ELEMENT_CODE == item.CODE && x.COST_CENTER_CODE == item.CENTER_CODE);
                        if (data != null)
                        {
                            item.Values = new decimal[14]
                            {
                                data.Sum(x => x.VALUE_JAN) ?? 0,
                                data.Sum(x => x.VALUE_FEB) ?? 0,
                                data.Sum(x => x.VALUE_MAR) ?? 0,
                                data.Sum(x => x.VALUE_APR) ?? 0,
                                data.Sum(x => x.VALUE_MAY) ?? 0,
                                data.Sum(x => x.VALUE_JUN) ?? 0,
                                data.Sum(x => x.VALUE_JUL) ?? 0,
                                data.Sum(x => x.VALUE_AUG) ?? 0,
                                data.Sum(x => x.VALUE_SEP) ?? 0,
                                data.Sum(x => x.VALUE_OCT) ?? 0,
                                data.Sum(x => x.VALUE_NOV) ?? 0,
                                data.Sum(x => x.VALUE_DEC) ?? 0,
                                data.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                data.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                            };

                        }
                    }
                    lstResult.Add(item);
                }
                else
                {
                    // add children of item which have chilren to lookup 
                    if (lstCostElements.Count > 0)
                    {
                        lookup = lookup
                            .SelectMany(l => l)
                            .Concat(lstCostElements)
                            .ToLookup(x => x.PARENT_CODE);
                    }
                    // re-enqueue item to queue
                    st.Enqueue(item);
                }
            }
            return lstResult;
        }

        #endregion
        
        public override T_BP_TYPE GetBudgetType()
        {
            return UnitOfWork.Repository<TypeRepo>().GetFirstWithFetch(x => x.OBJECT_TYPE == TemplateObjectType.Department && x.ELEMENT_TYPE == ElementType.ChiPhi && x.BUDGET_TYPE == BudgetType.KinhDoanh);
        }
    }
}
