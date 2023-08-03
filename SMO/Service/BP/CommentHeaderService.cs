using SMO.Core.Entities;
using SMO.Core.Entities.CM;
using SMO.Repository.Implement.CM;
using System;

namespace SMO.Service.BP
{
    public class CommentHeaderService : GenericService<T_CM_HEADER_BP_COMMENT, HeaderBPCommentRepo>
    {
        public override void Create()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var content = ObjDetail.CONTENT;
                var header = new T_CM_HEADER_BP_COMMENT();
                var pkId = ObjDetail.PKID;
                header = CurrentRepository.Get(pkId);
                if (header != null)
                {
                    ObjDetail = header;
                    ObjDetail.NUMBER_COMMENTS++;
                    ObjDetail = CurrentRepository.Update(ObjDetail);
                }
                else
                {
                    ObjDetail.NUMBER_COMMENTS = 1;

                    if (ProfileUtilities.User != null)
                    {
                        ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        ObjDetail.CREATE_DATE = CurrentRepository.GetDateDatabase();
                    }
                    ObjDetail = CurrentRepository.Create(ObjDetail);
                }
                var comment = new T_CM_COMMENT
                {
                    CONTENTS = content,
                    REFRENCE_ID = pkId,
                    CODE = Guid.NewGuid().ToString(),
                    CREATE_BY = ProfileUtilities.User?.USER_NAME,
                    CREATE_DATE = CurrentRepository.GetDateDatabase()
                };
                UnitOfWork.Repository<CommentRepo>().Create(comment);

                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
            }
        }

        internal bool IsSelfUploadTemplate(string orgCode, string referenceCode)
        {
            return new CostPLService().IsSelfUpload(orgCode, referenceCode);
        }

        internal void GetHeader()
        {
            var header = GetFirstByExpression(
                        x => x.REFERENCE_CODE == ObjDetail.REFERENCE_CODE &&
                        x.ORG_CODE == ObjDetail.ORG_CODE &&
                        x.YEAR == ObjDetail.YEAR &&
                        x.BUDGET_TYPE == ObjDetail.BUDGET_TYPE &&
                        x.OBJECT_TYPE == ObjDetail.OBJECT_TYPE &&
                        x.ELEMENT_TYPE == ObjDetail.ELEMENT_TYPE &&
                        x.VERSION == ObjDetail.VERSION);
            if (header == null)
            {
                ObjDetail.PKID = Guid.NewGuid().ToString();
            }
            else
            {
                ObjDetail = header;
            }
        }

        internal void GetComments()
        {
            ObjDetail.Comments = UnitOfWork.Repository<CommentRepo>().GetCommentsBP(ObjDetail);
        }
    }
}
