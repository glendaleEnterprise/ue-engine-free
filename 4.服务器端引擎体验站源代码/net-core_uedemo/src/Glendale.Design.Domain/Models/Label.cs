using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    public class Label: CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        [Required,MaxLength(100)]
        public virtual string LabelName { get; set; }
         
        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 位置坐标
        /// </summary>
        [MaxLength(200)]
        public virtual string Position { get; set; }

        /// <summary>
        /// 文件标识名(保存的文件夹)
        /// </summary>
        [MaxLength(50)]
        public virtual string BlobName { get; set; }
         
    }
}
