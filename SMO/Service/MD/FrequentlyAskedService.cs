using SMO.Core.Entities;
using SMO.Repository.Implement.MD;

using System;

namespace SMO.Service.MD
{
    public class FrequentlyAskedService : GenericService<T_FAQ_FREQUENTLY_ASKED, FrequentlyAskedRepo>
    {
        public T_FAQ_QUESTION ObjQuestion { get; set; }
        public FrequentlyAskedService() : base()
        {
            ObjQuestion = new T_FAQ_QUESTION();
        }


        public void CreateQuestion()
        {
            try
            {
                ObjQuestion.PKID = Guid.NewGuid().ToString();
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<FAQQuestionRepo>().Create(ObjQuestion);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Create()
        {
            ObjDetail.PKID = Guid.NewGuid().ToString();
            base.Create();
        }
    }
}
