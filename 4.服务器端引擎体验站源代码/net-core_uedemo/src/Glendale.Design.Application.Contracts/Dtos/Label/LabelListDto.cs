using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Label
{
    public class LabelListDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 标签名称
        /// </summary>        
        public virtual string LabelName { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 局部链接位置坐标
        /// </summary>
        public virtual string Position { get; set; }         

        /// <summary>
        /// 模型Id
        /// </summary>       
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 文件标识名
        /// </summary>        
        public virtual string BlobName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>       
        public virtual string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public virtual long ByteSize { get; set; }

        /// <summary>
        /// 后缀名
        /// </summary>       
        public virtual string Suffix { get; set; }
    }
}
