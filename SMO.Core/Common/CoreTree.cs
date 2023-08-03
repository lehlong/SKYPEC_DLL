using SMO.Core.Entities;

using System;

namespace SMO.Core.Common
{
    [Serializable]
    public abstract class CoreTree : BaseEntity, ICoreTree
    {
        public CoreTree() : base()
        {

        }
        public virtual string CODE { get; set; }
        public virtual string PARENT_CODE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual string NAME { get; set; }
    }
}
