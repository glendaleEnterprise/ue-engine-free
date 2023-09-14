using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentVer
{
    public class DocumentVersionDto : FullAuditedEntityDto<Guid>
    {
         
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        ///// <summary>
        ///// 名称
        ///// </summary>
        //public virtual string DocumentName { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public virtual DocTypeEnum DocumentType { get; set; }
        /// <summary>
        /// 模型轻量化名称
        /// </summary>
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
        /// 场景配置
        /// </summary>
        public virtual string SceneConfig { get; set; }
        /// <summary>
        /// 同步数据状态
        /// </summary>
        //public virtual SyncStatusEnum SyncStatus { get; set; }

        /// <summary>
        /// 是否当前版本
        /// </summary>
        public virtual bool IsCurrent { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
