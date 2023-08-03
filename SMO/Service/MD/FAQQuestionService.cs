using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.Service.AD;
using SMO.Service.CF;
using SMO.Service.CM;

using System;
using System.Linq;

namespace SMO.Service.MD
{
    public class FAQQuestionService : GenericService<T_FAQ_QUESTION, FAQQuestionRepo>
    {
        public FAQQuestionService() : base()
        {
        }

        public void UpdateAnswer()
        {
            try
            {
                ObjDetail.STATUS = true;
                base.Update();
                CreateEmail();
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        public void CreateEmail()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ObjDetail.EMAIL) || string.IsNullOrWhiteSpace(ObjDetail.ANSWER))
                {
                    return;
                }
                string TIEU_DE_CAU_HOI = "{TIEU_DE_CAU_HOI}";
                string NOI_DUNG_CAU_HOI = "{NOI_DUNG_CAU_HOI}";
                string NOI_DUNG_CAU_TRA_LOI = "{NOI_DUNG_CAU_TRA_LOI}";
                string NGAY_HOI = "{NGAY_HOI}";
                string NGUOI_HOI = "{NGUOI_HOI}";

                var serviceTemplate = new ConfigTemplateNotifyService();
                serviceTemplate.GetAll();
                if (serviceTemplate.ObjList.Count() == 0)
                {
                    return;
                }

                var serviceConfig = new SystemConfigService();
                serviceConfig.GetConfig();

                var template = serviceTemplate.ObjList.FirstOrDefault();

                var serviceEmail = new EmailNotifyService();

                var subjectTemplate = template.FEED_BACK_SUBJECT;
                var bodyTemplate = template.FEED_BACK_BODY;

                if (string.IsNullOrWhiteSpace(subjectTemplate) || string.IsNullOrWhiteSpace(bodyTemplate))
                {
                    return;
                }

                var contentSubject = subjectTemplate
                        .Replace(TIEU_DE_CAU_HOI, ObjDetail.SUBJECT)
                        .Replace(NOI_DUNG_CAU_HOI, ObjDetail.CONTENTS)
                        .Replace(NOI_DUNG_CAU_TRA_LOI, ObjDetail.ANSWER)
                        .Replace(NGUOI_HOI, ObjDetail.NAME)
                        .Replace(NGAY_HOI, ObjDetail.CREATE_DATE.Value.ToString(Global.DateTimeToStringFormat));

                var contentBody = bodyTemplate
                         .Replace(TIEU_DE_CAU_HOI, ObjDetail.SUBJECT)
                        .Replace(NOI_DUNG_CAU_HOI, ObjDetail.CONTENTS)
                        .Replace(NOI_DUNG_CAU_TRA_LOI, ObjDetail.ANSWER)
                        .Replace(NGUOI_HOI, ObjDetail.NAME)
                        .Replace(NGAY_HOI, ObjDetail.CREATE_DATE.Value.ToString(Global.DateTimeToStringFormat));

                serviceEmail.ObjDetail = new T_CM_EMAIL()
                {
                    PKID = Guid.NewGuid().ToString(),
                    EMAIL = ObjDetail.EMAIL,
                    SUBJECT = contentSubject,
                    CONTENTS = contentBody
                };
                serviceEmail.Create();
            }
            catch (Exception)
            {

            }
        }
    }
}
