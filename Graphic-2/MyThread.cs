using System;
using System.Collections.Generic;
using Graphic_2.Calculations;
using System.Linq;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Graphic_2
{
    /// <summary>
    /// Поток апроксимації файлів 1 папки
    /// Зберігає загальні файли 
    /// </summary>
    public class MyThread
    {
        private object thisLock = new object();
        private bool _isCanceled;
        public bool isCanceled
        {
            get
            {
                return _isCanceled;
            }
            set
            {
                _isCanceled = value;
                folderFiles.isCanceled = value;
            }
        }
        private FolderFiles folderFiles { get; set; }
        private DateTime startTime { get; set; }
        private bool addFilesOn { get; set; }
        public List<string> log { get; private set; }
        private string logFileName { get; set; }
        private SortedDictionary<int, string> _files { get; set; }
        private SortedDictionary<int, string> files
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
                try
                {
                    fileKey = _files.Keys.First();
                    file = _files[fileKey];
                }
                catch
                {
                    fileKey = -1;
                    file = null;
                }
            }
        }
        private string file { get; set; }
        private int fileKey { get; set; }
        private string sourceIs3FilesFileName { get; set; }
        private int kind { get; set; }
        private string path { get; set; }
        public string folder { get; private set; }
        public int filesCount { get; private set; }             
        public int Progress { get; private set; }
        public double elapsedTimeMS { get; private set; }

        /// <summary>
        /// Тип вхідних даних
        /// 3 загальних файли чи купа індексованих
        /// </summary>
        public bool sourceIs3Files { get; private set; }

        /// <summary>
        /// Статистика кількості d1
        /// </summary>
        public long d1Count { get; private set; }
        /// <summary>
        /// Статистика кількості d2
        /// </summary>
        public long d2Count { get; private set; }
        /// <summary>
        /// Для відображення статистики прочитаних строк
        /// </summary>
        public long totalLinesInCount { get; private set; }
        /// <summary>
        /// Для відображення статистики записаних строк
        /// </summary>
        public long totalLinesOutCount { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Files"></param>
        /// <param name="Path"></param>
        /// <param name="Folder"></param>
        /// <param name="AddFilesOn"></param>
        /// <param name="SourceIs3Files"></param>
        public MyThread(string SourceIs3FilesFileName, string Path, string Folder, bool AddFilesOn, bool SourceIs3Files)
        {
            sourceIs3FilesFileName = SourceIs3FilesFileName;
            
            Progress = 0;
            path = Path;
            folder = Folder;
            kind = Folder.Equals("DCSSeg") ? ApproximationKind.DCSSeg : ApproximationKind.DCSin;
            
            addFilesOn = AddFilesOn;
            totalLinesInCount = 0;
            totalLinesOutCount = 0;
            d1Count = 0;
            d2Count = 0;
            sourceIs3Files = SourceIs3Files;
            log = new List<string>();
            folderFiles = new FolderFiles(path, folder);

            isCanceled = false;

            logFileName = folder + ".log";
        }

        /// <summary>
        /// Обробка купи індексованих файлів
        /// </summary>
        public void ThreadNDC()
        {
            write2LogFile("".PadLeft(50, '_'));
            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Початок роботи потоку {0}: ", Thread.CurrentThread.Name) + '\n');

            filesCount = 0;
            startTime = DateTime.Now;
            /*
            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Визначення файлів"));
            
            //DirectoryInfo directoryInfo = new DirectoryInfo(path); 
            
            List<string> folderFiles = new List<string>();
            
            int i = 0; 
            try
            {
                foreach (string enumerateFile in Directory.EnumerateFiles(path, "*.txt", SearchOption.TopDirectoryOnly).AsParallel())
                {
                    if (isCanceled)
                    {
                        break;
                    }
                    folderFiles.Add(enumerateFile);
                    filesCount++;
                    i++;
                }

            }
            catch(Exception ex)
            {
                write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + 
                    string.Format("Помилка завантаження файлів з теки {0}", folder) + '\n' + 
                    ex.Message + 
                    ex.StackTrace
                    );
            }

            startTime = DateTime.Now;

            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Знайдено {0} файлів", folderFiles.Count()));

            files = processFiles(folderFiles.ToArray(), path, folder, "txt");

            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Файли відсортовано"));
            */
            string resultFile = Path.Combine(GlobalVariables.resultPath, (kind == ApproximationKind.DCSin ? folder.Replace("C", "") + ".txt" : "DSSig.txt"));
            string resultFileH = Path.Combine(GlobalVariables.resultPathH, (kind == ApproximationKind.DCSin ? folder.Replace("C", "") + "H.txt" : "DSSigH.txt"));

            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Визначено теку результатів: {0}", resultFile));
            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Визначено теку результатів H: {0}", resultFileH));

            try
            {
                Directory.CreateDirectory(GlobalVariables.resultPath);
                if (addFilesOn)
                {
                    Directory.CreateDirectory(GlobalVariables.resultPathH);
                }
            }
            catch (Exception ex)
            {
                write2LogFile("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message);
                return;
            }


            
            Thread thread_folderFileNumber = new Thread(new ThreadStart(folderFiles.Calc)) { Name = folder + "(folderFileNumber)" };
            thread_folderFileNumber.Start();

            int lastKey = -1;
            string lastFile = "";
            string lastResult = "";
            //int fileReadErrorCount = 0;
            int i = 1;
            //foreach(int key in files.Keys)
            while(!isCanceled && (!folderFiles.isCalculated || (folderFiles.isCalculated && i <= folderFiles.filesCount)))
            {
                try
                {
                    //string file = files[key];
                    int key = i;
                    string file = Path.Combine(path, folder + i.ToString() + ".txt");
                    string[] lines = null;
                    try
                    {
                        lines = File.ReadAllLines(file);
                    }
                    catch(Exception ex)
                    {
                        write2LogFile(ex.Message);
                        /*
                        if (fileReadErrorCount > 100)
                        {
                            break;
                        }
                        else
                        {
                            fileReadErrorCount++;
                            i++;
                            continue;
                        }
                        */
                        i++;
                        continue;

                    }

                    Approximation approximation = new Approximation(lines, folder, file, path, kind, addFilesOn, sourceIs3Files);
                    approximation.Process();

                    d1Count += approximation.d1Count;
                    d2Count += approximation.d2Count;

                    totalLinesInCount += approximation.Data.Data.Count();

                    if (approximation.resultData.Count > 0)
                    {
                        try
                        {
                            if(!lastResult.Equals(string.Empty))
                            {
                                if (lastResult.Equals(approximation.resultData.First()))
                                {
                                    approximation.resultData.RemoveAt(0);
                                    approximation.resultDataH.RemoveAt(0);
                                }
                                else
                                {
                                    write2LogFile("Крайові значення не збігаються\n" + lastFile + '\n' + file);
                                    MessageBox.Show(string.Format("Не совпадает стык файла №{0} с файлом №{1}\n", lastKey, key) + lastFile + '\n' + file);
                                }
                            }
                            
                        }
                        catch (Exception e)
                        {
                            write2LogFile("Помилка видалення крайового для запобігання дублю у місці склейки значень значення з таблиці результатів\n"
                                + file + '\n'
                                + e.Message + '\n' + e.StackTrace
                                );
                            MessageBox.Show("Помилка видалення крайового для запобігання дублю у місці склейки значень значення з таблиці результатів\n"
                                + file + '\n'
                                + e.Message + '\n' + e.StackTrace
                                );
                        }

                        lastResult = approximation.resultData.Last();

                        /*
                        resultData.AddRange(approximation.resultData);
                        resultDataH.AddRange(approximation.resultDataH);
                        */

                        try
                        {
                            File.AppendAllLines(resultFile, approximation.resultData);
                            if (addFilesOn)
                            {
                                Directory.CreateDirectory(GlobalVariables.resultPathH);
                                File.AppendAllLines(resultFileH, approximation.resultDataH);
                            }
                            totalLinesOutCount += approximation.resultData.Count();
                        }
                        catch (Exception ex)
                        {
                            write2LogFile("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message + '\n' + ex.StackTrace);
                            MessageBox.Show("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message);
                        }

                    }

                    lastKey = key;
                    lastFile = file;

                    write2LogFile(approximation.log);
                }
                catch (Exception ex)
                {
                    write2LogFile("Помилка у потоці "+ Thread.CurrentThread.Name + '\n' + lastFile + "\n\n" + ex.Message + '\n' + ex.StackTrace);
                    MessageBox.Show("Помилка у потоці " + Thread.CurrentThread.Name + '\n' + lastFile + "\n\n" + ex.Message + '\n' + ex.StackTrace);
                }
                i++;
                Progress = i;

                filesCount = folderFiles.filesCount;

                DateTime currentTime = DateTime.Now;
                double passedTime = (currentTime - startTime).TotalMilliseconds;
                elapsedTimeMS = (filesCount - Progress) / (Progress / passedTime);

                //Thread.Sleep(200);
            }

            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Кінець роботи потоку {0}: ", Thread.CurrentThread.Name));
        }

        private SortedDictionary<int, string> processFiles(string[] sFiles, string Path, string Folder, string Extension)
        {
            SortedDictionary<int, string> folderFiles = new SortedDictionary<int, string>();
            foreach (string file in sFiles)
            {
                string Key = file.Substring(Path.Length + 1, file.Length - Path.Length - Extension.Length - 2);
                Key = Key.Substring(Folder.Length);
                try
                {
                    folderFiles.Add(Convert.ToInt32(Key), file);
                }
                catch (Exception ex)
                {
                    write2LogFile("Помилка визначення індексу файла\n" + file + '\n' + ex.Message + '\n' + ex.StackTrace);
                    MessageBox.Show("Помилка визначення індексу файла\n" + file + '\n' + ex.Message);
                    return null;
                }
            }
            return folderFiles;
        }

        /// <summary>
        /// Обробка 3-х файлів
        /// </summary>
        public void Thread3fDC()
        {
            write2LogFile("".PadLeft(50, '_'));
            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Початок роботи потоку {0}: ", Thread.CurrentThread.Name) + '\n');

            try
            {
                filesCount = File.ReadLines(sourceIs3FilesFileName).Count();
                startTime = DateTime.Now;

                string resultFile = Path.Combine(GlobalVariables.resultPath, (kind == ApproximationKind.DCSin ? folder.Replace("C", "") + ".txt" : "DSSig.txt"));
                string resultFileH = Path.Combine(GlobalVariables.resultPathH, (kind == ApproximationKind.DCSin ? folder.Replace("C", "") + "H.txt" : "DSSigH.txt"));
                try
                {
                    Directory.CreateDirectory(GlobalVariables.resultPath);
                    if (addFilesOn)
                    {
                        Directory.CreateDirectory(GlobalVariables.resultPathH);
                    }
                }
                catch (Exception ex)
                {
                    write2LogFile("Помилка створення теки загальних рузультатів" + GlobalVariables.resultPath + '\n' + ex.Message + '\n' + ex.StackTrace);
                    MessageBox.Show("Помилка створення теки загальних рузультатів" + GlobalVariables.resultPath + '\n' + ex.Message);
                    return;
                }

                SourceData blockData = new SourceData(kind);
                Approximation lastApproximation = null;

                int i = 0;
                //http://ru.stackoverflow.com/questions/476784/%D0%91%D1%8B%D1%81%D1%82%D1%80%D0%B0%D1%8F-%D0%BE%D0%B1%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D0%BA%D0%B0-%D0%B1%D0%BE%D0%BB%D1%8C%D1%88%D0%B8%D1%85-%D1%84%D0%B0%D0%B9%D0%BB%D0%BE%D0%B2/476795
                foreach (var line in File.ReadLines(sourceIs3FilesFileName))
                {
                    if (isCanceled)
                    {
                        break;
                    }

                    line2dataRow converter = new line2dataRow(line);
                    if (converter.Success)
                    {

                        //Статистика
                        d1Count += converter.d1Count;
                        d2Count += converter.d2Count;

                        //Початок блоку
                        if (converter.Key == 0)
                        {

                            if (blockData.Data.Count > 0)
                            {
                                //Перевірка на відповідність даних у зоні склейки
                                if (!blockData.Data[blockData.Data.Keys.Last()].y.Equals(converter.Row.y))
                                {
                                    write2LogFile(string.Format("Помилка вхідних даних: невідповідність даних у зоні склейки файлу {0}, рядок {1}", sourceIs3FilesFileName, i));
                                    MessageBox.Show(string.Format("Не совпадает стык файла {0}, строка {1}", sourceIs3FilesFileName, i));
                                }
                                Approximation approximation = new Approximation(blockData, folder, sourceIs3FilesFileName, path, kind, addFilesOn, sourceIs3Files);
                                Thread3fDC_BlockProcess(approximation, blockData, lastApproximation, resultFile, resultFileH);
                                lastApproximation = approximation;
                            }
                            else
                            {

                            }
                        }
                        blockData.Data.Add(converter.Key, converter.Row);
                    }

                    i++;
                    Progress = i;

                    DateTime currentTime = DateTime.Now;
                    double passedTime = (currentTime - startTime).TotalMilliseconds;
                    elapsedTimeMS = (filesCount - Progress) / (Progress / passedTime);

                    //Thread.Sleep(200);
                }

                //Обробка останнього блоку
                if (blockData.Data.Count > 0)
                {
                    Approximation approximation = new Approximation(blockData, folder, sourceIs3FilesFileName, path, kind, addFilesOn, sourceIs3Files);
                    Thread3fDC_BlockProcess(approximation, blockData, lastApproximation, resultFile, resultFileH);
                    lastApproximation = approximation;
                }

            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                write2LogFile("Помилка у потоці " + Thread.CurrentThread.Name + '\n' + ex.Message);
                MessageBox.Show("Помилка у потоці " + Thread.CurrentThread.Name + '\n' + ex.Message + '\n' + ex.StackTrace);
            }

            write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Кінець роботи потоку {0}: ", Thread.CurrentThread.Name));
        }
        private void Thread3fDC_BlockProcess(Approximation approximation, SourceData blockData, Approximation lastApproximation, string resultFile, string resultFileH)
        {
            //Апроксимація
            approximation.Process();
            write2LogFile(approximation.log);            

            //Прибирання дублів у зоні склейки
            try
            {
                if (lastApproximation != null && lastApproximation.resultData.Count > 0 && approximation.resultData.Count > 0)
                {
                    if (lastApproximation.resultData.Last().Equals(approximation.resultData.First()))
                    {
                        approximation.resultData.RemoveAt(0);
                        approximation.resultDataH.RemoveAt(0);
                    }
                }
            }
            catch (Exception e)
            {
                write2LogFile("Помилка видалення крайового для запобігання дублю у місці склейки значень значення з таблиці результатів\n"
                    + file + '\n'
                    + e.Message + '\n' + e.StackTrace
                    );
                MessageBox.Show("Помилка видалення крайового для запобігання дублю у місці склейки значень значення з таблиці результатів\n"
                    + file + '\n'
                    + e.Message + '\n' + e.StackTrace
                    );
            }

            //Статистика
            totalLinesInCount += approximation.Data.Data.Count;

            //Запис до файлу
            try
            {
                File.AppendAllLines(resultFile, approximation.resultData);
                if (addFilesOn)
                {
                    File.AppendAllLines(resultFileH, approximation.resultDataH);
                }
                totalLinesOutCount += approximation.resultData.Count();
            }
            catch (Exception ex)
            {
                write2LogFile("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка збереження загальних результатів до " + resultFile + '\n' + ex.Message);
            }

            //Очистка блоку
            blockData.Data = new SortedDictionary<int, dataRow>();
        }


        public void write2LogFile(List<string> Log)
        {
            if (GlobalVariables.logIsSaveable)
            {
                lock (this)
                {
                    try
                    {
                        File.AppendAllLines(logFileName, Log);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        public void write2LogFile(string Log)
        {
            if (GlobalVariables.logIsSaveable) {
                lock (this)
                {
                    try
                    {
                        File.AppendAllText(logFileName, Log + '\n');
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}