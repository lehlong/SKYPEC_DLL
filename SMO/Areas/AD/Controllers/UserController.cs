using SMO.Service.AD;

using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.AD.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _service;

        public UserController()
        {
            _service = new UserService();
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult IndexOrganize()
        {
            return PartialView(_service);
        }


        [AuthorizeCustom(Right = "R140")]
        public ActionResult BuildTreeUser(string idUserSelected)
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            var lstNodeUser = _service.BuildTreeUser();
            ViewBag.zNodeUser = oSerializer.Serialize(lstNodeUser);
            ViewBag.IdUserSelected = idUserSelected;
            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTreeRight(string userName, string orgCode)
        {
            var lstNode = _service.BuildTreeRight(userName, orgCode);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTreeOrg(string userName)
        {
            var lstNode = _service.BuildTreeOrg(userName);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            return PartialView();
        }

        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult List(UserService service)
        {
            service.Search();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserGroupOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserUserGroup = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserUserGroup = true;
            param.IsFetch_ListUserRole = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleForAdd(UserService service)
        {
            service.SearchRoleForAdd();
            return PartialView(service);
        }


        [MyValidateAntiForgeryToken]
        public ActionResult AddRole(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddRoleToUser(string lstRole, string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.AddRoleToUser(lstRole, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListRoleForAdd(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteRoleOfUser(string lstRole, string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.DeleteRoleOfUser(lstRole, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserGroupForAdd(UserService service)
        {
            service.SearchUserGroupForAdd();
            return PartialView(service);
        }

        public ActionResult AddUserGroup(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddUserGroupToUser(string lstUserGroup, string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.AddUserGroupToUser(lstUserGroup, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListUserGroupForAdd(); SubmitListUserGroupOfUser(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult CreateUserOrganzie(string parent)
        {
            _service.ObjDetail.ORGANIZE_CODE = parent;
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult CreateUserOrganzie(UserService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.CreateUserOrganzie();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"BuildTreeUser('{service.ObjDetail.USER_NAME}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }


        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                dynamic param = new ExpandoObject();
                param.IsFetch_ListUserUserGroup = true;
                param.IsFetch_ListUserRight = true;
                param.IsFetch_ListUserRole = true;
                param.IsFetch_ListUserCustomer = true;
                param.IsFetch_Contractor = true;
                _service.Get(id, param);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult EditRightOfUser(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                dynamic param = new ExpandoObject();
                param.IsFetch_ListUserOrg = true;
                _service.Get(userName, param);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R140")]
        public ActionResult EditOrgOfUser(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateOrgOfUser(string userName, string orgList)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateOrgOfUser(userName, orgList);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "LoadTabDanhSachQuyen();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateRightOfUser(string userName, string rightList, string statusList, string orgCode)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateRightOfUser(userName, rightList, statusList, orgCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "BuildTreeRight(); $('#cmdResetQuyen').show();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult ResetRightOfUser(string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ResetRightOfUser(userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "BuildTreeRight(); $('#cmdResetQuyen').hide();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult ResetPassword(string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ResetPassword(userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = $"BuildTreeUser('{service.ObjDetail.USER_NAME}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.Delete(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteUserGroupOfUser(string lstUserGroup, string userName)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.DeleteUserGroupOfUser(lstUserGroup, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListUserGroupOfUser(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult ToggleActive(string id)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ToggleActive(id);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        public ActionResult UpdatePhongBan(List<NodeUser> lstNode)
        {
            var result = new TransferObject
            {
                State = true,
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdatePhongBan(lstNode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}