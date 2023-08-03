using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using SMO.AppCode.Utilities;
using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Core.Entities.BP.REVENUE_CF;
using SMO.Core.Entities.BP.REVENUE_CF.REVENUE_CF_DATA_BASE;
using SMO.Core.Entities.MD;
using SMO.Helper;
using SMO.Models;
using SMO.Repository.Implement.BP;
using SMO.Repository.Implement.BP.REVENUE_CF;
using SMO.Repository.Implement.BP.REVENUE_CF.REVENUE_CF_DATA_BASE;
using SMO.Repository.Implement.MD;
using SMO.Service.BP.REVENUE_CF;
using SMO.Service.Class;
using SMO.Service.Common;
using SMO.ServiceInterface.BP.RevenueCF;

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
    public class RevenueCFService : BaseBPService<T_BP_REVENUE_CF, RevenueCFRepo, T_MD_REVENUE_CF_ELEMENT, T_BP_REVENUE_CF_VERSION, T_BP_REVENUE_CF_HISTORY, RevenueCFHistoryRepo>, IRevenueCFService
    {
        private readonly List<Point> InvalidCellsList;
        private readonly List<string> ListColumnName;
        private readonly List<string> ListColumnNameDataBase;
        private readonly int StartRowData;
        public List<T_BP_REVENUE_CF_HISTORY> ObjListHistory { get; set; }
        public List<T_BP_REVENUE_CF_VERSION> ObjListVersion { get; set; }
        public List<T_BP_REVENUE_CF_SUM_UP_DETAIL> ObjListSumUpHistory { get; set; }
        public RevenueCFService()
        {
            this.StartRowData = 6;
            this.ListColumnName = new List<string> {
                "MÃ CÔNG TY",
                "TÊN CÔNG TY",
                "MÃ DỰ ÁN",
                "TÊN DỰ ÁN",
                "MÃ LOẠI HÌNH",
                "TÊN LOẠI HÌNH KINH DOANH",
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
                "TRUNG BÌNH THÁNG",
                "GHI CHÚ"
            };
            this.ListColumnNameDataBase = new List<string>
            {
                "MÃ CÔNG TY",
                "TÊN CÔNG TY",
                "MÃ DỰ ÁN",
                "TÊN DỰ ÁN",
                "MÃ LOẠI HÌNH",
                "TÊN LOẠI HÌNH KINH DOANH",
                "MÃ KHOẢN MỤC",
                "TÊN KHOẢN MỤC",
                "HÀNG HÓA, DỊCH VỤ",
                "ĐƠN VỊ TÍNH",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THỜI GIAN", "ĐƠN GIÁ", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THÀNH TIỀN",
                "SỐ LƯỢNG", "THÀNH TIỀN",
                "GHI CHÚ"
            };
            this.InvalidCellsList = new List<Point>();
            this.ObjListHistory = new List<T_BP_REVENUE_CF_HISTORY>();
            this.ObjListVersion = new List<T_BP_REVENUE_CF_VERSION>();
            this.ObjListSumUpHistory = new List<T_BP_REVENUE_CF_SUM_UP_DETAIL>();
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
                var historyReview = UnitOfWork.Repository<RevenueCFReviewRepo>()
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
                        ModulType = ModulType.RevenueCF,
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.ChoPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.ChuaTrinhDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.DaPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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

            // update version property
            try
            {
                UnitOfWork.BeginTransaction();
                // update version table
                UnitOfWork.Repository<RevenueCFVersionRepo>().Update(x => x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.VERSION == ObjDetail.VERSION, x => x.IS_DELETED = 1, x => x.UPDATE_BY = currentUser);

                // update main table
                ObjDetail.IS_DELETED = true; CurrentRepository.Update(ObjDetail);

                // create history log
                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.ChoPheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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

                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.TuChoi}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.Repository<RevenueCFReviewRepo>()
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
                var reviewHeader = UnitOfWork.Repository<RevenueCFReviewRepo>()
                    .GetFirstWithFetch(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year && x.DATA_VERSION == version && !x.IS_SUMMARY && x.REVIEW_USER == currentUser);
                if (reviewHeader != null)
                {
                    reviewHeader.IS_END = true;
                    UnitOfWork.Repository<RevenueCFReviewRepo>().Update(reviewHeader);
                }

                var totalReviewUsers = UnitOfWork.Repository<UserReviewRepo>()
                    .GetManyWithFetch(x => x.TIME_YEAR == ObjDetail.TIME_YEAR).Count;

                var totalUsersEndReview = UnitOfWork.Repository<RevenueCFReviewRepo>()
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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

                var review = UnitOfWork.Repository<RevenueCFReviewRepo>()
                    .GetFirstWithFetch(x => x.ORG_CODE == orgCode
                                                && x.TIME_YEAR == year
                                                && x.DATA_VERSION == version
                                                && !x.IS_SUMMARY
                                                && x.REVIEW_USER == currentUser);
                if (review == null)
                {
                    var reviewId = Guid.NewGuid().ToString();
                    UnitOfWork.Repository<RevenueCFReviewRepo>().Create(
                        new T_BP_REVENUE_CF_REVIEW
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
                    var plReviewService = new RevenueCFReviewService();
                    plReviewService.ObjDetail.TIME_YEAR = year;
                    plReviewService.ObjDetail.ORG_CODE = orgCode;
                    var dataCost = plReviewService.SummaryCenterVersion(out IList<T_BP_REVENUE_CF_DATA> detailCostElements);
                    dataCost = dataCost.OrderBy(x => x.C_ORDER)
                                                    .GroupBy(x => x.CODE)
                                                    .Select(x => x.First()).ToList();
                    var elements = plReviewService.PrepareListReview(dataCost);
                    UnitOfWork.Repository<RevenueCFReviewResultRepo>()
                        .Create((from e in elements.Where(x => !x.IS_GROUP)
                                 select new T_BP_REVENUE_CF_REVIEW_RESULT
                                 {
                                     PKID = Guid.NewGuid().ToString(),
                                     REVENUE_CF_ELEMENT_CODE = e.CODE,
                                     HEADER_ID = reviewId,
                                     RESULT = false,
                                     TIME_YEAR = year,
                                     CREATE_BY = currentUser
                                 }).ToList());
                }
                else
                {
                    review.IS_END = true;
                    UnitOfWork.Repository<RevenueCFReviewRepo>().Update(review);
                }

                UnitOfWork.Commit();

                if (totalReviewUsers <= totalUsersEndReview)
                {
                    SendNotify(Activity.AC_KET_THUC_TOAN_BO_THAM_DINH);
                }

                SendNotify(Activity.AC_KET_THUC_THAM_DINH);

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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.TKS_PheDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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
                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = ObjDetail.ORG_CODE,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        NOTES = note,
                        VERSION = ObjDetail.VERSION,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        ACTION = Approve_Action.TuChoiKiemSoat,
                        ACTION_USER = currentUser,
                        ACTION_DATE = DateTime.Now,
                        CREATE_BY = currentUser
                    });

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.TKS_TuChoi}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();
                UnitOfWork.Repository<RevenueCFReviewRepo>()
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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

                UnitOfWork.Repository<RevenueCFReviewRepo>()
                    .Update(x => x.ORG_CODE == orgCode
                                && x.TIME_YEAR == year
                                && x.DATA_VERSION == version
                                && x.IS_SUMMARY, x => x.IS_END = true);

                //TODO: Cập nhật lại code thực thi
                UnitOfWork.GetSession().CreateSQLQuery($"UPDATE T_BP_REVENUE_CF_DATA SET STATUS = '{Approve_Status.TKS_TrinhDuyet}' WHERE ORG_CODE = '{ObjDetail.ORG_CODE}' AND TEMPLATE_CODE = '{ObjDetail.TEMPLATE_CODE}' AND TIME_YEAR = {ObjDetail.TIME_YEAR}").ExecuteUpdate();

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
                var childOrg = GetCenter(childOrgCode);
                var parentOrg = GetCenter(parentOrgCode);
                var template = UnitOfWork.Repository<TemplateRepo>().Get(templateCode);
                var noteParent = $"Yêu cầu đơn vị [{childOrg.NAME}] cập nhật lại kế hoạch của mẫu [{(string.IsNullOrWhiteSpace(templateCode) ? "Dữ liệu tổng hợp" : $"{template.CODE}-{template.NAME}")}]. Lý do: [{comment}]";

                UnitOfWork.BeginTransaction();
                // Tạo history của đơn vị cha
                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                       ModulType = ModulType.RevenueCF,
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
                    UnitOfWork.Repository<RevenueCFReviewRepo>().Update(x => x.DATA_VERSION == ObjDetail.VERSION
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

                UnitOfWork.Repository<RevenueCFHistoryRepo>()
                    .Create(new T_BP_REVENUE_CF_HISTORY()
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
            var listTemplateCode = this.UnitOfWork.Repository<TemplateDetailRevenueCFRepo>().Queryable().Where(
                    x => lstChildOrg.Contains(x.CENTER_CODE) && x.TIME_YEAR == this.ObjDetail.TIME_YEAR
                    && x.TIME_YEAR == (ObjDetail.TIME_YEAR == 0 ? DateTime.Now.Year : ObjDetail.TIME_YEAR)
                ).Select(x => x.TEMPLATE_CODE).Distinct().ToList();
            var findRevenueCF = this.CurrentRepository.Queryable().Where(
                    x => listTemplateCode.Contains(x.TEMPLATE_CODE) && !lstChildOrg.Contains(x.ORG_CODE) && x.TIME_YEAR == this.ObjDetail.TIME_YEAR
                );

            this.ObjList.AddRange(findRevenueCF);
            this.ObjList = this.ObjList.OrderBy(x => x.ORG_CODE).ThenBy(x => x.TEMPLATE_CODE).ToList();
        }

        /// <summary>
        /// Lấy lịch sử tổng hợp của một đơn vị
        /// </summary>
        /// <param name="orgCode"></param>
        public override void GetSumUpHistory(string orgCode, int year = 0, int version = 0)
        {
            var query = UnitOfWork.GetSession().QueryOver<T_BP_REVENUE_CF_SUM_UP_DETAIL>();
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

        public IList<T_MD_REVENUE_CF_ELEMENT> GetDataRevenue(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            out IList<T_BP_REVENUE_CF_DATA> detailRevenueData,
            out bool isDrillDownApply,
            ViewDataCenterModel model)
        {
            isDrillDownApply = model.IS_DRILL_DOWN;
            if (!model.IS_HAS_NOT_VALUE && !model.IS_HAS_VALUE &&
                (!string.IsNullOrEmpty(model.TEMPLATE_CODE) || model.VERSION == null || model.VERSION.Value == -1))
            {
                detailRevenueData = null;
                detailRevenueElements = null;
                return null;
            }
            var isHasValue = model.IS_HAS_VALUE ? model.IS_HAS_NOT_VALUE ? (bool?)null : true : false;
            var isParent = !IsLeaf(model.ORG_CODE);
            if (!string.IsNullOrEmpty(model.TEMPLATE_CODE))
            {
                // view template Data
                detailRevenueData = null;
                var lstOrgs = new List<string>();
                if (isParent)
                {
                    lstOrgs.AddRange(GetListOfChildrenCenter(model.ORG_CODE).Select(x => x.CODE));
                }
                else
                {
                    lstOrgs.Add(model.ORG_CODE);
                }
                var elements = GetDataRevenuePreview(out detailRevenueElements, model.TEMPLATE_CODE, lstOrgs, model.YEAR, model.VERSION, isHasValue);
                var sumElements = new T_MD_REVENUE_CF_ELEMENT
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
                    Values = new decimal[12]
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
                detailRevenueElements = null;
                // disabled drill down
                isDrillDownApply = false;
                return SummarySumUpCenter(out detailRevenueData, model.YEAR, model.ORG_CODE, null, isHasValue, templateId: null);
            }
            else
            {
                // xem dữ liệu được tổng hợp cho đơn vị
                detailRevenueElements = null;
                return SummaryCenterVersion(out detailRevenueData, model.ORG_CODE, model.YEAR, model.VERSION, model.IS_DRILL_DOWN);
            }
        }

        public override IList<T_MD_REVENUE_CF_ELEMENT> GetDetailPreviewSumUp(string centerCode, string elementCode, int year)
        {
            var plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                .GetCFDataByCenterCode(null, new List<string> { centerCode }, year, null, null);

            return plDataRevenueElements.Where(x => x.REVENUE_CF_ELEMENT_CODE == elementCode)
                .Select(x => (T_MD_REVENUE_CF_ELEMENT)x)
                .OrderBy(x => x.C_ORDER)
                .ToList();
        }

        private IList<T_MD_REVENUE_CF_ELEMENT> GetDataRevenuePreview(
            out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            List<string> lstOrgs,
            int year,
            int? version,
            bool? isHasValue)
        {
            if (lstOrgs is null)
            {
                throw new ArgumentNullException(nameof(lstOrgs));
            }

            detailRevenueElements = new List<T_MD_TEMPLATE_DETAIL_REVENUE_CF>();
            var lstElements = new List<T_MD_REVENUE_CF_ELEMENT>();
            foreach (var orgCode in lstOrgs)
            {
                var elements = GetDataRevenuePreview(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detail, templateId, orgCode, year, version, isHasValue);
                detailRevenueElements = SummaryElement(detailRevenueElements, detail);
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
        private IList<T_MD_REVENUE_CF_ELEMENT> SummaryUpElement(IList<T_MD_REVENUE_CF_ELEMENT> lst1, IList<T_MD_REVENUE_CF_ELEMENT> lst2)
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
                    for (int i = 0; i < 12; i++)
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

        public override T_BP_REVENUE_CF_VERSION GetHeader(string templateCode, string centerCode, int year, int? version)
        {
            templateCode = templateCode ?? string.Empty;
            if (version == null)
            {
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                .GetManyByExpression(x =>
                x.TIME_YEAR == year &&
                x.ORG_CODE == centerCode &&
                x.TEMPLATE_CODE == templateCode).OrderByDescending(x => x.VERSION).FirstOrDefault();
            }
            else
            {
                if (!string.IsNullOrEmpty(templateCode))
                {
                    return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetFirstByExpression(x =>
                    x.TIME_YEAR == year &&
                    x.VERSION == version &&
                    x.TEMPLATE_CODE == templateCode);
                }
                else
                {
                    return UnitOfWork.Repository<RevenueCFVersionRepo>()
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
            ObjListVersion = UnitOfWork.Repository<RevenueCFVersionRepo>()
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
            var query = this.UnitOfWork.GetSession().QueryOver<T_BP_REVENUE_CF_HISTORY>();
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
                this.ObjListHistory = UnitOfWork.Repository<RevenueCFHistoryRepo>().GetManyByExpression(
                    x => x.ORG_CODE == orgCode &&
                    x.TEMPLATE_CODE == templateId &&
                    x.TIME_YEAR == year.Value
                ).ToList();
            }
            else
            {
                this.ObjListHistory = UnitOfWork.Repository<RevenueCFHistoryRepo>().GetManyByExpression(
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


            var lstDetailTemplate = this.UnitOfWork.Repository<TemplateDetailRevenueCFRepo>().GetManyByExpression(x => x.TIME_YEAR == this.ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == this.ObjDetail.TEMPLATE_CODE);

            var lstElement = this.UnitOfWork.Repository<RevenueCFElementRepo>().GetManyByExpression(x => x.TIME_YEAR == this.ObjDetail.TIME_YEAR);

            // Kiểm tra mẫu này đã được thiết kế cho năm ngân sách đang chọn chưa
            if (lstDetailTemplate.Count == 0)
            {
                this.State = false;
                this.ErrorMessage = $"Mẫu khai báo [{this.ObjDetail.TEMPLATE_CODE}] năm [{this.ObjDetail.TIME_YEAR}] chưa định nghĩa các khoản mục!";
                return;
            }

            // Kiểm tra revenue_center có thuộc mẫu thiết kế không
            // Có để trống dữ liệu tại 2 cột Mã đơn vị, và mã khoản mục không
            var lstOrgInTemplate = lstDetailTemplate.Select(x => x.CENTER_CODE).Distinct().ToList();
            for (int i = this.StartRowData; i < dataTable.Rows.Count; i++)
            {
                var orgCodeInExcel = dataTable.Rows[i][4].ToString().Trim();
                var elementCodeInExcel = dataTable.Rows[i][6].ToString().Trim();
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
                this.ConvertData(dataTable, lstElement.ToList(), startColumn: 10, endColumn: ListColumnNameDataBase.Count - 5, isDataBase);
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
                        .FirstOrDefault(x => x.CENTER_CODE == dataTable.Rows[i][4].ToString().Trim() &&
                        x.ELEMENT_CODE == dataTable.Rows[i][6].ToString().Trim());
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
                this.ConvertData(dataTable, lstElement.ToList(), startColumn: 8, endColumn: ListColumnName.Count - 3, isDataBase);
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
        public override void ConvertData(DataTable dataTable, List<T_MD_REVENUE_CF_ELEMENT> lstElement, int startColumn, int endColumn, bool isDataBase)
        {
            var iActualRow = dataTable.Rows.Count;
            for (int i = this.StartRowData; i < iActualRow; i++)
            {
                for (int j = startColumn; j < endColumn; j++)
                {
                    if (isDataBase && (j - startColumn) % 4 == 1)
                    {
                        // ignore column time 
                        continue;
                    }
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
                var elementCodeInExcel = dataTable.Rows[i][6].ToString().Trim();
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
                            if ((isDataBase && (j - startColumn) % 4 == 3 && j <= endColumn)
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
            var orgCode = ProfileUtilities.User.ORGANIZE_CODE;
            var currentUserName = ProfileUtilities.User?.USER_NAME;

            // Lưu file vào database
            var fileStream = new FILE_STREAM()
            {
                PKID = Guid.NewGuid().ToString(),
                FILESTREAM = request.Files[0]
            };
            FileStreamService.InsertFile(fileStream);

            // Xác định version dữ liệu
            var revenueCFCurrent = CurrentRepository.Queryable().FirstOrDefault(x => x.ORG_CODE == orgCode
                && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE);

            if (revenueCFCurrent != null && !(revenueCFCurrent.STATUS == Approve_Status.TuChoi || revenueCFCurrent.STATUS == Approve_Status.ChuaTrinhDuyet))
            {
                this.State = false;
                this.ErrorMessage = "Mẫu khai báo này không ở trạng thái Từ chối hoặc Chưa trình duyệt!";
                return;
            }

            var versionNext = 1;
            if (revenueCFCurrent != null)
            {
                versionNext = revenueCFCurrent.VERSION + 1;
            }

            // Lấy dữ liệu của version hiện tại
            var dataCurrent = new List<T_BP_REVENUE_CF_DATA>();
            if (revenueCFCurrent != null)
            {
                dataCurrent = UnitOfWork.Repository<RevenueCFDataRepo>().Queryable().Where(x => x.ORG_CODE == orgCode
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE).ToList();
            }

            //Insert dữ liệu
            try
            {
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
                if (revenueCFCurrent != null)
                {
                    // Cập nhật next version vào bảng chính
                    revenueCFCurrent.VERSION = versionNext;
                    revenueCFCurrent.IS_DELETED = false;
                    CurrentRepository.Update(revenueCFCurrent);
                }
                else
                {
                    // Tạo mới bản ghi revenue pl
                    CurrentRepository.Create(new T_BP_REVENUE_CF()
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
                        CREATE_BY = currentUserName
                    });
                }

                // Đưa next version vào bảng log
                UnitOfWork.Repository<RevenueCFVersionRepo>().Create(new T_BP_REVENUE_CF_VERSION()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    FILE_ID = fileStream.PKID,
                    CREATE_BY = currentUserName
                });

                // Tạo mới bản ghi log trạng thái
                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = orgCode,
                    TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                    VERSION = versionNext,
                    TIME_YEAR = ObjDetail.TIME_YEAR,
                    ACTION = Approve_Action.NhapDuLieu,
                    ACTION_DATE = DateTime.Now,
                    ACTION_USER = currentUserName,
                    CREATE_BY = currentUserName
                });


                // Insert data vào history
                foreach (var item in dataCurrent)
                {
                    var revenueDataHis = (T_BP_REVENUE_CF_DATA_HISTORY)item;
                    UnitOfWork.Repository<RevenueCFDataHistoryRepo>().Create(revenueDataHis);
                    UnitOfWork.Repository<RevenueCFDataRepo>().Delete(item);
                }


                // Insert dữ liệu vào bảng data
                for (int i = this.StartRowData; i < actualRows; i++)
                {
                    var revenueData = new T_BP_REVENUE_CF_DATA()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        PROFIT_CENTER_CODE = tableData.Rows[i][4].ToString().Trim(),
                        COMPANY_CODE = tableData.Rows[i][0].ToString().Trim(),
                        PROJECT_CODE = tableData.Rows[i][2].ToString().Trim(),
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        VERSION = versionNext,
                        REVENUE_CF_ELEMENT_CODE = tableData.Rows[i][6].ToString().Trim(),
                        VALUE_JAN = tableData.Rows[i][8] as decimal?,
                        VALUE_FEB = tableData.Rows[i][9] as decimal?,
                        VALUE_MAR = tableData.Rows[i][10] as decimal?,
                        VALUE_APR = tableData.Rows[i][11] as decimal?,
                        VALUE_MAY = tableData.Rows[i][12] as decimal?,
                        VALUE_JUN = tableData.Rows[i][13] as decimal?,
                        VALUE_JUL = tableData.Rows[i][14] as decimal?,
                        VALUE_AUG = tableData.Rows[i][15] as decimal?,
                        VALUE_SEP = tableData.Rows[i][16] as decimal?,
                        VALUE_OCT = tableData.Rows[i][17] as decimal?,
                        VALUE_NOV = tableData.Rows[i][18] as decimal?,
                        VALUE_DEC = tableData.Rows[i][19] as decimal?,
                        DESCRIPTION = tableData.Rows[i][22].ToString(),
                        CREATE_BY = currentUserName
                    };
                    UnitOfWork.Repository<RevenueCFDataRepo>().Create(revenueData);
                }
                UnitOfWork.Commit();
                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_NHAP_DU_LIEU,
                        OrgCode = orgCode,
                        TemplateCode = ObjDetail.TEMPLATE_CODE,
                        TimeYear = ObjDetail.TIME_YEAR,
                        ModulType = ModulType.RevenueCF,
                        UserSent = currentUserName
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
            var orgCode = ProfileUtilities.User?.ORGANIZE_CODE;
            var revenueCFDataRepo = UnitOfWork.Repository<RevenueCFDataRepo>();
            var revenueCFDataBaseRepo = UnitOfWork.Repository<RevenueCFDataBaseRepo>();
            var currentUser = ProfileUtilities.User?.USER_NAME;

            // Lưu file vào database
            var fileStream = new FILE_STREAM()
            {
                PKID = Guid.NewGuid().ToString(),
                FILESTREAM = request.Files[0]
            };
            FileStreamService.InsertFile(fileStream);

            // Xác định version dữ liệu
            var revenueCFCurrent = GetFirstWithFetch(x => x.ORG_CODE == orgCode
                && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE);

            if (revenueCFCurrent != null && !(revenueCFCurrent.STATUS == Approve_Status.TuChoi || revenueCFCurrent.STATUS == Approve_Status.ChuaTrinhDuyet))
            {
                this.State = false;
                this.ErrorMessage = "Mẫu khai báo này không ở trạng thái Từ chối hoặc Chưa trình duyệt!";
                return;
            }

            var versionNext = 1;
            if (revenueCFCurrent != null)
            {
                versionNext = revenueCFCurrent.VERSION + 1;
            }

            // Lấy dữ liệu của version hiện tại
            var dataCurrent = new List<T_BP_REVENUE_CF_DATA>();
            var dataCurrentBase = new List<T_BP_REVENUE_CF_DATA_BASE>();
            if (revenueCFCurrent != null)
            {
                dataCurrent = revenueCFDataRepo.GetManyWithFetch(x => x.ORG_CODE == orgCode
                    && x.TIME_YEAR == ObjDetail.TIME_YEAR && x.TEMPLATE_CODE == ObjDetail.TEMPLATE_CODE).ToList();

                dataCurrentBase = revenueCFDataBaseRepo.GetManyWithFetch(x => x.ORG_CODE == orgCode
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
                if (revenueCFCurrent != null)
                {
                    // Cập nhật next version vào bảng chính
                    revenueCFCurrent.VERSION = versionNext;
                    revenueCFCurrent.IS_DELETED = false;
                    CurrentRepository.Update(revenueCFCurrent);
                }
                else
                {
                    // Tạo mới bản ghi revenue pl
                    CurrentRepository.Create(new T_BP_REVENUE_CF()
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
                UnitOfWork.Repository<RevenueCFVersionRepo>().Create(new T_BP_REVENUE_CF_VERSION()
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
                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY()
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
                    var revenueDataHis = (T_BP_REVENUE_CF_DATA_HISTORY)item;
                    UnitOfWork.Repository<RevenueCFDataHistoryRepo>().Create(revenueDataHis);
                    revenueCFDataRepo.Delete(item);
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
                    CenterCode = x[4].ToString().Trim(),
                    ElementCode = x[6].ToString().Trim(),
                    CompanyCode = x[0].ToString().Trim(),
                    ProjectCode = x[2].ToString().Trim(),
                });

                foreach (var key in lookUp.Select(x => x.Key))
                {

                    var revenueData = new T_BP_REVENUE_CF_DATA()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        PROFIT_CENTER_CODE = key.CenterCode,
                        COMPANY_CODE = key.CompanyCode,
                        PROJECT_CODE = key.ProjectCode,
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        VERSION = versionNext,
                        STATUS = Approve_Status.ChuaTrinhDuyet,
                        REVENUE_CF_ELEMENT_CODE = key.ElementCode,
                        VALUE_JAN = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[13].ToString())),
                        VALUE_FEB = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[17].ToString())),
                        VALUE_MAR = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[21].ToString())),
                        VALUE_APR = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[25].ToString())),
                        VALUE_MAY = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[29].ToString())),
                        VALUE_JUN = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[33].ToString())),
                        VALUE_JUL = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[37].ToString())),
                        VALUE_AUG = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[41].ToString())),
                        VALUE_SEP = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[45].ToString())),
                        VALUE_OCT = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[49].ToString())),
                        VALUE_NOV = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[53].ToString())),
                        VALUE_DEC = lookUp[key].Sum(x => UtilsCore.StringToDecimal(x[57].ToString())),
                        CREATE_BY = currentUser
                    };
                    revenueCFDataRepo.Create(revenueData);
                }

                // Insert data vào base data history
                var lstBaseDataHistory = (from x in dataCurrentBase
                                          select (T_BP_REVENUE_CF_DATA_BASE_HISTORY)x).ToList();
                UnitOfWork.Repository<RevenueCFDataBaseHistoryRepo>().Create(lstObj: lstBaseDataHistory);
                revenueCFDataBaseRepo.Delete(dataCurrentBase);

                // Insert dữ liệu vào bảng data
                for (int i = this.StartRowData; i < actualRows; i++)
                {
                    var centerCode = tableData.Rows[i][4].ToString().Trim();

                    var j = 10;
                    var revenueData = new T_BP_REVENUE_CF_DATA_BASE()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        ORG_CODE = orgCode,
                        PROFIT_CENTER_CODE = centerCode,
                        PROJECT_CODE = tableData.Rows[i][2].ToString().Trim(),
                        COMPANY_CODE = tableData.Rows[i][0].ToString().Trim(),
                        TEMPLATE_CODE = ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = ObjDetail.TIME_YEAR,
                        VERSION = versionNext,
                        REVENUE_CF_ELEMENT_CODE = tableData.Rows[i][6].ToString().Trim(),
                        MATERIAL = tableData.Rows[i][8].ToString().Trim(),
                        UNIT = tableData.Rows[i][9].ToString().Trim(),

                        QUANTITY_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M1 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M1 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M2 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M2 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M3 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M3 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M4 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M4 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M5 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M5 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M6 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M6 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M7 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M7 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M8 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M8 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M9 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M9 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M10 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M10 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M11 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M11 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        QUANTITY_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        TIME_M12 = tableData.Rows[i][j++].ToString().Trim(),
                        PRICE_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),
                        AMOUNT_M12 = UtilsCore.StringToDecimal(tableData.Rows[i][j++].ToString().Trim()),

                        DESCRIPTION = tableData.Rows[i][ListColumnNameDataBase.Count - 1].ToString().Trim(),

                        CREATE_BY = currentUser
                    };
                    revenueData.AMOUNT_YEAR = revenueData.AMOUNT_M1 + revenueData.AMOUNT_M2 + revenueData.AMOUNT_M3 + revenueData.AMOUNT_M4 + revenueData.AMOUNT_M5 + revenueData.AMOUNT_M6 + revenueData.AMOUNT_M7 + revenueData.AMOUNT_M8 + revenueData.AMOUNT_M9 + revenueData.AMOUNT_M10 + revenueData.AMOUNT_M11 + revenueData.AMOUNT_M12;

                    revenueCFDataBaseRepo.Create(revenueData);
                }
                UnitOfWork.Commit();

                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_NHAP_DU_LIEU,
                        OrgCode = orgCode,
                        TemplateCode = ObjDetail.TEMPLATE_CODE,
                        TimeYear = ObjDetail.TIME_YEAR,
                        ModulType = ModulType.RevenueCF,
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

        public override IList<T_BP_REVENUE_CF_VERSION> GetVersions(string orgCode, string templateId, int year)
        {
            templateId = templateId ?? string.Empty;
            var lstVersions = GetVersionsNumber(orgCode, templateId, year);
            return UnitOfWork.Repository<RevenueCFVersionRepo>()
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
            return UnitOfWork.Repository<RevenueCFVersionRepo>()
                .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TEMPLATE_CODE != string.Empty)
                .Select(x => x.TEMPLATE_CODE)
                .Distinct()
                .ToList();
        }

        private IList<string> GetTemplateData(List<string> lstOrgCodes, int? year = null)
        {
            return UnitOfWork.Repository<RevenueCFDataRepo>()
                .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE != string.Empty && (!year.HasValue || x.TIME_YEAR == year.Value))
                .Select(x => x.TEMPLATE_CODE)
                .Distinct()
                .ToList();
        }

        private IList<string> GetTemplateDataHistory(List<string> lstOrgCodes, int? year = null)
        {
            return UnitOfWork.Repository<RevenueCFDataHistoryRepo>()
                .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE != string.Empty && (!year.HasValue || x.TIME_YEAR == year.Value))
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
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
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
                return UnitOfWork.Repository<RevenueCFDataHistoryRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFDataHistoryRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE == templateId)
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
        }

        private IList<int> GetDataYears(List<string> lstOrgCodes, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<RevenueCFDataRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE))
                    .Select(x => x.TIME_YEAR)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFDataRepo>()
                    .GetManyByExpression(x => lstOrgCodes.Contains(x.ORG_CODE) && x.TEMPLATE_CODE == templateId)
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
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == year)
                    .Select(x => x.VERSION)
                    .OrderByDescending(x => x)
                    .ToList();
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetManyWithFetch(x => x.TEMPLATE_CODE == templateId && x.TIME_YEAR == year && x.ORG_CODE == orgCode)
                    .Select(x => x.VERSION)
                    .OrderByDescending(x => x)
                    .ToList();
            }

            //ObjDetail.ORG_CODE = orgCode;
            //var lstOrgCodes = new List<string>() { orgCode };
            //var lstTemplateCurrentUserSelfUpload = new List<int>();
            //templateId = templateId ?? string.Empty;
            //if (IsLeaf())
            //{
            //    // is leaf
            //    lstOrgCodes.Add(orgCode);
            //}
            //else
            //{
            //    // is group
            //    // get all child
            //    if (!string.IsNullOrEmpty(templateId))
            //    {
            //        lstOrgCodes.AddRange(GetListOfChildrenCenter(orgCode).Select(x => x.CODE));
            //    }
            //}

            //lstTemplateCurrentUserSelfUpload.AddRange(GetVersionCurrentUserSelfUpload(orgCode, templateId, year));
            //return GetVersionTemplateData(lstOrgCodes, templateId, year)
            //    .Union(GetVersionTemplateDataHistory(lstOrgCodes, templateId, year))
            //    .Union(lstTemplateCurrentUserSelfUpload)
            //    .OrderByDescending(x => x)
            //    .ToList();
        }

        #endregion

        public override IList<int> GetTemplateVersion(string templateId, string centerCode, int year)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                return UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
                    .GetManyByExpression(x => x.ORG_CODE == centerCode && x.TIME_YEAR == year)
                    .Select(x => x.DATA_VERSION).Distinct().OrderByDescending(x => x).ToList();
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetManyByExpression(x => x.TEMPLATE_CODE == templateId && x.ORG_CODE == centerCode && x.TIME_YEAR == year)
                    .Select(x => x.VERSION).Distinct().OrderByDescending(x => x).ToList();
            }
        }

        /// <summary>
        /// generate excel file and store in path
        /// </summary>
        /// <param name="outFileStream"></param>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="year"></param>
        public override void GenerateTemplate(ref MemoryStream outFileStream, string path, string templateId, int year)
        {
            var dataRevenue = PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements, templateId, year, ignoreAuth: true);

            if (dataRevenue.Count == 0 || detailRevenueElements.Count == 0)
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
                templateWorkbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.RevenueCF));
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);

                //Số hàng và số cột hiện tại
                int numRowCur = 0;
                int NUM_CELL = 23;

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
                rowHeader1.Cells[0].SetCellValue(rowHeader1.Cells[0].StringCellValue + $" {template.Organize?.Parent?.NAME.ToUpper()}");

                var rowHeader2 = ReportUtilities.CreateRow(ref sheet, 1, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader2, NUM_CELL);
                rowHeader2.Cells[0].SetCellValue($"{template.Organize.NAME}");
                rowHeader2.Cells[2].SetCellValue(template.TITLE.ToUpper());
                rowHeader2.Cells[11].SetCellValue(rowHeader2.Cells[11].StringCellValue + $" {year}");

                var rowHeader3 = ReportUtilities.CreateRow(ref sheet, 2, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader3, NUM_CELL);
                rowHeader3.Cells[1].SetCellValue(template.CREATE_BY);
                //rowHeader2.Cells[14].SetCellValue(string.Concat(rowHeader2.Cells[14].StringCellValue, " ", template.));
                #endregion

                #region Details

                numRowCur = 6;
                var number = 1;
                var rowHeightDetail = sheet.GetRow(6).Height;
                foreach (var detail in detailRevenueElements.GroupBy(x => x.CENTER_CODE)
                    .Select(x => x.First())
                    .OrderByDescending(x => x.CENTER_CODE))
                {
                    foreach (var item in dataRevenue
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

                        rowCur.Cells[0].SetCellValue(detail.Center.COMPANY_CODE);
                        rowCur.Cells[0].CellStyle = styleCellDetail;

                        rowCur.Cells[1].SetCellValue(detail.Center.Company.NAME);
                        rowCur.Cells[1].CellStyle = styleCellDetail;

                        rowCur.Cells[2].SetCellValue(detail.Center.PROJECT_CODE);
                        rowCur.Cells[2].CellStyle = styleCellDetail;

                        rowCur.Cells[3].SetCellValue(detail.Center.Project.NAME);
                        rowCur.Cells[3].CellStyle = styleCellDetail;

                        rowCur.Cells[4].SetCellValue(detail.CENTER_CODE);
                        rowCur.Cells[4].CellStyle = styleCellDetail;

                        rowCur.Cells[5].SetCellValue(detail.Center.NAME);
                        rowCur.Cells[5].CellStyle = styleCellDetail;

                        rowCur.Cells[6].SetCellValue(item.CODE);
                        rowCur.Cells[6].CellStyle = styleCellDetail;

                        rowCur.Cells[7].SetCellValue($"{space}{item.NAME}");
                        if (item.IS_GROUP)
                        {
                            rowCur.Cells[7].CellStyle = styleCellBold;
                            rowCur.Cells[7].CellStyle.SetFont(fontBold);
                        }
                        else
                        {
                            rowCur.Cells[7].CellStyle = styleCellDetail;
                        }

                        for (int i = 8; i < 20; i++)
                        {
                            rowCur.Cells[i].SetCellValue(string.Empty);
                            rowCur.Cells[i].SetCellType(CellType.Numeric);
                            rowCur.Cells[i].CellStyle = styleCellNumber;

                        }

                        rowCur.Cells[20].SetCellFormula($"SUM(M{numRowCur + 1}:X{numRowCur + 1})");
                        rowCur.Cells[20].CellStyle = styleCellNumberColor;
                        rowCur.Cells[20].SetCellType(CellType.Formula);

                        rowCur.Cells[21].SetCellFormula($"Y{numRowCur + 1}/12");
                        rowCur.Cells[21].CellStyle = styleCellNumberColor;
                        rowCur.Cells[21].SetCellType(CellType.Formula);

                        rowCur.Cells[22].SetCellValue(string.Empty);
                        rowCur.Cells[22].CellStyle = styleCellDetail;

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
            var dataCost = PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailCostElements, templateId, year, ignoreAuth: true);

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
                templateWorkbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.RevenueCF));
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
                rowHeader2.Cells[10].SetCellValue(rowHeader2.Cells[10].StringCellValue + $" {year}");

                var rowHeader3 = ReportUtilities.CreateRow(ref sheet, 2, NUM_CELL);

                ReportUtilities.CreateCell(ref rowHeader3, NUM_CELL);
                rowHeader3.Cells[1].SetCellValue(template.CREATE_BY);
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

                        rowCur.Cells[0].SetCellValue(detail.Center.COMPANY_CODE);
                        rowCur.Cells[0].CellStyle = styleCellDetail;

                        rowCur.Cells[1].SetCellValue(detail.Center.Company.NAME);
                        rowCur.Cells[1].CellStyle = styleCellDetail;

                        rowCur.Cells[2].SetCellValue(detail.Center.PROJECT_CODE);
                        rowCur.Cells[2].CellStyle = styleCellDetail;

                        rowCur.Cells[3].SetCellValue(detail.Center.Project.NAME);
                        rowCur.Cells[3].CellStyle = styleCellDetail;

                        rowCur.Cells[4].SetCellValue(detail.CENTER_CODE);
                        rowCur.Cells[4].CellStyle = styleCellDetail;

                        rowCur.Cells[5].SetCellValue(detail.Center.NAME);
                        rowCur.Cells[5].CellStyle = styleCellDetail;

                        rowCur.Cells[6].SetCellValue(item.CODE);
                        rowCur.Cells[6].CellStyle = styleCellDetail;

                        rowCur.Cells[7].SetCellValue($"{space}{item.NAME}");
                        if (item.IS_GROUP)
                        {
                            rowCur.Cells[7].CellStyle = styleCellBold;
                            rowCur.Cells[7].CellStyle.SetFont(fontBold);
                        }
                        else
                        {
                            rowCur.Cells[7].CellStyle = styleCellDetail;
                        }

                        rowCur.Cells[8].SetCellValue(string.Empty);
                        rowCur.Cells[8].CellStyle = styleCellDetail;

                        rowCur.Cells[9].SetCellValue(string.Empty);
                        rowCur.Cells[9].CellStyle = styleCellDetail;

                        int j = 10;
                        for (; j < NUM_CELL - 1; j++)
                        {
                            rowCur.Cells[j].SetCellValue(string.Empty);
                            if (j < NUM_CELL - 5 && (j - 9) % 4 == 2)
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

        public override IList<NodeDataFlow> BuildDataFlowTree(string orgCode, int year, int? version, int? sumUpVersion)
        {
            var revenueCenterRepo = UnitOfWork.Repository<CostCenterRepo>();

            if (revenueCenterRepo.GetManyByExpression(x => x.PARENT_CODE == orgCode).Count > 0 && version == null)
            {
                // nếu muốn xem tất cả các bản tổng hợp thì để version và sumUpVersion = null ở lần gọi đầu tiên
                if (!sumUpVersion.HasValue)
                {
                    var revenueCFData = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
                        .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year);
                    return (from data in revenueCFData.GroupBy(x => x.SUM_UP_VERSION).Select(x => x.First())
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
                    var revenueCFData = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
                        .GetManyByExpression(x => x.ORG_CODE == orgCode && x.TIME_YEAR == year
                         && x.SUM_UP_VERSION == sumUpVersion.Value);
                    return (from data in revenueCFData.GroupBy(x => x.FROM_ORG_CODE).Select(x => x.First())
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
                var sumupDetails = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
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
        /// Lấy dữ liệu được sum up lên đơn vị theo version và khoản mục
        /// </summary>
        /// <param name="year"></param>
        /// <param name="centerCode"></param>
        /// <param name="elementCode"></param>
        /// <param name="sumUpVersion"></param>
        /// <returns></returns>
        private (IList<T_BP_REVENUE_CF_DATA>, bool) GetDataSumUp(int year, string centerCode, string elementCode, int sumUpVersion)
        {
            var plDataRepo = UnitOfWork.Repository<RevenueCFDataRepo>();
            var revenuePlReviewCommentRepo = UnitOfWork.Repository<RevenueCFReviewCommentRepo>();
            var plVersionRepo = UnitOfWork.Repository<RevenueCFRepo>();
            var plDataHistoryRepo = UnitOfWork.Repository<RevenueCFDataHistoryRepo>();

            var lstDetails = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
                .GetManyByExpression(x => x.ORG_CODE == centerCode &&
                    x.TIME_YEAR == year &&
                    x.SUM_UP_VERSION == sumUpVersion);
            var lookup = lstDetails.ToLookup(x => x.FROM_ORG_CODE);
            var lstChildren = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);

            var isCorp = string.IsNullOrEmpty(GetCenter(centerCode).PARENT_CODE);
            var isLeafCenter = IsLeaf(centerCode);

            var data = new List<T_BP_REVENUE_CF_DATA>();
            foreach (var key in lookup.Select(x => x.Key))
            {
                var revenueCF = new List<T_BP_REVENUE_CF_DATA>();
                var isLeaf = IsLeaf(key);
                if (isLeaf)
                {
                    foreach (var item in lookup[key])
                    {
                        if (lstChildren.Contains(GetTemplate(item.TEMPLATE_CODE).ORG_CODE))
                        {
                            // đơn vị con tự nộp
                            revenueCF.AddRange(plDataRepo.GetCFDataByCenterCode(item.FROM_ORG_CODE, lstChildren.ToList(), year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList());
                        }
                        else
                        {
                            // được nộp hộ
                            var dataFromOrg = plDataRepo.GetCFDataByOrgCode(item.FROM_ORG_CODE, year, item.TEMPLATE_CODE, item.DATA_VERSION).ToList();
                            revenueCF.AddRange(dataFromOrg.Where(x => lstChildren.Contains(x.ORG_CODE)));
                        }
                    }
                }
                else
                {
                    if (isCorp)
                    {
                        foreach (var item in lookup[key])
                        {
                            revenueCF.AddRange(plDataRepo.GetCFDataByOrgCode(key, year, string.Empty, item.DATA_VERSION).ToList());
                        }
                    }
                    else if(isLeafCenter)
                    {
                        revenueCF = plDataRepo.GetCFDataByCenterCode(key, lstChildren.ToList(), year, null, sumUpVersion).ToList();
                    } else
                    {
                        foreach (var item in lookup[key])
                        {
                            revenueCF.AddRange(plDataRepo.GetCFDataByOrgCode(key, year, string.Empty, item.DATA_VERSION).ToList());
                        }
                    }
                }
                data.AddRange(revenueCF.Where(x => x.REVENUE_CF_ELEMENT_CODE == elementCode &&
                lookup[key].Select(y => y.TEMPLATE_CODE).Contains(x.TEMPLATE_CODE)));
                if (data == null || data.Count() == 0)
                {
                    continue;
                }
            }
            return (data, !isLeafCenter);
        }

        /// <summary>
        /// Get list of revenue elements have summed up
        /// </summary>
        /// <param name="centerCode">Cost center code has summed up</param>
        /// <param name="elementCode">Element code want to get detail</param>
        /// <param name="year">Year summed up</param>
        /// <param name="version">Version summed up</param>
        /// <returns>Returns list of revenue elements have summed up</returns>
        public override IEnumerable<T_MD_REVENUE_CF_ELEMENT> GetDetailSumUp(string centerCode, string elementCode, int year, int version, int? sumUpVersion, bool isCountComments, bool? isShowFile = null)
        {
            var plDataRepo = UnitOfWork.Repository<RevenueCFDataRepo>();
            var revenuePlReviewCommentRepo = UnitOfWork.Repository<RevenueCFReviewCommentRepo>();
            var plVersionRepo = UnitOfWork.Repository<RevenueCFRepo>();
            var plDataHistoryRepo = UnitOfWork.Repository<RevenueCFDataHistoryRepo>();

            if (IsLeaf(centerCode))
            {
                var parentCenterCode = GetCenter(centerCode).PARENT_CODE;
                var lstChildren = GetListOfChildrenCenter(parentCenterCode).Select(x => x.CODE);

                var (data, isCorp) = GetDataSumUp(year, parentCenterCode, elementCode, sumUpVersion.Value);
                #region Tìm dựa vào đơn vị tổng hợp
                var lookupData = data.Where(x => x.ORG_CODE == centerCode).ToLookup(x => x.TEMPLATE_CODE);
                foreach (var key in lookupData.Select(x => x.Key))
                {
                    if (lookupData[key].First().Template.IS_BASE)
                    {
                        var isNewestVersion = UnitOfWork.Repository<RevenueCFVersionRepo>()
                            .GetNewestByExpression(x => x.TEMPLATE_CODE == key && x.TIME_YEAR == year,
                            order: x => x.VERSION, isDescending: true)
                            .VERSION == lookupData[key].First().VERSION;
                        foreach (var item in lookupData[key])
                        {
                            var lstElements = lookupData[key].Where(x => x.PROFIT_CENTER_CODE == item.PROFIT_CENTER_CODE);
                            var element = (T_MD_REVENUE_CF_ELEMENT)item;
                            element.IsBase = true;
                            element.Values = new decimal[13]
                            {
                                lstElements.Sum(x => x.VALUE_JAN) ?? 0,
                                lstElements.Sum(x => x.VALUE_FEB) ?? 0,
                                lstElements.Sum(x => x.VALUE_MAR) ?? 0,
                                lstElements.Sum(x => x.VALUE_APR) ?? 0,
                                lstElements.Sum(x => x.VALUE_MAY) ?? 0,
                                lstElements.Sum(x => x.VALUE_JUN) ?? 0,
                                lstElements.Sum(x => x.VALUE_JUL) ?? 0,
                                lstElements.Sum(x => x.VALUE_AUG) ?? 0,
                                lstElements.Sum(x => x.VALUE_SEP) ?? 0,
                                lstElements.Sum(x => x.VALUE_OCT) ?? 0,
                                lstElements.Sum(x => x.VALUE_NOV) ?? 0,
                                lstElements.Sum(x => x.VALUE_DEC) ?? 0,
                                0
                            };
                            element.Values[12] = element.Values.Sum();
                            element.NAME = item.Template.NAME;
                            yield return element;
                        }
                        if (isNewestVersion)
                        {
                            // get data from base
                            var baseData = UnitOfWork.Repository<RevenueCFDataBaseRepo>()
                                .GetManyWithFetch(x => x.REVENUE_CF_ELEMENT_CODE == elementCode && x.ORG_CODE == centerCode && x.TEMPLATE_CODE == key && x.VERSION == lookupData[key].First().VERSION && x.TIME_YEAR == year);
                            foreach (var item in baseData)
                            {
                                yield return (T_MD_REVENUE_CF_ELEMENT)item;
                            }
                        }
                        else
                        {
                            // get data from base history
                            var baseDataHistory = UnitOfWork.Repository<RevenueCFDataBaseHistoryRepo>()
                                .GetManyWithFetch(x => x.REVENUE_CF_ELEMENT_CODE == elementCode && x.ORG_CODE == centerCode && x.TEMPLATE_CODE == key && x.VERSION == lookupData[key].First().VERSION && x.TIME_YEAR == year);

                            foreach (var item in baseDataHistory)
                            {
                                yield return (T_MD_REVENUE_CF_ELEMENT)item;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in lookupData[key])
                        {
                            var element = (T_MD_REVENUE_CF_ELEMENT)item;
                            element.IS_GROUP = false; // count comment is in the review, drill down will show the file base so it still will be group
                            yield return element;
                        }
                    }
                }

                #endregion
            }
            else
            {
                var comments = new List<T_BP_REVENUE_CF_REVIEW_COMMENT>();
                if (isCountComments)
                {
                    comments = revenuePlReviewCommentRepo.GetManyWithFetch(
                            x => x.TIME_YEAR == year &&
                            x.ORG_CODE == GetCorp().CODE &&
                            x.REVENUE_CF_ELEMENT_CODE == elementCode).ToList();
                }
                var (data, isCorp) = GetDataSumUp(year, centerCode, elementCode, version);
                var lookupData = data.ToLookup(x => x.ORG_CODE);
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
                    var element = new T_MD_REVENUE_CF_ELEMENT
                    {
                        ORG_CODE = key,
                        CENTER_CODE = key,
                        CODE = elementCode,
                        IsChildren = IsLeaf(key),
                        //IS_GROUP = IsLeaf(key) ? false : true,
                        IS_GROUP = true,
                        TEMPLATE_CODE = lookupData[key].First().TEMPLATE_CODE,
                        TIME_YEAR = year,
                        VERSION = isCorp ? lookupData[key].First().VERSION : version, // cấp tập đoàn thì sẽ là data_version còn nếu cấp dưới thì sẽ là sumup version
                        ORG_NAME = lookupData[key].First().Organize.NAME,
                        Values = new decimal[13]
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
                                0
                            },
                        NUMBER_COMMENTS = isCountComments ? isChild ?
                        $"{parentComments}" :
                        $"{parentComments + childComments}|{parentComments}" : $"{childComments}|0"
                    };
                    element.Values[12] = element.Values.Sum();
                    yield return element;
                }
            }
        }


        public override IEnumerable<T_MD_REVENUE_CF_ELEMENT> GetDetailSumUpTemplate(string elementCode, int year, int version, string templateCode, string centerCode)
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
                    var baseData = UnitOfWork.Repository<RevenueCFDataBaseRepo>()
                        .GetManyWithFetch(x => x.TEMPLATE_CODE == templateCode
                        && x.TIME_YEAR == year
                        && x.REVENUE_CF_ELEMENT_CODE == elementCode
                        && x.VERSION == version
                        && x.PROFIT_CENTER_CODE == centerCode);
                    return from item in baseData
                           select (T_MD_REVENUE_CF_ELEMENT)item;
                }
                else
                {
                    // old data
                    // get data in table base data history
                    var baseData = UnitOfWork.Repository<RevenueCFDataBaseHistoryRepo>()
                        .GetManyWithFetch(x => x.TEMPLATE_CODE == templateCode
                        && x.TIME_YEAR == year
                        && x.REVENUE_CF_ELEMENT_CODE == elementCode
                        && x.VERSION == version
                        && x.PROFIT_CENTER_CODE == centerCode);
                    return from item in baseData
                           select (T_MD_REVENUE_CF_ELEMENT)item;
                }
            }
        }

        /// <summary>
        /// Get template to ObjDetail first before call this method
        /// Get data revenue elements include detail revenue elements.
        /// </summary>
        /// <param name="detailRevenueElements">out detail revenue elemts</param>
        /// <param name="year">which year of template</param>
        /// <param name="version">which version of template</param>
        /// <returns>Returns list revenue elemts with their parents and their value</returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> GetDataRevenuePreview(
            out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            string centerCode = "",
            int? year = null,
            int? version = null,
            bool? isHasValue = null)
        {
            var pureLstItems = PreparePureList(out detailRevenueElements, templateId, year.Value, centerCode);
            var sum = GetSumDescendants(detailRevenueElements, pureLstItems, parent_id: string.Empty, templateId, year, version).Distinct().ToList();
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

        public IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements, int year)
        {
            // get all revenue elements
            var allRevenueElements = UnitOfWork.Repository<RevenueCFElementRepo>().GetManyByExpression(x => x.TIME_YEAR == year);
            // get revenue elements in details revenue elements
            var revenueElements = from d in detailRevenueElements
                                  select d.Element;
            // lookup revenue elements by center code
            var lookupElementsCenter = detailRevenueElements.ToLookup(x => x.CENTER_CODE);

            var pureLstItems = new List<T_MD_REVENUE_CF_ELEMENT>();
            // loop through all center
            foreach (var ctCode in lookupElementsCenter.Select(l => l.Key))
            {
                // lookup revenue elements
                var lookupElements = lookupElementsCenter[ctCode].ToLookup(x => x.Element.PARENT_CODE);
                foreach (var code in lookupElements.Select(l => l.Key))
                {
                    var level = 0;
                    // temp list
                    var lst = new List<T_MD_REVENUE_CF_ELEMENT>();
                    // add all leaf to temp list item
                    lst.AddRange(from item in lookupElements[code]
                                 select new T_MD_REVENUE_CF_ELEMENT
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
                        var element = allRevenueElements.FirstOrDefault(x => x.CODE == parentCode);
                        if (element != null)
                        {
                            parentCode = element.PARENT_CODE;
                            element.CENTER_CODE = ctCode;
                            element.LEVEL = level;
                            element.IS_GROUP = true;
                            lst.Add((T_MD_REVENUE_CF_ELEMENT)element.CloneObject());     // must to clone to other object because it reference to other center
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
        /// <param name="detailRevenueElements"></param>
        /// <param name="templateId"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            int year,
            string centerCode = "",
            bool ignoreAuth = false)
        {
            templateId = templateId ?? string.Empty;
            var template = GetTemplate(templateId);
            var currentUserCenterCode = ProfileUtilities.User.ORGANIZE_CODE;
            var childOrgRevenues = GetListOfChildrenCenter(currentUserCenterCode).Select(x => x.CODE);

            if (ignoreAuth || childOrgRevenues.Contains(template.ORG_CODE) || currentUserCenterCode.Equals(template.ORG_CODE) || template.ORG_CODE == centerCode)
            {
                detailRevenueElements = UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                    .GetManyWithFetch(x => x.TEMPLATE_CODE.Equals(templateId) && x.TIME_YEAR == year, x => x.Center);
            }
            else
            {
                // get details revenue elements
                if (string.IsNullOrEmpty(centerCode))
                {
                    var lstChildCenterCodes = UnitOfWork.Repository<CostCenterRepo>()
                        .GetManyByExpression(x => x.PARENT_CODE == currentUserCenterCode || x.CODE.Equals(currentUserCenterCode))
                        .Select(x => x.CODE);
                    detailRevenueElements = UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                        .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && lstChildCenterCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year);
                }
                else
                {
                    detailRevenueElements = UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                        .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && x.CENTER_CODE == centerCode && x.TIME_YEAR == year);
                }
            }
            return PreparePureList(detailRevenueElements, year);
        }

        /// <summary>
        /// Xem theo template
        /// </summary>
        /// <param name="detailRevenueElements"></param>
        /// <param name="templateId"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> PreparePureListForTemplate(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            string templateId,
            int year)
        {

            detailRevenueElements = UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(templateId) && x.TIME_YEAR == year);

            return PreparePureList(detailRevenueElements, year);
        }



        /// <summary>
        /// Xem theo center code
        /// </summary>
        /// <param name="detailRevenueElements"></param>
        /// <param name="centerCodes"></param>
        /// <returns></returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> PreparePureList(out IList<T_MD_TEMPLATE_DETAIL_REVENUE_CF> detailRevenueElements,
            IList<string> centerCodes,
            int year)
        {
            // Tìm mẫu nộp hộ
            var listTemplateCodes = this.UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                .GetManyByExpression(x => centerCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year)
                .Select(x => x.TEMPLATE_CODE).Distinct().ToList();
            var findRevenueCF = this.CurrentRepository.GetManyByExpression(
                    x => listTemplateCodes.Contains(x.TEMPLATE_CODE) && !centerCodes.Contains(x.ORG_CODE));
            var lst = new List<T_MD_TEMPLATE_DETAIL_REVENUE_CF>();
            detailRevenueElements = new List<T_MD_TEMPLATE_DETAIL_REVENUE_CF>();
            foreach (var template in listTemplateCodes)
            {
                lst.AddRange(UnitOfWork.Repository<TemplateDetailRevenueCFRepo>()
                    .GetManyByExpression(x => x.TEMPLATE_CODE.Equals(template) && centerCodes.Contains(x.CENTER_CODE) && x.TIME_YEAR == year));
            }

            detailRevenueElements = lst;

            return PreparePureList(detailRevenueElements, year);
        }

        /// <summary>
        /// Sum up data revenue center by center code and year (Tổng hợp dữ liệu tại phòng ban)
        /// </summary>
        /// <param name="revenueCF">Output header revenue pl</param>
        /// <param name="centerCode">Revenue center code want to sum up</param>
        /// <param name="year">Year want to sum up</param>
        public override void SumUpDataCenter(out T_BP_REVENUE_CF_VERSION revenueCF, string centerCode, int year)
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
                revenueCF = null;
                return;
            }

            var lstData = new List<T_BP_REVENUE_CF_DATA>();
            revenueCF = new T_BP_REVENUE_CF_VERSION();

            try
            {
                UnitOfWork.BeginTransaction();
                var sumUpDetailRepo = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>();
                var revenueCFDataRepo = UnitOfWork.Repository<RevenueCFDataRepo>();

                // get list all children centers in revenue center tree
                var lstCostCenters = GetListOfChildrenCenter(centerCode);

                // get all data have approved
                var revenuePlDataApproved = revenueCFDataRepo.GetManyByExpression(x => x.ORG_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE) ||
                x.STATUS == Approve_Status.DaPheDuyet &&
                x.TIME_YEAR == year &&
                lstCostCenters.Any(c => c.CODE.Equals(x.ORG_CODE)));

                if (revenuePlDataApproved.Count == 0)
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
                var lookup = revenuePlDataApproved.ToLookup(x => x.REVENUE_CF_ELEMENT_CODE);
                foreach (var code in lookup.Select(x => x.Key))
                {
                    // TODO: check if all value of months are equal 0
                    if (lookup[code].Count() == 1)
                    {
                        lstData.Add((T_BP_REVENUE_CF_DATA)lookup[code].First().CloneObject());
                    }
                    else
                    {
                        lstData.Add(new T_BP_REVENUE_CF_DATA
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
                            REVENUE_CF_ELEMENT_CODE = lookup[code].First().REVENUE_CF_ELEMENT_CODE
                        });
                    }
                }

                // get current version in revenue pl data
                var newestCF = UnitOfWork.Repository<RevenueCFRepo>()
                    .GetNewestByExpression(x => x.ORG_CODE.Equals(centerCode) && x.TIME_YEAR == year, x => x.CREATE_DATE, true);
                var currentUser = ProfileUtilities.User?.USER_NAME;

                var versionPl = 1;
                if (newestCF != null)
                {
                    newestCF.VERSION++;
                    versionPl = newestCF.VERSION;
                    newestCF.UPDATE_BY = currentUser;
                    newestCF.STATUS = Approve_Status.ChuaTrinhDuyet;
                    CurrentRepository.Update(newestCF);
                }
                else
                {
                    // create header revenue pl
                    CurrentRepository.Create(new T_BP_REVENUE_CF
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

                // insert to revenue pl version
                revenueCF = new T_BP_REVENUE_CF_VERSION
                {
                    PKID = Guid.NewGuid().ToString(),
                    ORG_CODE = centerCode,
                    TEMPLATE_CODE = string.Empty,
                    VERSION = versionPl,
                    TIME_YEAR = year,
                    CREATE_BY = currentUser
                };
                UnitOfWork.Repository<RevenueCFVersionRepo>().Create(revenueCF);

                foreach (var item in lstData)
                {
                    item.ORG_CODE = centerCode;
                    item.PROFIT_CENTER_CODE = string.Empty;
                    item.TEMPLATE_CODE = string.Empty;
                    item.PKID = Guid.NewGuid().ToString();
                    item.VERSION = versionPl;
                    item.TIME_YEAR = year;
                    item.STATUS = Approve_Status.ChuaTrinhDuyet;
                    item.DESCRIPTION = string.Empty;
                    item.CREATE_BY = currentUser;
                }

                // get all revenue pl data current with centercode and year
                var lstRevenuePlDataOldVersion = revenueCFDataRepo
                    .GetManyByExpression(x => x.ORG_CODE.Equals(centerCode) && x.TIME_YEAR == year);

                // delete them from table pl data
                // TODO: chỉnh sửa câu lệnh sql
                _ = UnitOfWork.GetSession().CreateSQLQuery($"DELETE FROM T_BP_REVENUE_CF_DATA WHERE ORG_CODE = '{centerCode}' AND TIME_YEAR = {year}")
                    .ExecuteUpdate();

                // insert to pl data history
                UnitOfWork.Repository<RevenueCFDataHistoryRepo>().Create((from pl in lstRevenuePlDataOldVersion
                                                                          select (T_BP_REVENUE_CF_DATA_HISTORY)pl).ToList());

                // insert to pl history
                UnitOfWork.Repository<RevenueCFHistoryRepo>().Create(new T_BP_REVENUE_CF_HISTORY
                {
                    ACTION = Approve_Action.TongHop,
                    ACTION_USER = currentUser,
                    CREATE_BY = currentUser,
                    ORG_CODE = ProfileUtilities.User.ORGANIZE_CODE,
                    ACTION_DATE = DateTime.Now,
                    PKID = Guid.NewGuid().ToString(),
                    TIME_YEAR = year,
                    TEMPLATE_CODE = string.Empty,
                    VERSION = revenueCF.VERSION,
                });

                // create new lstData
                revenueCFDataRepo.Create(lstData.ToList());

                // get list revenue pl which have org revenue in revenue center summary
                var lstRevenueCF = revenuePlDataApproved.ToLookup(x => new { OrgCode = x.ORG_CODE, TemplateCode = x.TEMPLATE_CODE, TemplateVersion = x.VERSION });

                // create list sum up detail
                sumUpDetailRepo.Create((from c in lstRevenueCF.Select(x => x.Key)
                                        select new T_BP_REVENUE_CF_SUM_UP_DETAIL
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

                // remove session
                foreach (var item in lstData)
                {
                    UnitOfWork.GetSession().Evict(item);
                }

                NotifyUtilities.CreateNotify(
                    new NotifyPara()
                    {
                        Activity = Activity.AC_TONG_HOP,
                        OrgCode = ProfileUtilities.User.ORGANIZE_CODE,
                        TimeYear = year,
                        ModulType = ModulType.RevenueCF,
                        UserSent = currentUser
                    });
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
                Exception = e;
                revenueCF = null;
            }
        }

        /// <summary>
        /// Summary sum up revenue center
        /// </summary>
        /// <param name="plDataRevenueElements">List of data revenue element output</param>
        /// <param name="year">Year want to summary</param>
        /// <param name="centerCode">Revenue center code want to summary</param>
        /// <param name="version">Version want to summary</param>
        /// <returns>Returns list of revenue element with their value</returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> SummarySumUpCenter(
            out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements,
            int year,
            string centerCode,
            int? version,
            bool? isHasValue = null,
            string templateId = "")
        {
            // get newest revenue pl data by center code
            plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                .GetCFDataByCenterCode(null, new List<string> { centerCode }, year, templateId, version);
            plDataRevenueElements = plDataRevenueElements.Where(x => x.STATUS == Approve_Status.DaPheDuyet).ToList();
            return SummaryCenter(plDataRevenueElements, centerCode, year, isHasValue);
        }

        /// <summary>
        /// Calculate data of each revenue element with their parents.
        /// Data of parents calculated by sum of all children
        /// </summary>
        /// <param name="details">List detail revenue</param>
        /// <param name="pureItems">List elements want to calculate data</param>
        /// <param name="parent_id">Start calculate from which parent. string Empty to the root</param>
        /// <param name="year">Which year of data</param>
        /// <param name="version">Which version of data</param>
        /// <returns>Returns list of revenue element with their data</returns>
        private IEnumerable<T_MD_REVENUE_CF_ELEMENT> GetSumDescendants(
            IEnumerable<T_MD_TEMPLATE_DETAIL_REVENUE_CF> details,
            IEnumerable<T_MD_REVENUE_CF_ELEMENT> pureItems,
            string parent_id,
            string templateId,
            int? year = null,
            int? version = null)
        {
            var lookup = pureItems.ToLookup(x => x.PARENT_CODE);
            foreach (var centerCode in lookup.Select(x => x.Key))
            {
                var revenuePlData = GetVersionData(templateId, centerCode, year, version);

                Queue<T_MD_REVENUE_CF_ELEMENT> st = new Queue<T_MD_REVENUE_CF_ELEMENT>(lookup[parent_id]);

                while (st.Count > 0)
                {
                    // get first item in queue
                    var item = st.Dequeue();
                    // variable to check should return item or not
                    bool shouldReturn = true;
                    // lst to store children of item which have children
                    var lstHasChild = new List<T_MD_REVENUE_CF_ELEMENT>();
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
                                    detail.PlData = revenuePlData.FirstOrDefault(x => x.REVENUE_CF_ELEMENT_CODE == detail.ELEMENT_CODE && x.PROFIT_CENTER_CODE == detail.CENTER_CODE);
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
                            var data = revenuePlData.Where(x => x.REVENUE_CF_ELEMENT_CODE == item.CODE && x.PROFIT_CENTER_CODE == item.CENTER_CODE);
                            if (data != null)
                            {
                                item.Values = new decimal[12]
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
                                //data.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                //data.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
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
        /// Get revenue CF data by version
        /// </summary>
        /// <param name="templateCode">Template code of CF data</param>
        /// <param name="orgCode">Center code of CF data</param>
        /// <param name="year">Which year of data</param>
        /// <param name="version">Which version of data</param>
        /// <returns>Returns Revenue CF Data</returns>
        private IList<T_BP_REVENUE_CF_DATA> GetVersionData(
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
                return UnitOfWork.Repository<RevenueCFDataRepo>().GetCFDataByOrgCode(orgCode, year.Value, templateCode, version);
            }
            else if (lstChildren.Contains(template.ORG_CODE))
            {
                return UnitOfWork.Repository<RevenueCFDataRepo>().GetCFDataByOrgCode(template.ORG_CODE, year.Value, templateCode, version);
            }
            else
            {
                return UnitOfWork.Repository<RevenueCFDataRepo>().GetCFDataByCenterCode(template.ORG_CODE, new List<string> { centerCode }, year.Value, templateCode, version);
                // get 
            }
        }

        /// <summary>
        /// Summary data of a center with newest data
        /// </summary>
        /// <param name="plDataRevenueElements">List revenue pl data want to out</param>
        /// <param name="centerCode">Center code want to summary data</param>
        /// <returns>Returns list revenue elements with their data</returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> SummaryCenterOut(out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements,
                                                              string centerCode,
                                                              int year,
                                                              int? version,
                                                              bool? isHasValue = null)
        {
            if (!version.HasValue)
            {
                // get newest revenue pl data by center code
                plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                    .GetManyByExpression(x => x.ORG_CODE.Equals(centerCode) && x.TIME_YEAR == year);
            }
            else
            {
                ObjDetail.ORG_CODE = centerCode;
                if (IsLeaf())
                {
                    // get all data have approved
                    plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                        .GetManyByExpression(x => x.ORG_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE));
                }
                else
                {
                    // get list all children centers in revenue center tree
                    var lstCostCenters = UnitOfWork.Repository<CostCenterRepo>().GetManyByExpression(x => x.PARENT_CODE == centerCode);

                    // get all data have approved
                    plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                        .GetManyByExpression(x => x.STATUS == Approve_Status.DaPheDuyet && (x.ORG_CODE == centerCode && !string.IsNullOrEmpty(x.TEMPLATE_CODE) ||
                    x.TIME_YEAR == year &&
                    lstCostCenters.Any(c => c.CODE.Equals(x.ORG_CODE))));
                }
            }

            return SummaryCenter(plDataRevenueElements, centerCode, year, isHasValue);
        }

        /// <summary>
        /// Get data has summed up (history)
        /// Lấy dữ liệu đã được tổng hợp lên đơn vị cha theo version
        /// </summary>
        /// <param name="plDataRevenueElements">List out data</param>
        /// <param name="centerCode">Org code của đơn vị được tổng hợp</param>
        /// <param name="year">Năm dữ liệu muốn xem</param>
        /// <param name="version">Version của dữ liệu muốn xem. Null thì sẽ lấy mới nhất</param>
        /// <returns></returns>
        public IList<T_MD_REVENUE_CF_ELEMENT> SummaryCenterVersion(out IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements,
                                                              string centerCode,
                                                              int year,
                                                              int? version,
                                                              bool isDrillDown = false)
        {
            if (isDrillDown)
            {
                plDataRevenueElements = GetAllSumUpDetails(centerCode, year, version.Value);
            }
            else
            {
                plDataRevenueElements = UnitOfWork.Repository<RevenueCFDataRepo>()
                    .GetCFDataByOrgCode(centerCode, year, string.Empty, version);
            }
            return SummaryCenter(plDataRevenueElements, centerCode, year);
        }

        /// <summary>
        /// Lấy danh sách tất cả các data đã được tổng hợp lên cho centerCode theo version và năm
        /// </summary>
        /// <param name="centerCode"></param>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private IList<T_BP_REVENUE_CF_DATA> GetAllSumUpDetails(string centerCode, int year, int version)
        {
            var detailsSumUp = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
               .GetManyWithFetch(x => x.ORG_CODE == centerCode && x.TIME_YEAR == year && x.SUM_UP_VERSION == version);

            var lstChildren = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);
            var lstDetails = new List<T_BP_REVENUE_CF_DATA>();
            var lstResult = new List<T_BP_REVENUE_CF_DATA>();
            var plDataRepo = UnitOfWork.Repository<RevenueCFDataRepo>();
            foreach (var detail in detailsSumUp)
            {
                var details = plDataRepo
                    .GetCFDataByCenterCode(
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

            var lookupElement = lstDetails.ToLookup(x => x.REVENUE_CF_ELEMENT_CODE);
            foreach (var key in lookupElement.Select(x => x.Key))
            {
                if (lookupElement[key].Count() == 1)
                {
                    lstResult.AddRange(lookupElement[key]);
                }
                else
                {
                    var data = lookupElement[key];
                    var lookupCenter = data.ToLookup(x => x.ORG_CODE);
                    foreach (var orgCode in lookupCenter.Select(x => x.Key))
                    {
                        if (lookupCenter[orgCode].Count() == 1)
                        {
                            lstResult.AddRange(lookupCenter[orgCode]);
                        }
                        else
                        {
                            var dataOrgCode = lookupCenter[orgCode];
                            lstResult.Add(new T_BP_REVENUE_CF_DATA
                            {
                                ORG_CODE = orgCode,
                                REVENUE_CF_ELEMENT_CODE = key,
                                RevenueElement = lookupCenter[orgCode].First().RevenueElement,
                                Organize = lookupCenter[orgCode].First().Organize,
                                ProfitCenter = lookupCenter[orgCode].First().ProfitCenter,
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
                            });
                        }
                    }

                }
            }
            return lstResult;
        }

        private IList<T_MD_REVENUE_CF_ELEMENT> SummaryCenter(IList<T_BP_REVENUE_CF_DATA> plDataRevenueElements, string centerCode, int year,
                                                          bool? isHasValue = null)
        {

            // get all revenue elements
            var allRevenueElement = UnitOfWork.Repository<RevenueCFElementRepo>().GetManyByExpression(x => x.TIME_YEAR == year);
            // get all child
            var childrenCodes = GetListOfChildrenCenter(centerCode).Select(x => x.CODE);
            // list store pure items to send to view
            var pureLstItems = new List<T_MD_REVENUE_CF_ELEMENT>();

            // lookup revenue elements by parent code
            var lookupElements = plDataRevenueElements.GroupBy(x => x.REVENUE_CF_ELEMENT_CODE)
                .Select(x => x.First())
                .ToLookup(x => x.RevenueElement.PARENT_CODE);

            var childrenCode = GetListOfChildrenCenter(centerCode)
                .Select(x => x.CODE)
                .ToList();
            childrenCode.Add(centerCode);

            foreach (var code in lookupElements.Select(l => l.Key))
            {
                // set level start by 0
                var level = 0;
                // temp list
                var lst = new List<T_MD_REVENUE_CF_ELEMENT>();
                // add all leaf to temp list item with child = true
                lst.AddRange(from item in lookupElements[code]
                             let orgCode = childrenCodes.Contains(item.ORG_CODE) || item.ORG_CODE == centerCode ? item.ORG_CODE : item.ORG_CODE
                             select new T_MD_REVENUE_CF_ELEMENT
                             {
                                 CENTER_CODE = item.ORG_CODE,
                                 C_ORDER = item.RevenueElement.C_ORDER,
                                 NAME = item.RevenueElement.NAME,
                                 PARENT_CODE = item.RevenueElement.PARENT_CODE,
                                 CODE = item.RevenueElement.CODE,
                                 LEVEL = 0,
                                 IS_GROUP = false,
                                 ORG_NAME = childrenCode.Contains(item.ORG_CODE) ? item.Organize.NAME : item.ProfitCenter.NAME,
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
                    var element = allRevenueElement.FirstOrDefault(x => x.CODE == parentCode);
                    if (element != null)
                    {
                        // if find parent in all revenue element
                        parentCode = element.PARENT_CODE;
                        element.CENTER_CODE = centerCode;
                        element.LEVEL = level;
                        element.IS_GROUP = true;
                        element.TEMPLATE_CODE = lookupElements[code].FirstOrDefault().TEMPLATE_CODE;
                        element.ORG_CODE = lookupElements[code].FirstOrDefault().ORG_CODE;
                        lst.Add((T_MD_REVENUE_CF_ELEMENT)element.CloneObject());     // must to clone to other object because it reference to other center
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
            var sum = GetSumDescendants(plDataRevenueElements, pureLstItems, parentId: string.Empty).Distinct().ToList();
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
        /// Calculate data for list revenue element
        /// </summary>
        /// <param name="revenueCFData">List revenue pl data containt value</param>
        /// <param name="pureItems">List of revenue element want to calculate</param>
        /// <param name="parentId">Parent id want to start. Empty for root</param>
        /// <returns>Returns list of revenue element has calculated data</returns>
        private IEnumerable<T_MD_REVENUE_CF_ELEMENT> GetSumDescendants(
            IList<T_BP_REVENUE_CF_DATA> revenueCFData,
            IList<T_MD_REVENUE_CF_ELEMENT> pureItems,
            string parentId)
        {
            // set tổng năm
            // tạo element tổng
            var lstResult = new List<T_MD_REVENUE_CF_ELEMENT>
            {
                // set tổng năm
                // tạo element tổng
                new T_MD_REVENUE_CF_ELEMENT
                {
                    NAME = "TỔNG CỘNG",
                    LEVEL = 0,
                    PARENT_CODE = null,
                    IS_GROUP = true,
                    IsChildren = false,
                    C_ORDER = 0,
                    CODE = string.Empty,
                    Values = new decimal[13]
                    {
                        revenueCFData.Sum(x => x.VALUE_JAN) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_FEB) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_MAR) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_APR) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_MAY) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_JUN) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_JUL) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_AUG) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_SEP) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_OCT) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_NOV) ?? 0,
                        revenueCFData.Sum(x => x.VALUE_DEC) ?? 0,
                        0
                    }
                }
            };
            lstResult[0].Values[12] = lstResult[0].Values.Sum();
            var lookup = pureItems.ToLookup(i => i.PARENT_CODE);
            Queue<T_MD_REVENUE_CF_ELEMENT> st = new Queue<T_MD_REVENUE_CF_ELEMENT>(lookup[parentId]);
            while (st.Count > 0)
            {
                // get first item in queue
                var item = st.Dequeue();
                // variable to check should return item or not
                bool shouldReturn = true;
                // lst to store children of item which have children
                var lstRevenueElements = new List<T_MD_REVENUE_CF_ELEMENT>();
                // loop through items which have parent id = item id
                foreach (var i in lookup[item.CODE])
                {
                    if (lookup[i.CODE].Count() > 0)
                    {
                        shouldReturn = false;
                        lstRevenueElements.Add(i);
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

                            var treeData = revenueCFData.Where(x => x.REVENUE_CF_ELEMENT_CODE.Equals(i.CODE));
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
                                item.HasAssignValue = true;

                                foreach (var d in treeData)
                                {
                                    var values = new decimal[12];
                                    values[0] = treeData.Sum(x => x.VALUE_JAN) ?? 0;
                                    values[1] = treeData.Sum(x => x.VALUE_FEB) ?? 0;
                                    values[2] = treeData.Sum(x => x.VALUE_MAR) ?? 0;
                                    values[3] = treeData.Sum(x => x.VALUE_APR) ?? 0;
                                    values[4] = treeData.Sum(x => x.VALUE_MAY) ?? 0;
                                    values[5] = treeData.Sum(x => x.VALUE_JUN) ?? 0;
                                    values[6] = treeData.Sum(x => x.VALUE_JUL) ?? 0;
                                    values[7] = treeData.Sum(x => x.VALUE_AUG) ?? 0;
                                    values[8] = treeData.Sum(x => x.VALUE_SEP) ?? 0;
                                    values[9] = treeData.Sum(x => x.VALUE_OCT) ?? 0;
                                    values[10] = treeData.Sum(x => x.VALUE_NOV) ?? 0;
                                    values[11] = treeData.Sum(x => x.VALUE_DEC) ?? 0;
                                    i.Values = values;
                                    var clone = (T_MD_REVENUE_CF_ELEMENT)i.Clone();
                                    //yield return clone;
                                    lstResult.Add(clone);
                                    break;
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
                        var data = revenueCFData.Where(x => x.REVENUE_CF_ELEMENT_CODE == item.CODE && x.ORG_CODE == item.CENTER_CODE);
                        if (data != null)
                        {
                            item.Values = new decimal[12]
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
                                //data.Sum(x => x.VALUE_SUM_YEAR) ?? 0,
                                //data.Sum(x => x.VALUE_SUM_YEAR_PREVENTIVE) ?? 0,
                            };
                        }
                    }
                    lstResult.Add(item);
                }
                else
                {
                    // add children of item which have chilren to lookup 
                    if (lstRevenueElements.Count > 0)
                    {
                        lookup = lookup
                            .SelectMany(l => l)
                            .Concat(lstRevenueElements)
                            .ToLookup(x => x.PARENT_CODE);
                    }
                    // re-enqueue item to queue
                    st.Enqueue(item);
                }
            }
            return lstResult;
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
            workbook.SetSheetName(0, ModulType.GetTextSheetName(ModulType.RevenueCF));
            fs.Close();
            ISheet sheet = workbook.GetSheetAt(0);
            var metaData = ExcelHelper.GetExcelMeta(html);
            var NUM_CELL = string.IsNullOrEmpty(templateId) ? 17 : 23;

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
            var name = "DỮ LIỆU KẾ HOẠCH DOANH THU DÒNG TIỀN";
            var centerName = GetCenter(centerCode).NAME.ToUpper();
            var template = GetTemplate(templateId);
            var templateName = template != null ? $"Mẫu khai báo: {template.CODE} - {template.NAME}" : "Tổng hợp dữ liệu";

            ExcelHelperBP.InitHeaderFile(ref sheet, year, centerName, version, NUM_CELL, templateName, unit, name, exchangeRate);
        }

        #endregion
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

            var lstDetails = UnitOfWork.Repository<RevenueCFSumUpDetailRepo>()
            .GetManyByExpression(x => x.ORG_CODE == parentCenterCode &&
                x.TIME_YEAR == year &&
                x.SUM_UP_VERSION == version);
            var templateVersion = lstDetails.FirstOrDefault(y => y.TEMPLATE_CODE == templateCode)?.DATA_VERSION;
            if (templateVersion.HasValue)
            {
                // get file upload base
                return UnitOfWork.Repository<RevenueCFVersionRepo>()
                    .GetManyWithFetch(x => x.TIME_YEAR == year && x.TEMPLATE_CODE == templateCode && x.VERSION == templateVersion.Value, x => x.FileUpload)
                    .Select(x => x.FileUpload)
                    .ToList();
            }
            else
            {
                return new List<T_CM_FILE_UPLOAD>();
            }

        }

        public override T_BP_TYPE GetBudgetType()
        {
            return UnitOfWork.Repository<TypeRepo>().GetFirstWithFetch(x => x.OBJECT_TYPE == TemplateObjectType.Department && x.ELEMENT_TYPE == ElementType.DoanhThu && x.BUDGET_TYPE == BudgetType.DongTien);
        }
    }
}
