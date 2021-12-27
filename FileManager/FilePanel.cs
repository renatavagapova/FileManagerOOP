using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManager
{
    class FilePanel
    {
        #region Parameters
        public int panelH = 20;
        public static int PANEL_HEIGHT = Program.startLine;
        public static int PANEL_WIDTH = UI.windWidth / 2;
        #endregion

        #region Panel location

        private int top;
        public int Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (0 <= value && value <= Console.WindowHeight - FilePanel.PANEL_HEIGHT)
                {
                    this.top = value;
                }
                else
                {
                    throw new Exception(String.Format("Выход координаты top ({0}) файловой панели за допустимое значение", value));
                }
            }
        }

        private int left;
        public int Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (0 <= value && value <= Console.WindowWidth - FilePanel.PANEL_WIDTH)
                {
                    this.left = value;
                }
                else
                {
                    throw new Exception(String.Format("Выход координаты left ({0}) файловой панели за пределы окна", value));
                }
            }
        }

        private int height = FilePanel.PANEL_HEIGHT;
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (0 < value && value <= Console.WindowHeight)
                {
                    this.height = value;
                }
                else
                {
                    throw new Exception(String.Format("Высота файловой панели {0} больше размера окна", value));
                }
            }
        }

        private int width = FilePanel.PANEL_WIDTH;
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (0 < value && value <= Console.WindowWidth)
                {
                    this.width = value;
                }
                else
                {
                    throw new Exception(String.Format("Ширина файловой панели {0} больше размера окна", value));
                }
            }
        }
        #endregion

        #region Panel state

        private string path;
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                DirectoryInfo di = new DirectoryInfo(value);
                if (di.Exists)
                {
                    this.path = value;
                }
                else
                {
                    throw new Exception(String.Format("Путь {0} не существует", value));
                }
            }
        }

        private int activeObjectIndex = 0;
        private int firstObjectIndex = 0;
        private int displayedObjectsAmount = PANEL_HEIGHT - 2;
        private bool active;
        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
            }
        }
        private bool discs;
        public bool isDiscs
        {
            get
            {
                return this.discs;
            }
        }

        #endregion

        public List<FileSystemInfo> fsObjects = new List<FileSystemInfo>();

        #region Ctor

        public FilePanel()
        {
            SetDiscs();
        }

        public FilePanel(string path)
        {
            this.path = path;
            SetLists();
        }

        #endregion

        public FileSystemInfo GetActiveObject()
        {
            if (fsObjects != null && fsObjects.Count != 0)
            {
                return fsObjects[activeObjectIndex];
            }
            throw new Exception("Список объектов панели пуст");
        }

        public bool FindFile(string name)
        {
            int index = 0;
            foreach (FileSystemInfo file in this.fsObjects)
            {
                if (file != null && file.Name == name)
                {
                    activeObjectIndex = index;
                    if (activeObjectIndex > displayedObjectsAmount)
                    {
                        firstObjectIndex = activeObjectIndex;
                    }
                    UpdateContent(false);
                    return true;
                }
                index++;
            }
            return false;
        }

        #region Navigations

        public void KeyboardProcessing(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    ScrollUp();
                    break;
                case ConsoleKey.DownArrow:
                    ScrollDown();
                    break;
                case ConsoleKey.Home:
                    GoBegin();
                    break;
                case ConsoleKey.End:
                    GoEnd();
                    break;
                case ConsoleKey.PageUp:
                    PageUp();
                    break;
                case ConsoleKey.PageDown:
                    PageDown();
                    break;
                default:
                    break;
            }

        }

        private void ScrollDown()
        {
            if (this.activeObjectIndex >= this.firstObjectIndex + this.displayedObjectsAmount - 1)
            {
                this.firstObjectIndex += 1;
                if (this.firstObjectIndex + this.displayedObjectsAmount >= this.fsObjects.Count)
                {
                    this.firstObjectIndex = this.fsObjects.Count - this.displayedObjectsAmount;
                }
                this.activeObjectIndex = this.firstObjectIndex + this.displayedObjectsAmount - 1;
                this.UpdateContent(false);
            }

            else
            {
                if (this.activeObjectIndex >= this.fsObjects.Count - 1)
                {
                    return;
                }
                this.DeactivateObject(this.activeObjectIndex);
                this.activeObjectIndex++;
                this.ActivateObject(this.activeObjectIndex);
            }
        }

        private void ScrollUp()
        {
            if (this.activeObjectIndex <= this.firstObjectIndex)
            {
                this.firstObjectIndex -= 1;
                if (this.firstObjectIndex < 0)
                {
                    this.firstObjectIndex = 0;
                }
                this.activeObjectIndex = firstObjectIndex;
                this.UpdateContent(false);
            }
            else
            {
                this.DeactivateObject(this.activeObjectIndex);
                this.activeObjectIndex--;
                this.ActivateObject(this.activeObjectIndex);
            }
        }

        private void GoEnd()
        {
            if (this.firstObjectIndex + this.displayedObjectsAmount < this.fsObjects.Count)
            {
                this.firstObjectIndex = this.fsObjects.Count - this.displayedObjectsAmount;
            }
            this.activeObjectIndex = this.fsObjects.Count - 1;
            this.UpdateContent(false);
        }

        private void GoBegin()
        {
            this.firstObjectIndex = 0;
            this.activeObjectIndex = 0;
            this.UpdateContent(false);
        }

        private void PageDown()
        {
            if (this.activeObjectIndex + this.displayedObjectsAmount < this.fsObjects.Count)
            {
                this.firstObjectIndex += this.displayedObjectsAmount;
                this.activeObjectIndex += this.displayedObjectsAmount;
            }
            else
            {
                this.activeObjectIndex = this.fsObjects.Count - 1;
            }
            this.UpdateContent(false);
        }

        private void PageUp()
        {
            if (this.activeObjectIndex > this.displayedObjectsAmount)
            {
                this.firstObjectIndex -= this.displayedObjectsAmount;
                if (this.firstObjectIndex < 0)
                {
                    this.firstObjectIndex = 0;
                }

                this.activeObjectIndex -= this.displayedObjectsAmount;

                if (this.activeObjectIndex < 0)
                {
                    this.activeObjectIndex = 0;
                }
            }
            else
            {
                this.firstObjectIndex = 0;
                this.activeObjectIndex = 0;
            }
            this.UpdateContent(false);
        }

        #endregion

        #region Fill panels

        public void SetLists()
        {
            if (this.fsObjects.Count != 0)
            {
                this.fsObjects.Clear();
            }

            this.discs = false;

            DirectoryInfo levelUpDirectory = null;
            this.fsObjects.Add(levelUpDirectory);

            //Directories

            string[] directories = Directory.GetDirectories(this.path);
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                this.fsObjects.Add(di);
            }

            //Files

            string[] files = Directory.GetFiles(this.path);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                this.fsObjects.Add(fi);
            }
        }

        public void SetDiscs()
        {
            if (this.fsObjects.Count != 0)
            {
                this.fsObjects.Clear();
            }

            this.discs = true;

            DriveInfo[] discs = DriveInfo.GetDrives();
            foreach (DriveInfo disc in discs)
            {
                if (disc.IsReady)
                {
                    DirectoryInfo di = new DirectoryInfo(disc.Name);
                    this.fsObjects.Add(di);
                }
            }
        }

        #endregion

        #region Display methods

        public void Show()
        {
            this.Clear();
            StringBuilder caption = new StringBuilder();
            if (this.discs)
            {
                caption.Append(' ').Append("Диски").Append(' ');
            }
            else
            {
                caption.Append(' ').Append(this.path).Append(' ');
            }
            UI.PrintString(caption.ToString(), this.left + this.width / 2 - caption.ToString().Length / 2, this.top - 1, ConsoleColor.White, ConsoleColor.Black);

            this.PrintContent();
        }

        public void Clear()
        {
            for (int i = 0; i < this.height; i++)
            {
                string space = new String(' ', width);
                Console.SetCursorPosition(this.left, this.top + i);
                Console.Write(space);
            }
        }

        private void PrintContent()
        {
            if (this.fsObjects.Count == 0)
            {
                return;
            }
            int count = 0;

            int lastElement = this.firstObjectIndex + this.displayedObjectsAmount;
            if (lastElement > this.fsObjects.Count)
            {
                lastElement = this.fsObjects.Count;
            }


            if (this.activeObjectIndex >= this.fsObjects.Count)
            {
                activeObjectIndex = 0;
            }

            for (int i = this.firstObjectIndex; i < lastElement; i++)
            {
                Console.SetCursorPosition(this.left + 1, this.top + count + 1);

                if (i == this.activeObjectIndex && this.active == true)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                this.PrintObject(i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                count++;
            }
        }

        private void ClearContent()
        {
            for (int i = 1; i < this.height - 1; i++)
            {
                string space = new String(' ', this.width - 2);
                Console.SetCursorPosition(this.left + 1, this.top + i);
                Console.Write(space);
            }
        }
        private void ClearInfo()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 1; i <= 8; i++)
            {
                string space = new String(' ', this.width - 2);
                Console.SetCursorPosition(1, 10 + i);
                Console.Write(space);
            }
        }


        private void PrintObject(int index)
        {
            if (index < 0 || this.fsObjects.Count <= index)
            {
                throw new Exception(String.Format("Невозможно вывести объект c индексом {0}. Выход индекса за диапазон", index));
            }

            int currentCursorTopPosition = Console.CursorTop;
            int currentCursorLeftPosition = Console.CursorLeft;

            if (!this.discs && index == 0)
            {
                Console.Write("..");

                return;
            }

            Console.Write("{0}", fsObjects[index].Name);

            int h = Console.BufferHeight;

            int w = Console.BufferWidth;

            Console.SetCursorPosition(1, 54);
            ClearInfo();
            Console.SetCursorPosition(1, 54);
            InfoWrite(index);

            Console.SetCursorPosition(currentCursorLeftPosition + this.width / 2, currentCursorTopPosition);
            if (fsObjects[index] is DirectoryInfo)
            {
                Console.Write("{0}", ((DirectoryInfo)fsObjects[index]).LastWriteTime);
            }
            else
            {
                Console.Write("{0}", ((FileInfo)fsObjects[index]).Length);
            }
        }
        public void InfoWrite(int index)
        {
            if (this.discs)
            {
                GetFullData(index);
            }

            else if (fsObjects[index] is DirectoryInfo)
            {
                Console.WriteLine("Полное имя : " + ((DirectoryInfo)fsObjects[index]).FullName);
                //Console.WriteLine("Имя диска: " + ((DirectoryInfo)fsObjects[index]).Name);
                Console.WriteLine(" Имя диска: " + ((DirectoryInfo)fsObjects[index]).Parent);
                Console.WriteLine(" Дата создания: " + ((DirectoryInfo)fsObjects[index]).CreationTime);
                Console.WriteLine(" Имя диска: " + ((DirectoryInfo)fsObjects[index]).Exists);
                Console.WriteLine(" Атрибуты: " + ((DirectoryInfo)fsObjects[index]).Attributes);
                Console.WriteLine(" Расширение файла: " + ((DirectoryInfo)fsObjects[index]).Extension);
            }
            else
            {
                Console.Write("{0}", ((FileInfo)fsObjects[index]).Length);
            }
        }

        /// <summary>
        /// Полная информация о диске
        /// </summary>
        public void GetFullData(int index)
        {
            foreach (var drive in DriveInfo.GetDrives())
            {

                if (fsObjects[index].Name == drive.Name)
                {
                    try
                    {
                        Console.WriteLine("Имя диска: " + drive.Name);
                        Console.WriteLine(" Файловая система: " + drive.DriveFormat);
                        Console.WriteLine(" Тип диска: " + drive.DriveType);
                        Console.WriteLine(" Объем доступного свободного места (в байтах): " + drive.AvailableFreeSpace);
                        Console.WriteLine("Готов ли диск: " + drive.IsReady);
                        //Console.WriteLine("Корневой каталог диска: " + drive.RootDirectory);
                        Console.WriteLine(" Общий объем свободного места, доступного на диске (в байтах): " + drive.TotalFreeSpace);
                        Console.WriteLine(" Размер диска (в байтах): " + drive.TotalSize);
                        Console.WriteLine(" Метка тома диска: " + drive.VolumeLabel);
                    }
                    catch
                    {
                    }
                }
            }
        }


        public void UpdatePanel()
        {
            this.firstObjectIndex = 0;
            this.activeObjectIndex = 0;
            this.Show();
        }

        public void UpdateContent(bool updateList)
        {
            if (updateList)
            {
                this.SetLists();
            }
            this.ClearContent();
            this.PrintContent();
        }

        private void ActivateObject(int index)
        {
            int offsetY = this.activeObjectIndex - this.firstObjectIndex;
            Console.SetCursorPosition(this.left + 1, this.top + offsetY + 1);

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            this.PrintObject(index);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private void DeactivateObject(int index)
        {
            int offsetY = this.activeObjectIndex - this.firstObjectIndex;
            Console.SetCursorPosition(this.left + 1, this.top + offsetY + 1);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            this.PrintObject(index);
        }
        #endregion
    }
}
