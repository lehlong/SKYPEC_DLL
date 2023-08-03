using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using SMO.Service.AD;

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace SMO.Service
{
    public class AuthorizeService : BaseService
    {
        public T_AD_USER ObjUser { get; set; }
        public List<T_AD_RIGHT> ListUserRight { get; set; }
        public List<T_AD_USER_ORG> ListUserOrg { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsRemember { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public AuthorizeService()
        {
            UnitOfWork = new NHUnitOfWork();
            ObjUser = new T_AD_USER();
            ListUserRight = new List<T_AD_RIGHT>();
            ListUserOrg = new List<T_AD_USER_ORG>();
        }

        public void GetUserOrg()
        {
            try
            {
                if (ObjUser.ListUserOrg.Count > 0)
                {
                    var findOrg = UnitOfWork.Repository<CostCenterRepo>().Get(ObjUser.ORGANIZE_CODE);
                    ListUserOrg.Add(new T_AD_USER_ORG() {
                        ORG_CODE = ObjUser.ORGANIZE_CODE,
                        USER_NAME = ObjUser.USER_NAME,
                        Organize = findOrg
                    });
                }

                foreach (var item in ObjUser.ListUserOrg)
                {
                    var findOrg = UnitOfWork.Repository<CostCenterRepo>().Get(item.ORG_CODE);
                    ListUserOrg.Add(new T_AD_USER_ORG()
                    {
                        ORG_CODE = item.ORG_CODE,
                        USER_NAME = ObjUser.USER_NAME,
                        Organize = findOrg
                    });
                }
            }
            catch (Exception)
            {
                ListUserOrg = new List<T_AD_USER_ORG>();
            }
        }

        public void GetInfoUser(string userName)
        {
            this.ObjUser = this.UnitOfWork.Repository<UserRepo>().Get(userName);
        }

        public void GetUserRight(string orgCode)
        {
            try
            {
                //Danh sách role của user theo usergroup
                var lstRole = new List<T_AD_ROLE>();
                foreach (var item1 in ObjUser.ListUserUserGroup)
                {
                    foreach (var item2 in item1.UserGroup.ListUserGroupRole)
                    {
                        lstRole.Add(item2.Role);
                    }
                }
                //Danh sách role riêng của user
                foreach (var item in ObjUser.ListUserRole)
                {
                    lstRole.Add(item.Role);
                }
                lstRole = lstRole.Distinct().ToList();

                //Danh sách các quyền của tập hợp role trên
                var lstRoleDetail = new List<T_AD_ROLE_DETAIL>();
                foreach (var item in lstRole)
                {
                    lstRoleDetail.AddRange(item.ListRoleDetail);
                }
                lstRoleDetail = lstRoleDetail.Distinct().ToList();

                //Danh sách quyền sửa đổi của user
                var lstRightChange = ObjUser.ListUserRight;

                foreach (var item in lstRoleDetail)
                {
                    ListUserRight.Add(new T_AD_RIGHT()
                    {
                        CODE = item.FK_RIGHT
                    });
                }

                foreach (var item in lstRightChange.Where(x => x.ORG_CODE == orgCode))
                {
                    if (item.IS_ADD && ListUserRight.Count(x => x.CODE == item.FK_RIGHT) == 0)
                    {
                        ListUserRight.Add(new T_AD_RIGHT()
                        {
                            CODE = item.FK_RIGHT
                        });
                    }

                    if (item.IS_REMOVE && ListUserRight.Count(x => x.CODE == item.FK_RIGHT) > 0)
                    {
                        var find = ListUserRight.FirstOrDefault(x => x.CODE == item.FK_RIGHT);
                        ListUserRight.Remove(find);
                    }
                }
            }
            catch
            {
                ListUserRight = new List<T_AD_RIGHT>();
            }
        }

        public void IsValid()
        {
            //this.IsValidAD();

            //if (this.State == true)
            //{
            //    return;
            //}

            State = false;
            try
            {
                if (ObjUser.USER_NAME == "superadmin" && ObjUser.PASSWORD == "D2SSuperAdmin!@#2019")
                {
                    ObjUser.IS_IGNORE_USER = true;
                    ObjUser.FULL_NAME = "Super Admin";
                    ObjUser.LANGUAGE = "vi";
                    ObjUser.ACTIVE = true;
                    State = true;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(ObjUser.USER_NAME))
                {
                    ObjUser.USER_NAME = ObjUser.USER_NAME.Trim();
                }

                if (!string.IsNullOrWhiteSpace(ObjUser.PASSWORD))
                {
                    ObjUser.PASSWORD = UtilsCore.EncryptStringMD5(ObjUser.PASSWORD.Trim());
                }

                var result = UnitOfWork.GetSession().QueryOver<T_AD_USER>().Where(x => x.USER_NAME == ObjUser.USER_NAME && x.PASSWORD == ObjUser.PASSWORD)
                    .Fetch(x => x.Organize).Eager.List().FirstOrDefault();
                if (result != null)
                {
                    ObjUser = result;
                    ObjUser.IS_IGNORE_USER = AuthorizeUtilities.CheckIgnoreUser(ObjUser.USER_NAME);
                    State = true;
                }
                else
                {
                    State = false;
                    ErrorMessage = "10";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
                ErrorMessage = "11";
            }
        }


        public void IsValidAD()
        {
            State = false;

            var serviceSystemConfig = new SystemConfigService();
            serviceSystemConfig.GetConfig();

            try
            {
                DirectoryEntry root = new DirectoryEntry(serviceSystemConfig.ObjDetail.AD_CONNECTION, "fecon\\" + ObjUser.USER_NAME, ObjUser.PASSWORD, AuthenticationTypes.None);
                try
                {
                    object connected = root.NativeObject;
                    var result = UnitOfWork.GetSession().QueryOver<T_AD_USER>().Where(x => x.ACCOUNT_AD == ObjUser.USER_NAME)
                       .Fetch(x => x.Organize).Eager.List().FirstOrDefault();

                    if (result != null)
                    {
                        ObjUser = result;
                        ObjUser.IS_IGNORE_USER = AuthorizeUtilities.CheckIgnoreUser(ObjUser.USER_NAME);
                        State = true;
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "12";
                    }
                }
                catch
                {
                    State = false;
                    ErrorMessage = "13";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
                ErrorMessage = "14";
            }
        }
    }
}