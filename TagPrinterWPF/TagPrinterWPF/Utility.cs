using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TagPrinterWPF
{
    

    class Utility
    {
        public static Queue<Message> _list = new Queue<Message>();
        public static ObservableCollection<Message> Loglist = new ObservableCollection<Message>();
        public static Queue<string> msg_forfile = new Queue<string>();
        public static bool startlog = false;
        public static void Log(string _msg, string color = "Black", [CallerMemberName] string fun = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {

            try
            {
                string[] s = filePath.Split('\\');
                int index = s.Length - 1;

                Message msg = new Message();
                msg.DateTime = getDatetime3();
                if (_msg.ToUpper().Contains("ERROR") != true && _msg.ToUpper().Contains("EXCEPTION") != true && _msg.ToUpper().Contains("FALSE") != true)
                {
                    //if (eGateRFIDConfig.LogLevel == 1 && s[index].Contains("obj") == true)
                    //{
                    //    return;
                    //}
                    msg.ProcMessageColor = color;
                }
                else
                {
                    msg.ProcMessageColor = "Red";
                }
                msg.LogMessage = _msg + " \t-- " + lineNumber + "/" + fun + "/" + s[index];
                msg_forfile.Enqueue(msg.LogMessage);
                _list.Enqueue(msg);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }

        }

        public void WriteLogFile(string filename)
        {
            using (StreamWriter sw = File.AppendText(filename))
            {
                while (startlog)
                {
                    Thread.Sleep(50);
                    if (msg_forfile.Count > 0)
                    {
                        try
                        {
                            string m = msg_forfile.Dequeue();
                            sw.WriteLine(m);

                        }
                        catch (Exception ex)
                        {
                            sw.WriteLine(ex.StackTrace);
                        }
                    }
                    sw.Flush();
                }
                sw.WriteLine("Processlog stop!");
            }
        }

        public static string getDatetime3()
        {
            return DateTime.Now.ToString("MMdd.HHmmss");
        }
        public static string getDatetime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string getDatetime2()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        }
        public static string getDatetime4()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static bool LoadDataBaseConfig(string path)
        {
            bool result = false;
            string key = "";
            string value = "";
            //string connection = "";
            string appPath = System.Environment.CurrentDirectory;
            try
            {
                foreach (string line in File.ReadLines($"{appPath}\\{path}"))
                {
                    //Log($"Read from config:{line}");

                    string[] para = line.Split("=");
                    if (line.StartsWith("#") == true)
                    {
                        continue;
                    }

                    //if (para.Length != 2)
                    //{
                    //    continue;
                    //}
                    if (para.Length == 2)
                    {
                        key = para[0].Trim();
                        value = para[1].Trim();
                        Log($"[{key}]:[{value}]");
                    }

                    if (line.ToLower().Contains("server"))
                    {
                        App.SQLConnection = line;
                    }
                    else if (key == "PTR_IP" && para.Length == 2)
                    {
                        App.PTR_IP = value;
                    }
                    else if(key == "enablePTR" && para.Length == 2)
                    {
                        if (value == "true")
                            App.enablePTR = true;
                        else
                        {
                            App.enablePTR=false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            //Log($"LoadDataBaseConfig: [{path}]\n connection: {connection}");
            return result;

        }


    }
}
