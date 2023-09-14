using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 公开状态
    /// </summary>
    public enum OpenStatusEnum : short
    {
        /// <summary>
        /// 公开
        /// </summary>
        [Description("公开")]
        Public = 1,
        /// <summary>
        /// 私有
        /// </summary>
        [Description("私有")]
        Private = 0,
    }
}
