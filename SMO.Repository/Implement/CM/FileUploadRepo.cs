using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;

namespace SMO.Repository.Implement.CM
{
    public class FileUploadRepo : GenericRepository<T_CM_FILE_UPLOAD>, IFileUploadRepo
    {
        public FileUploadRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
