using System;
using System.Collections.Generic;
using System.IO; // это для работы с файлами
using System.Xml.Serialization; //это для сохранения классов — что и есть серилизация (стрёмное слово)
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Graphic_2
{
    public partial class Form_MainForm : Form
    {

        private string iniFileName { get; set; }
        internal string logFileName { get; set; }
        public List<string> log { get; set; } = new List<string>();
        public string DCPath
        {
            get
            {
                return textBox_DCPath.Text;
            }
            set
            {
                textBox_DCPath.Text = value;
            }
        }
        public bool addFilesOn
        {
            get
            {
                return checkBox_AdditionalFiles.Checked;
            }
            set
            {
                checkBox_AdditionalFiles.Checked = value;
            }
        }
        public bool StatisticOn
        {
            get
            {
                return checkBox_Statistic.Checked;
            }
            set
            {
                checkBox_Statistic.Checked = value;
            }
        }
        public int Progress {
            get
            {
                return progressBar1.Value;
            }
            set
            {
                if(value > 100)
                {
                    progressBar1.Value = 100;
                }
                else
                {
                    if (value < 0)
                    {
                        progressBar1.Value = 0;
                    }
                    else
                    {
                        try
                        {
                            progressBar1.Value = value;
                        }
                        catch
                        {
                            progressBar1.Value = 0;
                        }
                    }
                }
            }
        }
        public string toolStripStatus
        {
            get
            {
                return toolStripStatusLabel1.Text;
            }
            set
            {
                lock (this)
                {
                    toolStripStatusLabel1.Text = value;
                }
            }
        }
        public bool OkIsEnabled
        {
            get
            {
                return button_Ok.Enabled;
            }
            set
            {
                button_Ok.Enabled = value;
            }
        }
        private DateTime startTime { get; set; }
        private Analysis analysis { get; set; }

        public Form_MainForm()
        {
            InitializeComponent();
            iniFileName = "Graphic-2.xml";
            logFileName = "Graphic-2.log";
        }

        /// <summary>
        /// Початкові налаштування
        /// https://habrahabr.ru/sandbox/24807/
        /// </summary>
        public class iniSettings 
        {
            public string DCPath { get; set; }
            public string WAPPath { get; set; }
            public bool addFilesOn { get; set; }
            public bool StatisticOn { get; set; }
            public int MainFormX { get; set; }
            public int MainFormY { get; set; }
            public int MainFormHeight { get; set; }
            public int MainFormWidth { get; set; }
        }

        private void Form_MainForm_Load(object sender, EventArgs e)
        {
            // загружаем данные настроек из файла 
            
            try
            {
                using (Stream stream = new FileStream(iniFileName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(iniSettings));

                    // в тут же созданную копию класса iniSettings под именем iniSet
                    iniSettings iniSet = (iniSettings)serializer.Deserialize(stream);

                    DCPath = iniSet.DCPath;
                    addFilesOn = iniSet.addFilesOn;
                    StatisticOn = iniSet.StatisticOn;
                    this.Location = new Point(iniSet.MainFormX, iniSet.MainFormY);
                    this.Height = iniSet.MainFormHeight;
                    this.Width = iniSet.MainFormWidth;
                }
            }
            catch(Exception) { }
            

            //Logging
            log.Add("".PadLeft(50, '_'));
            log.Add("<log>");
            log.Add("Початок: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.Add("");
            write2LogFile(log);
            log.Clear();

            analysis = new Analysis(this);
        }

        private void Form_MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // создаём копию класса iniSettings с именем iniSet
            iniSettings iniSet = new iniSettings();

            // записываем в переменные класса текущие координаты верхнего левого угла окна
            iniSet.DCPath = DCPath;
            iniSet.addFilesOn = addFilesOn;
            iniSet.StatisticOn = StatisticOn;
            iniSet.MainFormX = this.Location.X;
            iniSet.MainFormY = this.Location.Y;
            iniSet.MainFormHeight = this.Height;
            iniSet.MainFormWidth = this.Width;

            // выкидываем класс iniSet целиком в файл program.xml
            /*
            using (Stream writer = new FileStream(iniFileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(iniSettings));
                serializer.Serialize(writer, iniSet);
            }
            */

            try
            {
                if (analysis.threadsAreOn)
                {
                    analysis.Cancel("Зупинено під час закриття програми");
                }
                else
                {
                    analysis.Cancel();
                }
                
                backgroundWorker1.CancelAsync();
            }
            catch { }

            log.Clear();
            log.Add("");
            log.Add("Завершення роботи програми: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.Add("</log>");

            //Збереження логів
            write2LogFile(log);
            
        }

        private void button_Look4DCFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DCPath = folderBrowserDialog1.SelectedPath;
            }
    }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            //http://www.codeproject.com/Tips/83317/BackgroundWorker-and-ProgressBar-demo
            try
            {
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex) { }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            //if (analysis.threadsAreOn)
            {
                analysis.Cancel("Расчёт прерван пользователем");
                backgroundWorker1.CancelAsync();
            }
            /*
            else
            {
                Application.Exit();
            }
            */
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            startTime = DateTime.Now;
            analysis.Start();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;

            string elapsedTimeText = "";
            try
            {
                TimeSpan elapsedTime = TimeSpan.FromMilliseconds(analysis.elapsedTimeMS);
                elapsedTimeText = elapsedTime.ToString(@"hh\.mm\.ss");
            }
            catch {}

            try
            {
                //label_TimeOfProgress.Text = string.Format("{0} час/мин/сек", elapsedTime.ToString(@"hh\.mm\.ss"));
                label_Progress.Text = string.Format("{0}%", Progress);
                label_TimeOfProgress.Text = string.Format("Прошло: {0} час/мин/сек", (DateTime.Now - startTime).ToString(@"hh\.mm\.ss"));
                label_ElapsedTimeOfProgress.Text = string.Format("Осталось: {0} час/мин/сек", elapsedTimeText);

            }
            catch (Exception)
            {
                label_TimeOfProgress.Text = "";
                label_ElapsedTimeOfProgress.Text = "";
            }

        }

        public void button_Cancel_SetTextSafe(string newText)
        {
            if (button_Cancel.InvokeRequired) button_Cancel.Invoke(new Action<string>((s) => button_Cancel.Text = s), string.Format("Отмена{0}", newText.Equals("0") ? "" : " (" + newText + ")"));
            else button_Cancel.Text = string.Format("Отмена{0}", newText.Equals("0") ? "" : " (" + newText + ")");
        }
        public void label_Progress_SetColorSafe(Color color)
        {
            if (label_Progress.InvokeRequired)
                label_Progress.Invoke(
                    new Action<Color>((c) => 
                    label_Progress.ForeColor = c), 
                    color);
            else label_Progress.ForeColor = color;
        }

        private void button_CreateSourceFiles_Click(object sender, EventArgs e)
        {
            try
            {
                string file = "2DCSin";
                int count = 200000;
                Directory.CreateDirectory(DCPath + '\\' + file);
                for (int i = 0; i < count; i++)
                {
                    File.Copy(DCPath + '\\' + file + ".txt", DCPath + '\\' + file + '\\' + file + i.ToString() + ".txt");
                }
                MessageBox.Show("Ok.");
                File.Move(DCPath + '\\' + file + ".txt", DCPath + '\\' + file.Replace("D", "D1") + ".txt");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + '\n' + ex.StackTrace);
            }
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
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        public void write2LogFile(string Log)
        {
            if (GlobalVariables.logIsSaveable)
            {
                lock (this)
                {
                    try
                    {
                        File.AppendAllText(logFileName, Log + '\n');
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}
