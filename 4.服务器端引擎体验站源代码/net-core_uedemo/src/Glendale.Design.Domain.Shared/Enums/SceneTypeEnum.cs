using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 场景类型
    /// </summary>
    public enum SceneTypeEnum
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
