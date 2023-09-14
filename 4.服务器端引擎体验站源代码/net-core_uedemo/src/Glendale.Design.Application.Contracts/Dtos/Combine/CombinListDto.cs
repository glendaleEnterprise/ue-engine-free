using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Combine
{
    public class CombineListDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 所属目录Id
        /// </summary>         
        public virtual Guid FolderId { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Guid ProjectId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string CombineName { get; set; }
        /// <summary>
        /// 场景视角
        /// </summary>
        public virtual string Camera { get; set; }
        /// <summary>
        /// 是否启用GIS
        /// </summary>
        public virtual bool IsGis { get; set; }

        /// <summary>
        /// 开启GIS类型
        /// </summary>        
        public virtual string GisType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
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


        public virtual ICollection<ShowCombineDetailDto> CombineDetails { get; set; }
        /// <summary>
        /// 合模压平
        /// </summary>
        public virtual ICollection<ShowCombineFlattenDto> CombineFlattens { get; set; }
        /// <summary>
        /// 合模裁剪
        /// </summary>
        public virtual ICollection<ShowCombineFlattenDto> CombineCuts { get; set; }

        #region 自定义属性
        public virtual string CreationName { get; set; }

        
        #endregion
    }
}
