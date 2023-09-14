using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 分享功能
    /// </summary>
    public class ShareRecord : FullAuditedEntity<Guid>
    {
        #region 属性

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        [MaxLength(100)]
        public virtual string ShareTile { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }         

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
        /// 设置有效期(天数) 0=无限制
        /// </summary>
        public virtual int EffectiveDay { get; set; }

        /// <summary>
        /// 是否超期||超出分享次数
        /// </summary>
        //[NotMapped]
        //public bool IsExceed
        //{
        //    get {
        //        //当前时间-创建时间=分享天数   21 > 5
        //        if ((DateTime.Now - CreationTime).Days > EffectiveDay)
        //        {
        //            return true;//过期
        //        }
        //        if(ShareCount>0 && PvmCount> ShareCount)
        //        {
        //            return true;  //超出分享次数
        //        }
        //        return false; 
        //    }
        //}

        /// <summary>
        /// 状态分享
        /// </summary>
        ///  
        //private  StareStateEnum shareState {get;set;}
        //public virtual StareStateEnum ShareState {
        //    get {
        //        if (shareState != StareStateEnum.Close && shareState!=StareStateEnum.LongOpen) {
        //            if (IsExceed)
        //            {
        //                return StareStateEnum.Overdue;
        //            }
        //        }
        //        return shareState; 

        //    }
        //    set {
        //        if (value != StareStateEnum.Close&&value!=StareStateEnum.LongOpen) {
        //            if (IsExceed)
        //                value = StareStateEnum.Overdue;
        //            else
        //                value = StareStateEnum.Open;
        //        }
        //        shareState = value;
        //    }
        //}

        /// <summary>
        /// 分享状态
        /// </summary> 
        [NotMapped]
        public virtual StareStateEnum ShareState { get; set; }

        /// <summary>
        /// 模型Id/合模Id
        /// </summary>         
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }
        #endregion

        #region 导航属性

        #endregion
        public ShareRecord()
        {

        }
        
    }
}
