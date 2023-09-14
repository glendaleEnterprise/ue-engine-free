using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 漫游类型
    /// </summary>
    public enum RoamingTypeEnum
    {
        /// <summary>
        /// 自定义视点漫游
        /// </summary>
        [Description("自定义视点漫游")]
        ViewPortRoam = 1,
        /// <summary>
        /// 第一/三人称漫游
        /// </summary>
        [Description("第一/三人称漫游")]
        Frist = 0,
    }
}
