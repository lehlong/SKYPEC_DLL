using SMO.Core.Entities;
using SMO.Repository.Implement.CF;
using SMO.Service.AD;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Hosting;

using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;

namespace SMO.Service.MD
{
    public class ConfigTableauService : GenericService<T_CF_TABLEAU, ConfigTableauRepo>
    {
        public List<NodeTableau> BuildTree()
        {
            var lstNode = new List<NodeTableau>();
            GetAll();
            foreach (var item in ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeTableau()
                {
                    id = item.PKID,
                    pId = item.PARENT_ID,
                    name = item.NAME,
                    isGroup = item.IS_GROUP ? "true" : "false",
                    isParent = item.IS_GROUP ? "true" : "false",
                    open = "true"
                };
                lstNode.Add(node);
            }
            return lstNode;
        }

        public void ValidateTableau()
        {
            try
            {
                var serviceSystemConfig = new SystemConfigService();
                serviceSystemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PASSWORD) ||
                    string.IsNullOrWhiteSpace(serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PROTOCOL) ||
                    string.IsNullOrWhiteSpace(serviceSystemConfig.ObjDetail.TABLEAU_SERVER_URL) ||
                    string.IsNullOrWhiteSpace(serviceSystemConfig.ObjDetail.TABLEAU_SERVER_USER) ||
                    string.IsNullOrWhiteSpace(serviceSystemConfig.ObjDetail.TABLEAU_SERVER_API_VERSION))
                {
                    this.State = false;
                    this.ErrorMessage = "Thông tin cấu hình hệ thống tableau trong system config chưa đầy đủ!";
                    return;
                }

                var protocol = ServerProtocol.Http;
                if (serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PROTOCOL.ToUpper() == "HTTPS")
                {
                    protocol = ServerProtocol.Https;
                }
                var urlTableau = new TableauServerUrls(protocol,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_URL,
                    "", 1000, ServerVersion.Server9,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_API_VERSION);

                var signIn = new TableauServerSignIn(urlTableau,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_USER,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PASSWORD, new TaskStatusLogs());
                try
                {
                    if (!signIn.ExecuteRequest())
                    {
                        this.State = false;
                        this.ErrorMessage = "Thông tin cấu hình username, password tableau trong system config không đúng!";
                        return;
                    }
                }
                catch
                {
                    this.State = false;
                    this.ErrorMessage = "Thông tin cấu hình hệ thống tableau trong system config không đúng!";
                    return;
                }

                // Kiểm tra workbook
                var requestWorkbook = new DownloadWorkbooksList(urlTableau, signIn);
                requestWorkbook.ExecuteRequest();
                var findWorkbook = requestWorkbook.Workbooks.FirstOrDefault(x => x.Name == this.ObjDetail.WORKBOOK_NAME);
                if (findWorkbook == null)
                {
                    this.State = false;
                    this.ErrorMessage = "Thông tin workbook name không đúng!";
                    return;
                }
                this.ObjDetail.WORKBOOK_ID = findWorkbook.Id;
                this.ObjDetail.WORKBOOK_CONTENT_URL = findWorkbook.ContentUrl;

                // Kiểm tra view
                var requestView = new DownloadViewsForWorkbookList(findWorkbook.Id, urlTableau, signIn);
                requestView.ExecuteRequest();
                var findView = requestView.Views.FirstOrDefault(x => x.Name == this.ObjDetail.VIEW_NAME);
                if (findView == null)
                {
                    this.State = false;
                    this.ErrorMessage = "Thông tin view name không đúng!";
                    return;
                }
                this.ObjDetail.VIEW_ID = findView.Id;
                this.ObjDetail.VIEW_CONTENT_URL = findView.ContentUrl.Replace("/sheets/", "/");

                // Download thumnail
                var requestPreview = new DownloadView(urlTableau, signIn);
                var bytePreview = requestPreview.GetPreviewImage(this.ObjDetail.WORKBOOK_ID, this.ObjDetail.VIEW_ID);
                var folderPath = HostingEnvironment.MapPath("\\PreviewViewTableau");
                Directory.CreateDirectory(folderPath);
                using (MemoryStream mStream = new MemoryStream(bytePreview))
                {
                    var image = Image.FromStream(mStream);
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random()).Next().ToString() + ".png";
                    image.Save(Path.Combine(folderPath, fileName));
                    this.ObjDetail.PATH_IMAGE_PREVIEW = fileName;
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public string GetTableauTicket()
        {
            try
            {
                var serviceSystemConfig = new SystemConfigService();
                serviceSystemConfig.GetConfig();

                var protocol = ServerProtocol.Http;
                if (serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PROTOCOL.ToUpper() == "HTTPS")
                {
                    protocol = ServerProtocol.Https;
                }
                var urlTableau = new TableauServerUrls(protocol,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_URL_LOCALHOST,
                    "", 1000, ServerVersion.Server9,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_API_VERSION);

                var signIn = new TableauServerSignIn(urlTableau,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_USER,
                    serviceSystemConfig.ObjDetail.TABLEAU_SERVER_PASSWORD, new TaskStatusLogs());

                var ticket = new TableauServerTicket(urlTableau, signIn);
                return ticket.Ticket();
            }
            catch
            {
                return "";
                //return ex.ToString();
            }
        }

        public override void Create()
        {
            this.ValidateTableau();
            if (!this.State)
            {
                return;
            }

            this.ObjDetail.PKID = Guid.NewGuid().ToString();
            base.Create();
        }

        public override void Update()
        {
            this.ValidateTableau();
            if (!this.State)
            {
                return;
            }
            base.Update();
        }

        public void UpdateTree(List<NodeTableau> lstNode)
        {
            try
            {
                var strSql = "";
                var order = 0;
                UnitOfWork.BeginTransaction();

                foreach (var item in lstNode)
                {
                    var isParent = lstNode.Count(x => x.pId == item.id) > 0;
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql = "UPDATE T_CF_TABLEAU SET C_ORDER = :C_ORDER, PARENT_ID = :PARENT_ID WHERE PKID = :PKID";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PARENT_ID", item.pId)
                            .SetParameter("PKID", item.id)
                            .ExecuteUpdate();
                    }
                    else
                    {
                        strSql = "UPDATE T_CF_TABLEAU SET C_ORDER = :C_ORDER, PARENT_ID = '' WHERE PKID = :PKID";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PKID", item.id)
                            .ExecuteUpdate();
                    }
                    order++;
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                foreach (var item in lstId)
                {
                    UnitOfWork.BeginTransaction();
                    if (!CheckExist(x => x.PARENT_ID == item))
                    {
                        CurrentRepository.Delete(item);
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "Nhóm này đang chứa các biểu đồ!";
                        UnitOfWork.Rollback();
                        return;
                    }
                    UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }

        }
    }
}
