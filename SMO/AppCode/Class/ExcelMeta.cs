using System.Collections.Generic;

namespace SMO.AppCode.Class
{
    public class ExcelMeta
    {
        public IList<IList<ExcelCellMeta>> MetaTBody { get; set; }
        public IList<IList<ExcelCellMeta>> MetaTHead { get; set; }
        public double[] ColumnWidths { get; set; }
        public ExcelMeta()
        {
            MetaTBody = new List<IList<ExcelCellMeta>>();
            MetaTHead = new List<IList<ExcelCellMeta>>();
        }
    }
}