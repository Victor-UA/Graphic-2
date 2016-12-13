using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphic_2.Calculations
{
    class dataRow
    {
        public double y { get; set; }
        public double z { get; set; }
        /// <summary>
        /// 0 - d1
        /// 1 - d2
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Кут у точці SP
        /// </summary>
        public double SPAngle { get; set; }

        public dataRow()
        {
            y = 0;
            z = 0;
            status = 0;
            SPAngle = 0;
        }
    }
}
