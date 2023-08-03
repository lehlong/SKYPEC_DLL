using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class RevenueElementService : GenericService<T_MD_REVENUE_PL_ELEMENT, RevenuePLElementRepo>
    {
        internal IList<NodeRevenueElement> GetNodeRevenueElement()
        {
            GetAll();
            // get all cost center
            var lstCostElm = ObjList.OrderBy(x => x.C_ORDER).ToList();
            var lstNode = new List<NodeRevenueElement>();

            foreach (var item in lstCostElm)
            {
                var node = new NodeRevenueElement()
                {
                    id = item.CODE,
                    pId = item.PARENT_CODE,
                    name = $"<span class='pre-whitespace'>{item.CODE} - {item.NAME}</span>",
                    type = Budget.REVENUE_ELEMENT.ToString()
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

        internal object GetDetailSumUp(string centerCode, string elementCode, int year, int version)
        {
            throw new NotImplementedException();
        }
    }
}