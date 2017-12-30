using System;
using System.IO;
using System.Text;

namespace TxtFileIO
{
    class TxtFileIO
    {
        public static string[] TxtFileRead(string file_name)     //读txt函数
        {
            string[] strs = new string[1000];
            if (File.Exists(file_name))     //判断文件是否存在
            {
                using (StreamReader sr = new StreamReader(file_name))
                {
                    string line;
                    int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        strs[i++] = line;
                    }
                }
            }
            return strs;
        }

        public static bool TxtFileWrite(string file_name, string[] strs,bool writeMode)   //x写txt函数
        {
            try
            {
                if (!File.Exists(file_name))  //判断文件是否存在
                {
                    File.Create(file_name);   //不存在的话，则创建文件
                }
            }
            catch(Exception e)     //捕获异常
            {
                Console.WriteLine(e.Message);
                return false;   //返回false
            }

            using (StreamWriter sr = new StreamWriter(file_name,writeMode))   //追加模式: writeMode=true
            {
                int i = 0;
                while ((strs[i]) != null)    
                {
                    sr.WriteLine(strs[i++]);
                }
                sr.Close();
                return true;
            }
        }
    }
}
