using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.DocumentVerThan
{
    public class MetadataListDto
    {
        /// <summary>
        /// 旧版构件
        /// </summary>
       public virtual MetadataCreateDto SourceMetadata { get; set; }
        /// <summary>
        /// 新版构件
        /// </summary>
        public virtual MetadataCreateDto DestinationMetadata { get; set; }

        /// <summary>
        /// 新增构件
        /// </summary>
        public virtual MetadataCreateDto AddMetadata { get; set; }
        /// <summary>
        /// 修改构件
        /// </summary>
        public virtual MetadataCreateDto UpdateMetadata { get; set; }
        /// <summary>
        /// 删除构件
        /// </summary>
        public virtual MetadataCreateDto DeleteMetadata { get; set; }


    }
}
