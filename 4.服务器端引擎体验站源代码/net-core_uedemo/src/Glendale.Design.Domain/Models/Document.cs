using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 文档资料
    /// </summary>
    public class Document : FullAuditedEntityWithUser<Guid, IdentityUser>
    {
        #region 属性 

        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 目录Id
        /// </summary>
        public virtual Guid ProjectFolderId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required, MaxLength(50)]
        public virtual string DocumentName { get; set; }

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
        /// 模型大小 kb
        /// </summary>
        public virtual long Size { get; set; }

        /// <summary>
        /// 版本号(0.1)
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
        /// 轻量化异常
        /// </summary>
        public virtual string Exception { get; set; }

        /// <summary>
        /// 场景视角
        /// </summary>
        public virtual string Camera { get; set; }

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
        /// 模型矩阵
        /// </summary>
        [MaxLength(500)]
        public virtual string Matrix { get; set; }

        /// <summary>
        /// 模型配置
        /// </summary>
        public virtual string ModelConfig { get; set; }
        /// <summary>
        /// 场景配置
        /// </summary>
        [MaxLength(2000)]
        public virtual string SceneConfig { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>         
        public virtual string BlobName { get; set; }

        #endregion

        #region 导航属性

        [NotMapped]
        public virtual Project Project { get; set; }

        [NotMapped]
        public virtual ProjectFolder ProjectFolder { get; set; }

        /// <summary>
        /// 文档版本
        /// </summary>
        [NotMapped]
        public virtual ICollection<DocumentVersion> DocumentVersion { get; set; }

        /// <summary>
        /// 合模明细
        /// </summary>
        [NotMapped]
        public virtual ICollection<CombineDetail> CombineDetail { get; set; }

        /// <summary>
        /// 变更日志
        /// </summary>
        [NotMapped]
        public virtual ICollection<DocumentLog> DocumentLog { get; set; }
        #endregion

        public Document()
        {

        }

        public Document(Guid id) : base(id)
        {

        }
    }
}
