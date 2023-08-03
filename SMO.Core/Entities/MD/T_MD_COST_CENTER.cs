using SMO.Core.Common;

using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.MD
{
    [Serializable]
    public class T_MD_COST_CENTER : CoreCenter
    {
        public virtual string SAP_CODE { get; set; }
        private T_MD_COST_CENTER parent;

        public virtual bool IS_GROUP { get; set; }
        public virtual T_MD_COST_CENTER Parent
        {
            get => string.IsNullOrEmpty(PARENT_CODE) ? null : parent;
            set => parent = value;
        }
        public virtual IList<T_AD_USER> ListUser { get; set; }

        public T_MD_COST_CENTER() : base()
        {
            ListUser = new List<T_AD_USER>();
        }
    }
}
