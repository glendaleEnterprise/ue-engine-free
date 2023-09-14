using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 合模
    /// </summary>
	public class Combine : FullAuditedEntityWithUser<Guid,IdentityUser>
    {
        #region 属性

        /// <summary>
        /// 所属目录Id
        /// </summary>
        [Required]
        public virtual Guid FolderId { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        [Required]
        public virtual Guid ProjectId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required, MaxLength(50)]
        public virtual string CombineName { get; set; }
        /// <summary>
        /// 场景视角
        /// </summary>
        [Required, MaxLength(500)]
        public virtual string Camera { get; set; }
        /// <summary>
        /// 是否启用GIS
        /// </summary>
        public virtual bool IsGis { get; set; }

        /// <summary>
        /// 开启GIS类型
        /// </summary>
        [MaxLength(50)]
        public virtual string GisType { get; set; }
        /// <summary>
        /// 合模说明
        /// </summary>
        [MaxLength(500)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        public virtual bool IsOpen { get; set; }

        /// <summary>
        /// 曝光度
        /// </summary>
        public virtual float Luminance { get; set; }

        /// <summary>
        /// 太阳光
        /// </summary>
        public virtual float Sunlight { get; set; }

        /// <summary>
        /// 场景配置
        /// </summary>
        public virtual string SceneConfig { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>         
        public virtual string BlobName { get; set; }
        #endregion

        #region 导航属性
        /// <summary>
        /// 合模明细
        /// </summary>
        [NotMapped]
        public virtual ICollection<CombineDetail> CombineDetails { get; set; }

        /// <summary>
        /// 合模压平
        /// </summary>
        [NotMapped]
        public virtual ICollection<CombineFlatten> CombineFlattens { get; set; }
        /// <summary>
        /// 合模裁剪
        /// </summary>
        public virtual ICollection<CombineFlatten> CombineCuts { get; set; }
        /// <summary>
        /// 合模日志
        /// </summary>
        [NotMapped]
        public virtual ICollection<CombineLog> CombineLogs { get; set; }
        #endregion

        public Combine()
        {
        }

        public Combine(Guid id, Guid projectId, string combineName, string camera, string remark
            , ICollection<CombineDetail> combineDetail
            , ICollection<CombineFlatten> combineFlattens) : base(id)
        {
            ProjectId = projectId;
            CombineName = combineName;
            Camera = camera;
            Remark = remark;
            CombineDetails = combineDetail;
            CombineFlattens=combineFlattens;
        }
    }
}
