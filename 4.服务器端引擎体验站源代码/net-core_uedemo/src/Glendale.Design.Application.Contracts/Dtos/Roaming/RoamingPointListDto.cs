using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 自定义漫游设置
    /// </summary>
    public class RoamingPointListDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 漫游ID
        /// </summary>
        public virtual Guid? RoamingId { get; set; }

        /// <summary>
        /// 漫游记录
        /// </summary>
        [MaxLength(200)]
        public virtual string Record { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int Sort { get; set; }
    }
}