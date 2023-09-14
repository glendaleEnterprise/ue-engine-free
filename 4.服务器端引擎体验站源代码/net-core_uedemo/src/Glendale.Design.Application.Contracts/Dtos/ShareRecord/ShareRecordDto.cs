using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ShareRecord
{
    /// <summary>
    /// 合摸
    /// </summary>
    public class ShareRecordDto : FullAuditedEntityDto<Guid>
    {
        #region 属性
        /// <summary>
        /// 项目Id
        /// </summary>
        [MaxLength(100)]
        public virtual string ShareTile { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }         

        /// <summary>
        /// 分享链接
        /// </summary>
        [MaxLength(200)]
        public virtual string ShareLink { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        [MaxLength(50)]
        public virtual string Auth { get; set; }

        /// <summary>
        /// 是否校验
        /// </summary>
        public virtual bool IsVerify { get; set; }

        /// <summary>
        /// 设置分享次数 0=无限制
        /// </summary>
        public virtual int ShareCount { get; set; }

        /// <summary>
        /// 预览次数
        /// </summary>
        public virtual int PvmCount { get; set; }

        /// <summary>
        /// 有效期(天数)
        /// </summary>
        public virtual int EffectiveDay { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>         
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }
         
        #endregion

         
    }
}
