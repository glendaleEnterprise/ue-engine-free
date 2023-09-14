using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ViewPoint
{
    /// <summary>
    /// 视点设置
    /// </summary>
    public class ViewPointListDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>      
        public virtual string ViewName { get; set; }

         
        public virtual string Position { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>       
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>       
        public virtual string Remark { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>         
        public virtual string BlobName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual int? Status { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual string SceneTypeName { get; set; }

    }
}