using SMO.Core.Entities;
using SMO.Repository.Implement.CM;

using System;

namespace SMO.Service.CM
{
    public class CommentService : GenericService<T_CM_COMMENT, CommentRepo>
    {
        public CommentService() : base()
        {

        }

        public void GetComments()
        {
            ObjList = UnitOfWork.Repository<CommentRepo>().GetCommentsOfDocument(ObjDetail.REFRENCE_ID);
        }

        //internal void GetComments(T_CM_HEADER_BP_COMMENT header)
        //{
        //    ObjList = UnitOfWork.Repository<CommentRepo>().GetCommentsBP(header).ToList();
        //}

        public override void Create()
        {
            ObjDetail.CODE = Guid.NewGuid().ToString();
            ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
            base.Create();
        }
    }
}
