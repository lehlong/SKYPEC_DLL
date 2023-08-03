using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class RightService : GenericService<T_AD_RIGHT, RightRepo>
    {
        public RightService() : base()
        {

        }

        public List<NodeRight> BuildTree()
        {
            var lstNode = new List<NodeRight>();
            GetAll();
            foreach (var item in ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
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
                    base.Create();
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

        public void UpdateTree(List<NodeRight> lstNode)
        {
            try
            {
                var strSql = new StringBuilder();
                var order = 0;
                foreach (var item in lstNode)
                {
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql.AppendFormat("UPDATE T_AD_RIGHT SET C_ORDER = {0}, PARENT = N'{1}' WHERE CODE = N'{2}';", order, item.pId, item.id);
                    }
                    else
                    {
                        strSql.AppendFormat("UPDATE T_AD_RIGHT SET C_ORDER = {0} WHERE CODE = N'{1}';", order, item.id);
                    }
                    order++;
                }
                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().CreateSQLQuery(strSql.ToString()).ExecuteUpdate();
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
                    if (!CheckExist(x => x.PARENT == item))
                    {
                        CurrentRepository.Delete(item);
                    }
                    else
                    {
                        State = false;
                        ErrorMessage = "Quyền này đang là cha của các quyền khác.";
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
