using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.Combine
{
    public class ShowCombineFlattenDto
    {
        /// <summary>
        /// 类型 0：压平； 1：裁剪；
        /// </summary>
        public virtual int FlattenType { get; set; }
        /// <summary>
        /// 名称
        /// </summary>      
        public virtual string FlattenName { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public virtual double FlattenHeight { get; set; }
        /// <summary>
        /// 范围
        /// </summary>

        public virtual string FlattenScope { get; set; }
    }
}
