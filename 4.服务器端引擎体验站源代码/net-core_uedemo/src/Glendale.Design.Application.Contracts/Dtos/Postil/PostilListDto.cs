using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Postil
{
    public class PostilListDto : AuditedEntityDto<Guid>
    {
        #region 属性
        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>       
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 模型文件Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }

        /// <summary>
        /// 模型版本Id
        /// </summary>
        public virtual Guid DocumentVersionId { get; set; }

        /// <summary>
        /// 批注标题
        /// </summary>       
        public virtual string Title { get; set; }

        /// <summary>
        /// 问题分类
        /// </summary>
        public virtual PostilCategoryEnum PostilCategory { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>       
        public virtual string Describe { get; set; }
        /// <summary>
        /// 处理建议
        /// </summary>         
        public virtual string Advise { get; set; }
        /// <summary>
        /// 批注缩略图
        /// </summary>        
        public virtual string ImgPath { get; set; }
        /// <summary>
        /// 视点位置
        /// </summary>       
        public virtual string ViewPosition { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual PostilStatusEnum Status { get; set; }

        /// <summary>
        /// 处理者Id
        /// </summary>
        public virtual Guid? HandlerUserId { get; set; }

        /// <summary>
        /// 处理日期
        /// </summary>
        public virtual DateTime? HandlerDateTime { get; set; }

        /// <summary>
        /// 处理说明
        /// </summary>        
        public virtual string HandlerRemark { get; set; }

        /// <summary>
        /// 关闭日期
        /// </summary>
        public virtual DateTime? CloseDateTime { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>         
        public virtual string DocumentName { get; set; }
        /// <summary>
        /// 模型版本
        /// </summary>         
        public virtual string DocumentVersionNo { get; set; }
        #endregion

                

        #region 自定义属性
        /// <summary>
        /// 创建人名字
        /// </summary>
        public virtual string CreationName { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        public virtual string StatusName { get; set; }       
       
        #endregion
    }
}
