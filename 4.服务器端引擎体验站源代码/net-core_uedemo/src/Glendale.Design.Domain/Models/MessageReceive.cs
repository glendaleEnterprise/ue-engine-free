using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 系统消息阅读记录
    /// </summary>      
    public class MessageReceive: CreationAuditedEntity<Guid>
    {

        [Required]
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


        [NotMapped]
        public virtual Message Message { get; set; }

        public MessageReceive()
        {

        }

        public MessageReceive(Guid id)
            : base(id)
        {

        }
    }
}
