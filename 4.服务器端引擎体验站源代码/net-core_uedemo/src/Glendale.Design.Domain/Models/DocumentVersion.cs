using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 文档版本
    /// </summary>
    public class DocumentVersion : FullAuditedEntity<Guid>
    {
        #region 属性       
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        //[Required, MaxLength(50)]
        //public virtual string DocumentName { get; set; }
        
        /// <summary>
        /// 模型轻量化名称
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelName { get; set; }

        /// <summary>
        /// Model格式 rvt/osgb/....
        /// </summary>
        public virtual string ModelFormat { get; set; }

        /// <summary>
        /// 模型大小
        /// </summary>
        public virtual long Size { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>        
        public virtual double VersionNo { get; set; } = 0.1;
        /// <summary>
        /// 文档类型
        /// </summary>
        public virtual DocTypeEnum DocumentType { get; set; }
        /// <summary>
        /// 轻量化状态
        /// </summary>
        [Required]
        public virtual DocStatusEnum Status { get; set; }
        /// <summary>
        /// 同步数据状态
        /// </summary>
        //[Required]
        //public virtual SyncStatusEnum SyncStatus { get; set; }
         
        /// <summary>
        /// 是否当前版本
        /// </summary>
        public virtual bool IsCurrent { get; set; }

        /// <summary>
        /// 轻量化异常
        /// </summary>
        public virtual string Exception { get; set; }

        

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }
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
        #endregion

        #region
        /// <summary>
        /// 文档
        /// </summary>
        [NotMapped]
        public virtual Document Document { get; set; }

        /// <summary>
        /// 批注
        /// </summary>
        [NotMapped]
        public virtual ICollection<Postil> Postil { get; set; }

        #endregion
        
    }
}
