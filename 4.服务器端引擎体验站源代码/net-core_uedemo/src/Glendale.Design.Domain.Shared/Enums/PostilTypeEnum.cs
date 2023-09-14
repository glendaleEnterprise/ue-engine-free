using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 批注类型(单模型、合模)
    /// </summary>
    public enum PostilTypeEnum : short
    {
        /// <summary>
        /// 单模型
        /// </summary>
        [Description("单模型")]
        Single = 0,
        /// <summary>
        /// 合模
        /// </summary>
        [Description("合模")]
        Multi = 1,         
    }
}
