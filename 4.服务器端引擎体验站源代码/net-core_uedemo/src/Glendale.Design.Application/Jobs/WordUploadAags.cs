using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glendale.Design.Jobs
{
    public class WordUploadAags
    {    
        /// <summary>
        /// 目标文件夹路径
        /// </summary>
       public string DirPath { get; set; }
        /// <summary>
        /// 文件完整路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
