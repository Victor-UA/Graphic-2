using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Graphic_2
{
    static class Program
    {
        /// <summary>
        /// Graphic-2
        /// Завдання для kvn1977 з olx.ua
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_MainForm());
        }
    }
}
