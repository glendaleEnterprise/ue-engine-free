using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 分享状态
    /// </summary>
    public enum StareStateEnum
    {
        /// <summary>
        /// 分享
        /// </summary>
        [Description("分享")]
        Open = 1,

        /// <summary>
        /// 超期
        /// </summary>
        [Description("超期")]
        Overdue = 0,

        /// <summary>
        /// 超出分享次数   
        /// </summary>
        [Description("超出分享次数")]
        BeyondShare = -1,         
    }
}
