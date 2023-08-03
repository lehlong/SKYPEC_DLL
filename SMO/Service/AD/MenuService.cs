using NHibernate.Transform;

using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.AD
{
    public class MenuService : GenericService<T_AD_MENU, MenuRepo>
    {
        public List<T_AD_MENU> ListMenuRole { get; set; }
        public MenuService() : base()
        {
            ListMenuRole = new List<T_AD_MENU>();
        }

        public List<NodeMenu> BuildTree()
        {
            var lstNode = new List<NodeMenu>();
            GetAll();
            foreach (var item in ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeMenu()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.DESCRIPTION
                };
                lstNode.Add(node);
            }
            return lstNode;
        }

        public override void Create()
        {
            try
            {
                if (!CheckExist(x => x.CODE == ObjDetail.CODE))
                {
                    try
                    {
                        UnitOfWork.BeginTransaction();
                        if (ProfileUtilities.User != null)
                        {
                            ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                            ObjDetail.CREATE_DATE = CurrentRepository.GetDateDatabase();
                        }

                        CurrentRepository.Create(ObjDetail);

                        var lstLang = UnitOfWork.Repository<DictionaryRepo>().Queryable().Where(x => x.FK_DOMAIN == "LANG" && x.LANG == "vi").ToList();
                        foreach (var item in lstLang)
                        {
                            var obj = new T_AD_LANGUAGE()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                FK_CODE = ObjDetail.CODE,
                                OBJECT_TYPE = "M",
                                LANG = item.CODE,
                                VALUE = ObjDetail.CODE
                            };
                            UnitOfWork.Repository<LanguageRepo>().Create(obj);
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
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        public void UpdateTree(List<NodeMenu> lstNode)
        {
            try
            {
                var strSql = "";
                var order = 0;
                UnitOfWork.BeginTransaction();

                foreach (var item in lstNode)
                {
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql = "UPDATE T_AD_MENU SET C_ORDER = :C_ORDER, PARENT = :PARENT WHERE CODE = :CODE";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PARENT", item.pId)
                            .SetParameter("CODE", item.id)
                            .ExecuteUpdate();
                    }
                    else
                    {
                        strSql = "UPDATE T_AD_MENU SET C_ORDER = :C_ORDER, PARENT = '' WHERE CODE = :CODE";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("CODE", item.id)
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
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();

                var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => lstId.Contains(x.FK_CODE) && x.OBJECT_TYPE == "M").ToList();
                UnitOfWork.Repository<LanguageRepo>().Delete(lstLang);
                CurrentRepository.Delete(lstId);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void GetMenuRole()
        {
            GetAllMenuWithLanguage();
            if (ProfileUtilities.User.IS_IGNORE_USER)
            {
                ListMenuRole = ObjList;
                return;
            }

            var lstMenuRoleTemp = new List<T_AD_MENU>();

            foreach (var item in ProfileUtilities.UserRight)
            {
                var menu = ObjList.Where(x => x.FK_RIGHT == item.CODE).ToList();
                if (menu != null)
                {
                    lstMenuRoleTemp.AddRange(menu);
                }
            }

            foreach (var item in lstMenuRoleTemp)
            {
                GetParent(item);
            }
            ListMenuRole = ListMenuRole.OrderBy(x => x.C_ORDER).ToList();
        }

        public void GetParent(T_AD_MENU item)
        {
            foreach (var menu in ObjList)
            {
                if (item.CODE == menu.CODE)
                {
                    if (!ListMenuRole.Any(x => x.CODE == menu.CODE))
                    {
                        ListMenuRole.Add(menu);
                    }
                    if (!string.IsNullOrWhiteSpace(menu.PARENT))
                    {
                        var parent = ObjList.FirstOrDefault(x => x.CODE == menu.PARENT);
                        GetParent(parent);
                    }
                    break;
                }
            }
        }

        public void GetAllMenuWithLanguage()
        {
            string strSql = @"SELECT A.* , B.VALUE AS LANGUAGE_VALUE
                              FROM T_AD_MENU A LEFT JOIN T_AD_LANGUAGE B
                                ON A.CODE = B.FK_CODE AND B.OBJECT_TYPE = 'M' AND B.LANG = '{0}' ORDER BY A.C_ORDER";
            strSql = string.Format(strSql, ProfileUtilities.User.LANGUAGE);
            ObjList = UnitOfWork.GetSession().CreateSQLQuery(strSql)
                .SetResultTransformer(Transformers.AliasToBean(typeof(T_AD_MENU)))
                .List<T_AD_MENU>().ToList();
        }
    }
}
