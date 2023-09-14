using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Enums
{
    /// <summary>
    /// 文档类型
    /// </summary>
    public enum DocTypeEnum : short
    {
         
        /// <summary>
        /// 模型
        /// </summary>
        [Display(Name = "模型")]
        Model = 1,         
        /// <summary>
        /// GIS数据
        /// </summary>
        [Display(Name = "GIS")]
        GIS = 2,
        /// <summary>
        /// UE模型
        /// </summary>
        [Display(Name ="UE")]
        UE=3,
        /// <summary>
        ///CAD图纸
        /// </summary>
        [Display(Name = "CAD")]
        CAD = 4,

    }
}
