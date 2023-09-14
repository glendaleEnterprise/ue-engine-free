using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;


namespace Glendale.Design.Dtos
{
    public class LogsListDto : CreationAuditedEntityDto<Guid>
    {
        #region 属性
        /// <summary>
        /// 操作名称
        /// </summary> 
        public virtual string Name { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary> 
        public virtual string ModuleName { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public virtual string Content { get; set; }
        #endregion

        /// <summary>
        /// 用户名
        /// </summary> 
        public virtual string UserName { get; set; }
    }
}
