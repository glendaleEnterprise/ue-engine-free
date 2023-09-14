using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos
{
    public class LogsCreateUpdateDto
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        [MaxLength(50)]
        public virtual string Name { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        [MaxLength(50)]
        public virtual string ModuleName { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        [MaxLength(200)]
        public virtual string Content { get; set; }
    }
}
