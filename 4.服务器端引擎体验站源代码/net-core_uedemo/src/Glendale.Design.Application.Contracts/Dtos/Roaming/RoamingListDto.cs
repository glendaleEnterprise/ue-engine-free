using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 漫游设置
    /// </summary>
    public class RoamingListDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 漫游类型
        /// </summary>
        public virtual RoamingTypeEnum RoamingType { get; set; }

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
        /// <summary>
        /// 漫游记录
        /// </summary>
        public virtual string Record { get; set; }
    }
}