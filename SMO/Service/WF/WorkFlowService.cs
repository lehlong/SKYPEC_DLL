using SMO.Core.Entities.WF;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.WF;
using SMO.Service.Class.WF;
using SMO.Service.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.WF
{
    public class WorkFlowService : GenericService<T_WF_PROCESS, ProcessRepo>
    {
        public T_WF_PROCESS ObjProcess { get; set; }
        public T_WF_ACTIVITY ObjActivity { get; set; }
        public T_WF_ACTIVITY_COM ObjCom { get; set; }
        public WorkFlowService() : base()
        {
            ObjProcess = new T_WF_PROCESS();
            ObjActivity = new T_WF_ACTIVITY();
            ObjCom = new T_WF_ACTIVITY_COM();
        }

        public List<NodeWorkFlow> BuildTree()
        {
            var lstNode = new List<NodeWorkFlow>();
            GetAll();
            var idParentAll = Guid.NewGuid().ToString();
            lstNode.Add(new NodeWorkFlow()
            {
                id = idParentAll,
                name = "Danh sách workflow",
                type = "ALL"
            });

            foreach (var process in ObjList)
            {
                lstNode.Add(new NodeWorkFlow()
                {
                    id = process.CODE,
                    name = process.NAME,
                    type = WorkFlowType.Process.ToString(),
                    pId = idParentAll
                });

                foreach (var activity in process.Activities.OrderBy(x => x.C_ORDER))
                {
                    lstNode.Add(new NodeWorkFlow()
                    {
                        id = activity.CODE,
                        name = activity.NAME,
                        pId = activity.PROCESS_CODE,
                        type = WorkFlowType.Activity.ToString()
                    });
                }
            }

            return lstNode;
        }

        public void GetProcessById(string id)
        {
            ObjProcess = UnitOfWork.Repository<ProcessRepo>().Get(id);
        }

        public void GetActivityById(string id)
        {
            ObjActivity = UnitOfWork.Repository<ActivityRepo>().Get(id);
        }

        public void GetComById(string id)
        {
            ObjCom = UnitOfWork.Repository<ActivityComRepo>().Get(id);
        }

        public void CreateProcess()
        {
            try
            {
                ObjProcess.ACTIVE = true;
                if (!UnitOfWork.Repository<ProcessRepo>().CheckExist(x => x.CODE == ObjProcess.CODE))
                {
                    UnitOfWork.BeginTransaction();
                    ObjProcess.CREATE_BY = ProfileUtilities.User?.USER_NAME;
                    UnitOfWork.Repository<ProcessRepo>().Create(ObjProcess);
                    UnitOfWork.Commit();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void CreateActivity()
        {
            try
            {
                if (!UnitOfWork.Repository<ActivityRepo>().CheckExist(x => x.CODE == ObjActivity.CODE))
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.Repository<ActivityRepo>().Create(ObjActivity);
                    UnitOfWork.Commit();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void UpdateProcess()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                if (ProfileUtilities.User != null)
                {
                    ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                }
                UnitOfWork.Repository<ProcessRepo>().Update(ObjProcess);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void UpdateActivity()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                if (ProfileUtilities.User != null)
                {
                    ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                }
                UnitOfWork.Repository<ActivityRepo>().Update(ObjActivity);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        internal void UpdateUserInformation(ConfigSendViewModel model)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var activityUserRepo = UnitOfWork.Repository<ActivityUserRepo>();
                if (model.Receivers.Count == 0 || model.Receivers == null)
                {
                    // delete all
                    activityUserRepo.Delete(x => x.ACTIVITY_CODE == model.ActivityCode
                    && x.ORG_CODE == model.OrgCode
                    && model.Sender == x.USER_SENDER);
                }
                else
                {

                    // find old receivers
                    var currentReceivers = activityUserRepo
                        .GetManyWithFetch(x => x.ACTIVITY_CODE == model.ActivityCode
                        && x.ORG_CODE == model.OrgCode
                        && model.Sender == x.USER_SENDER);

                    // delete receiver
                    var lstDeleteReceivers = currentReceivers.Where(x => !model.Receivers.Contains(x.USER_RECEIVER));
                    activityUserRepo.Delete(lstDeleteReceivers.ToList());

                    // lst new receivers
                    var lstNewReceivers = model.Receivers.Where(x => !currentReceivers.Select(y => y.USER_RECEIVER).Contains(x));

                    var currentUser = ProfileUtilities.User.USER_NAME;
                    // create new receivers
                    activityUserRepo.Create((from receiver in lstNewReceivers
                                             select new T_WF_ACTIVITY_USER
                                             {
                                                 ACTIVITY_CODE = model.ActivityCode,
                                                 ORG_CODE = model.OrgCode,
                                                 USER_SENDER = model.Sender,
                                                 USER_RECEIVER = receiver,
                                                 PKID = Guid.NewGuid().ToString(),
                                                 CREATE_BY = currentUser
                                             }).ToList());
                }
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
            }
        }

        public void UpdateCom()
        {
            if (UnitOfWork.Repository<ActivityComRepo>().CheckExist(x => x.TYPE_NOTIFY == ObjCom.TYPE_NOTIFY && x.ACTIVITY_CODE == ObjCom.ACTIVITY_CODE))
            {
                State = false;
                MesseageCode = "1101";
            }
            else
            {
                try
                {
                    UnitOfWork.BeginTransaction();
                    if (ProfileUtilities.User != null)
                    {
                        ObjCom.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    }
                    ObjCom.CONTENTS = ObjCom.CONTENTS.Substring(3);
                    ObjCom.CONTENTS = ObjCom.CONTENTS.Substring(ObjCom.CONTENTS.Length - 4);
                    UnitOfWork.Repository<ActivityComRepo>().Update(ObjCom);
                    UnitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    UnitOfWork.Rollback();
                    State = false;
                    Exception = ex;
                }
            }
        }

        public void ToggleActiveCom(object id)
        {
            try
            {
                var com = UnitOfWork.Repository<ActivityComRepo>().Get(id);
                UnitOfWork.BeginTransaction();

                if (ProfileUtilities.User != null)
                {
                    com.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                }

                com.ACTIVE = !com.ACTIVE;
                UnitOfWork.Repository<ActivityComRepo>().Update(com);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        /// <summary>
        /// Tạo mới communication với obj là ObjCom
        /// </summary>
        internal void SaveCom()
        {
            if (string.IsNullOrEmpty(ObjCom.CONTENTS) ||
            string.IsNullOrEmpty(ObjCom.ACTIVITY_CODE) ||
            string.IsNullOrEmpty(ObjCom.TYPE_NOTIFY))
            {
                State = false;
                ErrorMessage = "Không được để trống các trường.";
                return;
            }
            try
            {
                if (!string.IsNullOrEmpty(ObjCom.PKID))
                {
                    UnitOfWork.BeginTransaction();
                    var find = UnitOfWork.Repository<ActivityComRepo>().Get(ObjCom.PKID);
                    // update
                    find.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    ObjCom.CONTENTS = ObjCom.CONTENTS.Substring(3);
                    ObjCom.CONTENTS = ObjCom.CONTENTS.Substring(0, ObjCom.CONTENTS.Length - 4);
                    find.CONTENTS = ObjCom.CONTENTS;
                    find.SUBJECT = ObjCom.SUBJECT;
                    find.TYPE_NOTIFY = ObjCom.TYPE_NOTIFY;
                    UnitOfWork.Repository<ActivityComRepo>().Update(find);
                    UnitOfWork.Commit();
                }
                else if (!UnitOfWork.Repository<ActivityComRepo>().CheckExist(x => x.TYPE_NOTIFY == ObjCom.TYPE_NOTIFY && x.ACTIVITY_CODE == ObjCom.ACTIVITY_CODE))
                {
                    UnitOfWork.BeginTransaction();
                    ObjCom.PKID = Guid.NewGuid().ToString();
                    ObjCom.CREATE_BY = ProfileUtilities.User.USER_NAME;
                    ObjCom.ACTIVE = true;
                    UnitOfWork.Repository<ActivityComRepo>().Create(ObjCom);
                    UnitOfWork.Commit();
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        internal IList<string> GetDetailInformationSenders(string workflow, string organize)
        {
            return UnitOfWork.Repository<ActivityUserRepo>()
                .GetManyWithFetch(x => x.ACTIVITY_CODE == workflow && x.ORG_CODE == organize)
                .Select(x => x.USER_SENDER)
                .Distinct()
                .ToList();
        }

        internal IList<NodeWorkFlow> GetNodeWorkflow()
        {
            var lstNode = new List<NodeWorkFlow>();
            GetAll();

            foreach (var process in ObjList)
            {
                lstNode.Add(new NodeWorkFlow()
                {
                    id = process.CODE,
                    name = process.NAME,
                    type = WorkFlowType.Process.ToString(),
                    pId = null,
                    open = true.ToString()
                });

                foreach (var activity in process.Activities.OrderBy(x => x.C_ORDER))
                {
                    lstNode.Add(new NodeWorkFlow()
                    {
                        id = activity.CODE,
                        name = activity.NAME,
                        pId = activity.PROCESS_CODE,
                        type = WorkFlowType.Activity.ToString()
                    });
                }
            }
            return lstNode;
        }

        internal IList<string> GetDetailInformationWorkflow(string organize)
        {
            return UnitOfWork.Repository<ActivityUserRepo>()
                            .GetManyWithFetch(x => x.ORG_CODE == organize)
                            .Select(x => x.ACTIVITY_CODE)
                            .Distinct()
                            .ToList();
        }

        internal IList<string> GetDetailInformationReceivers(string workflow, string organize, string sender)
        {
            return UnitOfWork.Repository<ActivityUserRepo>()
                .GetManyWithFetch(x => x.ACTIVITY_CODE == workflow && x.ORG_CODE == organize && x.USER_SENDER == sender)
                .Select(x => x.USER_RECEIVER)
                .Distinct()
                .ToList();
        }

        internal IList<NodeOrganize> GetNodeOrganize()
        {
            var lstNode = new List<NodeOrganize>();
            var organizes = UnitOfWork.Repository<CostCenterRepo>().GetAll();

            var organizeConfig = UnitOfWork.Repository<ActivityUserRepo>()
                .GetAll().Select(x => x.ORG_CODE)
                .Distinct()
                .ToList();
            foreach (var item in organizes.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeOrganize()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    open = string.IsNullOrEmpty(item.PARENT_CODE).ToString(),
                    name = item.NAME,
                    type = WorkFlowType.Organize.ToString(),
                    icon = "/Content/zTreeStyle/img/diy/donvi.gif"
                };
                node.@checked = organizeConfig.Contains(node.id).ToString();
                lstNode.Add(node);
            }
            return lstNode;
        }

        internal IList<NodeUser> GetNodeUser()
        {
            var lstNode = new List<NodeUser>();
            var serviceOrganize = new CostCenterService();
            serviceOrganize.GetAll();
            foreach (var organize in serviceOrganize.ObjList.OrderBy(x => x.C_ORDER))
            {
                var nodeOrganize = new NodeUser()
                {
                    id = organize.CODE,
                    pId = organize.PARENT_CODE,
                    open = string.IsNullOrEmpty(organize.PARENT_CODE).ToString(),
                    name = organize.NAME,
                    icon = "/Content/zTreeStyle/img/diy/donvi.gif",
                    type = "Organize"
                };
                lstNode.Add(nodeOrganize);
                foreach (var user in organize.ListUser.OrderBy(x => x.USER_NAME))
                {
                    var nodeUser = new NodeUser()
                    {
                        id = user.USER_NAME,
                        pId = organize.CODE,
                        name = user.FULL_NAME + "<span class='spMaOnTree'>" + user.USER_NAME + "</span>",
                        userName = user.USER_NAME,
                        fullName = user.FULL_NAME,
                        icon = "/Content/zTreeStyle/img/diy/user.gif",
                        type = WorkFlowType.Sender.ToString()
                    };
                    lstNode.Add(nodeUser);
                }
            }
            return lstNode;
        }

        internal void DeleteComs(string strListSelected)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var lstId = strListSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.Repository<ActivityComRepo>().Delete(lstId);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        /// <summary>
        /// Lấy danh sách các giao thức dựa vào mã activity
        /// </summary>
        /// <param name="activityCode">Mã activity</param>
        /// <returns></returns>
        internal IList<T_WF_ACTIVITY_COM> GetComs(string activityCode)
        {
            return UnitOfWork.Repository<ActivityComRepo>().GetManyWithFetch(x => x.ACTIVITY_CODE == activityCode);
        }
    }
}
