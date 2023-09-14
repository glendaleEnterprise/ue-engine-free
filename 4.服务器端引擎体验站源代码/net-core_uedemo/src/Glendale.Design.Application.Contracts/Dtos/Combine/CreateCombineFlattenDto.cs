using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.Combine
{
    public class CreateCombineFlattenDto
    {
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid? CombineId { get; set; }

        /// <summary>
        /// 类型 0：压平； 1：裁剪；
        /// </summary>
        public virtual int FlattenType { get; set; }
        /// <summary>
        /// 压平名称
        /// </summary>
        [MaxLength(50)]
        public virtual string FlattenName { get; set; }

        /// <summary>
        /// 压平高度
        /// </summary>
        public virtual double FlattenHeight { get; set; }
        /// <summary>
        /// 压平范围
        /// </summary>

        public virtual string FlattenScope { get; set; }
    }
}
