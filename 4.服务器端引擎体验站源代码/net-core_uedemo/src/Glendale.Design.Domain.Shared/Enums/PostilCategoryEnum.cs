using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 问题分类
    /// </summary>
    public enum PostilCategoryEnum
    {
        /// <summary>
        /// 一般问题
        /// </summary>
        [Description("一般问题")]
        Default = 0,
        /// <summary>
        /// 严重问题
        /// </summary>
        [Description("严重问题")]
        Grave = 1,
        /// <summary>
        /// 特大问题
        /// </summary>
        [Description("特大问题")]
        Most = 2,
    }
}
