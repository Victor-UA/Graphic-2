using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Graphic_2
{
    static class GlobalVariables
    {
        public static bool logIsSaveable { get; set; } = true;
        private static string _resultPath;
        public static string resultPath
        {
            get
            {
                return _resultPath;
            }
            set
            {
                _resultPath = value;
                resultPathH = Path.Combine(value, "DSSigH");
            }
        }
        public static string resultPathH { get; private set; }
    }
}
