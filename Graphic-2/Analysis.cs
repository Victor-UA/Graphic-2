using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Graphic_2
{
    /// <summary>
    /// Запускає потоки аналізу файлів у папках folders
    /// </summary>
    class Analysis
    {
        private Form_MainForm mainForm { get; set; }
        private List<SortedDictionary<int, string>> files { get; set; }
        public bool isCanceled { get; private set; }
        private string[] folders { get; set; }
        private string extension { get; set; }
        private MyThread[] myThreads { get; set; }
        private Thread[] threads { get; set; }
        public List<string> threadLogs { get; private set; }

        /// <summary>
        /// Чи працює хоча б один потік
        /// </summary>
        public bool threadsAreOn { get; private set; }
        private int _threadsAliveCount;
        public int threadsAliveCount
        {
            get
            {
                return _threadsAliveCount;
            }
            set
            {
                if (value != _threadsAliveCount)
                {
                    _threadsAliveCount = value;
                    mainForm.button_Cancel_SetTextSafe(value.ToString());
                }
            }
        }
        public double elapsedTimeMS { get; private set; }

        private List<SortedDictionary<int, string>> folder3Files { get; set; }

        /// <summary>
        /// Тип вхідних даних
        /// 3 загальних файли чи купа індексованих
        /// </summary>
        public bool sourceIs3Files { get; private set; }        

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Form"></param>
        public Analysis(Form_MainForm Form)
        {
            mainForm = Form;
            folders = new string[3] { "1DCSin", "2DCSin" , "DCSSeg" };
            extension = "txt";
            elapsedTimeMS = 0;
            sourceIs3Files = true;
        }
        public void Cancel()
        {
            try
            {
                foreach (MyThread myThread in myThreads)
                {
                    try
                    {
                        myThread.isCanceled = true;
                    }
                    catch { }
                    
                }
                isCanceled = true;
            }
            catch (Exception) { }
        }
        public void Cancel(string text)
        {
            Cancel();
            mainForm.toolStripStatus = text;
            mainForm.write2LogFile('\n' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + ">>>>>>>>>>" + text);
        }

        /// <summary>
        /// Головний метод, запускається фоновим потоком
        /// </summary>
        public void Start()
        {
            mainForm.write2LogFile('\n' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Запуск процесу аналізу\n");
            mainForm.toolStripStatus = "Запуск процесса анализа";

            folder3Files = new List<SortedDictionary<int, string>>();
            files = new List<SortedDictionary<int, string>>();
            threadsAreOn = false;
            threadsAliveCount = 0;
            isCanceled = false;
            threadLogs = new List<string>();

            mainForm.toolStripStatus = "Удаление папки результатов";
            //Видалення теки результатів
            GlobalVariables.resultPath = Path.Combine(Directory.GetParent(mainForm.DCPath).FullName, "Graphic-2");
            try
            {
                
                if (Directory.Exists(GlobalVariables.resultPath))
                {
                    mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + "Видалення теки результатів: " + GlobalVariables.resultPath);
                    Directory.Delete(GlobalVariables.resultPath, true);
                    mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + "Теку результатів видалено\n");
                }
            }
            catch(DirectoryNotFoundException) { }
            catch(Exception ex)
            {
                mainForm.write2LogFile("Помилка під час видалення теки результатів\n" + ex.Message + '\n' + ex.StackTrace);
                MessageBox.Show("Помилка під час видалення теки результатів\n" + ex.Message);
                return;
            }

            //Перейменування теки, яка містить символ 'С' з кирилиці
            try
            {
                if (Directory.Exists(mainForm.DCPath + '\\' + "DСSSeg"))
                {
                    Directory.Move(mainForm.DCPath + '\\' + "DСSSeg", mainForm.DCPath + '\\' + "DCSSeg");
                }
            }
            catch { }

            mainForm.toolStripStatus = "Определение типа исходных данных";
            //Визначення типу вхідних даних
            //1.Три (може бути менше) великі файли, які містять склеєну інформацію з маленьких індексованих файлів
            //  або
            //2.Теки з файлами з індексом у імені
            try
            {
                process3Files(Directory.GetFiles(mainForm.DCPath, "*DCS*." + extension, SearchOption.TopDirectoryOnly));
                process3Files(Directory.GetFiles(mainForm.DCPath, "DСSSeg." + extension, SearchOption.TopDirectoryOnly));
                if (folder3Files.Count > 0)
                {
                    sourceIs3Files = true;
                }
                else
                {
                    sourceIs3Files = false;
                }
            }
            catch (Exception ex)
            {
                mainForm.write2LogFile("Помилка визначення типу вхідних даних.\n" + mainForm.DCPath + '\n' +
                    ex.Message + '\n' + ex.StackTrace);
                sourceIs3Files = false;
            }

            
            //Створення потоків

            //3 Файли
            if (sourceIs3Files)
            {
                myThreads = new MyThread[folder3Files.Count];
                threads = new Thread[folder3Files.Count];
                //for (int i = 2; i < 3; i++)
                for (int i = 0; i < folder3Files.Count; i++)
                {
                    try
                    {                        
                        //folder створюється для подальшого визначення типу апроксимації
                        string folder = folders[folder3Files[i].Keys.First()];
                        mainForm.toolStripStatus = string.Format("Создание потока {0}", folder);
                        mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Створення потоку {0}", folders[i]));
                        myThreads[i] = new MyThread(folder3Files[i].Values.First(), mainForm.DCPath, folder, false, sourceIs3Files);
                        threads[i] = new Thread(new ThreadStart(myThreads[i].Thread3fDC)) { Name = folder };
                        threads[i].IsBackground = true;
                        if (isCanceled)
                        {
                            break;
                        }
                        threads[i].Start();
                        mainForm.toolStripStatus = string.Format("Поток {0} запущен", folder);
                        mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Потік {0} запущено", folders[i]));
                    }
                    catch (Exception ex)
                    {
                        mainForm.write2LogFile("Помилка створення потоків.\n" + mainForm.DCPath + '\n' + ex.Message + '\n' + ex.StackTrace);
                        MessageBox.Show("Помилка створення потоків.\n" + mainForm.DCPath + '\n' +  ex.Message);
                        return;
                    }
                }
                mainForm.toolStripStatus = "Все потоки созданы";
            }
            //Купа індексованих файлів
            else
            {
                myThreads = new MyThread[folders.Length];
                threads = new Thread[folders.Length];
                for (int i = 0; i < folders.Length; i++)
                {
                    try
                    {
                        if(Directory.Exists(mainForm.DCPath + @"\" + folders[i]))
                        {
                            /*
                            mainForm.toolStripStatus = string.Format("Создание потока {0}: поиск файлов", folders[i]);
                            mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Створення потоку {0}: визначення файлів", folders[i]));

                            string[] folderFiles = Directory.GetFiles(mainForm.DCPath + @"\" + folders[i], "*." + extension, SearchOption.TopDirectoryOnly);

                            mainForm.toolStripStatus = string.Format("Создание потока {0}: найдено {1} файлов", folders[i], folderFiles.Count());
                            mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Створення потоку {0}: знайдено {1} файлів", folders[i], folderFiles.Count()));

                            files.Add(processFiles(folderFiles, mainForm.DCPath + @"\" + folders[i], folders[i], extension));

                            mainForm.toolStripStatus = string.Format("Создание потока {0}: файлы отсортированы", folders[i]);
                            mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Створення потоку {0}: файли відсортовано", folders[i], folderFiles.Count()));
                            */
                            myThreads[i] = new MyThread("", mainForm.DCPath + @"\" + folders[i], folders[i], mainForm.addFilesOn, sourceIs3Files);
                            threads[i] = new Thread(new ThreadStart(myThreads[i].ThreadNDC)) { Name = folders[i] };
                            threads[i].IsBackground = true;

                            if (isCanceled)
                            {
                                break;
                            }
                            else
                            {
                                threads[i].Start();
                            }

                            mainForm.toolStripStatus = string.Format("Создание потока {0}: поток запущен", folders[i]);
                            mainForm.write2LogFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + string.Format("Створення потоку {0}: потік запущено", folders[i]));
                        }
                        else
                        {
                            files.Add(new SortedDictionary<int, string>());
                        }
                    }
                    catch (Exception ex)
                    {
                        mainForm.write2LogFile("Помилка визначення файлів завдання.\n" + mainForm.DCPath + @"\" + folders[i] + '\n' +
                            ex.Message + '\n' + ex.StackTrace);
                        MessageBox.Show("Помилка визначення файлів завдання.\n" + mainForm.DCPath + @"\" + folders[i] + '\n' +
                            ex.Message);
                        return;
                    }
                }
                mainForm.toolStripStatus = "Все потоки созданы";
            }

            //Визначаємо прогрес і час, який залишився до завершення
            while (true)
            {
                int progress = 0;
                int totalFiles = 0;
                double maxThreadElapsedTime = 0;
                bool _threadsAreOn = false;
                int _threadsAliveCount = 0;
                string toolStripStatus = "";
                bool threadsAreLoaded = true;
                threadLogs = new List<string>();
                for (int i = 0; i < threads.Length; i++)
                {
                    if (threads[i] != null)
                    {
                        try
                        {
                            _threadsAreOn = _threadsAreOn || threads[i].IsAlive;
                            _threadsAliveCount += threads[i].IsAlive ? 1 : 0;
                            progress += myThreads[i].Progress;
                            threadsAreLoaded = threadsAreLoaded && myThreads[i].filesCount > 0;
                            toolStripStatus += 
                                myThreads[i].folder + 
                                ": (" + 
                                string.Format("{0}/{1}", myThreads[i].Progress, myThreads[i].filesCount) +
                                ")  "
                                ;
                            totalFiles += myThreads[i].filesCount;
                            if (myThreads[i].elapsedTimeMS > maxThreadElapsedTime)
                            {
                                maxThreadElapsedTime = myThreads[i].elapsedTimeMS;
                            }
                        }
                        catch { }
                    }

                    //log.Add(string.Format("{0} N={1} Progress={2}", DateTime.Now, i, myThreads[i].Progress));
                }
                
                threadsAreOn = _threadsAreOn;
                threadsAliveCount = _threadsAliveCount;

                elapsedTimeMS = maxThreadElapsedTime;

                if(threadsAreLoaded)
                {
                    mainForm.label_Progress_SetColorSafe(System.Drawing.SystemColors.ControlText);
                }
                else
                {
                    mainForm.label_Progress_SetColorSafe(System.Drawing.Color.Silver);
                }

                try
                {
                    mainForm.backgroundWorker1.ReportProgress(progress * 100 / totalFiles);
                }
                catch
                {
                    mainForm.backgroundWorker1.ReportProgress(0);
                }
                try
                {
                    if(!isCanceled) mainForm.toolStripStatus = toolStripStatus;
                }
                catch { }

                if (!threadsAreOn)
                {
                    break;
                }
                Thread.Sleep(1000);
            }

            mainForm.write2LogFile('\n' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ' ' + "Завершення процесу аналізу");
            
            mainForm.toolStripStatus = "Статистика";
            //Статистика
            if (mainForm.StatisticOn)
            {
                long d1Count = 0;
                long d2Count = 0;
                Dictionary<string, long> statistics = new Dictionary<string, long>();
                for (int i = 0; i < myThreads.Length; i++)
                {
                    //Статистика прочитаного
                    if (myThreads[i] != null)
                    {
                        d1Count += myThreads[i].d1Count;
                        d2Count += myThreads[i].d2Count;

                        string folder = myThreads[i].folder;
                        if (statistics.Keys.Contains(folder))
                        {
                            statistics[folder] += myThreads[i].totalLinesInCount;
                        }
                        else
                        {
                            statistics.Add(folder, myThreads[i].totalLinesInCount);
                        }

                        //Статистика записаного
                        folder = myThreads[i].folder.Replace("C", "");
                        if (statistics.Keys.Contains(folder))
                        {
                            statistics[folder] += myThreads[i].totalLinesOutCount;
                        }
                        else
                        {
                            statistics.Add(folder, myThreads[i].totalLinesOutCount);
                        }
                    }                    
                }
                try
                {
                    MessageBox.Show(string.Format(
                    "Загружено:\n" +
                    "1DCSin [X+Y]:\t{0}\n" +
                    "DСSSeg d1 [X+Y]:\t{1}\n" +
                    "DСSSeg d2 [X+Y]:\t{2}\n" +
                    "2DCSin [X+Y]:\t{3}\n" +
                    "\nВыгружено:\n" +
                    "1DSin.txt [Y]:\t{4}\n" +
                    "DSSig.txt [Y]:\t{5}\n" +
                    "2DSin.txt [Y]:\t{6}\n",

                    //Прочитано
                    statistics.Keys.Contains("1DCSin") ? statistics["1DCSin"] * 2 : 0,
                    d1Count * 2,
                    d2Count * 2,
                    statistics.Keys.Contains("2DCSin") ? statistics["2DCSin"] * 2 : 0,

                    //Записано
                    statistics.Keys.Contains("1DSin") ? statistics["1DSin"] : 0,
                    statistics.Keys.Contains("DSSeg") ? statistics["DSSeg"] : 0,
                    statistics.Keys.Contains("2DSin") ? statistics["2DSin"] : 0
                    ));
                }
                catch { }
                if (!isCanceled)
                {
                    mainForm.toolStripStatus = "Завершено";
                }
                else
                {
                    mainForm.toolStripStatus = "Расчёт прерван пользователем";
                }
            }
            

        }
        /// <summary>
        /// Сортування файлів за індексом в імені
        /// </summary>
        /// <param name="sFiles"></param>
        /// <param name="Path"></param>
        /// <param name="Folder"></param>
        /// <param name="Extension"></param>
        /// <returns></returns>
        
        private void process3Files(string[] Files)
        {
            if (Files.Length > 0)
            {
                foreach (string file in Files)
                {
                    SortedDictionary<int, string> row = new SortedDictionary<int, string>();
                    int index = -1;
                    for (int i = 0; i < folders.Length; i++)
                    {
                        if (file.Substring(mainForm.DCPath.Length).Contains(folders[i]))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0)
                    {
                        row.Add(index, file);
                        folder3Files.Add(row);
                    }
                    else
                    {
                        mainForm.write2LogFile("Помилка. Ім'я файлу не збігається з еталонним, тип проксимації не визначено\n" + file);
                    }
                }

            }
        }        
    }
}
