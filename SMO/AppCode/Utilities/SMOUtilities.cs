using Microsoft.AspNet.SignalR;

using Newtonsoft.Json;

using NHibernate.Proxy;

using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Hubs;
using SMO.Service;
using SMO.Service.AD;
using SMO.Service.CM;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SMO
{
    public class SMOUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgCode"></param>
        /// <param name="service"></param>
        /// <param name="transferObject"></param>
        /// <param name="paramObject"></param>
        public static void GetMessage(string msgCode, BaseService service, TransferObject transferObject, params object[] paramObject)
        {
            var id = "ERR" + DateTime.Now.ToString("yyMMddHHmmssFF");
            var code = msgCode;
            if (!string.IsNullOrWhiteSpace(service.MesseageCode))
            {
                code = service.MesseageCode;
            }
            transferObject.State = service.State;
            transferObject.Message.Code = code;
            transferObject.Message.Message = MessageUtilities.GetMessage(code);
            if (!string.IsNullOrWhiteSpace(transferObject.Message.Message))
            {
                transferObject.Message.Message = string.Format(transferObject.Message.Message, paramObject);
            }

            if (!string.IsNullOrWhiteSpace(service.ErrorMessage))
            {
                transferObject.Message.Detail = service.ErrorMessage + "<br/>";
            }

            if (service.Exception != null)
            {
                transferObject.Message.Detail += service.Exception.ToString();
                //transferObject.Message.Detail += "Xem chi tiết lỗi theo mã lỗi sau : " + id;
                Log.Error(id + Environment.NewLine + service.Exception.ToString());
            }
        }

        public static void GetMessage(string msgCode, TransferObject transferObject, params object[] paramObject)
        {
            transferObject.Message.Code = msgCode;
            transferObject.Message.Message = MessageUtilities.GetMessage(msgCode);
            if (!string.IsNullOrWhiteSpace(transferObject.Message.Message))
            {
                transferObject.Message.Message = string.Format(transferObject.Message.Message, paramObject);
            }
        }

        /// <summary>
        /// Lấy dữ liệu nhiều bảng, chỉ với một lần kết nối với database
        /// Params : listSelect là một List<SqlSelectMutil>. Cho nên khi khởi tạo dữ liệu phải thật cẩn thận. Dynamic object gồm 2 thuộc tính : Table, Where
        /// Khai báo như sau (chú ý : nếu có mệnh đề where thì object.Where = "CONDITION" - không cần thêm chữ WHERE ở condition)
        /// var lstSelect = new List<SqlSelectMutil>();
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_AD_ROLE", Where = ""});
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_AD_USER", Where = "1=1" });
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_MD_BATCH", Where = "" });
        /// </summary>
        /// <param name="listSelect"></param>
        /// <returns>Dataset chứa các table kết quả. Các table đó có TableName = object.Table đã được truyền vào</returns> 
        public static DataSet GetMultilpleTable(List<SqlSelectMutil> listSelect)
        {
            DataSet dsResult = new DataSet();
            if (listSelect.Count > 0)
            {
                var strSelect = new System.Text.StringBuilder();
                foreach (var item in listSelect)
                {
                    if (string.IsNullOrEmpty(item.SQL))
                    {
                        var select = item.Select;
                        if (string.IsNullOrWhiteSpace(item.Select))
                        {
                            select = "*";
                        }
                        if (string.IsNullOrWhiteSpace(item.Where))
                        {
                            strSelect.Append(string.Format(" SELECT {0} FROM {1};", select, item.Table));
                        }
                        else
                        {
                            strSelect.Append(string.Format(" SELECT {0} FROM {1} WHERE {2};", select, item.Table, item.Where));
                        }
                    }
                    else
                    {
                        strSelect.Append(item.SQL);
                    }
                }

                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString))
                {
                    SqlCommand objCmd = new SqlCommand(strSelect.ToString())
                    {
                        Connection = objConn,
                        CommandType = CommandType.Text
                    };
                    try
                    {
                        objConn.Open();
                        var adapter = new SqlDataAdapter(objCmd);
                        adapter.Fill(dsResult);
                        int i = 0;
                        foreach (var item in listSelect)
                        {
                            dsResult.Tables[i].TableName = item.Table;
                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objConn.Close();
                    }
                }
            }
            return dsResult;
        }

        /// <summary>
        /// Kiểm tra class is Proxy do Nhibernate tạo ra
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsProxy(object obj)
        {
            return NHibernateProxyHelper.IsProxy(obj);
        }

        public static void SendNotify(List<string> lstUserNotify)
        {
            JsonSerializerSettings serializerSetting = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            foreach (var user in lstUserNotify)
            {
                Thread thread = new Thread(() =>
                {
                    var serviceNotify = new NotifyService();
                    serviceNotify.GetNotifyOfUser(user);
                    if (serviceNotify.IntCountNew > 0)
                    {
                        hubContext.Clients.Group(user).RefreshNotify(JsonConvert.SerializeObject(serviceNotify, serializerSetting));
                    }
                });
                thread.Start();
            }
        }

        public static string BytesToSize(double bytes)
        {
            var sizes = new string[] { "Bytes", "KB", "MB", "GB", "TB" };
            if (bytes == 0) return "0 Byte";
            var i = Convert.ToInt32(Math.Floor(Math.Log(bytes) / Math.Log(1024)));
            return (Math.Round(bytes / Math.Pow(1024, i), 2)).ToString() + ' ' + sizes[i];
        }

        public static string GetIconOfFile(string ext)
        {
            ext = ext.ToLower();
            var icon = "file-48.png";
            switch (ext)
            {
                case ".xlsx":
                    icon = "excel-48.png";
                    break;
                case ".xls":
                    icon = "excel-48.png";
                    break;
                case ".docx":
                    icon = "word-48.png";
                    break;
                case ".doc":
                    icon = "word-48.png";
                    break;
                case ".pptx":
                    icon = "powerpoint-48.png";
                    break;
                case ".ppt":
                    icon = "powerpoint-48.png";
                    break;

                case ".txt":
                    icon = "txt-48.png";
                    break;

                case ".zip":
                    icon = "zip-48.png";
                    break;
                case ".7z":
                    icon = "zip-48.png";
                    break;
                case ".rar":
                    icon = "zip-48.png";
                    break;

                case ".jpg":
                    icon = "image-48.png";
                    break;
                case ".png":
                    icon = "image-48.png";
                    break;
                case ".bmp":
                    icon = "image-48.png";
                    break;
                case ".jpeg":
                    icon = "image-48.png";
                    break;

                case ".psd":
                    icon = "photoshop-48.png";
                    break;
                case ".dwg":
                    icon = "dwg-48.png";
                    break;
                case ".dxf":
                    icon = "dxf-48.png";
                    break;

                case ".pdf":
                    icon = "pdf-48.png";
                    break;

                default:
                    break;
            }
            return icon;
        }

        public static string SendEmail()
        {
            string strResult = "";
            try
            {
                var service = new EmailNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_HOST) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_PASSWORD) ||
                    systemConfig.ObjDetail.MAIL_PORT == 0 ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_USER))
                {
                    return "Chưa cấu hình hệ thống Email";
                }


                var checkDate = DateTime.Now.AddDays(-2);
                var lstEmail = service.UnitOfWork.GetSession().QueryOver<T_CM_EMAIL>().Where(
                        x => !x.IS_SEND &&
                        x.CREATE_DATE > checkDate
                        // && x.NUMBER_RETRY < 6
                    ).OrderBy(x => x.CREATE_DATE).Asc
                    .Take(10).Skip(0)
                    .List();
                foreach (var item in lstEmail)
                {
                    try
                    {
                        SmtpClient client = new SmtpClient
                        {
                            Host = systemConfig.ObjDetail.MAIL_HOST,
                            Port = systemConfig.ObjDetail.MAIL_PORT,
                            EnableSsl = systemConfig.ObjDetail.MAIL_IS_SSL,
                            UseDefaultCredentials = false,
                            DeliveryMethod = SmtpDeliveryMethod.Network
                        };

                        NetworkCredential credential = new NetworkCredential
                        {
                            UserName = systemConfig.ObjDetail.MAIL_USER,
                            Password = systemConfig.ObjDetail.MAIL_PASSWORD
                        };
                        client.Credentials = credential;

                        MailMessage message = new MailMessage
                        {
                            From = new MailAddress(systemConfig.ObjDetail.MAIL_USER, "FECON-EBIDDING")
                        };
                        foreach (var email in item.EMAIL.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.To.Add(email);
                        }

                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;
                        message.Subject = item.SUBJECT;
                        message.Body = item.CONTENTS;

                        client.SendCompleted += (s, e) =>
                        {
                            if (e.Error != null)
                            {
                                strResult += $"EMAIL {item.PKID} : {item.EMAIL}  bị lỗi : {e.Error.ToString()}";
                                item.NUMBER_RETRY++;
                            }
                            else
                            {
                                item.IS_SEND = true;
                            }
                        };
                        //client.Send(message);
                        client.SendMailAsync(message).Wait();
                    }
                    catch (Exception ex)
                    {
                        item.IS_SEND = false;
                        item.NUMBER_RETRY++;
                        strResult += $"EMAIL {item.PKID} : {item.EMAIL} bị lỗi : {ex}";
                    }
                    item.IS_SEND = true;
                    service.ObjDetail = item;
                    service.Update();
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }

            return strResult;
        }

        #region Compare versions
        /// <summary>
        /// compare 2 list in multiple centers
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="lstSource"></param>
        /// <param name="lstCompare"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IDictionary<string, DiffType> CompareVersion<E>(IList<E> lstSource,
                                                                 IList<E> lstCompare,
                                                                 string prefix = "") where E : CoreElement
        {
            var resultDiff = new Dictionary<string, DiffType>();

            var lookupSource = lstSource.ToLookup(x => x.CENTER_CODE);
            var lookupCompare = lstCompare.ToLookup(x => x.CENTER_CODE);
            foreach (var key in lookupSource.Select(x => x.Key))
            {
                if (lookupCompare[key].Count() == 0)
                {
                    resultDiff.Add($"{prefix}{key}", DiffType.ADD);
                }
                else
                {
                    var diffs = CompareVersion(lookupSource[key], lookupCompare[key], $"{prefix}{key}_");
                    foreach (var diff in diffs)
                    {
                        resultDiff.Add(diff.Key, diff.Value);
                    }
                }
            }

            return resultDiff;
        }

        /// <summary>
        /// compare 2 list with the same center
        /// </summary>
        /// <typeparam name="E">type of core element</typeparam>
        /// <param name="lstSource"></param>
        /// <param name="lstCompare"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static IDictionary<string, DiffType> CompareVersion<E>(
            IEnumerable<E> lstSource,
            IEnumerable<E> lstCompare,
            string prefix) where E : CoreElement
        {
            var resultDiff = new Dictionary<string, DiffType>();

            foreach (var item in lstSource)
            {
                var itemDiff = lstCompare.FirstOrDefault(x => x.CODE == item.CODE);
                if (itemDiff == null)
                {
                    resultDiff.Add($"{prefix}{item.CODE}", DiffType.ADD);
                }
                else
                {
                    var diffs = item.CompareObject(itemDiff, $"{prefix}{item.CODE}_");
                    foreach (var diff in diffs)
                    {
                        resultDiff.Add(diff.Key, diff.Value);
                    }
                    // remove item in compare lst
                    lstCompare = lstCompare.Where(x => x.CODE != item.CODE);
                }
            }

            foreach (var item in lstCompare)
            {
                resultDiff.Add($"{prefix}{item.CODE}", DiffType.DELELTE);
            }

            return resultDiff;
        }
        #endregion

        public static bool CheckRight(string right)
        {
            if (ProfileUtilities.User.IS_IGNORE_USER)
            {
                return true;
            }
            if (ProfileUtilities.UserRight.Exists(x => x.CODE == right))
            {
                return true;
            }
            return false;
        }
    }
}