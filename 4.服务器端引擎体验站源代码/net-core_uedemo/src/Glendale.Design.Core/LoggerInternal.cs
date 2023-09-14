using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glendale.Design
{
    public  class LoggerInternal
    {
        public static void WriteLog(string msg)
        {
            string rootPath = AppContext.BaseDirectory;
            string savePath = Path.Combine(rootPath, "CustomLog");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            using (var fs = new StreamWriter(Path.Combine(savePath, "test.txt"), true))
            {
                fs.WriteLineAsync(msg);
            }
        }
    }
}
