using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
namespace Glendale.Design.Models
{
    /// <summary>
    /// 模型版本比较
    /// </summary>
    public class DocumentVerThan : CreationAuditedEntityWithUser<Guid, IdentityUser>
    {
        #region 属性

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(50)]
        public virtual string Title { get; set; }

        /// <summary>
        /// 源模型版本Id
        /// </summary>
        public virtual Guid SourceDocVerId { get; set; }

        /// <summary>
        /// 目标模型版本Id
        /// </summary>
        public virtual Guid DestinationDocVerId { get; set; }

        /// <summary>
        /// 新增的构件
        /// </summary>
        public virtual string AddMetadata { get; set; }

        /// <summary>
        /// 修改的构件
        /// </summary>
        public virtual string UpdateMetadata { get; set; }

        /// <summary>
        /// 删除的构件
        /// </summary>
        public virtual string DeleteMetadata { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public virtual string Remark { get; set; }

        #endregion


        #region 导航属性

        [NotMapped]
        public virtual DocumentVersion SourceDocVer { get; set; }

        [NotMapped]
        public virtual DocumentVersion DestinationDocVer { get; set; }
        #endregion
    }
}
