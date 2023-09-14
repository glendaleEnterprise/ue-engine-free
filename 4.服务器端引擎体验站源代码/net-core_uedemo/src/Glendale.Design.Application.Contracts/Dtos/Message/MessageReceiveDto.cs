using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Message
{
    public class MessageReceiveDto : FullAuditedEntityDto<Guid>
    {
        public virtual Guid ProjectId { get; set; }
        /// <summary>
        /// 消息主键
        /// </summary>        
        public Guid MessageId { get; set; }

        /// <summary>
        /// 接收消息用户主键
        /// </summary>       
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>      
        public bool IsRead { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>     
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 阅读次数
        /// </summary>       
        public int ReadCount { get; set; }
    }
}
