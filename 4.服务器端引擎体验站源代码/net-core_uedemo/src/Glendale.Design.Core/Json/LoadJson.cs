using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glendale.Design.Json
{
    /// <summary>
    /// 通过文件流加载json文件
    /// </summary>
    public class LoadJson
    {
        public static JToken GetJsonFile(string fileName,string key)
        {          
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            JObject jsonObject;

            using (StreamReader file = new StreamReader(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jsonObject = (JObject)JToken.ReadFrom(reader);
                    return jsonObject[key];
                }
            }
        }
    }
}
