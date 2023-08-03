using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class RevenueCFElementService : GenericService<T_MD_REVENUE_CF_ELEMENT, RevenueCFElementRepo>
    {
        internal IList<NodeCostElement> GetNodeCostElement(int year)
        {
            this.ObjDetail.TIME_YEAR = year;
            this.Search();
            // get all cost center
            var lstCostElm = ObjList.OrderBy(x => x.C_ORDER).ToList();
            var lstNode = new List<NodeCostElement>();

            foreach (var item in lstCostElm)
            {
                var node = new NodeCostElement()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    name = $"{item.CODE} - {item.NAME}",
                    type = Budget.COST_ELEMENT.ToString(),
                    isParent = item.IS_GROUP ? "true" : "false",
                    isSap = "false",
                    icon = "/Content/zTreeStyle/img/diy/dong.png"
                };

                lstNode.Add(node);
            }

            return lstNode;
        }

        public List<NodeCostElement> GetNodeSapCostElement(int year)
        {
            this.ObjDetail.TIME_YEAR = year;
            this.Search();
            var lstSapCostElm = UnitOfWork.Repository<SAPMdPaymentRepo>().GetAll().OrderBy(x => x.CODE).ToList();
            var lstNode = new List<NodeCostElement>();

            foreach (var item in lstSapCostElm)
            {
                var find = this.ObjList.FirstOrDefault(x => x.CODE == item.CODE);
                if (find == null || find.IS_GROUP)
                {
                    var node = new NodeCostElement()
                    {
                        id = item.CODE,
                        pId = item.PARENT_CODE ?? string.Empty,
                        name = $"{item.CODE} - {item.NAME}",
                        isParent = lstSapCostElm.Count(x => x.PARENT_CODE == item.CODE) > 0 ? "true" : "false",
                        isSap = "true"
                    };

                    lstNode.Add(node);
                }
            }
            return lstNode;
        }

        public override void Create()
        {
            try
            {
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE && x.TIME_YEAR == this.ObjDetail.TIME_YEAR))
                {
                    ObjDetail.PARENT_CODE = ObjDetail.PARENT_CODE ?? string.Empty;
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

        /// <summary>
        /// Copy cây cấu trúc từ năm này qua năm khác
        /// </summary>
        /// <param name="year"></param>
        /// <param name="yearCopy"></param>
        public void Copy(int year, int yearCopy)
        {
            try
            {
                var find = this.CurrentRepository.GetManyByExpression(x => x.TIME_YEAR == year);
                if (find.Count() > 0)
                {
                    this.State = false;
                    this.ErrorMessage = $"Cấu trúc của năm {year} đã được tạo trước đó rồi! Bạn cần xóa hết khoản mục trước khi copy!";
                    return;
                }

                var findCopy = this.CurrentRepository.GetManyByExpression(x => x.TIME_YEAR == yearCopy);
                if (findCopy.Count() == 0)
                {
                    this.State = false;
                    this.ErrorMessage = $"Cấu trúc của năm {yearCopy} chưa được tạo!";
                    return;
                }

                UnitOfWork.BeginTransaction();
                foreach (var item in findCopy)
                {
                    var newElement = new T_MD_REVENUE_CF_ELEMENT()
                    {
                        CODE = item.CODE,
                        PARENT_CODE = item.PARENT_CODE ?? string.Empty,
                        NAME = item.NAME,
                        C_ORDER = item.C_ORDER,
                        IS_GROUP = item.IS_GROUP,
                        ACTIVE = item.ACTIVE,
                        TIME_YEAR = year
                    };
                    this.CurrentRepository.Create(newElement);
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

        public void UpdateTree(List<NodeCostCenter> lstNode, List<string> lstRemove, List<string> lstAdd, int year)
        {
            try
            {
                lstNode = lstNode ?? new List<NodeCostCenter>();
                lstRemove = lstRemove ?? new List<string>();
                lstAdd = lstAdd ?? new List<string>();
                this.ObjDetail.TIME_YEAR = year;
                this.Search();

                UnitOfWork.BeginTransaction();

                // Xóa phần tử
                foreach (var item in lstRemove)
                {
                    var find = this.ObjList.FirstOrDefault(x => x.CODE == item);
                    if (find != null)
                    {
                        this.CurrentRepository.Delete(find);
                        this.ObjList.Remove(find);
                    }
                }

                //Thêm phần tử mới
                foreach (var item in lstAdd)
                {
                    var findSap = UnitOfWork.Repository<SAPMdPaymentRepo>().Get(item);
                    if (findSap != null)
                    {
                        var newElement = new T_MD_REVENUE_CF_ELEMENT()
                        {
                            CODE = item,
                            PARENT_CODE = string.Empty,
                            NAME = findSap.NAME,
                            IS_GROUP = false,
                            ACTIVE = true,
                            TIME_YEAR = year
                        };
                        this.CurrentRepository.Create(newElement);
                        this.ObjList.Add(newElement);
                    }
                }

                // Cập nhật thứ tự
                var order = 0;
                foreach (var item in lstNode)
                {
                    var find = this.ObjList.FirstOrDefault(x => x.CODE == item.id);
                    if (find != null)
                    {
                        find.PARENT_CODE = item.pId ?? string.Empty;
                        find.C_ORDER = order;
                        this.CurrentRepository.Update(find);
                    }
                    order++;
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

        public void Delete(string code, int year)
        {
            try
            {
                var find = this.CurrentRepository.GetFirstByExpression(x => x.CODE == code && x.TIME_YEAR == year);

                if (find != null)
                {
                    UnitOfWork.BeginTransaction();
                    this.CurrentRepository.Delete(find);
                    UnitOfWork.Commit();
                }
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