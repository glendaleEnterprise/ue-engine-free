using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.DocumentVerThan
{
    public class MetadataCreateDto
    {
        /// <summary>
        /// 旧版plid#新版plid(例如：f46c292c-c84a-48d0-a812-edbacf6b7565#78e367a3-4362-4542-a020-112d7a903797)
        /// </summary>
        public virtual string Guid { get; set; }
        
        /// <summary>
        /// 构件Id数组
        /// </summary>
        public virtual string[] ExternalIdList { get; set; }
    }


}
