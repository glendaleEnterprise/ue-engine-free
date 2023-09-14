using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Glendale.Design.Dtos.Combine
{
    public class CombineCreateUpdateDto
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
        [MaxLength(50)]
        public virtual string CombineName { get; set; }
        /// <summary>
        /// 场景视角
        /// </summary>
        [MaxLength(500)]
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

        #region 属性关系表
        public virtual ICollection<CreateCombineDetailDto> CombineDetails { get; set; }

        public virtual ICollection<CreateCombineFlattenDto> CombineFlattens { get; set; }

        public virtual ICollection<CreateCombineFlattenDto> CombineCuts { get; set; }
        #endregion
    }
}
