using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Document
{
    public class DocumentDto : EntityDto<Guid>
    {

        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 项目目录Id
        /// </summary>
        public virtual Guid ProjectFolderId { get; set; }       
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string DocumentName { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public virtual DocTypeEnum DocumentType { get; set; }
         
        /// <summary>
        /// 模型轻量化名称
        /// </summary>
        public virtual string ModelName { get; set; }
        /// <summary>
        /// 格式rvt/osgb...
        /// </summary>
        public virtual string ModelFormat { get; set; }
        /// <summary>
        /// 模型大小
        /// </summary>
        public virtual long Size { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public virtual string VersionNo { get; set; }
              
        /// <summary>
        /// 轻量化状态
        /// </summary>
        public virtual DocStatusEnum Status { get; set; }
        /// <summary>
        /// 轻量化异常
        /// </summary>
        public virtual string Exception { get; set; }
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
        /// 场景视角
        /// </summary>
        public virtual string Camera { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
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
        /// 文档版本
        /// </summary>
        public virtual DocumentVersionDto DocumentVersion { get; set; }
    }
}
