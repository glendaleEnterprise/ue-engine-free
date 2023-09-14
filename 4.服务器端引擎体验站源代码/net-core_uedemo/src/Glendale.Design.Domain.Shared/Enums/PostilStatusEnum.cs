using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 处理状态
    /// </summary>
    public enum PostilStatusEnum:short
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        待处理 = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        已处理 = 1,         
        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        已关闭 = 2
    }
     
}
