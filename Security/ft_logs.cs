using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_logs
    {
        public static SortedDictionary<string,List<string>> logs = new SortedDictionary<string,List<string>>();
        public static string dir;


        public static void setLog(string level, string text)
        {
            if (!logs.ContainsKey(level))
            {
                logs.Add(level, new List<string>());
            }
            logs[level].Add(text.TrimEnd());         
        }

        public static void init()
        {
            WriteLine("-- Log timestamp " + DateTime.Now.ToString("hh:mm:ss tt") + " ---------","" );
        }

        public static void WriteLine(string str, string level)
        {
            run(str, level,"Line");
        }
        public static void Write(string str, string level)
        {
            run(str, level,"inLine");
        }
        public static string run(string str, string level,string style)
        {                    
            string filePath = dir + "\\ Log_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);  

            if (!dirInfo.Exists) dirInfo.Create();

            FileStream fileStream = null;
            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }

            level = (level.Equals("")) ? "":"[" + level + "]" ;
            StreamWriter log = new StreamWriter(fileStream);
            if (style.Equals("Line"))
            {
                log.WriteLine("{0} {1}", level, str.ToString());
            }
            else {
                log.Write("{0} {1}", level, str.ToString());
            }
            log.Close();

            return filePath;
        }




    }
}
