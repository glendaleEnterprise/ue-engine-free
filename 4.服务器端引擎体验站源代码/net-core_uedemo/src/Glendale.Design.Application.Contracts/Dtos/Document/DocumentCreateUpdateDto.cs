using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;

namespace Glendale.Design.Dtos.Document
{
    public class DocumentCreateUpdateDto
    {
        [Required]
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 项目目录
        /// </summary>
        [Required]
        public virtual Guid ProjectFolderId { get; set; }
         
        /// <summary>
        /// 名称
        /// </summary>
        [Required, MaxLength(50)]
        public virtual string DocumentName { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public virtual DocTypeEnum DocumentType { get; set; }

        /// <summary>
        /// 模型轻量化名称
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelName { get; set; }
        /// <summary>
        /// BIM/GIS格式
        /// </summary>
        public virtual string ModelFormat { get; set; }
        /// <summary>
        /// 模型大小
        /// </summary>
        public virtual long Size { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>        
        public virtual double VersionNo { get; set; }     
        
        /// <summary>
        /// 场景视角
        /// </summary>
        public virtual string Camera { get; set; }

        /// <summary>
        /// 模型矩阵
        /// </summary>
        public virtual string Matrix { get; set; }

        /// <summary>
        /// 模型配置
        /// </summary>
        public virtual string ModelConfig { get; set; }

        /// <summary>
        /// 场景配置
        /// </summary>
        public virtual string SceneConfig { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }
         
        /// <summary>
        /// 是否公开
        /// </summary>
        public virtual bool IsPublic { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>         
        public virtual string BlobName { get; set; }

        /// <summary>
        /// 变更日志
        /// </summary>
        //public virtual DocumentLogCreateUpdateDto DocumentLog { get; set; }      


    }
}
