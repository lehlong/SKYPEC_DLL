﻿using SharpSapRfc;

using SMO.Core.Common;

namespace SMO.Core.Entities.MD
{
    public class T_MD_INTERNAL_ORDER : CoreCenter
    {
        // code and parent code of profit center will be generate when create tree
        // it will be generated by COMPANY_CODE + PROJECT_CODE + CENTER_CODE
        // real center code to mapping stored in REAL_CENTER_CODE
        [RfcStructureField("AUFNR")]
        public override string CODE { get; set; }
        [RfcStructureField("KTEXT")]
        public override string NAME { get; set; }
        public virtual string PROJECT_CODE { get; set; }
        public virtual string PROJECT_NAME { get; set; }
        public virtual string BLOCK_CODE { get; set; }
        public virtual string BLOCK_NAME { get; set; }
        public virtual string IO_LEVEL1_CODE { get; set; }
        public virtual string IO_LEVEL1_NAME { get; set; }
        public virtual string IO_LEVEL2_CODE { get; set; }
        public virtual string IO_LEVEL2_NAME { get; set; }
        public virtual string REAL_CENTER_CODE { get; set; }
        public virtual bool IS_GROUP { get; set; }
    }
}