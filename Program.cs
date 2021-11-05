using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace FileManagerByRedman
{
    // Класс записывающий ошибки в файл
    static class Logger
    {
        public static void WriteLine(string message)
        {
            using StreamWriter sw = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\random_name_exception.txt", true);
            sw.WriteLine(String.Format("{0,-23} {1}", DateTime.Now.ToString() + ":", message));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var errorMessege = "";
            var error = 0;
            var list = 1;
            var dsk = 1;
            var exit = 0;
            var copyDir = "";
            var pasteDir = "";
            string dir = ConfigurationManager.AppSettings["drctr"];
            int copyFile = 2;
            Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            while (exit == 0)
            {
                // Добавление в list "dirnames" названий папок и файлов в текущей директории
                if (dsk == 0)
                {
                    var minElement = list * Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) - Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]);
                    var maxElement = minElement + Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) - 1;
                    List<string> dirnames = new();
                    try
                    {
                        DirectoryInfo dirInfo = new(dir);
                        DirectoryInfo[] dirInfos = dirInfo.GetDirectories("*.*");
                        FileInfo[] fileNames = dirInfo.GetFiles("*.*");

                        foreach (DirectoryInfo d in dirInfos)
                        {
                            dirnames.Add(d.Name);
                        }

                        foreach (FileInfo fi in fileNames)
                        {
                            dirnames.Add(fi.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        error = 1;
                        errorMessege = e.Message;
                        Logger.WriteLine(e.Message);

                        dir = @"C:\";
                        currentConfig.AppSettings.Settings["drctr"].Value = dir;
                        currentConfig.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }

                    Console.WriteLine($"| {dir} ");
                    
                    //Вывод нужных нам папок и файлов на экран
                    for (var i = 0; (i < dirnames.Count) & (i <= maxElement); i++)
                    {
                        if (i >= minElement)
                        {
                            Console.WriteLine("     | {0}", dirnames[i]);
                        }
                    }
                    //Вывод номеров элементов, которые показаны на экране
                    //Если всё не вместилось в один лист
                    if (dirnames.Count > 50)
                    {
                        if (maxElement <= dirnames.Count)
                        {
                            Console.WriteLine($"Элементы {minElement + 1} - {maxElement + 1} / {dirnames.Count}");
                        }
                        else
                        {
                            Console.WriteLine($"Элементы {minElement + 1} - {dirnames.Count} / {dirnames.Count}");
                        }
                    }
                }
                //Вывод списка дисков и запрос у пользователя
                //на какой диск нужно перейти
                if (dsk == 1)
                {
                    try
                    {
                        //Вывод списка дисков
                        Console.WriteLine("Выберите диск:");
                        string[] Drives = Environment.GetLogicalDrives();
                        foreach (string s in Drives)
                        {
                            Console.WriteLine(s);
                        }
                        //разделительная линия
                        for (int i = 0; i < Console.BufferWidth; i++)
                        {
                            Console.SetCursorPosition(i, Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) + 2);
                            Console.WriteLine('-');
                        }
                        //запрос у пользователя
                        //на какой диск нужно перейти
                        var commandd = Console.ReadLine();
                        string[] wordds = commandd.Split(' ');
                        dir = wordds[1];
                        currentConfig.AppSettings.Settings["drctr"].Value = dir;
                        currentConfig.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                        dsk = 0;
                        Console.Clear();
                        continue;
                    }
                    catch (Exception e)
                    {
                        error = 1;
                        errorMessege = e.Message;
                        Logger.WriteLine(e.Message);

                        dir = @"C:\";
                        currentConfig.AppSettings.Settings["drctr"].Value = dir;
                        currentConfig.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                }
                //разделительная линия
                for (int i = 0; i < Console.BufferWidth; i++)
                {
                    Console.SetCursorPosition(i, Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) + 2);
                    Console.WriteLine('-');
                }
                //Вывод информации о текущей директории
                DirectoryInfo di = new(dir);
                Console.WriteLine("Имя каталога: " + di.Name);
                try
                {
                    Console.WriteLine("Занято: " + GetDirSize(dir) + " байт");
                }
                catch
                {
                    if (dir.Length == 3)
                    {
                        DriveInfo drives = new(dir);
                        Console.WriteLine("Занято: " + (drives.TotalSize - drives.TotalFreeSpace) + " байт");
                    }
                    else
                    {
                        Console.WriteLine("Размер не определён");
                    }
                }
                Console.WriteLine("Атрибуты: " + di.Attributes);
                //разделительная линия
                for (int i = 0; i < Console.BufferWidth; i++)
                {
                    Console.SetCursorPosition(i, Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) + 6);
                    Console.WriteLine('-');
                }
                if (error == 1)
                {
                    Console.WriteLine("Ошибка: " + errorMessege);
                    error = 0;
                }
                for (int i = 0; i < Console.BufferWidth; i++)
                {
                    Console.SetCursorPosition(i, Convert.ToInt32(ConfigurationManager.AppSettings["elmnts"]) + 8);
                    Console.WriteLine('-');
                }
                //запрос у пользователя команды
                string command = Console.ReadLine();
                string[] words = command.Split(' ');
                try
                {
                    //выполнение команд
                    if (words[0] == "del") //удаление
                    {
                        if (words[1] == "file") //удаление файла
                        {
                            RemoveAt(ref words, 0);
                            RemoveAt(ref words, 0);

                            string words1 = string.Join(" ", words);

                            string dirToDel = dir + "/" + words1;

                            FileInfo fileDel = new(dirToDel);
                            if (fileDel.Exists)
                            {
                                fileDel.Delete();
                            }
                        }
                        else if (words[1] == "dir") //удаление папки
                        {
                            RemoveAt(ref words, 0);
                            RemoveAt(ref words, 0);

                            string words1 = string.Join(" ", words);

                            string dirToDel = dir + "/" + words1;
                            Directory.Delete(dirToDel, true);
                        }
                    }
                    else if (words[0] == "exit") //завершение работы программы
                    {
                        exit = 1;
                    }
                    else if (words[0] == "go") //перейти
                    {
                        if (words[1] == "..") //перейти на директорию вниз
                        {
                            list = 1;
                            if (dir.Length <= 3)
                            {
                                dsk = 1;
                            }
                            else
                            {
                                string[] dirs = dir.Split(@"\");
                                RemoveAt(ref dirs, dirs.Length - 1);
                                RemoveAt(ref dirs, dirs.Length - 1);
                                dir = string.Join(@"\", dirs) + @"\";
                                currentConfig.AppSettings.Settings["drctr"].Value = dir;
                                currentConfig.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");
                            }
                        }
                        if (words[1] == "to") //перейти к директории по пути
                        {
                            list = 1;
                            RemoveAt(ref words, 0);
                            RemoveAt(ref words, 0);
                            string words1 = string.Join(" ", words);
                            char[] words2 = words1.ToCharArray();
                            if (words2[words2.Length - 1] == Convert.ToChar(@"\"))
                            {
                                dir = words1;
                            }
                            else
                            {
                                dir = words1 + @"\";
                            }
                            currentConfig.AppSettings.Settings["drctr"].Value = dir;
                            currentConfig.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        else //перейти в директорию находящуюся в текущей
                        {
                            list = 1;
                            RemoveAt(ref words, 0);
                            string words1 = string.Join(" ", words);
                            dir = dir + words1 + @"\";
                            currentConfig.AppSettings.Settings["drctr"].Value = dir;
                            currentConfig.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                    }
                    else if (words[0] == "copy") //скопировать
                    {
                        if (words[1] == "file") //файл
                        {
                            copyFile = 1;
                        }
                        if (words[1] == "dir") //папку
                        {
                            copyFile = 0;
                        }
                        RemoveAt(ref words, 0);
                        RemoveAt(ref words, 0);
                        pasteDir = string.Join(" ", words);
                        copyDir = dir + pasteDir;
                    }
                    else if (words[0] == "paste") //вставить
                    {
                        if (copyFile == 1) //файл
                        {
                            FileInfo fileInf = new(copyDir);
                            fileInf.CopyTo(dir + pasteDir);
                        }
                        if (copyFile == 0) //папку
                        {
                            CopyDir(copyDir, dir + pasteDir + @"\");
                        }
                    }
                    else if (words[0] == "list") //сменить лист
                    {
                        list = Convert.ToInt32(words[1]);
                    }
                    else if (words[0] == "create") 
                    {
                        if (words[1] == "file")
                        {
                            RemoveAt(ref words, 0);
                            RemoveAt(ref words, 0);
                            string words1 = string.Join(" ", words);
                            if (!File.Exists(dir + words1)) File.Create(dir + words1);
                            StreamWriter file = new(dir + words1);
                            file.Close();
                        }
                        if (words[1] == "dir")
                        {
                            RemoveAt(ref words, 0);
                            RemoveAt(ref words, 0);
                            string words1 = string.Join(" ", words);
                            DirectoryInfo dirInfo96 = new(dir + words1);
                            if (!dirInfo96.Exists)
                            {
                                dirInfo96.Create();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    error = 1;
                    errorMessege = e.Message;
                    Logger.WriteLine(e.Message);
                }
                    Console.Clear();
            }
        }
        //метод "удаления" элементов массива
        static void RemoveAt(ref string[] array, int index)
        {
            string[] newArray = new string[array.Length - 1];

            for (int i = 0; i < index; i++)
            {
                newArray[i] = array[i];
            }

            for (int i = index + 1; i < array.Length; i++)
            {
                newArray[i - 1] = array[i];
            }
            array = newArray;
        }
        //метод получения заполненности диска (в байтах)
        static long GetDirSize(string path)
        {
            long size = 0;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                size += (new FileInfo(file)).Length;
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
                size += GetDirSize(dir);
            return size;
        }
        //метод копирования папки с файлами или пустой
        static void CopyDir(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + @"\" + Path.GetFileName(s1);
                File.Copy(s1, s2);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                CopyDir(s, ToDir + @"\" + Path.GetFileName(s));
            }
        }
    }
} 
