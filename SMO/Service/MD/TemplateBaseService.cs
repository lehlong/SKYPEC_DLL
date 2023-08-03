using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using SMO.Service.Class;
using SMO.Service.Common;

using System;
using System.Collections.Generic;
using System.Web;

namespace SMO.Service.MD
{
    public class TemplateBaseService : GenericService<T_BP_TEMPLATE_BASE, TemplateBaseRepo>
    {
        public TemplateBaseService() : base()
        {

        }

        public void UploadFile(HttpRequestBase request)
        {
            try
            {
                var lstFileStream = new List<FILE_STREAM>();

                for (int i = 0; i < request.Files.AllKeys.Length; i++)
                {
                    var file = request.Files[i];
                    var code = Guid.NewGuid().ToString();
                    var fileStream = new FILE_STREAM()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        FILESTREAM = request.Files[i]
                    };
                    lstFileStream.Add(fileStream);
                }
                FileStreamService.InsertFile(lstFileStream);

                UnitOfWork.BeginTransaction();

                foreach (var item in lstFileStream)
                {
                    this.CurrentRepository.Create(new T_BP_TEMPLATE_BASE()
                    {
                        TEMPLATE_CODE = this.ObjDetail.TEMPLATE_CODE,
                        TIME_YEAR = this.ObjDetail.TIME_YEAR,
                        FILE_ID = item.PKID,
                        PKID = Guid.NewGuid().ToString()
                    });
                }

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
