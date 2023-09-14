using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Enums
{
    public enum ModelStatusEnum
    {
        /// <summary>
        /// 上传中
        /// </summary>
        [Display(Name = "上传中")]
        Uploading = 0,
        /// <summary>
        /// 轻量化中
        /// </summary>
        [Display(Name = "轻量化中")]
        Lightweighting = 1,
        /// <summary>
        /// 完成轻量化
        /// </summary>
        [Display(Name = "完成轻量化")]
        FinishLightweight = 2,
        /// <summary>
        /// 完成同步数据
        /// </summary>
        [Display(Name = "完成同步数据")]
        FinishSync = 3,
        /// <summary>
        /// 轻量化失败
        /// </summary>
        [Display(Name = "轻量化失败")]
        LightweightFail = 4,
        /// <summary>
        /// 同步失败
        /// </summary>
        [Display(Name = "同步失败")]
        SyncFail = 5,
        /// <summary>
        /// 同步中
        /// </summary>
        [Display(Name = "同步中")]
        Syncing = 6,
    }
}
