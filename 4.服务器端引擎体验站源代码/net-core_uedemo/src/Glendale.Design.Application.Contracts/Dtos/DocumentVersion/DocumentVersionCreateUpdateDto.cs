using System;
using System.ComponentModel.DataAnnotations;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Enums;

namespace Glendale.Design.Dtos.DocumentVer
{
    public class DocumentVersionCreateUpdateDto
    {         
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }       

        /// <summary>
        /// 模型轻量化名称
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelName { get; set; }
         
        /// <summary>
        /// 模型大小
        /// </summary>
        public virtual long Size { get; set; }
        /// <summary>
        /// 版本号 0=》小版本+0.1  -1=》大版本
        /// </summary>       
        public virtual double VersionNo { get; set; }        
       

        /// <summary>
        /// 是否当前版本
        /// </summary>
        public virtual bool IsCurrent { get; set; }
        /// <summary>
        /// 模型矩阵
        /// </summary>
        [MaxLength(500)]
        public virtual string Matrix { get; set; }

        /// <summary>
        /// 场景配置
        /// </summary>
        [MaxLength(2000)]
        public virtual string SceneConfig { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }
       
    }
}
