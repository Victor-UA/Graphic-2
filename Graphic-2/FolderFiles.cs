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
    /// Підрахунок кількості файлів у теці
    /// </summary>
    public class FolderFiles
    {
        public bool isCanceled { get; set; }
        public bool isCalculated { get; private set; }
        private DateTime startTime { get; set; }
        private string path { get; set; }
        public string folder { get; private set; }
        public int filesCount { get; private set; }             

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Folder"></param>
        public FolderFiles(string Path, string Folder)
        {
            isCanceled = false;
            filesCount = 0;
            path = Path;
            folder = Folder;
            isCalculated = false;
        }
        
        public void Calc()
        {
            int i = 1;

            int lastFileExistsIndex = i;
            //Мінімальний індекс, з яким файлу не існує
            int minIndexFileNotExists = 0;
            //Різницю між відомим і граничним індексом, яку можна пройти перебором
            int iterateDelta = 3;
            int k = 10;
            while (!isCanceled)
            {
                //Файл існує
                if (File.Exists(fileName(i)))
                {
                    lastFileExistsIndex = i;
                    filesCount = i;
                    if (minIndexFileNotExists == 0)
                    {
                        i *= k;
                    }
                    else
                    {
                        //Різниця між відомим і граничним індексом, достатня для проходу перебором
                        if (minIndexFileNotExists - i <= iterateDelta)
                        {
                            for (int n = i; n < minIndexFileNotExists; n++)
                            {
                                if (!File.Exists(fileName(n)))
                                {
                                    isCalculated = true;
                                    return;
                                }
                                filesCount = n;
                            }
                            isCalculated = true;
                            return;
                        }
                        else
                        {
                            i = (i + minIndexFileNotExists) / 2;
                        }
                    }
                }

                //Файлу не існує
                else
                {
                    if(i <= minIndexFileNotExists || minIndexFileNotExists == 0)
                    {
                        minIndexFileNotExists = i;
                    }
                    int tmp = i;
                    i = (i + lastFileExistsIndex) / 2;
                }
            }
        }
        
        private string fileName(int i)
        {
            return Path.Combine(path, folder + i.ToString() + ".txt");
        }
    }
}