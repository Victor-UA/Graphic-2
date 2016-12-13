using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphic_2.Calculations
{
    /// <summary>
    /// Клас для конвертації текстової строки до строки таблиці даних
    /// </summary>
    class line2dataRow
    {

        private string _Line;

        public string Line
        {
            get
            {
                return _Line;
            }
            set
            {
                _Line = value;
                Process();
            }
        }

        //Результат
        public int Key { get; private set; }
        public dataRow Row { get; private set; }
        public int d1Count { get; private set; }
        public int d2Count { get; private set; }
        public bool Success { get; set; }

        public line2dataRow(string line)
        {
            Line = line;
        }

        private void Process()
        {
            Success = false;
            d1Count = 0;
            d2Count = 0;
            char splitChar = ' ';
            string[] cells = Line.Split(splitChar);
            if (cells.Length < 2)
            {
                splitChar = '\t';
                cells = Line.Split(splitChar);
            }
            if (cells.Length > 1)
            {
                Row = new dataRow();
                Key = Convert.ToInt32(cells[0]);
                Row.y = Convert.ToInt64(cells[1]);

                if (cells.Length > 2)
                {
                    Row.status = Convert.ToInt32(cells[2]);
                    switch (Row.status)
                    {
                        case 0: d1Count++;
                            break;
                        case 1: d2Count++;
                            break;
                    }
                }
                Success = true;
            }
        }
    }

}
