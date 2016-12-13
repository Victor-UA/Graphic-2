using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Graphic_2.Calculations
{
    class Approximation
    {
        private string[] _sData;

        /// <summary>
        /// Вхідні дані
        /// </summary>
        internal string[] sData
        {
            get
            {
                return _sData;
            }
            set
            {
                _sData = value;
                sData2Data();
            }
        }

        /// <summary>
        /// Оброблені вхідні дані
        /// </summary>
        internal SourceData Data { get; set; }

        /// <summary>
        /// Опція обробки додаткових файлів
        /// </summary>
        private bool addFilesOn { get; set; }
        internal List<string> resultData { get; set; }
        internal List<string> resultDataH { get; set; }
        private string folder { get; set; }
        private string file { get; set; }
        private string path { get; set; }

        /// <summary>
        /// Тип вхідних даних
        /// 3 загальних файли чи купа індексованих
        /// </summary>
        public bool sourceIs3Files { get; private set; }

        /// <summary>
        /// Статистика кількості d1
        /// </summary>
        public int d1Count { get; private set; }
        /// <summary>
        /// Статистика кількості d2
        /// </summary>
        public int d2Count { get; private set; }

        /// <summary>
        /// ApproximationKind
        /// </summary>
        private int kind { get; set; }
        public List<string> log { get; private set; }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Folder"></param>
        /// <param name="File"></param>
        /// <param name="Path"></param>
        /// <param name="Kind"></param>
        /// <param name="AddFilesOn"></param>
        public Approximation(string Folder, string File, string Path, int Kind, bool AddFilesOn, bool SourceIs3Files)
        {
            folder = Folder;
            file = File;
            path = Path;
            kind = Kind;
            d1Count = 0;
            d2Count = 0;
            log = new List<string>();
            resultData = new List<string>();
            resultDataH = new List<string>();
            addFilesOn = AddFilesOn;
            sourceIs3Files = SourceIs3Files;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Folder"></param>
        /// <param name="File"></param>
        /// <param name="Path"></param>
        /// <param name="Kind"></param>
        /// <param name="AddFilesOn"></param>
        public Approximation(string[] Data, string Folder, string File, string Path, int Kind, bool AddFilesOn, bool SourceIs3Files) : this(Folder, File, Path, Kind, AddFilesOn, SourceIs3Files)
        {
            sData = Data;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Folder"></param>
        /// <param name="File"></param>
        /// <param name="Path"></param>
        /// <param name="Kind"></param>
        /// <param name="AddFilesOn"></param>
        public Approximation(SourceData data, string Folder, string File, string Path, int Kind, bool AddFilesOn, bool SourceIs3Files) : this(Folder, File, Path, Kind, AddFilesOn, SourceIs3Files)
        {
            Data = data;
        }
        public void Process()
        {
            if (kind == ApproximationKind.DCSin )
            {
                switch (Data.Data.Count())
                {
                    case 2:
                        Sinus2P(Data.Data);
                        break;
                    case 3:
                        c2d3P(Data.Data);
                        break;
                    default:
                        log.Add(string.Format("Помилка апроксимації {0}: кількість точок не [2..3] ", file));
                        MessageBox.Show(string.Format("Помилка апроксимації {0}: кількість точок не [2..3] ", file));
                        break;
                }
            }
            else
            {
                DCSSeg(Data.Data);
            }
            if (!sourceIs3Files && addFilesOn && resultData != null)
            {
                writeResults2Files();
            }
        }
        
        /// <summary>
        /// Прибирання дублю у зоні склейки проміжкових результатів,
        /// запис до resultData, resultDataH
        /// </summary>
        /// <param name="result"></param>
        /// <param name="resultH"></param>
        private void write2ResultData(List<string> result, List<string> resultH)
        {
            if (resultData.Count > 0)
            {
                try
                {
                    if (resultData.Last().Equals(result.First()))
                    {
                        result.RemoveAt(0);
                        resultH.RemoveAt(0);
                    }
                }
                catch (Exception)
                {
                    { }
                }
            }
            resultData.AddRange(result);
            resultDataH.AddRange(resultH);
        }
        
        private void sData2Data()
        {
            Data = new SourceData();
            Data.Kind = kind;
            try
            {
                for (int i = 0; i < sData.Length; i++)
                {
                    line2dataRow converter = new line2dataRow(sData[i]);
                    if (converter.Success)
                    {
                        Data.Data.Add(converter.Key, converter.Row);
                        if (kind == ApproximationKind.DCSSeg)
                        {
                            if (converter.Row.status == 0)
                            {
                                d1Count++;
                            }
                            else
                            {
                                d2Count++;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Data = null;
                log.Add(string.Format("Помилка конвертації даних з файлу {0}\n", file) + e.Message + '\n' + e.StackTrace);
                MessageBox.Show(string.Format("Помилка конвертації даних з файлу {0}\n", file) + e.Message);
            }
            
            
        }

        /// <summary>
        /// Запис результатів до файлу
        /// </summary>
        private void writeResults2Files()
        {
            string resultPath = Path.Combine(GlobalVariables.resultPath, folder.Replace("C", ""));
            //string resultPathH = Path.Combine(GlobalVariables.resultPath, folder.Replace("C", ""));
            string resultFile = Path.Combine(resultPath, file.Substring(path.Length + 1).Replace("C", ""));
            Directory.CreateDirectory(resultPath);
            try
            {
                File.WriteAllLines(resultFile, resultData);
            }
            catch (Exception ex)
            {
                log.Add("Помилка збереження результатів до " + resultFile + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка збереження результатів до " + resultFile + '\n' + ex.Message);
            }
        }

        /// <summary>
        /// /// Апроксимація Синусом
        /// </summary>
        /// <param name="data"></param>
        public void Sinus2P(SortedDictionary<int, dataRow> data)
        {

            List<string> result = new List<string>();
            List<string> resultH = new List<string>();

            int x0 = data.Keys.First();
            int x1 = data.Keys.Last();
            double y0 = data[x0].y;
            double y1 = data[x1].y;

            try
            {
                for (int i = 0; i <= (x1-x0); i++)
                {
                    double y = data.Keys.Contains(x0 + i) ? data[x0 + i].y : 
                        Math.Sin(Math.PI * i / (x1 - x0) + Math.PI / 2) * (y0 - y1) / 2
                        + y0 + (y1 - y0) / 2;

                    result.Add(string.Format("{0:0.}", y));
                    resultH.Add(string.Format("{0:0.}", y / 2));
                }
                write2ResultData(result, resultH);
            }
            catch(Exception ex)
            {
                resultData = null;
                log.Add("Помилка апроксимації синусом!\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка апроксимації синусом!\n" + file + '\n' + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Апроксимація c2d по 3-х точках
        /// </summary>
        /// <param name="data"></param>
        public void c2d3P(SortedDictionary<int, dataRow> data)
        {

            List<string> result = new List<string>();
            List<string> resultH = new List<string>();

            int x0 = 0;
            int x1 = 0;
            int x2 = 0;
            int n = 0;
            foreach (int key in data.Keys)
            {
                switch (n)
                {
                    case 0:
                        x0 = key;
                        break;
                    case 1:
                        x1 = key;
                        break;
                    case 2:
                        x2 = key;
                        break;
                }
                n++;
            }
            double y0 = data[x0].y;
            double y1 = data[x1].y;
            double y2 = data[x2].y;

            double dY1 = y1 - y0;
            if(dY1 == 0) { dY1 = 1; }
            double dY2 = y2 - y1;
            if (dY2 == 0) { dY2 = 1; }

            double k1 = x1 * x1 * dY2 / (x1 * dY2 + (x2 - x1) * dY1);
            double k2 = x2 - (x2 - x1 - dY2 * (x1 - k1) / dY1);

            try
            {
                for (int i = x0; i <= x1; i++)
                {
                    double t = (0.5 * (k1 - i) / (i - x0) + Math.Sqrt((Math.Pow(0.5 * (k1 - i) / (i - x0), 2)-0.25*(i-x1)/(i-x0)))) / 0.25;
                    double y = data.Keys.Contains(i) ? data[i].y : 
                        (
                            y0 + dY1/(1+t*(1+0.25*t))
                        );
                    result.Add(string.Format("{0:0.}", y));
                    resultH.Add(string.Format("{0:0.}", y / 2));
                }
                for (int i = x1+1; i <= x2; i++)
                {
                    double t = (0.5 * (k2 - i) / (i - x1) + Math.Sqrt((Math.Pow(0.5 * (k2 - i) / (i - x1), 2) - 0.25 * (i - x2) / (i - x1)))) / 0.25;
                    double y = data.Keys.Contains(i) ? data[i].y : 
                        (
                            y1 + (dY2 + dY2 * t) / (1 + t * (1 + 0.25 * t))
                        );
                    result.Add(string.Format("{0:0.}", y));
                    resultH.Add(string.Format("{0:0.}", y / 2));
                }
                write2ResultData(result, resultH);
            }
            catch (Exception ex)
            {
                result = null;
                log.Add("Помилка апроксимації c2d!\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка апроксимації c2d!\n" + file + '\n' + ex.Message);
                return;
            }                       
        }
        
        /// <summary>
        /// /// Апроксимація c2d по 2-х точках
        /// </summary>
        /// <param name="data"></param>
        public void c2d2P(SortedDictionary<int, dataRow> data) 
        {

            List<string> result = new List<string>();
            List<string> resultH = new List<string>();

            int x0 = 0;
            int x1 = 0;
            int n = 0;
            foreach (int key in data.Keys)
            {
                switch (n)
                {
                    case 0:
                        x0 = key;
                        break;
                    case 1:
                        x1 = key;
                        break;
                }
                n++;
            }
            double y0 = data[x0].y;
            double y1 = data[x1].y;

            //Данные и углы з Алгоритм+Graphic-2+_п.2_.xls
            double dY = y1 - y0 == 0 ? 1 : y1 - y0;
            double dX = x1 - x0;



            //2-3 з Алгоритм+Graphic-2+_п.2_.xls
            double ab = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            double angle1 = Math.Atan(dY / dX) * 180 / Math.PI;
            double angle2 = Math.Abs(data[x0].SPAngle - angle1);
            double angle3 = Math.Abs(angle1 - data[x1].SPAngle);

            //Перегин
            double inflectionX = ab 
                * Math.Tan(angle3*Math.PI/180) 
                * Math.Cos(data[x0].SPAngle * Math.PI / 180)
                / 
                ( 
                    Math.Cos(angle2 * Math.PI / 180)
                    *
                    ( Math.Tan(angle2 * Math.PI / 180)
                    + Math.Tan(angle3 * Math.PI / 180)
                    )
                );
            double inflectionY = ab
                * Math.Tan(angle3 * Math.PI / 180)
                * Math.Sin(data[x0].SPAngle * Math.PI / 180)
                /
                (
                    Math.Cos(angle2 * Math.PI / 180)
                    *
                    (Math.Tan(angle2 * Math.PI / 180)
                    + Math.Tan(angle3 * Math.PI / 180)
                    )
                );

            //Перебираємо значення xi від x0 до x1
            n = 0;
            try
            {
                for (int i = x0; i <= x1; i++)
                {
                    double y;
                    if (data.Keys.Contains(i))
                    {
                        y = data[i].y;
                    }
                    else
                    {
                        double t =
                            (
                                0.5 * ((inflectionX - n) / n)
                                + Math.Sqrt(
                                    Math.Pow(0.5 * ((inflectionX - n) / n), 2)
                                    - 0.25 * (n - dX)/n 
                                )
                            ) / 0.25; 
                        double dYi = (dY + inflectionY * t) / (1 + t * (1 + 0.25 * t));
                        y = y0 + dYi;
                    }

                    result.Add(string.Format("{0:0.}", y));
                    resultH.Add(string.Format("{0:0.}", y / 2));
                    n++;
                }
                write2ResultData(result, resultH);
            }
            catch (Exception ex)
            {
                result = null;
                log.Add("Помилка апроксимації c2d!\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка апроксимації c2d!\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// Обробка файлів DCSSeg
        /// </summary>
        /// <param name="data"></param>
        public void DCSSeg(SortedDictionary<int, dataRow> data)
        {
            try
            {
                if (data.Count > 1)
                {
                    int last1X = data.Keys.First();
                    int lastX = data.Keys.First();

                    //Попередня -1 строка
                    dataRow last1Row = data[lastX];
                    //Попередня строка
                    dataRow lastRow = data[lastX];

                    //Непарність d2
                    bool odd = true;

                    //Розрахунок початкових даних
                    //Робоча строка: lastRow
                    foreach (int X in data.Keys)
                    {
                        dataRow Row = data[X];

                        if (X != lastX && lastX != last1X)
                        {
                            SortedDictionary<int, dataRow> currentRows = new SortedDictionary<int, dataRow>();
                            currentRows.Add(lastX, lastRow);
                            currentRows.Add(X, Row);
                            if (last1Row.status == lastRow.status && last1Row.status == 0)
                            {
                                odd = true;                                
                            }
                            else
                            {
                                odd = odd ? false : true;
                                double dY = lastRow.y - last1Row.y == 0 ? 1 : lastRow.y - last1Row.y;
                                double dY1 = Row.y - lastRow.y == 0 ? 1 : Row.y - lastRow.y;
                                double dX = lastX - last1X;
                                double dX1 = X - lastX;

                                double angle1 = 180 / Math.PI * (
                                    odd ? Math.Atan(dY / dX) : Math.Atan(dX / dY)
                                    );
                                double angle2 = 180 / Math.PI * (
                                    odd ? Math.Atan(dY1 / dX1) : Math.Atan(dX1 / dY1)
                                    );
                                
                                double minABSAngle = Math.Abs(angle1) >= Math.Abs(angle2) ? angle2 : angle1;
                                lastRow.SPAngle = lastRow.status == 0 ? 0 :
                                    odd ?
                                        minABSAngle * 2 / 3 : 
                                        (dY / Math.Abs(dY)) * 90 - (minABSAngle * 2 / 3);
                            }
                        }

                        last1X = lastX;
                        last1Row = lastRow;
                        lastX = X;
                        lastRow = Row;
                    }

                    lastX = data.Keys.First();
                    lastRow = data[lastX];

                    //парність d2
                    foreach (int X in data.Keys)
                    {
                        dataRow Row = data[X];

                        if (X != lastX)
                        {
                            SortedDictionary<int, dataRow> currentRows = new SortedDictionary<int, dataRow>();
                            currentRows.Add(lastX, lastRow);
                            currentRows.Add(X, Row);
                            if (lastRow.status == Row.status && lastRow.status == 0)
                            {
                                //Апроксимація Синусом
                                Sinus2P(currentRows);
                            }
                            else
                            {
                                //Апроксимація c2d
                                c2d2P(currentRows);
                            }
                        }

                        lastX = X;
                        lastRow = Row;
                    }
                }
                else
                {
                    resultData = null;
                    log.Add("Помилка апроксимації DCSSeg!\n" + file + '\n' + "Замало точок: менше за 2");
                    MessageBox.Show("Помилка апроксимації DCSSeg!\n" + file + '\n' + "Замало точок: менше за 2");
                }
            }
            catch(Exception ex)
            {
                resultData = null;
                log.Add("Помилка апроксимації DCSSeg!\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка апроксимації DCSSeg!\n" + file + '\n' + ex.Message);
                return;
            }
        }
    }
}