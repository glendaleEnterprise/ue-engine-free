using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 模型图纸表
    /// </summary>
    public class ModelDrawing : Entity<int>
    {
        /// <summary>
        /// 视图名称
        /// </summary>
        [MaxLength(100)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Dwg文件名称
        /// </summary>
        [MaxLength(100)]
        public virtual string FileName { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        [MaxLength(50)]
        public virtual string Type { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        [MaxLength(36)]
        public virtual string Guid { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        [MaxLength(36)]
        public virtual string ModelName { get; set; }
      
    }
}
