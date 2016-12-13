using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphic_2.Calculations
{
    class SourceData
    {
        public SortedDictionary<int, dataRow> Data {get;set;} //Key = x
        public int Kind { get; set; } //ApproximationKind
        public SourceData()
        {
            Data = new SortedDictionary<int, dataRow>();
            Kind = -1;
        }
        public SourceData(int kind) : this()
        {
            Kind = kind;
        }
    }
}
