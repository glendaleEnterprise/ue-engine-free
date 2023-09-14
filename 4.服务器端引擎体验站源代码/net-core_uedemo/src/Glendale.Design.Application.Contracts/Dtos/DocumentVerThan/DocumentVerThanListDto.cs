using Glendale.Design.Dtos.DocumentVer;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentVerThan
{
    public  class DocumentVerThanListDto : AuditedEntityDto<Guid>
    {
        #region 
        /// <summary>
        /// 标题
        /// </summary>        
        public virtual string Title { get; set; }

        /// <summary>
        /// 源模型版本Id
        /// </summary>
        public virtual Guid SourceDocVerId { get; set; }

        /// <summary>
        /// 目标模型版本Id
        /// </summary>
        public virtual Guid DestinationDocVerId { get; set; }

        /// <summary>
        /// 新增的构件
        /// </summary>
        public virtual string AddMetadata { get; set; }

        /// <summary>
        /// 修改的构件
        /// </summary>
        public virtual string UpdateMetadata { get; set; }

        /// <summary>
        /// 删除的构件
        /// </summary>
        public virtual string DeleteMetadata { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        public virtual string Remark { get; set; }

        #endregion

        #region 导航属性
        public virtual DocumentVersionDto SourceDocVer { get; set; }

        public virtual DocumentVersionDto DestinationDocVer { get; set; }

        #endregion

        #region 补充属性
        public virtual string CreationName { get; set; }
        #endregion
    }
}
