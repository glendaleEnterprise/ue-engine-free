using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 第一人称漫游设置
    /// </summary>
    public class RoamingFristCreateDto : RoamingCreateDto
    {
        /// <summary>
        /// 第一/第三人称漫游轨迹   
        /// </summary>
        public ICollection<RoamingPointCreateUpdateDto> RoamingPoints { get; set; }        

    }

    public class RoamingViewPortCreateDto : RoamingCreateDto
    {   
        /// <summary>
        /// 自定义视点漫游视点集合
        /// </summary>
        public ICollection<ViewPortPointCreateDto> ViewPortPoints { get; set; }

    }


    public class RoamingCreateDto
    {
        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        [MaxLength(100)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 方案时长
        /// </summary>
        public virtual decimal? Time { get; set; }
        /// <summary>
        /// 方案备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int Sort { get; set; }
    }
}