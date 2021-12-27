using System;
using System.Configuration;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetBufferSize(3840, 2160);

            //Process p = Process.GetCurrentProcess();
            //ShowWindow(p.MainWindowHandle, 3);



            //Console.SetWindowSize(Console.WindowWidth, Console.WindowHeight);


            Console.CursorVisible = false;

            ReadAllSettings();


            FileManager manager = new FileManager();

            UIElements.FirstLine();
            //UIElements.SecondLine();


            manager.Explore();

            if (FileManager.NumLineStr != "18")
            {
                AddUpdateAppSettings(FileManager.l, FileManager.NumLineStr);
            }
        }

        static void NewSettings()
        {
            ReadAllSettings();

            //Console.WriteLine("Имя: ");
            AddUpdateAppSettings(FileManager.l, FileManager.NumLineStr);
            ReadSetting(FileManager.l);
        }

        public static int startLine;

        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    startLine = 20;
                }
                else
                {
                    appSettings.GetValues(0);

                    foreach (var key in appSettings.AllKeys)
                    {
                        if (key == "l")
                        {
                            try
                            {
                                startLine = Convert.ToInt32(appSettings[key]);

                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        static void ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";

                if (key == "l")
                {
                    try
                    {
                        startLine = Convert.ToInt32(appSettings[key]);

                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }


            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
