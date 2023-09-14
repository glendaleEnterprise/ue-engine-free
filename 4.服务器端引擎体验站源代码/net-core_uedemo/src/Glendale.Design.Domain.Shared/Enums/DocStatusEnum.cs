using System.ComponentModel;

namespace Glendale.Design.Enums
{
    public enum DocStatusEnum
    {       
        /// <summary>
        /// 待轻量化
        /// </summary>
        [Description("待轻量化")]
        Await,
        /// <summary>
        /// 轻量化中
        /// </summary>
        [Description("轻量化中")]
        Running,
        /// <summary>
        /// 轻量化成功
        /// </summary>
        [Description("轻量化成功")]
        Succeed,
        /// <summary>
        /// 轻量化失败
        /// </summary>
        [Description("轻量化失败")]
        Failure
    }
}
