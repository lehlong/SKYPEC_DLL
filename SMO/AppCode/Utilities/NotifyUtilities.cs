using SMO.Core.Entities;
using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.CM;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.WF;
using SMO.Service.Class;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO
{
    public static class NotifyUtilities
    {
        public static void CreateNotify(NotifyPara notifyPara)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            try
            {
                string NGUOI_THUC_HIEN = "{NGUOI_THUC_HIEN}";
                string NGUOI_NHAN_THONG_BAO = "{NGUOI_NHAN_THONG_BAO}";
                string MAU_KHAI_BAO = "{MAU_KHAI_BAO}";
                string NAM_NGAN_SACH = "{NAM_NGAN_SACH}";
                string LOAI_NGAN_SACH = "{LOAI_NGAN_SACH}";
                string DON_VI = "{DON_VI}";

                var parentOrg = UnitOfWork.Repository<CostCenterRepo>().Get(notifyPara.OrgCode);
                //if (string.IsNullOrEmpty(notifyPara.ChildOrgCode))
                //{

                //}
                //var childOrg = UnitOfWork.Repository<CostCenterRepo>().Get(notifyPara.ChildOrgCode);

                // Lấy thông tin Activity Com
                var lstActivityCom = UnitOfWork.Repository<ActivityComRepo>().GetManyByExpression(x => x.ACTIVITY_CODE == notifyPara.Activity.ToString()).ToList();
                if (lstActivityCom.Count == 0)
                {
                    return;
                }

                // Lấy thông tin activity user
                var lstActivityUser = UnitOfWork.Repository<ActivityUserRepo>().GetManyByExpression(
                    x => x.ACTIVITY_CODE == notifyPara.Activity.ToString() &&
                    x.ORG_CODE == notifyPara.OrgCode &&
                    x.USER_SENDER == notifyPara.UserSent
                    ).ToList();
                if (lstActivityUser.Count == 0)
                {
                    return;
                }

                var template = UnitOfWork.Repository<TemplateRepo>().Get(notifyPara.TemplateCode);

                #region Tạo notify trên trình duyệt
                var notifyComNotify = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "NOTIFY" && x.ACTIVE == true);
                if (notifyComNotify == null || string.IsNullOrWhiteSpace(notifyComNotify.CONTENTS))
                {
                    return;
                }

                var lstUserNotify = new List<string>();
                UnitOfWork.BeginTransaction();
                foreach (var item in lstActivityUser)
                {
                    if (item.UserReceiver == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(notifyPara.ChildOrgCode) && item.UserReceiver.ORGANIZE_CODE != notifyPara.ChildOrgCode)
                    {
                        continue;
                    }

                    string newId = Guid.NewGuid().ToString();
                    lstUserNotify.Add(item.USER_RECEIVER);
                    string strTemplate = @"
                    <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                        <div class='icon-circle bg-red'>
                            <i class='material-icons'>email</i>
                        </div>
                        <div class='menu-info'>
                            <span>{2}</span>
                            <p>
                                <i class='material-icons'>access_time</i> {3}
                            </p>
                        </div>
                    </a>";
                    string notifyContent = notifyComNotify.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, item.UserReceiver.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, notifyPara.TimeYear.ToString())
                        .Replace(DON_VI, parentOrg.NAME)
                        .Replace(LOAI_NGAN_SACH, ModulType.GetText(notifyPara.ModulType));

                    if (template != null)
                    {
                        notifyContent = notifyContent.Replace(MAU_KHAI_BAO, $"{template.CODE} - {template.NAME}");
                    }
                    else
                    {
                        notifyContent = notifyContent.Replace(MAU_KHAI_BAO, "Dữ liệu tổng hợp");
                    }

                    string strContent = string.Format(strTemplate, newId, "", notifyContent, DateTime.Now.ToString(Global.DateTimeToStringFormat));
                    string strRawContent = notifyContent;
                    UnitOfWork.Repository<NotifyRepo>().Create(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        CONTENTS_EN = strContent,
                        RAW_CONTENTS = strRawContent,
                        RAW_CONTENTS_EN = strRawContent,
                        USER_NAME = item.USER_RECEIVER
                    });
                }
                UnitOfWork.Commit();
                SMOUtilities.SendNotify(lstUserNotify);
                #endregion

                #region Tạo email
                var notifyComEmail = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "EMAIL" && x.ACTIVE == true);
                if (notifyComEmail == null || string.IsNullOrWhiteSpace(notifyComEmail.CONTENTS) || string.IsNullOrWhiteSpace(notifyComEmail.SUBJECT))
                {
                    return;
                }

                UnitOfWork.BeginTransaction();
                foreach (var item in lstActivityUser)
                {
                    if (item.UserReceiver == null || string.IsNullOrWhiteSpace(item.UserReceiver.EMAIL))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(notifyPara.ChildOrgCode) && item.UserReceiver.ORGANIZE_CODE != notifyPara.ChildOrgCode)
                    {
                        continue;
                    }

                    string contentSubject = notifyComEmail.SUBJECT
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, item.UserReceiver.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, notifyPara.TimeYear.ToString())
                        .Replace(DON_VI, parentOrg.NAME)
                        .Replace(LOAI_NGAN_SACH, ModulType.GetText(notifyPara.ModulType));

                    if (template != null)
                    {
                        contentSubject = contentSubject.Replace(MAU_KHAI_BAO, $"{template.CODE} - {template.NAME}");
                    }
                    else
                    {
                        contentSubject = contentSubject.Replace(MAU_KHAI_BAO, $"Dữ liệu tổng hợp");
                    }

                    string contentBody = notifyComEmail.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, item.UserReceiver.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, notifyPara.TimeYear.ToString())
                        .Replace(DON_VI, parentOrg.NAME)
                        .Replace(LOAI_NGAN_SACH, ModulType.GetText(notifyPara.ModulType));

                    if (template != null)
                    {
                        contentBody = contentBody.Replace(MAU_KHAI_BAO, $"{template.CODE} - {template.NAME}");
                    }
                    else
                    {
                        contentBody = contentBody.Replace(MAU_KHAI_BAO, $"Dữ liệu tổng hợp");
                    }

                    UnitOfWork.Repository<EmailRepo>().Create(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = item.UserReceiver.EMAIL,
                        SUBJECT = contentSubject,
                        CONTENTS = contentBody
                    });
                }
                UnitOfWork.Commit();
                #endregion

                #region Tạo sms
                var notifyComSms = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "SMS" && x.ACTIVE == true);
                if (notifyComSms == null || string.IsNullOrWhiteSpace(notifyComSms.CONTENTS))
                {
                    return;
                }

                UnitOfWork.BeginTransaction();
                foreach (var item in lstActivityUser)
                {
                    if (item.UserReceiver == null || string.IsNullOrWhiteSpace(item.UserReceiver.PHONE))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(notifyPara.ChildOrgCode) && item.UserReceiver.ORGANIZE_CODE != notifyPara.ChildOrgCode)
                    {
                        continue;
                    }

                    string contentBody = notifyComSms.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, item.UserReceiver.FULL_NAME)
                        .Replace(DON_VI, parentOrg.NAME)
                        .Replace(NAM_NGAN_SACH, notifyPara.TimeYear.ToString())
                        .Replace(LOAI_NGAN_SACH, ModulType.GetText(notifyPara.ModulType));

                    if (template != null)
                    {
                        contentBody = contentBody.Replace(MAU_KHAI_BAO, $"{template.CODE} - {template.NAME}");
                    }
                    else
                    {
                        contentBody = contentBody.Replace(MAU_KHAI_BAO, $"Dữ liệu tổng hợp");
                    }

                    UnitOfWork.Repository<SmsRepo>().Create(new T_CM_SMS()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        CONTENTS = contentBody,
                        PHONE_NUMBER = item.UserReceiver.PHONE
                    });
                }
                UnitOfWork.Commit();
                #endregion
            }
            catch
            {
                UnitOfWork.Rollback();
            }
        }

        public static void CreateNotifyChangePeriod(T_BP_PERIOD period, int year)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            try
            {
                string NGUOI_THUC_HIEN = "{NGUOI_THUC_HIEN}";
                string NGUOI_NHAN_THONG_BAO = "{NGUOI_NHAN_THONG_BAO}";
                string GIAI_DOAN = "{GIAI_DOAN}";
                string NAM_NGAN_SACH = "{NAM_NGAN_SACH}";
                string AC_CHUYEN_GIAI_DOAN = Activity.AC_CHUYEN_GIAI_DOAN.ToString();

                var allUsers = UnitOfWork.Repository<UserRepo>().GetAll();

                // Lấy thông tin Activity Com
                var lstActivityCom = UnitOfWork.Repository<ActivityComRepo>().GetManyByExpression(x => x.ACTIVITY_CODE == AC_CHUYEN_GIAI_DOAN).ToList();
                if (lstActivityCom.Count == 0)
                {
                    return;
                }

                #region Tạo notify trên trình duyệt
                var notifyComNotify = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "NOTIFY" && x.ACTIVE == true);
                if (notifyComNotify == null || string.IsNullOrWhiteSpace(notifyComNotify.CONTENTS))
                {
                    return;
                }

                var lstUserNotify = new List<string>();
                UnitOfWork.BeginTransaction();
                foreach (var user in allUsers)
                {
                    string newId = Guid.NewGuid().ToString();
                    lstUserNotify.Add(user.USER_NAME);
                    string strTemplate = @"
                    <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                        <div class='icon-circle bg-red'>
                            <i class='material-icons'>email</i>
                        </div>
                        <div class='menu-info'>
                            <span>{2}</span>
                            <p>
                                <i class='material-icons'>access_time</i> {3}
                            </p>
                        </div>
                    </a>";
                    string notifyContent = notifyComNotify.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, year.ToString())
                        .Replace(GIAI_DOAN, period.NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, user.FULL_NAME);

                    string strContent = string.Format(strTemplate, newId, "", notifyContent, DateTime.Now.ToString(Global.DateTimeToStringFormat));
                    string strRawContent = notifyContent;
                    UnitOfWork.Repository<NotifyRepo>().Create(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        CONTENTS_EN = strContent,
                        RAW_CONTENTS = strRawContent,
                        RAW_CONTENTS_EN = strRawContent,
                        USER_NAME = user.USER_NAME
                    });
                }
                UnitOfWork.Commit();
                SMOUtilities.SendNotify(allUsers.Select(x => x.USER_NAME).ToList());
                #endregion

                #region Tạo email
                var notifyComEmail = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "EMAIL" && x.ACTIVE == true);
                if (notifyComEmail == null || string.IsNullOrWhiteSpace(notifyComEmail.CONTENTS) || string.IsNullOrWhiteSpace(notifyComEmail.SUBJECT))
                {
                    return;
                }

                UnitOfWork.BeginTransaction();
                foreach (var user in allUsers)
                {
                    string contentSubject = notifyComEmail.SUBJECT
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, year.ToString())
                        .Replace(GIAI_DOAN, period.NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, user.FULL_NAME);

                    string contentBody = notifyComEmail.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, year.ToString())
                        .Replace(GIAI_DOAN, period.NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, user.FULL_NAME);

                    UnitOfWork.Repository<EmailRepo>().Create(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = user.EMAIL,
                        SUBJECT = contentSubject,
                        CONTENTS = contentBody
                    });
                }
                UnitOfWork.Commit();
                #endregion

                #region Tạo sms
                var notifyComSms = lstActivityCom.FirstOrDefault(x => x.TYPE_NOTIFY == "SMS" && x.ACTIVE == true);
                if (notifyComSms == null || string.IsNullOrWhiteSpace(notifyComSms.CONTENTS))
                {
                    return;
                }

                UnitOfWork.BeginTransaction();
                foreach (var user in allUsers)
                {
                    string contentBody = notifyComSms.CONTENTS
                        .Replace(NGUOI_THUC_HIEN, ProfileUtilities.User.FULL_NAME)
                        .Replace(NAM_NGAN_SACH, year.ToString())
                        .Replace(GIAI_DOAN, period.NAME)
                        .Replace(NGUOI_NHAN_THONG_BAO, user.FULL_NAME);

                    UnitOfWork.Repository<SmsRepo>().Create(new T_CM_SMS()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        CONTENTS = contentBody,
                        PHONE_NUMBER = user.PHONE
                    });
                }
                UnitOfWork.Commit();
                #endregion
            }
            catch
            {
                UnitOfWork.Rollback();
            }
        }

        internal static void CreateNotifyYeuCauCapDuoiDieuChinh(NotifyPara notifyPara)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            try
            {
                var template = UnitOfWork.Repository<TemplateRepo>().Get(notifyPara.TemplateCode);
                if(template == null)
                {
                    return;
                }
                #region Tạo notify trên trình duyệt

                var lstUserNotify = new List<string>
                {
                    template.CREATE_BY
                };
                UnitOfWork.BeginTransaction();
                
                    string newId = Guid.NewGuid().ToString();
                    string strTemplate = @"
                    <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                        <div class='icon-circle bg-red'>
                            <i class='material-icons'>email</i>
                        </div>
                        <div class='menu-info'>
                            <span>{2}</span>
                            <p>
                                <i class='material-icons'>access_time</i> {3}
                            </p>
                        </div>
                    </a>";
                    string strContent = string.Format(strTemplate, newId, "", notifyPara.Note, DateTime.Now.ToString(Global.DateTimeToStringFormat));
                    UnitOfWork.Repository<NotifyRepo>().Create(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        CONTENTS_EN = strContent,
                        RAW_CONTENTS = notifyPara.Note,
                        RAW_CONTENTS_EN = notifyPara.Note,
                        USER_NAME = template.CREATE_BY
                    });
                
                UnitOfWork.Commit();
                SMOUtilities.SendNotify(lstUserNotify);
                #endregion

            }
            catch
            {
                UnitOfWork.Rollback();
            }
        }
    }
}